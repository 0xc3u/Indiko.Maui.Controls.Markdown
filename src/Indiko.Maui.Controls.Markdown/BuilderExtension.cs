using Indiko.Maui.Controls.Markdown.Controls;
#if IOS || MACCATALYST || ANDROID || WINDOWS
using Indiko.Maui.Controls.Markdown.Handlers;
#endif

namespace Indiko.Maui.Controls.Markdown;

public static class BuilderExtension
{
    /// <summary>
    /// Registers everything <c>MarkdownView</c> needs. Math rendering uses the MAUI Graphics backend
    /// (no <c>UseSkiaSharp()</c> required), but this now also registers the native
    /// <see cref="ZoomImageView"/> handler that powers <c>MarkdownView.AllowImagePopup</c>'s
    /// full-screen zoomable image overlay. Call it in <c>MauiProgram</c>. If you don't use
    /// <c>AllowImagePopup</c>, nothing else depends on this call.
    /// </summary>
    public static MauiAppBuilder UseMarkdownView(this MauiAppBuilder builder)
    {
#if IOS || MACCATALYST || ANDROID || WINDOWS
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(ZoomImageView), typeof(ZoomImageViewHandler));
        });
#endif
        return builder;
    }
}
