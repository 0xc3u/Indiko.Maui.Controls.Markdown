namespace Indiko.Maui.Controls.Markdown;

/// <summary>
/// Provides data for the <see cref="MarkdownView.OnRenderError"/> event, raised when the
/// control fails to render markdown (for example, an incompatible Markdig version is loaded).
/// </summary>
public class MarkdownRenderErrorEventArgs : EventArgs
{
    /// <summary>The exception that caused the render failure.</summary>
    public Exception Exception { get; set; }

    /// <summary>A human-readable description of what failed.</summary>
    public string Message { get; set; }
}
