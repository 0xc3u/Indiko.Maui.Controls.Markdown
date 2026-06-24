using Microsoft.Maui.Controls;
using Xunit;
using static Indiko.Maui.Controls.Markdown.Tests.MarkdownTestHarness;

namespace Indiko.Maui.Controls.Markdown.Tests;

public class EmphasisRenderingTests
{
    [Fact]
    public void Bold_text_produces_a_bold_span()
    {
        var content = Render("This is **bold** text.");

        Assert.Contains(Spans(content),
            s => s.Text == "bold" && s.FontAttributes == FontAttributes.Bold);
    }

    [Fact]
    public void Italic_text_produces_an_italic_span()
    {
        var content = Render("This is *italic* text.");

        Assert.Contains(Spans(content),
            s => s.Text == "italic" && s.FontAttributes == FontAttributes.Italic);
    }

    [Fact]
    public void Strikethrough_produces_a_strikethrough_span()
    {
        var content = Render("This is ~~struck~~ text.");

        Assert.Contains(Spans(content),
            s => s.Text == "struck" && s.TextDecorations == TextDecorations.Strikethrough);
    }

    // A paragraph with multiple bold spans — including a whole-paragraph bold line and a slash
    // inside emphasis — must render each one bold. Guards the "renders as plain text" class of
    // report: when the content itself is correct, bold works (the real cause of that report was
    // leading indentation — see BlockRenderingTests.Indented_content_is_treated_as_a_code_block).
    [Fact]
    public void Paragraph_with_multiple_bold_spans_renders_each_bold()
    {
        const string md =
            "**Why .NET MAUI is great**\n\n" +
            "It builds **native cross-platform apps** from a **single C# codebase**, " +
            "with **hot reload** and a rich set of **controls/layouts**.";

        var boldTexts = Spans(Render(md))
            .Where(s => s.FontAttributes == FontAttributes.Bold)
            .Select(s => s.Text)
            .ToList();

        Assert.Contains("Why .NET MAUI is great", boldTexts);
        Assert.Contains("native cross-platform apps", boldTexts);
        Assert.Contains("single C# codebase", boldTexts);
        Assert.Contains("controls/layouts", boldTexts);
    }
}
