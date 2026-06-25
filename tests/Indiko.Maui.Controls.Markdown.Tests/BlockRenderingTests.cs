using Microsoft.Maui.Controls;
using Xunit;
using static Indiko.Maui.Controls.Markdown.Tests.MarkdownTestHarness;

namespace Indiko.Maui.Controls.Markdown.Tests;

public class BlockRenderingTests
{
    [Fact]
    public void Heading_uses_the_configured_heading_font_size()
    {
        var label = Labels(Render("# Title")).First();
        var span = label.FormattedText!.Spans.First();

        Assert.Equal(24d, span.FontSize); // H1FontSize default
    }

    [Fact]
    public void Headings_word_wrap_by_default()
    {
        var label = Labels(Render("# A very long heading that should wrap")).First();

        Assert.Equal(LineBreakMode.WordWrap, label.LineBreakMode);
    }

    [Fact]
    public void Headings_truncate_when_line_breaks_are_disallowed()
    {
        var view = new MarkdownView { AllowLineBreaksOnHeadlines = false, MarkdownText = "# A very long heading" };
        var label = Labels(view.Content!).First();

        // Falls back to LineBreakModeHeader, which defaults to TailTruncation.
        Assert.Equal(LineBreakMode.TailTruncation, label.LineBreakMode);
    }

    [Fact]
    public void Disallowed_line_breaks_respect_a_custom_LineBreakModeHeader()
    {
        var view = new MarkdownView
        {
            AllowLineBreaksOnHeadlines = false,
            LineBreakModeHeader = LineBreakMode.HeadTruncation,
            MarkdownText = "# A very long heading",
        };
        var label = Labels(view.Content!).First();

        Assert.Equal(LineBreakMode.HeadTruncation, label.LineBreakMode);
    }

    // Documents the gotcha behind the "renders as plain text" report: content indented by four
    // spaces is an indented code block, so the literal **markers** are shown and nothing is bold.
    [Fact]
    public void Indented_content_is_treated_as_a_code_block_not_bold()
    {
        var content = Render("    **not bold** because indented");

        Assert.DoesNotContain(Spans(content), s => s.FontAttributes == FontAttributes.Bold);
    }

    [Fact]
    public void Text_horizontal_alignment_is_applied_to_body_labels()
    {
        var view = new MarkdownView
        {
            TextHorizontalTextAlignment = TextAlignment.Justify,
            MarkdownText = "Some paragraph text."
        };

        var label = Labels(view.Content!).First();

        Assert.Equal(TextAlignment.Justify, label.HorizontalTextAlignment);
    }
}
