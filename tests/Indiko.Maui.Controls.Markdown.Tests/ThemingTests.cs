using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Indiko.Maui.Controls.Markdown.Theming;
using Xunit;
using static Indiko.Maui.Controls.Markdown.Tests.MarkdownTestHarness;

namespace Indiko.Maui.Controls.Markdown.Tests;

/// <summary>
/// Regression tests for the theming bridge: setting a <see cref="MarkdownTheme"/> must copy the
/// palette values into the individual bindable properties the renderer reads. Covers the colors
/// that were wired up most recently (HighlightColor and the alert accents).
/// </summary>
public class ThemingTests
{
    private static readonly Color RebeccaPurple = Color.FromArgb("#663399");

    private static MarkdownTheme ThemeWith(Action<MarkdownPalette> configure)
    {
        var theme = MarkdownThemeDefaults.GitHub.Clone();
        configure(theme.Palette);
        return theme;
    }

    [Fact]
    public void Theme_palette_highlight_color_flows_to_the_property()
    {
        var view = new MarkdownView { Theme = ThemeWith(p => p.HighlightColor = Colors.Orange) };

        AssertColor(Colors.Orange, view.HighlightColor);
    }

    [Fact]
    public void Theme_highlight_color_is_used_when_rendering_marked_text()
    {
        var view = new MarkdownView
        {
            Theme = ThemeWith(p => p.HighlightColor = Colors.Orange),
            MarkdownText = "before ==marked== after",
        };

        var span = Spans(view.Content!).FirstOrDefault(s => s.Text == "marked");
        Assert.NotNull(span);
        AssertColor(Colors.Orange, span!.BackgroundColor);
    }

    [Fact]
    public void Theme_palette_alert_colors_flow_to_the_properties()
    {
        var view = new MarkdownView
        {
            Theme = ThemeWith(p =>
            {
                p.InfoColor = Colors.Teal;
                p.WarningColor = Colors.Goldenrod;
                p.ErrorColor = Colors.Crimson;
                p.SuccessColor = Colors.SeaGreen;
                p.ImportantColor = Colors.Lime;
            }),
        };

        AssertColor(Colors.Teal, view.AlertInfoColor);
        AssertColor(Colors.Goldenrod, view.AlertWarningColor);
        AssertColor(Colors.Crimson, view.AlertErrorColor);
        AssertColor(Colors.SeaGreen, view.AlertSuccessColor);
        AssertColor(Colors.Lime, view.AlertImportantColor);
    }

    [Fact]
    public void Theme_heading_and_text_colors_flow_to_the_properties()
    {
        var view = new MarkdownView
        {
            Theme = ThemeWith(p =>
            {
                p.H1Color = Colors.Red;
                p.TextPrimary = RebeccaPurple;
                p.HyperlinkColor = Colors.DeepPink;
            }),
        };

        AssertColor(Colors.Red, view.H1Color);
        AssertColor(RebeccaPurple, view.TextColor);
        AssertColor(Colors.DeepPink, view.HyperlinkColor);
    }

    [Fact]
    public void Cloning_a_theme_copies_the_highlight_and_alert_colors()
    {
        var original = MarkdownThemeDefaults.GitHub.Clone();
        original.Palette.HighlightColor = Colors.Orange;
        original.Palette.ImportantColor = Colors.Lime;

        var clone = original.Clone();

        AssertColor(Colors.Orange, clone.Palette.HighlightColor);
        AssertColor(Colors.Lime, clone.Palette.ImportantColor);
    }

    [Fact]
    public void Theme_palette_image_popup_colors_flow_to_the_properties()
    {
        var view = new MarkdownView
        {
            Theme = ThemeWith(p =>
            {
                p.ImagePopupBackground = Colors.Navy;
                p.ImagePopupCloseButton = Colors.Gold;
            }),
        };

        AssertColor(Colors.Navy, view.ImagePopupBackgroundColor);
        AssertColor(Colors.Gold, view.ImagePopupCloseButtonColor);
    }

    [Fact]
    public void Image_popup_palette_colors_default_to_black_and_white()
    {
        var view = new MarkdownView { Theme = MarkdownThemeDefaults.GitHub.Clone() };

        AssertColor(Colors.Black, view.ImagePopupBackgroundColor);
        AssertColor(Colors.White, view.ImagePopupCloseButtonColor);
    }

    [Fact]
    public void Cloning_a_theme_copies_the_image_popup_colors()
    {
        var original = MarkdownThemeDefaults.GitHub.Clone();
        original.Palette.ImagePopupBackground = Colors.Navy;
        original.PaletteDark.ImagePopupCloseButton = Colors.Gold;

        var clone = original.Clone();

        AssertColor(Colors.Navy, clone.Palette.ImagePopupBackground);
        AssertColor(Colors.Gold, clone.PaletteDark.ImagePopupCloseButton);
    }
}
