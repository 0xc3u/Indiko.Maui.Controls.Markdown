namespace Indiko.Maui.Controls.Markdown;

public static class BuilderExtension
{
    /// <summary>
    /// Registers everything <c>MarkdownView</c> needs. Math rendering now uses the MAUI Graphics
    /// backend (no <c>UseSkiaSharp()</c> handler registration required), so this is currently a
    /// no-op kept for API compatibility — consumers should continue to call it in <c>MauiProgram</c>.
    /// </summary>
    public static MauiAppBuilder UseMarkdownView(this MauiAppBuilder builder)
    {
        return builder;
    }
}
