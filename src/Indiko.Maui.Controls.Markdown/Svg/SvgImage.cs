using System.Globalization;
using System.Numerics;
using System.Xml.Linq;
using Microsoft.Maui.Graphics;

namespace Indiko.Maui.Controls.Markdown.Svg;

/// <summary>
/// A lightweight SVG renderer built on <see cref="Microsoft.Maui.Graphics"/> (no SkiaSharp). Parses
/// an SVG document and draws it onto an <see cref="ICanvas"/> as an <see cref="IDrawable"/>, and can
/// rasterize it to a PNG byte array using the platform bitmap-export service.
///
/// Supported: viewBox + intrinsic width/height, &lt;g&gt; nesting, transforms (translate/scale/
/// rotate/matrix/skew), &lt;path&gt; (full command set incl. arcs), rect/circle/ellipse/line/
/// polyline/polygon, fill/stroke with hex/rgb/named colours, fill-rule, and the opacity family.
/// Not supported: gradients/patterns (url(...) paints are skipped), filters, text, clip/mask.
/// </summary>
internal sealed class SvgImage : IDrawable
{
    private readonly XElement _root;

    private SvgImage(XElement root)
    {
        _root = root;

        (ViewBoxX, ViewBoxY, ViewBoxWidth, ViewBoxHeight) = ParseViewBox(root.Attribute("viewBox")?.Value);
        IntrinsicWidth = ParseLength(root.Attribute("width")?.Value, 0f);
        IntrinsicHeight = ParseLength(root.Attribute("height")?.Value, 0f);

        if (ViewBoxWidth <= 0 || ViewBoxHeight <= 0)
        {
            // No usable viewBox — fall back to the intrinsic size.
            ViewBoxX = ViewBoxY = 0;
            ViewBoxWidth = IntrinsicWidth > 0 ? IntrinsicWidth : 100f;
            ViewBoxHeight = IntrinsicHeight > 0 ? IntrinsicHeight : 100f;
        }
    }

    public float ViewBoxX { get; }
    public float ViewBoxY { get; }
    public float ViewBoxWidth { get; }
    public float ViewBoxHeight { get; }
    public float IntrinsicWidth { get; }
    public float IntrinsicHeight { get; }

    public static SvgImage? Parse(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            return null;
        }

        try
        {
            XDocument doc = XDocument.Parse(xml);
            XElement? root = doc.Root;
            if (root is null || !root.Name.LocalName.Equals("svg", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return new SvgImage(root);
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }
    }

    /// <summary>Rasterizes the SVG to PNG bytes, or returns <c>null</c> if rendering is unavailable.</summary>
    public static byte[]? RenderToPng(string? xml)
    {
        SvgImage? svg = Parse(xml);
        if (svg is null)
        {
            return null;
        }

        (int width, int height) = svg.GetRasterSize();

#if ANDROID || IOS || MACCATALYST || WINDOWS
        var service = new Microsoft.Maui.Graphics.Platform.PlatformBitmapExportService();
        using BitmapExportContext context = service.CreateContext(width, height);
        svg.Draw(context.Canvas, new RectF(0, 0, width, height));
        using var stream = new MemoryStream();
        context.WriteToStream(stream);
        return stream.ToArray();
#else
        // The plain net10.0 target has no platform bitmap-export service; rasterization is a no-op.
        return null;
#endif
    }

    public (int Width, int Height) GetRasterSize()
    {
        float w = IntrinsicWidth > 0 ? IntrinsicWidth : ViewBoxWidth;
        float h = IntrinsicHeight > 0 ? IntrinsicHeight : ViewBoxHeight;

        const float max = 1024f;
        float scale = MathF.Min(1f, max / MathF.Max(w, h));
        return (Math.Max(1, (int)MathF.Round(w * scale)), Math.Max(1, (int)MathF.Round(h * scale)));
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Map the viewBox into dirtyRect preserving aspect ratio (xMidYMid meet).
        float scale = MathF.Min(dirtyRect.Width / ViewBoxWidth, dirtyRect.Height / ViewBoxHeight);
        float tx = dirtyRect.X + (dirtyRect.Width - ViewBoxWidth * scale) / 2f;
        float ty = dirtyRect.Y + (dirtyRect.Height - ViewBoxHeight * scale) / 2f;

        canvas.SaveState();
        canvas.Translate(tx, ty);
        canvas.Scale(scale, scale);
        canvas.Translate(-ViewBoxX, -ViewBoxY);

        foreach (XElement child in _root.Elements())
        {
            RenderElement(canvas, child, RenderState.Default);
        }

        canvas.RestoreState();
    }

    private void RenderElement(ICanvas canvas, XElement element, RenderState inherited)
    {
        string tag = element.Name.LocalName.ToLowerInvariant();

        // Containers / non-rendered definitions.
        switch (tag)
        {
            case "defs":
            case "title":
            case "desc":
            case "metadata":
            case "style":
            case "clippath":
            case "mask":
            case "symbol":
            case "lineargradient":
            case "radialgradient":
                return;
        }

        Dictionary<string, string> style = ParseInlineStyle(element.Attribute("style")?.Value);
        RenderState state = inherited.With(element, style);

        Matrix3x2 transform = SvgPaint.ParseTransform(element.Attribute("transform")?.Value);
        bool pushedTransform = transform != Matrix3x2.Identity;
        if (pushedTransform)
        {
            canvas.SaveState();
            canvas.ConcatenateTransform(transform);
        }

        switch (tag)
        {
            case "g":
            case "a":
            case "svg":
                foreach (XElement child in element.Elements())
                {
                    RenderElement(canvas, child, state);
                }
                break;

            case "path":
            {
                var path = new PathF();
                SvgPathData.AppendTo(Prop(element, style, "d"), path);
                FillAndStroke(canvas, path, state);
                break;
            }

            case "rect":
                FillAndStroke(canvas, BuildRect(element, style), state);
                break;

            case "circle":
            {
                float cx = Len(element, style, "cx"), cy = Len(element, style, "cy"), rr = Len(element, style, "r");
                var path = new PathF();
                path.AppendEllipse(cx - rr, cy - rr, rr * 2, rr * 2);
                FillAndStroke(canvas, path, state);
                break;
            }

            case "ellipse":
            {
                float cx = Len(element, style, "cx"), cy = Len(element, style, "cy");
                float rx = Len(element, style, "rx"), ry = Len(element, style, "ry");
                var path = new PathF();
                path.AppendEllipse(cx - rx, cy - ry, rx * 2, ry * 2);
                FillAndStroke(canvas, path, state);
                break;
            }

            case "line":
            {
                var path = new PathF();
                path.MoveTo(Len(element, style, "x1"), Len(element, style, "y1"));
                path.LineTo(Len(element, style, "x2"), Len(element, style, "y2"));
                FillAndStroke(canvas, path, state);
                break;
            }

            case "polyline":
            case "polygon":
                FillAndStroke(canvas, BuildPoly(Prop(element, style, "points"), close: tag == "polygon"), state);
                break;
        }

        if (pushedTransform)
        {
            canvas.RestoreState();
        }
    }

    private static void FillAndStroke(ICanvas canvas, PathF path, RenderState s)
    {
        if (s.FillEnabled)
        {
            canvas.FillColor = s.Fill.WithAlpha(s.Fill.Alpha * s.FillOpacity * s.Opacity);
            canvas.FillPath(path, s.FillRule);
        }

        if (s.StrokeEnabled && s.StrokeWidth > 0)
        {
            canvas.StrokeColor = s.Stroke.WithAlpha(s.Stroke.Alpha * s.StrokeOpacity * s.Opacity);
            canvas.StrokeSize = s.StrokeWidth;
            canvas.DrawPath(path);
        }
    }

    private static PathF BuildRect(XElement element, Dictionary<string, string> style)
    {
        float x = Len(element, style, "x"), y = Len(element, style, "y");
        float w = Len(element, style, "width"), h = Len(element, style, "height");
        float rx = Len(element, style, "rx"), ry = Len(element, style, "ry");
        float radius = MathF.Max(rx, ry);

        var path = new PathF();
        if (radius > 0)
        {
            path.AppendRoundedRectangle(x, y, w, h, radius);
        }
        else
        {
            path.AppendRectangle(x, y, w, h);
        }

        return path;
    }

    private static PathF BuildPoly(string? points, bool close)
    {
        var path = new PathF();
        if (string.IsNullOrWhiteSpace(points))
        {
            return path;
        }

        float[] n = points!.Split(new[] { ',', ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => float.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : 0f)
            .ToArray();

        for (int i = 0; i + 1 < n.Length; i += 2)
        {
            if (i == 0)
            {
                path.MoveTo(n[i], n[i + 1]);
            }
            else
            {
                path.LineTo(n[i], n[i + 1]);
            }
        }

        if (close && n.Length >= 2)
        {
            path.Close();
        }

        return path;
    }

    private static (float X, float Y, float W, float H) ParseViewBox(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return (0, 0, 0, 0);
        }

        float[] v = value!.Split(new[] { ',', ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => float.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : 0f)
            .ToArray();

        return v.Length >= 4 ? (v[0], v[1], v[2], v[3]) : (0, 0, 0, 0);
    }

    private static Dictionary<string, string> ParseInlineStyle(string? style)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(style))
        {
            return map;
        }

        foreach (string decl in style!.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            int colon = decl.IndexOf(':');
            if (colon > 0)
            {
                map[decl[..colon].Trim()] = decl[(colon + 1)..].Trim();
            }
        }

        return map;
    }

    private static string? Prop(XElement element, Dictionary<string, string> style, string name) =>
        style.TryGetValue(name, out string? v) ? v : element.Attribute(name)?.Value;

    private static float Len(XElement element, Dictionary<string, string> style, string name) =>
        ParseLength(Prop(element, style, name), 0f);

    internal static float ParseLength(string? value, float fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        value = value!.Trim();
        if (value.EndsWith('%'))
        {
            return fallback; // percentage lengths are not resolved
        }

        // Strip a trailing unit (px, pt, em, …); we treat all as user units.
        int end = 0;
        while (end < value.Length && (char.IsDigit(value[end]) || value[end] is '.' or '-' or '+' or 'e' or 'E'))
        {
            end++;
        }

        return float.TryParse(value.AsSpan(0, end), NumberStyles.Float, CultureInfo.InvariantCulture, out float f)
            ? f
            : fallback;
    }

    internal static float ParseFloat(string? value, float fallback) =>
        float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : fallback;

    /// <summary>Resolved presentation state, inherited down the element tree (value semantics).</summary>
    private struct RenderState
    {
        public bool FillEnabled;
        public Color Fill;
        public bool StrokeEnabled;
        public Color Stroke;
        public float StrokeWidth;
        public float Opacity;
        public float FillOpacity;
        public float StrokeOpacity;
        public WindingMode FillRule;
        public Color CurrentColor;

        public static RenderState Default => new()
        {
            FillEnabled = true,
            Fill = Colors.Black,
            StrokeEnabled = false,
            Stroke = Colors.Black,
            StrokeWidth = 1f,
            Opacity = 1f,
            FillOpacity = 1f,
            StrokeOpacity = 1f,
            FillRule = WindingMode.NonZero,
            CurrentColor = Colors.Black,
        };

        public readonly RenderState With(XElement element, Dictionary<string, string> style)
        {
            RenderState s = this;

            string? color = Prop(element, style, "color");
            if (color is not null && SvgPaint.TryParseColor(color, s.CurrentColor, out Color cc))
            {
                s.CurrentColor = cc;
            }

            string? fill = Prop(element, style, "fill");
            if (fill is not null)
            {
                s.FillEnabled = SvgPaint.TryParseColor(fill, s.CurrentColor, out s.Fill);
            }

            string? stroke = Prop(element, style, "stroke");
            if (stroke is not null)
            {
                s.StrokeEnabled = SvgPaint.TryParseColor(stroke, s.CurrentColor, out s.Stroke);
            }

            string? strokeWidth = Prop(element, style, "stroke-width");
            if (strokeWidth is not null)
            {
                s.StrokeWidth = ParseLength(strokeWidth, s.StrokeWidth);
            }

            string? opacity = Prop(element, style, "opacity");
            if (opacity is not null)
            {
                s.Opacity *= Math.Clamp(ParseFloat(opacity, 1f), 0f, 1f);
            }

            string? fillOpacity = Prop(element, style, "fill-opacity");
            if (fillOpacity is not null)
            {
                s.FillOpacity = Math.Clamp(ParseFloat(fillOpacity, 1f), 0f, 1f);
            }

            string? strokeOpacity = Prop(element, style, "stroke-opacity");
            if (strokeOpacity is not null)
            {
                s.StrokeOpacity = Math.Clamp(ParseFloat(strokeOpacity, 1f), 0f, 1f);
            }

            string? fillRule = Prop(element, style, "fill-rule");
            if (fillRule is not null)
            {
                s.FillRule = fillRule.Trim().Equals("evenodd", StringComparison.OrdinalIgnoreCase)
                    ? WindingMode.EvenOdd
                    : WindingMode.NonZero;
            }

            return s;
        }
    }
}
