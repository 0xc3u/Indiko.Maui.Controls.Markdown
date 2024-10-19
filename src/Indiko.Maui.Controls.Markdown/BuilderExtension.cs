
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Indiko.Maui.Controls.Markdown;

public static class BuilderExtension
{
    public static MauiAppBuilder UseMarkdownView(this MauiAppBuilder builder)
    {
        builder.UseSkiaSharp();
        return builder;
    }
}
