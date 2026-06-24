using System.Globalization;
using System.Numerics;
using System.Xml.Linq;
using Microsoft.Maui.Graphics;

namespace Indiko.Maui.Controls.Markdown.Svg;

/// <summary>
/// Builds <see cref="Microsoft.Maui.Graphics"/> gradient <see cref="Paint"/>s from SVG
/// <c>&lt;linearGradient&gt;</c>/<c>&lt;radialGradient&gt;</c> definitions, resolving
/// <c>gradientUnits</c>, <c>gradientTransform</c>, stops, and <c>href</c> inheritance.
///
/// Approximations: MAUI radial paints have no focal point (<c>fx</c>/<c>fy</c> are ignored) and no
/// <c>spreadMethod</c> (pad only); a radial radius in <c>userSpaceOnUse</c> is normalised against the
/// larger bounds dimension. These cover the vast majority of real-world (logo) gradients.
/// </summary>
internal static class SvgGradient
{
    /// <summary>Creates a fill <see cref="Paint"/> for the gradient <paramref name="id"/>, or null if it can't be resolved.</summary>
    public static Paint? Create(IReadOnlyDictionary<string, XElement> defs, string id, RectF bounds,
        float opacity, Color currentColor)
    {
        if (!defs.TryGetValue(id, out XElement? grad))
        {
            return null;
        }

        List<XElement> chain = BuildChain(defs, grad);

        string? Get(string name)
        {
            foreach (XElement el in chain)
            {
                string? value = Attr(el, name);
                if (value is not null)
                {
                    return value;
                }
            }

            return null;
        }

        List<PaintGradientStop> stops = ReadStops(chain, opacity, currentColor);
        if (stops.Count == 0)
        {
            return null;
        }

        bool userSpace = string.Equals(Get("gradientUnits"), "userSpaceOnUse", StringComparison.OrdinalIgnoreCase);
        Matrix3x2 transform = SvgPaint.ParseTransform(Get("gradientTransform"));

        if (grad.Name.LocalName.Equals("radialGradient", StringComparison.OrdinalIgnoreCase))
        {
            Vector2 center = Vector2.Transform(new Vector2(Coord(Get("cx"), 0.5f), Coord(Get("cy"), 0.5f)), transform);
            float r = Coord(Get("r"), 0.5f);

            (float cxRel, float cyRel) = userSpace
                ? ((center.X - bounds.X) / NonZero(bounds.Width), (center.Y - bounds.Y) / NonZero(bounds.Height))
                : (center.X, center.Y);
            float rRel = userSpace ? r / NonZero(MathF.Max(bounds.Width, bounds.Height)) : r;

            return new RadialGradientPaint
            {
                Center = new Point(cxRel, cyRel),
                Radius = rRel,
                GradientStops = stops.ToArray(),
            };
        }
        else
        {
            Vector2 p1 = Vector2.Transform(new Vector2(Coord(Get("x1"), 0f), Coord(Get("y1"), 0f)), transform);
            Vector2 p2 = Vector2.Transform(new Vector2(Coord(Get("x2"), 1f), Coord(Get("y2"), 0f)), transform);

            (float sx, float sy) = userSpace
                ? ((p1.X - bounds.X) / NonZero(bounds.Width), (p1.Y - bounds.Y) / NonZero(bounds.Height))
                : (p1.X, p1.Y);
            (float ex, float ey) = userSpace
                ? ((p2.X - bounds.X) / NonZero(bounds.Width), (p2.Y - bounds.Y) / NonZero(bounds.Height))
                : (p2.X, p2.Y);

            return new LinearGradientPaint
            {
                StartPoint = new Point(sx, sy),
                EndPoint = new Point(ex, ey),
                GradientStops = stops.ToArray(),
            };
        }
    }

    /// <summary>
    /// A representative solid colour for a gradient — used to approximate gradient strokes, which
    /// MAUI's <see cref="ICanvas"/> cannot paint (it only supports gradient fills).
    /// </summary>
    public static bool TryGetRepresentativeColor(IReadOnlyDictionary<string, XElement> defs, string id,
        Color currentColor, out Color color)
    {
        color = Colors.Black;
        if (!defs.TryGetValue(id, out XElement? grad))
        {
            return false;
        }

        List<PaintGradientStop> stops = ReadStops(BuildChain(defs, grad), 1f, currentColor);
        if (stops.Count == 0)
        {
            return false;
        }

        color = stops[stops.Count / 2].Color; // middle stop is a fair single-colour stand-in
        return true;
    }

    /// <summary>Walks the <c>href</c>/<c>xlink:href</c> inheritance chain, guarding against cycles.</summary>
    private static List<XElement> BuildChain(IReadOnlyDictionary<string, XElement> defs, XElement grad)
    {
        var chain = new List<XElement>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        XElement? current = grad;

        while (current is not null)
        {
            chain.Add(current);
            string? href = Attr(current, "href");
            if (href is null || !href.StartsWith('#'))
            {
                break;
            }

            string refId = href.Substring(1);
            if (!seen.Add(refId) || !defs.TryGetValue(refId, out current))
            {
                break;
            }
        }

        return chain;
    }

    private static List<PaintGradientStop> ReadStops(List<XElement> chain, float opacity, Color currentColor)
    {
        // Stops are taken from the first element in the inheritance chain that declares any.
        foreach (XElement el in chain)
        {
            List<XElement> stopEls = el.Elements()
                .Where(e => e.Name.LocalName.Equals("stop", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (stopEls.Count == 0)
            {
                continue;
            }

            var stops = new List<PaintGradientStop>(stopEls.Count);
            float lastOffset = 0f;

            foreach (XElement stop in stopEls)
            {
                Dictionary<string, string> style = ParseStyle(Attr(stop, "style"));

                float offset = Math.Clamp(ParseOffset(Prop(stop, style, "offset")), 0f, 1f);
                offset = MathF.Max(offset, lastOffset); // SVG requires non-decreasing offsets
                lastOffset = offset;

                Color color = SvgPaint.TryParseColor(Prop(stop, style, "stop-color") ?? "black", currentColor, out Color c)
                    ? c
                    : Colors.Black;
                float stopOpacity = Math.Clamp(ParseOffset(Prop(stop, style, "stop-opacity")), 0f, 1f);
                if (Prop(stop, style, "stop-opacity") is null)
                {
                    stopOpacity = 1f;
                }

                stops.Add(new PaintGradientStop(offset, color.WithAlpha(color.Alpha * stopOpacity * opacity)));
            }

            return stops;
        }

        return new List<PaintGradientStop>();
    }

    private static float Coord(string? value, float fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        value = value!.Trim();
        if (value.EndsWith('%'))
        {
            return float.TryParse(value.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out float pct)
                ? pct / 100f
                : fallback;
        }

        return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : fallback;
    }

    private static float ParseOffset(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0f;
        }

        value = value!.Trim();
        return value.EndsWith('%')
            ? (float.TryParse(value.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out float pct) ? pct / 100f : 0f)
            : (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : 0f);
    }

    private static float NonZero(float v) => MathF.Abs(v) < float.Epsilon ? 1f : v;

    /// <summary>Reads an attribute by local name, ignoring namespace (handles <c>href</c> and <c>xlink:href</c>).</summary>
    private static string? Attr(XElement element, string localName) =>
        element.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase))?.Value;

    private static string? Prop(XElement element, Dictionary<string, string> style, string name) =>
        style.TryGetValue(name, out string? v) ? v : Attr(element, name);

    private static Dictionary<string, string> ParseStyle(string? style)
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
}
