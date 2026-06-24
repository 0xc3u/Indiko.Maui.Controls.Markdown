using System.Drawing;
using CSharpMath.Rendering.FrontEnd;
using MauiCanvasType = Microsoft.Maui.Graphics.ICanvas;
using MauiColor = Microsoft.Maui.Graphics.Color;
using PathF = Microsoft.Maui.Graphics.PathF;
using LineCap = Microsoft.Maui.Graphics.LineCap;
using WindingMode = Microsoft.Maui.Graphics.WindingMode;
using Path = CSharpMath.Rendering.FrontEnd.Path;

namespace CSharpMath.MauiGraphics;

/// <summary>
/// A CSharpMath <see cref="ICanvas"/> that draws onto a <see cref="Microsoft.Maui.Graphics.ICanvas"/>.
/// MAUI Graphics equivalent of <c>CSharpMath.SkiaSharp.SkiaCanvas</c>, with no SkiaSharp dependency.
/// </summary>
public sealed class MauiCanvas : ICanvas
{
    private readonly MauiCanvasType _canvas;

    public MauiCanvas(MauiCanvasType canvas, bool antiAlias = true)
        : this(canvas, 0f, 0f, antiAlias)
    {
    }

    public MauiCanvas(MauiCanvasType canvas, float width, float height, bool antiAlias = true)
    {
        _canvas = canvas;
        _canvas.Antialias = antiAlias;
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Size of the drawing surface. MAUI's <see cref="Microsoft.Maui.Graphics.ICanvas"/> does not
    /// expose its bounds, so the caller supplies them (typically from an <see cref="IDrawable"/>'s
    /// <c>dirtyRect</c>). CSharpMath only reads these for alignment-based drawing; the point-based
    /// <c>Draw</c> overloads do not require them.
    /// </summary>
    public float Width { get; set; }

    public float Height { get; set; }

    public Color DefaultColor { get; set; }

    public Color? CurrentColor { get; set; }

    public PaintStyle CurrentStyle { get; set; }

    private MauiColor ResolveColor(Color? overrideColor = null) =>
        (overrideColor ?? CurrentColor ?? DefaultColor).ToMauiColor();

    public Path StartNewPath() => new MauiPath(this);

    public void FillRect(float left, float top, float width, float height)
    {
        _canvas.FillColor = ResolveColor();
        _canvas.FillRectangle(left, top, width, height);
    }

    public void StrokeRect(float left, float top, float width, float height)
    {
        // CSharpMath only uses StrokeRect to outline (debug) glyph boxes with an arbitrarily small
        // stroke; CSharpMath.SkiaSharp renders these with a zero-width — hence effectively invisible
        // — stroke. We mirror that hairline behaviour.
        _canvas.StrokeColor = ResolveColor();
        _canvas.StrokeSize = 0f;
        _canvas.DrawRectangle(left, top, width, height);
    }

    public void DrawLine(float x1, float y1, float x2, float y2, float lineThickness)
    {
        if (CurrentStyle == PaintStyle.Fill)
        {
            if (lineThickness <= 0f)
            {
                return;
            }

            _canvas.StrokeColor = ResolveColor();
            _canvas.StrokeSize = lineThickness;
            _canvas.StrokeLineCap = LineCap.Butt;
            _canvas.DrawLine(x1, y1, x2, y2);
        }
        else
        {
            // Non-fill styles render the line as a filled outline, matching SkiaCanvas.
            this.StrokeLineOutline(x1, y1, x2, y2, lineThickness);
        }
    }

    /// <summary>
    /// Draws a completed glyph/rule outline path, honouring the current paint style and an optional
    /// per-path foreground colour (set by <see cref="MauiPath"/>). Called on path disposal.
    /// </summary>
    internal void DrawPath(PathF path, Color? foreground)
    {
        MauiColor color = ResolveColor(foreground);
        if (CurrentStyle == PaintStyle.Fill)
        {
            _canvas.FillColor = color;
            _canvas.FillPath(path, WindingMode.NonZero);
        }
        else
        {
            _canvas.StrokeColor = color;
            _canvas.DrawPath(path);
        }
    }

    public void Save() => _canvas.SaveState();

    public void Restore() => _canvas.RestoreState();

    public void Translate(float dx, float dy) => _canvas.Translate(dx, dy);

    public void Scale(float sx, float sy) => _canvas.Scale(sx, sy);
}
