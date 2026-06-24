using System.Globalization;
using Microsoft.Maui.Graphics;

namespace Indiko.Maui.Controls.Markdown.Svg;

/// <summary>
/// Parses an SVG <c>path</c> <c>d</c> attribute into a <see cref="PathF"/>. Supports the full
/// command set (M/L/H/V/C/S/Q/T/A/Z, absolute and relative), including SVG's compact number syntax
/// (e.g. <c>10-20</c>, <c>1.5.5</c>, exponents) and elliptical arcs (converted to cubic Béziers).
/// </summary>
internal static class SvgPathData
{
    public static void AppendTo(string? d, PathF path)
    {
        if (string.IsNullOrWhiteSpace(d))
        {
            return;
        }

        var r = new Reader(d!);
        char cmd = '\0';
        PointF cur = default;       // current point
        PointF subStart = default;  // start of the current sub-path (for Z)
        PointF prevCubic = default; // last cubic control point (for S)
        PointF prevQuad = default;  // last quad control point (for T)
        char prevCmd = '\0';
        bool open = false;

        while (true)
        {
            r.SkipSeparators();
            if (r.AtEnd)
            {
                break;
            }

            char c = r.Peek();
            if (char.IsLetter(c))
            {
                cmd = c;
                r.Next();
            }
            else
            {
                // Implicit command repetition: after an M/m the following coordinate pairs are L/l.
                if (cmd == 'M')
                {
                    cmd = 'L';
                }
                else if (cmd == 'm')
                {
                    cmd = 'l';
                }
                else if (cmd == '\0')
                {
                    break; // malformed
                }
            }

            bool rel = char.IsLower(cmd);
            switch (char.ToUpperInvariant(cmd))
            {
                case 'M':
                {
                    float x = r.Number(), y = r.Number();
                    cur = rel ? new PointF(cur.X + x, cur.Y + y) : new PointF(x, y);
                    path.MoveTo(cur.X, cur.Y);
                    subStart = cur;
                    open = true;
                    break;
                }
                case 'L':
                {
                    float x = r.Number(), y = r.Number();
                    cur = rel ? new PointF(cur.X + x, cur.Y + y) : new PointF(x, y);
                    path.LineTo(cur.X, cur.Y);
                    break;
                }
                case 'H':
                {
                    float x = r.Number();
                    cur = new PointF(rel ? cur.X + x : x, cur.Y);
                    path.LineTo(cur.X, cur.Y);
                    break;
                }
                case 'V':
                {
                    float y = r.Number();
                    cur = new PointF(cur.X, rel ? cur.Y + y : y);
                    path.LineTo(cur.X, cur.Y);
                    break;
                }
                case 'C':
                {
                    float x1 = r.Number(), y1 = r.Number(), x2 = r.Number(), y2 = r.Number(), x = r.Number(), y = r.Number();
                    var c1 = rel ? new PointF(cur.X + x1, cur.Y + y1) : new PointF(x1, y1);
                    var c2 = rel ? new PointF(cur.X + x2, cur.Y + y2) : new PointF(x2, y2);
                    cur = rel ? new PointF(cur.X + x, cur.Y + y) : new PointF(x, y);
                    path.CurveTo(c1.X, c1.Y, c2.X, c2.Y, cur.X, cur.Y);
                    prevCubic = c2;
                    break;
                }
                case 'S':
                {
                    float x2 = r.Number(), y2 = r.Number(), x = r.Number(), y = r.Number();
                    var c1 = (char.ToUpperInvariant(prevCmd) is 'C' or 'S')
                        ? new PointF(2 * cur.X - prevCubic.X, 2 * cur.Y - prevCubic.Y)
                        : cur;
                    var c2 = rel ? new PointF(cur.X + x2, cur.Y + y2) : new PointF(x2, y2);
                    cur = rel ? new PointF(cur.X + x, cur.Y + y) : new PointF(x, y);
                    path.CurveTo(c1.X, c1.Y, c2.X, c2.Y, cur.X, cur.Y);
                    prevCubic = c2;
                    break;
                }
                case 'Q':
                {
                    float x1 = r.Number(), y1 = r.Number(), x = r.Number(), y = r.Number();
                    var c1 = rel ? new PointF(cur.X + x1, cur.Y + y1) : new PointF(x1, y1);
                    cur = rel ? new PointF(cur.X + x, cur.Y + y) : new PointF(x, y);
                    path.QuadTo(c1.X, c1.Y, cur.X, cur.Y);
                    prevQuad = c1;
                    break;
                }
                case 'T':
                {
                    float x = r.Number(), y = r.Number();
                    var c1 = (char.ToUpperInvariant(prevCmd) is 'Q' or 'T')
                        ? new PointF(2 * cur.X - prevQuad.X, 2 * cur.Y - prevQuad.Y)
                        : cur;
                    cur = rel ? new PointF(cur.X + x, cur.Y + y) : new PointF(x, y);
                    path.QuadTo(c1.X, c1.Y, cur.X, cur.Y);
                    prevQuad = c1;
                    break;
                }
                case 'A':
                {
                    float rx = r.Number(), ry = r.Number(), xRot = r.Number();
                    bool largeArc = r.Flag(), sweep = r.Flag();
                    float x = r.Number(), y = r.Number();
                    var end = rel ? new PointF(cur.X + x, cur.Y + y) : new PointF(x, y);
                    AppendArc(path, cur, rx, ry, xRot, largeArc, sweep, end);
                    cur = end;
                    break;
                }
                case 'Z':
                {
                    if (open)
                    {
                        path.Close();
                        open = false;
                    }
                    cur = subStart;
                    break;
                }
                default:
                    return; // unknown command — stop parsing defensively
            }

            prevCmd = cmd;
        }
    }

    /// <summary>Converts an SVG elliptical arc to cubic Béziers and appends them to <paramref name="path"/>.</summary>
    private static void AppendArc(PathF path, PointF start, float rx, float ry, float xAxisRotationDeg,
        bool largeArc, bool sweep, PointF end)
    {
        if (rx == 0 || ry == 0)
        {
            path.LineTo(end.X, end.Y);
            return;
        }

        rx = Math.Abs(rx);
        ry = Math.Abs(ry);
        double phi = xAxisRotationDeg * Math.PI / 180.0;
        double cosPhi = Math.Cos(phi), sinPhi = Math.Sin(phi);

        // Step 1: compute (x1', y1') — the start point in the rotated, midpoint-centred frame.
        double dx = (start.X - end.X) / 2.0, dy = (start.Y - end.Y) / 2.0;
        double x1p = cosPhi * dx + sinPhi * dy;
        double y1p = -sinPhi * dx + cosPhi * dy;

        // Correct out-of-range radii.
        double lambda = (x1p * x1p) / (rx * rx) + (y1p * y1p) / (ry * ry);
        if (lambda > 1)
        {
            double s = Math.Sqrt(lambda);
            rx *= (float)s;
            ry *= (float)s;
        }

        // Step 2: compute the centre (cx', cy').
        double rx2 = rx * rx, ry2 = ry * ry;
        double num = rx2 * ry2 - rx2 * y1p * y1p - ry2 * x1p * x1p;
        double den = rx2 * y1p * y1p + ry2 * x1p * x1p;
        double co = den == 0 ? 0 : Math.Sqrt(Math.Max(0, num / den));
        if (largeArc == sweep)
        {
            co = -co;
        }
        double cxp = co * (rx * y1p / ry);
        double cyp = co * -(ry * x1p / rx);

        // Step 3: the centre in the original coordinate system.
        double cx = cosPhi * cxp - sinPhi * cyp + (start.X + end.X) / 2.0;
        double cy = sinPhi * cxp + cosPhi * cyp + (start.Y + end.Y) / 2.0;

        // Step 4: start angle and angle sweep.
        double startAngle = Angle(1, 0, (x1p - cxp) / rx, (y1p - cyp) / ry);
        double deltaAngle = Angle((x1p - cxp) / rx, (y1p - cyp) / ry, (-x1p - cxp) / rx, (-y1p - cyp) / ry);
        if (!sweep && deltaAngle > 0)
        {
            deltaAngle -= 2 * Math.PI;
        }
        else if (sweep && deltaAngle < 0)
        {
            deltaAngle += 2 * Math.PI;
        }

        // Emit one cubic Bézier per <=90° segment.
        int segments = (int)Math.Ceiling(Math.Abs(deltaAngle) / (Math.PI / 2));
        if (segments == 0)
        {
            segments = 1;
        }
        double delta = deltaAngle / segments;
        double t = 8.0 / 3.0 * Math.Sin(delta / 4) * Math.Sin(delta / 4) / Math.Sin(delta / 2);

        double angle = startAngle;
        for (int i = 0; i < segments; i++)
        {
            double cos1 = Math.Cos(angle), sin1 = Math.Sin(angle);
            double angle2 = angle + delta;
            double cos2 = Math.Cos(angle2), sin2 = Math.Sin(angle2);

            PointF Map(double ex, double ey)
            {
                double px = cosPhi * (rx * ex) - sinPhi * (ry * ey) + cx;
                double py = sinPhi * (rx * ex) + cosPhi * (ry * ey) + cy;
                return new PointF((float)px, (float)py);
            }

            var p1 = Map(cos1 - t * sin1, sin1 + t * cos1);
            var p2 = Map(cos2 + t * sin2, sin2 - t * cos2);
            var p = Map(cos2, sin2);
            path.CurveTo(p1.X, p1.Y, p2.X, p2.Y, p.X, p.Y);
            angle = angle2;
        }
    }

    private static double Angle(double ux, double uy, double vx, double vy)
    {
        double dot = ux * vx + uy * vy;
        double len = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
        double a = Math.Acos(Math.Clamp(len == 0 ? 0 : dot / len, -1.0, 1.0));
        return (ux * vy - uy * vx) < 0 ? -a : a;
    }

    /// <summary>Scans SVG's compact number/flag syntax.</summary>
    private struct Reader
    {
        private readonly string _s;
        private int _i;

        public Reader(string s)
        {
            _s = s;
            _i = 0;
        }

        public readonly bool AtEnd => _i >= _s.Length;

        public readonly char Peek() => _s[_i];

        public void Next() => _i++;

        public void SkipSeparators()
        {
            while (_i < _s.Length && (char.IsWhiteSpace(_s[_i]) || _s[_i] == ','))
            {
                _i++;
            }
        }

        /// <summary>Reads a single arc flag (a lone '0' or '1', which may be unseparated).</summary>
        public bool Flag()
        {
            SkipSeparators();
            if (_i < _s.Length)
            {
                char c = _s[_i++];
                return c == '1';
            }
            return false;
        }

        public float Number()
        {
            SkipSeparators();
            int start = _i;
            if (_i < _s.Length && (_s[_i] == '+' || _s[_i] == '-'))
            {
                _i++;
            }

            bool dot = false;
            while (_i < _s.Length)
            {
                char c = _s[_i];
                if (c >= '0' && c <= '9')
                {
                    _i++;
                }
                else if (c == '.' && !dot)
                {
                    dot = true;
                    _i++;
                }
                else if (c == 'e' || c == 'E')
                {
                    _i++;
                    if (_i < _s.Length && (_s[_i] == '+' || _s[_i] == '-'))
                    {
                        _i++;
                    }
                }
                else
                {
                    break;
                }
            }

            if (_i == start)
            {
                _i++; // guard against an infinite loop on malformed input
                return 0f;
            }

            return float.TryParse(_s.AsSpan(start, _i - start), NumberStyles.Float, CultureInfo.InvariantCulture, out float v)
                ? v
                : 0f;
        }
    }
}
