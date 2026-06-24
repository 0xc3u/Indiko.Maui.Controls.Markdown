using System.Globalization;
using System.Numerics;
using Microsoft.Maui.Graphics;

namespace Indiko.Maui.Controls.Markdown.Svg;

/// <summary>Helpers for parsing SVG paint (colour) and transform attribute values.</summary>
internal static class SvgPaint
{
    /// <summary>
    /// Resolves an SVG paint value. Returns <c>false</c> for <c>none</c> (paint disabled). Handles
    /// hex, <c>rgb()</c>/<c>rgba()</c>, named colours (via <see cref="Color.TryParse"/>) and the
    /// <c>currentColor</c> keyword (resolved to <paramref name="currentColor"/>).
    /// </summary>
    public static bool TryParseColor(string? value, Color currentColor, out Color color)
    {
        color = Colors.Black;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value!.Trim();

        if (value.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (value.Equals("transparent", StringComparison.OrdinalIgnoreCase))
        {
            color = Colors.Transparent;
            return true;
        }

        if (value.Equals("currentColor", StringComparison.OrdinalIgnoreCase))
        {
            color = currentColor;
            return true;
        }

        // url(#gradient) and other paint servers are not supported — skip the paint rather than
        // drawing it solid black.
        if (value.StartsWith("url(", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (value.StartsWith("rgb", StringComparison.OrdinalIgnoreCase) && TryParseRgb(value, out color))
        {
            return true;
        }

        if (Color.TryParse(value, out Color? parsed) && parsed is not null)
        {
            color = parsed;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Extracts the fragment id from a <c>url(#id)</c> paint reference (used for gradient fills/strokes).
    /// </summary>
    public static bool TryGetUrlId(string? value, out string id)
    {
        id = string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value!.Trim();
        if (!value.StartsWith("url(", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        int open = value.IndexOf('(');
        int close = value.IndexOf(')', open + 1);
        if (close < 0)
        {
            return false;
        }

        string inner = value.Substring(open + 1, close - open - 1).Trim().Trim('"', '\'');
        if (inner.StartsWith('#'))
        {
            inner = inner.Substring(1);
        }

        if (inner.Length == 0)
        {
            return false;
        }

        id = inner;
        return true;
    }

    private static bool TryParseRgb(string value, out Color color)
    {
        color = Colors.Black;
        int open = value.IndexOf('(');
        int close = value.IndexOf(')');
        if (open < 0 || close < 0 || close < open)
        {
            return false;
        }

        string[] parts = value.Substring(open + 1, close - open - 1).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 3)
        {
            return false;
        }

        float Component(string p) =>
            p.EndsWith('%')
                ? float.Parse(p.TrimEnd('%'), CultureInfo.InvariantCulture) / 100f
                : float.Parse(p, CultureInfo.InvariantCulture) / 255f;

        try
        {
            float r = Component(parts[0]);
            float g = Component(parts[1]);
            float b = Component(parts[2]);
            float a = parts.Length >= 4 ? float.Parse(parts[3], CultureInfo.InvariantCulture) : 1f;
            color = new Color(r, g, b, a);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    /// <summary>Parses an SVG <c>transform</c> attribute (translate/scale/rotate/matrix/skewX/skewY).</summary>
    public static Matrix3x2 ParseTransform(string? value)
    {
        var result = Matrix3x2.Identity;
        if (string.IsNullOrWhiteSpace(value))
        {
            return result;
        }

        int i = 0;
        while (i < value!.Length)
        {
            int open = value.IndexOf('(', i);
            if (open < 0)
            {
                break;
            }

            int close = value.IndexOf(')', open);
            if (close < 0)
            {
                break;
            }

            string name = value.Substring(i, open - i).Trim();
            float[] a = value.Substring(open + 1, close - open - 1)
                .Split(new[] { ',', ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => float.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out float f) ? f : 0f)
                .ToArray();

            Matrix3x2 m = name switch
            {
                "translate" => Matrix3x2.CreateTranslation(a.ElementAtOrDefault(0), a.ElementAtOrDefault(1)),
                "scale" => Matrix3x2.CreateScale(a.ElementAtOrDefault(0), a.Length > 1 ? a[1] : a.ElementAtOrDefault(0)),
                "rotate" => a.Length >= 3
                    ? Matrix3x2.CreateRotation(Deg(a[0]), new Vector2(a[1], a[2]))
                    : Matrix3x2.CreateRotation(Deg(a.ElementAtOrDefault(0))),
                "matrix" when a.Length >= 6 => new Matrix3x2(a[0], a[1], a[2], a[3], a[4], a[5]),
                "skewx" or "skewX" => new Matrix3x2(1, 0, MathF.Tan(Deg(a.ElementAtOrDefault(0))), 1, 0, 0),
                "skewy" or "skewY" => new Matrix3x2(1, MathF.Tan(Deg(a.ElementAtOrDefault(0))), 0, 1, 0, 0),
                _ => Matrix3x2.Identity,
            };

            // SVG applies the transform list left-to-right with the leftmost outermost, so each new
            // matrix pre-multiplies the accumulated one.
            result = m * result;
            i = close + 1;
        }

        return result;
    }

    private static float Deg(float degrees) => degrees * MathF.PI / 180f;
}
