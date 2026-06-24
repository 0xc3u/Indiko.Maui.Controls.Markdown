using System.Drawing;
using CSharpMath.Rendering.FrontEnd;
using MauiCanvasType = Microsoft.Maui.Graphics.ICanvas;
using MauiColor = Microsoft.Maui.Graphics.Color;

namespace CSharpMath.MauiGraphics;

/// <summary>
/// Renders mixed text and LaTeX math onto a <see cref="Microsoft.Maui.Graphics.ICanvas"/>.
/// MAUI Graphics port of <c>CSharpMath.SkiaSharp.TextPainter</c>.
/// </summary>
public class TextPainter : TextPainter<MauiCanvasType, MauiColor>
{
    public bool AntiAlias { get; set; } = true;

    public override MauiColor UnwrapColor(Color color) => color.ToMauiColor();

    public override Color WrapColor(MauiColor color) => color.ToSystemDrawingColor();

    public override ICanvas WrapCanvas(MauiCanvasType canvas) => new MauiCanvas(canvas, AntiAlias);
}
