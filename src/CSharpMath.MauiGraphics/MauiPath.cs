using System.Drawing;
using CSharpMathPath = CSharpMath.Rendering.FrontEnd.Path;
using PathF = Microsoft.Maui.Graphics.PathF;

namespace CSharpMath.MauiGraphics;

/// <summary>
/// A <see cref="CSharpMath.Rendering.FrontEnd.Path"/> backed by a <see cref="PathF"/>.
/// CSharpMath fills this with glyph and rule outlines through the Typography glyph translator, then
/// draws it to the owning canvas when disposed. MAUI Graphics equivalent of
/// <c>CSharpMath.SkiaSharp.SkiaPath</c>.
/// </summary>
public sealed class MauiPath : CSharpMathPath
{
    private readonly MauiCanvas _owner;
    private readonly PathF _path = new();
    private bool _hasOpenContour;

    public MauiPath(MauiCanvas owner) => _owner = owner;

    public override Color? Foreground { get; set; }

    public override void MoveTo(float x0, float y0)
    {
        // Close any open contour before starting the next one so each glyph contour fills correctly.
        CloseOpenContour();
        _path.MoveTo(x0, y0);
        _hasOpenContour = true;
    }

    public override void LineTo(float x1, float y1) => _path.LineTo(x1, y1);

    // CSharpMath's Curve3 is a quadratic Bézier; Curve4 is a cubic Bézier.
    public override void Curve3(float x1, float y1, float x2, float y2) =>
        _path.QuadTo(x1, y1, x2, y2);

    public override void Curve4(float x1, float y1, float x2, float y2, float x3, float y3) =>
        _path.CurveTo(x1, y1, x2, y2, x3, y3);

    public override void CloseContour() => CloseOpenContour();

    public override void Dispose() => _owner.DrawPath(_path, Foreground);

    // Microsoft.Maui.Graphics.PathF.Close() throws (List<bool>.RemoveAt out of range) when there is
    // no open sub-path to close — unlike SkiaSharp's SKPath.Close(), which is a safe no-op. The
    // Typography glyph engine emits CloseContour before the first contour is started (and TrueType
    // glyphs may omit it entirely), so we track open state and only ever close a real contour.
    private void CloseOpenContour()
    {
        if (_hasOpenContour)
        {
            _path.Close();
            _hasOpenContour = false;
        }
    }
}
