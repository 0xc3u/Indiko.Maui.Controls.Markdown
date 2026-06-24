using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Xunit;

namespace Indiko.Maui.Controls.Markdown.Tests;

/// <summary>
/// Helpers to render markdown through a real <see cref="MarkdownView"/> and inspect the resulting
/// MAUI view tree. <see cref="MarkdownView"/> builds its content synchronously when MarkdownText is
/// set, so text/structure assertions need no dispatcher or running app host.
/// </summary>
internal static class MarkdownTestHarness
{
    /// <summary>Renders the markdown and returns the root content view the control produced.</summary>
    public static View Render(string markdown)
    {
        var view = new MarkdownView { MarkdownText = markdown };
        return view.Content
            ?? throw new InvalidOperationException("MarkdownView produced no content.");
    }

    /// <summary>Depth-first enumeration of every element in a rendered view tree.</summary>
    public static IEnumerable<Element> Descendants(Element root)
    {
        yield return root;

        IEnumerable<Element> children = root switch
        {
            Layout layout => layout.Children.OfType<Element>(),
            ContentView cv when cv.Content is not null => new Element[] { cv.Content },
            Border border when border.Content is Element be => new[] { be },
            _ => Enumerable.Empty<Element>()
        };

        foreach (var child in children)
            foreach (var descendant in Descendants(child))
                yield return descendant;
    }

    public static IEnumerable<Label> Labels(Element root) => Descendants(root).OfType<Label>();

    public static IEnumerable<Grid> Grids(Element root) => Descendants(root).OfType<Grid>();

    /// <summary>All spans across all labels in the tree, in document order.</summary>
    public static IEnumerable<Span> Spans(Element root) =>
        Labels(root).Where(l => l.FormattedText is not null)
                    .SelectMany(l => l.FormattedText.Spans);

    /// <summary>Asserts two colors are equal by RGBA components (avoids reference-equality pitfalls).</summary>
    public static void AssertColor(Color expected, Color? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.Red, actual.Red, 3);
        Assert.Equal(expected.Green, actual.Green, 3);
        Assert.Equal(expected.Blue, actual.Blue, 3);
        Assert.Equal(expected.Alpha, actual.Alpha, 3);
    }
}
