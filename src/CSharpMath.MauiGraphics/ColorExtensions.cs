using System.Drawing;
using MauiColor = Microsoft.Maui.Graphics.Color;

namespace CSharpMath.MauiGraphics;

/// <summary>
/// Conversions between <see cref="System.Drawing.Color"/> — the colour type used throughout
/// CSharpMath.Rendering — and <see cref="Microsoft.Maui.Graphics.Color"/>. This is the MAUI
/// Graphics counterpart of the SkiaSharp <c>ToNative()</c>/<c>FromNative()</c> extensions.
/// </summary>
internal static class ColorExtensions
{
    public static MauiColor ToMauiColor(this Color color) =>
        new MauiColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

    public static Color ToSystemDrawingColor(this MauiColor color)
    {
        color.ToRgba(out byte r, out byte g, out byte b, out byte a);
        return Color.FromArgb(a, r, g, b);
    }
}
