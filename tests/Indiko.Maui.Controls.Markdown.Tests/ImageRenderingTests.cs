using Microsoft.Maui.Controls;
using Xunit;
using static Indiko.Maui.Controls.Markdown.Tests.MarkdownTestHarness;

namespace Indiko.Maui.Controls.Markdown.Tests;

public class ImageRenderingTests
{
    // Regression for "images overflow the screen width": a standalone (image-only) paragraph must
    // place the image in a Star-width column so it is measured against the available width and
    // AspectFit scales large images down, instead of an Auto column that keeps the native size.
    [Fact]
    public void Standalone_image_uses_a_star_width_column()
    {
        var grid = Grids(Render("![alt](photo.png)")).First();

        Assert.Contains(grid.ColumnDefinitions, c => c.Width.IsStar);
    }

    // The counterpart: an image inline with text must keep Auto columns so it stays small and does
    // not disrupt the line flow.
    [Fact]
    public void Inline_image_with_text_uses_auto_width_columns()
    {
        var grid = Grids(Render("text ![alt](icon.png){width=16} more text")).First();

        Assert.All(grid.ColumnDefinitions, c => Assert.True(c.Width.IsAuto));
        Assert.DoesNotContain(grid.ColumnDefinitions, c => c.Width.IsStar);
    }
}
