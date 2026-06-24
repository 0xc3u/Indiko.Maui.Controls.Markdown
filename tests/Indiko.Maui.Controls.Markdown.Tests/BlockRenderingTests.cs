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
