using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Xunit;
using static Indiko.Maui.Controls.Markdown.Tests.MarkdownTestHarness;

namespace Indiko.Maui.Controls.Markdown.Tests;

/// <summary>
/// Regression tests for the recursive inline renderer (RenderInlines / RenderInline). These guard
/// the inline features added in the Tier 1 work: inline code, underscore + nested emphasis, the
/// "emphasis extras" (highlight / super- / subscript / inserted), and formatted link labels.
/// </summary>
public class InlineRenderingTests
{
    [Fact]
    public void Inline_code_is_rendered_with_the_code_color()
    {
        // Previously inline code spans were dropped entirely.
        var content = Render("Run the `dotnet build` command.");

        var span = Spans(content).FirstOrDefault(s => s.Text == "dotnet build");
        Assert.NotNull(span);
        AssertColor(Colors.BlueViolet, span!.TextColor); // default CodeBlockTextColor
    }

    [Fact]
    public void Underscore_bold_renders_bold()
    {
        Assert.Contains(Spans(Render("This is __bold__ text.")),
            s => s.Text == "bold" && s.FontAttributes == FontAttributes.Bold);
    }

    [Fact]
    public void Underscore_italic_renders_italic()
    {
        Assert.Contains(Spans(Render("This is _italic_ text.")),
            s => s.Text == "italic" && s.FontAttributes == FontAttributes.Italic);
    }

    [Fact]
    public void Combined_bold_italic_renders_both_attributes()
    {
        Assert.Contains(Spans(Render("This is ***both*** here.")),
            s => s.Text == "both" && s.FontAttributes == (FontAttributes.Bold | FontAttributes.Italic));
    }

    [Fact]
    public void Highlight_renders_with_the_highlight_background()
    {
        var content = Render("This is ==marked== text.");

        var span = Spans(content).FirstOrDefault(s => s.Text == "marked");
        Assert.NotNull(span);
        AssertColor(Color.FromArgb("#FFF59D"), span!.BackgroundColor); // default HighlightColor
    }

    [Fact]
    public void Inserted_renders_underlined()
    {
        Assert.Contains(Spans(Render("This is ++added++ text.")),
            s => s.Text == "added" && s.TextDecorations.HasFlag(TextDecorations.Underline));
    }

    [Fact]
    public void Superscript_renders_with_a_smaller_font()
    {
        var span = Spans(Render("E = mc^2^ today.")).FirstOrDefault(s => s.Text == "2");
        Assert.NotNull(span);
        Assert.True(span!.FontSize < 12d); // default body size is 12; super/subscript shrink it
    }

    [Fact]
    public void Subscript_renders_with_a_smaller_font()
    {
        var span = Spans(Render("Water is H~2~O.")).FirstOrDefault(s => s.Text == "2");
        Assert.NotNull(span);
        Assert.True(span!.FontSize < 12d);
    }

    [Fact]
    public void Strikethrough_still_renders()
    {
        Assert.Contains(Spans(Render("This is ~~struck~~ text.")),
            s => s.Text == "struck" && s.TextDecorations.HasFlag(TextDecorations.Strikethrough));
    }

    [Fact]
    public void Formatted_link_label_keeps_emphasis_and_is_tappable()
    {
        // The bold word inside a link label must stay bold, be coloured/underlined as a link, and
        // carry a tap gesture (the whole label is tappable).
        var content = Render("See [**bold** link](https://example.com).");

        var span = Spans(content).FirstOrDefault(s => s.Text == "bold");
        Assert.NotNull(span);
        Assert.Equal(FontAttributes.Bold, span!.FontAttributes);
        Assert.True(span.TextDecorations.HasFlag(TextDecorations.Underline));
        AssertColor(Colors.BlueViolet, span.TextColor); // default HyperlinkColor
        Assert.NotEmpty(span.GestureRecognizers);
    }

    [Fact]
    public void Inline_code_inside_a_heading_is_rendered()
    {
        // Headings previously rendered only text + bold/italic; rich inlines were dropped.
        var content = Render("# Title with `code` word");

        Assert.Contains(Spans(content), s => s.Text == "code");
    }
}
