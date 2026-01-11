using System.ComponentModel;

namespace Indiko.Maui.Controls.Markdown.Theming;

/// <summary>
/// Represents a complete theme for the MarkdownView control.
/// Combines color palette and typography settings similar to MudBlazor's MudTheme approach.
/// </summary>
public class MarkdownTheme : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private MarkdownPalette _palette;
    /// <summary>
    /// The color palette for this theme.
    /// </summary>
    public MarkdownPalette Palette
    {
        get => _palette;
        set
        {
            if (_palette != value)
            {
                if (_palette != null)
                    _palette.PropertyChanged -= OnPalettePropertyChanged;

                _palette = value;

                if (_palette != null)
                    _palette.PropertyChanged += OnPalettePropertyChanged;

                OnPropertyChanged(nameof(Palette));
            }
        }
    }

    private MarkdownPalette _paletteDark;
    /// <summary>
    /// The color palette for dark mode. If null, uses the light palette.
    /// </summary>
    public MarkdownPalette PaletteDark
    {
        get => _paletteDark;
        set
        {
            if (_paletteDark != value)
            {
                if (_paletteDark != null)
                    _paletteDark.PropertyChanged -= OnPalettePropertyChanged;

                _paletteDark = value;

                if (_paletteDark != null)
                    _paletteDark.PropertyChanged += OnPalettePropertyChanged;

                OnPropertyChanged(nameof(PaletteDark));
            }
        }
    }

    private MarkdownTypography _typography;
    /// <summary>
    /// Typography settings for this theme.
    /// </summary>
    public MarkdownTypography Typography
    {
        get => _typography;
        set
        {
            if (_typography != value)
            {
                if (_typography != null)
                    _typography.PropertyChanged -= OnTypographyPropertyChanged;

                _typography = value;

                if (_typography != null)
                    _typography.PropertyChanged += OnTypographyPropertyChanged;

                OnPropertyChanged(nameof(Typography));
            }
        }
    }

    private Aspect _imageAspect = Aspect.AspectFit;
    /// <summary>
    /// Default aspect ratio for images.
    /// </summary>
    public Aspect ImageAspect
    {
        get => _imageAspect;
        set
        {
            if (_imageAspect != value)
            {
                _imageAspect = value;
                OnPropertyChanged(nameof(ImageAspect));
            }
        }
    }

    private double _defaultImageWidth = 200d;
    /// <summary>
    /// Default width for images when no explicit size is specified.
    /// </summary>
    public double DefaultImageWidth
    {
        get => _defaultImageWidth;
        set
        {
            if (Math.Abs(_defaultImageWidth - value) > 0.01)
            {
                _defaultImageWidth = value;
                OnPropertyChanged(nameof(DefaultImageWidth));
            }
        }
    }

    private double _defaultImageHeight = 200d;
    /// <summary>
    /// Default height for images when no explicit size is specified.
    /// </summary>
    public double DefaultImageHeight
    {
        get => _defaultImageHeight;
        set
        {
            if (Math.Abs(_defaultImageHeight - value) > 0.01)
            {
                _defaultImageHeight = value;
                OnPropertyChanged(nameof(DefaultImageHeight));
            }
        }
    }

    /// <summary>
    /// Creates a new theme with default light mode settings.
    /// </summary>
    public MarkdownTheme()
    {
        _palette = new MarkdownPalette();
        _paletteDark = new MarkdownPaletteDark();
        _typography = new MarkdownTypography();

        _palette.PropertyChanged += OnPalettePropertyChanged;
        _paletteDark.PropertyChanged += OnPalettePropertyChanged;
        _typography.PropertyChanged += OnTypographyPropertyChanged;
    }

    private void OnPalettePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged($"Palette.{e.PropertyName}");
    }

    private void OnTypographyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged($"Typography.{e.PropertyName}");
    }

    /// <summary>
    /// Gets the appropriate palette based on the current app theme.
    /// </summary>
    /// <param name="appTheme">The current app theme</param>
    /// <returns>The palette for the specified theme</returns>
    public MarkdownPalette GetPalette(AppTheme appTheme)
    {
        return appTheme == AppTheme.Dark ? (PaletteDark ?? Palette) : Palette;
    }

    /// <summary>
    /// Creates a deep copy of this theme.
    /// </summary>
    public MarkdownTheme Clone()
    {
        return new MarkdownTheme
        {
            Palette = Palette.Clone(),
            PaletteDark = PaletteDark?.Clone() ?? new MarkdownPaletteDark(),
            Typography = Typography.Clone(),
            ImageAspect = ImageAspect,
            DefaultImageWidth = DefaultImageWidth,
            DefaultImageHeight = DefaultImageHeight
        };
    }
}

/// <summary>
/// Provides built-in theme presets for MarkdownView.
/// </summary>
public static class MarkdownThemeDefaults
{
    /// <summary>
    /// Creates the default light theme.
    /// </summary>
    public static MarkdownTheme Light => new();

    /// <summary>
    /// Creates a theme optimized for dark mode only.
    /// </summary>
    public static MarkdownTheme Dark
    {
        get
        {
            var theme = new MarkdownTheme();
            theme.Palette = new MarkdownPaletteDark();
            theme.PaletteDark = new MarkdownPaletteDark();
            return theme;
        }
    }

    /// <summary>
    /// Creates a high-contrast theme for accessibility.
    /// </summary>
    public static MarkdownTheme HighContrast
    {
        get
        {
            var theme = new MarkdownTheme();

            // Light mode - high contrast
            theme.Palette.TextPrimary = Colors.Black;
            theme.Palette.Background = Colors.White;
            theme.Palette.H1Color = Colors.Black;
            theme.Palette.H2Color = Colors.Black;
            theme.Palette.H3Color = Colors.Black;
            theme.Palette.HyperlinkColor = Color.FromArgb("#0000EE");

            // Dark mode - high contrast
            theme.PaletteDark.TextPrimary = Colors.White;
            theme.PaletteDark.Background = Colors.Black;
            theme.PaletteDark.H1Color = Colors.White;
            theme.PaletteDark.H2Color = Colors.White;
            theme.PaletteDark.H3Color = Colors.White;
            theme.PaletteDark.HyperlinkColor = Color.FromArgb("#FFFF00");

            return theme;
        }
    }

    /// <summary>
    /// Creates a compact theme with reduced font sizes and spacing.
    /// </summary>
    public static MarkdownTheme Compact
    {
        get
        {
            var theme = new MarkdownTheme();

            theme.Typography.H1FontSize = 22d;
            theme.Typography.H2FontSize = 18d;
            theme.Typography.H3FontSize = 16d;
            theme.Typography.H4FontSize = 14d;
            theme.Typography.H5FontSize = 13d;
            theme.Typography.H6FontSize = 12d;
            theme.Typography.BodyFontSize = 12d;
            theme.Typography.CodeFontSize = 11d;
            theme.Typography.TableHeaderFontSize = 12d;
            theme.Typography.TableRowFontSize = 11d;
            theme.Typography.LineHeight = 1.3;
            theme.Typography.ParagraphSpacing = 0.75;
            theme.Typography.ListItemSpacing = 2d;
            theme.Typography.ListIndent = 16d;

            return theme;
        }
    }

    /// <summary>
    /// Creates a sepia-toned theme for comfortable reading.
    /// </summary>
    public static MarkdownTheme Sepia
    {
        get
        {
            var theme = new MarkdownTheme();

            // Sepia palette
            theme.Palette.Background = Color.FromArgb("#F4ECD8");
            theme.Palette.Surface = Color.FromArgb("#EAE0CC");
            theme.Palette.TextPrimary = Color.FromArgb("#5C4B37");
            theme.Palette.TextSecondary = Color.FromArgb("#7A6A56");
            theme.Palette.H1Color = Color.FromArgb("#3D2E1F");
            theme.Palette.H2Color = Color.FromArgb("#4A3C2B");
            theme.Palette.H3Color = Color.FromArgb("#5C4B37");
            theme.Palette.HyperlinkColor = Color.FromArgb("#8B4513");
            theme.Palette.CodeBlockBackground = Color.FromArgb("#EAE0CC");
            theme.Palette.CodeBlockText = Color.FromArgb("#704214");
            theme.Palette.BlockQuoteBackground = Color.FromArgb("#E8DCC8");
            theme.Palette.BlockQuoteBorder = Color.FromArgb("#8B4513");
            theme.Palette.DividerColor = Color.FromArgb("#C9B896");

            // Dark sepia for dark mode
            theme.PaletteDark.Background = Color.FromArgb("#2D2416");
            theme.PaletteDark.Surface = Color.FromArgb("#3D3121");
            theme.PaletteDark.TextPrimary = Color.FromArgb("#E8DCC8");
            theme.PaletteDark.TextSecondary = Color.FromArgb("#C9B896");
            theme.PaletteDark.H1Color = Color.FromArgb("#F4ECD8");
            theme.PaletteDark.H2Color = Color.FromArgb("#E8DCC8");
            theme.PaletteDark.H3Color = Color.FromArgb("#D4C4A8");
            theme.PaletteDark.HyperlinkColor = Color.FromArgb("#DEB887");

            return theme;
        }
    }

    /// <summary>
    /// Creates a GitHub-styled theme.
    /// </summary>
    public static MarkdownTheme GitHub
    {
        get
        {
            var theme = new MarkdownTheme();

            // GitHub light palette
            theme.Palette.Background = Colors.White;
            theme.Palette.Surface = Color.FromArgb("#F6F8FA");
            theme.Palette.TextPrimary = Color.FromArgb("#24292F");
            theme.Palette.TextSecondary = Color.FromArgb("#57606A");
            theme.Palette.H1Color = Color.FromArgb("#24292F");
            theme.Palette.H2Color = Color.FromArgb("#24292F");
            theme.Palette.H3Color = Color.FromArgb("#24292F");
            theme.Palette.HyperlinkColor = Color.FromArgb("#0969DA");
            theme.Palette.CodeBlockBackground = Color.FromArgb("#F6F8FA");
            theme.Palette.CodeBlockBorder = Color.FromArgb("#D0D7DE");
            theme.Palette.CodeBlockText = Color.FromArgb("#24292F");
            theme.Palette.BlockQuoteBackground = Colors.White;
            theme.Palette.BlockQuoteBorder = Color.FromArgb("#D0D7DE");
            theme.Palette.BlockQuoteText = Color.FromArgb("#57606A");
            theme.Palette.DividerColor = Color.FromArgb("#D8DEE4");

            // GitHub dark palette
            theme.PaletteDark.Background = Color.FromArgb("#0D1117");
            theme.PaletteDark.Surface = Color.FromArgb("#161B22");
            theme.PaletteDark.TextPrimary = Color.FromArgb("#C9D1D9");
            theme.PaletteDark.TextSecondary = Color.FromArgb("#8B949E");
            theme.PaletteDark.H1Color = Color.FromArgb("#C9D1D9");
            theme.PaletteDark.H2Color = Color.FromArgb("#C9D1D9");
            theme.PaletteDark.H3Color = Color.FromArgb("#C9D1D9");
            theme.PaletteDark.HyperlinkColor = Color.FromArgb("#58A6FF");
            theme.PaletteDark.CodeBlockBackground = Color.FromArgb("#161B22");
            theme.PaletteDark.CodeBlockBorder = Color.FromArgb("#30363D");
            theme.PaletteDark.CodeBlockText = Color.FromArgb("#C9D1D9");
            theme.PaletteDark.BlockQuoteBackground = Color.FromArgb("#0D1117");
            theme.PaletteDark.BlockQuoteBorder = Color.FromArgb("#30363D");
            theme.PaletteDark.BlockQuoteText = Color.FromArgb("#8B949E");
            theme.PaletteDark.DividerColor = Color.FromArgb("#21262D");

            return theme;
        }
    }

    /// <summary>
    /// Creates a .NET Purple brand theme.
    /// </summary>
    public static MarkdownTheme DotNetPurple
    {
        get
        {
            var theme = new MarkdownTheme();

            // .NET Purple light palette
            theme.Palette.Primary = Color.FromArgb("#512BD4");
            theme.Palette.Secondary = Color.FromArgb("#DFD8F7");
            theme.Palette.HyperlinkColor = Color.FromArgb("#512BD4");
            theme.Palette.BlockQuoteBorder = Color.FromArgb("#512BD4");
            theme.Palette.CodeBlockText = Color.FromArgb("#512BD4");

            // .NET Purple dark palette
            theme.PaletteDark.Primary = Color.FromArgb("#B39DDB");
            theme.PaletteDark.Secondary = Color.FromArgb("#7C4DFF");
            theme.PaletteDark.HyperlinkColor = Color.FromArgb("#B39DDB");
            theme.PaletteDark.BlockQuoteBorder = Color.FromArgb("#B39DDB");
            theme.PaletteDark.CodeBlockText = Color.FromArgb("#CE93D8");

            return theme;
        }
    }

    /// <summary>
    /// Creates a One Dark theme inspired by Atom's One Dark syntax theme.
    /// </summary>
    public static MarkdownTheme OneDark
    {
        get
        {
            var theme = new MarkdownTheme();

            // One Dark palette (dark theme by default)
            theme.Palette.Background = Color.FromArgb("#282C34");
            theme.Palette.Surface = Color.FromArgb("#21252B");
            theme.Palette.TextPrimary = Color.FromArgb("#ABB2BF");
            theme.Palette.TextSecondary = Color.FromArgb("#5C6370");
            theme.Palette.H1Color = Color.FromArgb("#E06C75");
            theme.Palette.H2Color = Color.FromArgb("#E5C07B");
            theme.Palette.H3Color = Color.FromArgb("#61AFEF");
            theme.Palette.HyperlinkColor = Color.FromArgb("#61AFEF");
            theme.Palette.CodeBlockBackground = Color.FromArgb("#21252B");
            theme.Palette.CodeBlockBorder = Color.FromArgb("#3E4451");
            theme.Palette.CodeBlockText = Color.FromArgb("#98C379");
            theme.Palette.BlockQuoteBackground = Color.FromArgb("#21252B");
            theme.Palette.BlockQuoteBorder = Color.FromArgb("#56B6C2");
            theme.Palette.BlockQuoteText = Color.FromArgb("#5C6370");
            theme.Palette.TableHeaderBackground = Color.FromArgb("#21252B");
            theme.Palette.TableHeaderText = Color.FromArgb("#E5C07B");
            theme.Palette.TableRowBackground = Color.FromArgb("#282C34");
            theme.Palette.TableRowText = Color.FromArgb("#ABB2BF");
            theme.Palette.DividerColor = Color.FromArgb("#3E4451");
            theme.Palette.InfoColor = Color.FromArgb("#61AFEF");
            theme.Palette.WarningColor = Color.FromArgb("#E5C07B");
            theme.Palette.ErrorColor = Color.FromArgb("#E06C75");
            theme.Palette.SuccessColor = Color.FromArgb("#98C379");

            // Copy to dark palette (One Dark is primarily a dark theme)
            theme.PaletteDark.Background = theme.Palette.Background;
            theme.PaletteDark.Surface = theme.Palette.Surface;
            theme.PaletteDark.TextPrimary = theme.Palette.TextPrimary;
            theme.PaletteDark.TextSecondary = theme.Palette.TextSecondary;
            theme.PaletteDark.H1Color = theme.Palette.H1Color;
            theme.PaletteDark.H2Color = theme.Palette.H2Color;
            theme.PaletteDark.H3Color = theme.Palette.H3Color;
            theme.PaletteDark.HyperlinkColor = theme.Palette.HyperlinkColor;
            theme.PaletteDark.CodeBlockBackground = theme.Palette.CodeBlockBackground;
            theme.PaletteDark.CodeBlockBorder = theme.Palette.CodeBlockBorder;
            theme.PaletteDark.CodeBlockText = theme.Palette.CodeBlockText;
            theme.PaletteDark.BlockQuoteBackground = theme.Palette.BlockQuoteBackground;
            theme.PaletteDark.BlockQuoteBorder = theme.Palette.BlockQuoteBorder;
            theme.PaletteDark.BlockQuoteText = theme.Palette.BlockQuoteText;
            theme.PaletteDark.TableHeaderBackground = theme.Palette.TableHeaderBackground;
            theme.PaletteDark.TableHeaderText = theme.Palette.TableHeaderText;
            theme.PaletteDark.TableRowBackground = theme.Palette.TableRowBackground;
            theme.PaletteDark.TableRowText = theme.Palette.TableRowText;
            theme.PaletteDark.DividerColor = theme.Palette.DividerColor;

            return theme;
        }
    }

    /// <summary>
    /// Creates a One Light theme inspired by Atom's One Light syntax theme.
    /// </summary>
    public static MarkdownTheme OneLight
    {
        get
        {
            var theme = new MarkdownTheme();

            // One Light palette
            theme.Palette.Background = Color.FromArgb("#FAFAFA");
            theme.Palette.Surface = Color.FromArgb("#F0F0F0");
            theme.Palette.TextPrimary = Color.FromArgb("#383A42");
            theme.Palette.TextSecondary = Color.FromArgb("#A0A1A7");
            theme.Palette.H1Color = Color.FromArgb("#E45649");
            theme.Palette.H2Color = Color.FromArgb("#C18401");
            theme.Palette.H3Color = Color.FromArgb("#4078F2");
            theme.Palette.HyperlinkColor = Color.FromArgb("#4078F2");
            theme.Palette.CodeBlockBackground = Color.FromArgb("#F0F0F0");
            theme.Palette.CodeBlockBorder = Color.FromArgb("#D4D4D4");
            theme.Palette.CodeBlockText = Color.FromArgb("#50A14F");
            theme.Palette.BlockQuoteBackground = Color.FromArgb("#F0F0F0");
            theme.Palette.BlockQuoteBorder = Color.FromArgb("#0184BC");
            theme.Palette.BlockQuoteText = Color.FromArgb("#A0A1A7");
            theme.Palette.TableHeaderBackground = Color.FromArgb("#E5E5E6");
            theme.Palette.TableHeaderText = Color.FromArgb("#C18401");
            theme.Palette.TableRowBackground = Color.FromArgb("#FAFAFA");
            theme.Palette.TableRowText = Color.FromArgb("#383A42");
            theme.Palette.DividerColor = Color.FromArgb("#D4D4D4");
            theme.Palette.InfoColor = Color.FromArgb("#4078F2");
            theme.Palette.WarningColor = Color.FromArgb("#C18401");
            theme.Palette.ErrorColor = Color.FromArgb("#E45649");
            theme.Palette.SuccessColor = Color.FromArgb("#50A14F");

            // One Dark for dark mode
            theme.PaletteDark.Background = Color.FromArgb("#282C34");
            theme.PaletteDark.Surface = Color.FromArgb("#21252B");
            theme.PaletteDark.TextPrimary = Color.FromArgb("#ABB2BF");
            theme.PaletteDark.TextSecondary = Color.FromArgb("#5C6370");
            theme.PaletteDark.H1Color = Color.FromArgb("#E06C75");
            theme.PaletteDark.H2Color = Color.FromArgb("#E5C07B");
            theme.PaletteDark.H3Color = Color.FromArgb("#61AFEF");
            theme.PaletteDark.HyperlinkColor = Color.FromArgb("#61AFEF");
            theme.PaletteDark.CodeBlockBackground = Color.FromArgb("#21252B");
            theme.PaletteDark.CodeBlockBorder = Color.FromArgb("#3E4451");
            theme.PaletteDark.CodeBlockText = Color.FromArgb("#98C379");
            theme.PaletteDark.BlockQuoteBackground = Color.FromArgb("#21252B");
            theme.PaletteDark.BlockQuoteBorder = Color.FromArgb("#56B6C2");
            theme.PaletteDark.BlockQuoteText = Color.FromArgb("#5C6370");
            theme.PaletteDark.TableHeaderBackground = Color.FromArgb("#21252B");
            theme.PaletteDark.TableHeaderText = Color.FromArgb("#E5C07B");
            theme.PaletteDark.TableRowBackground = Color.FromArgb("#282C34");
            theme.PaletteDark.TableRowText = Color.FromArgb("#ABB2BF");
            theme.PaletteDark.DividerColor = Color.FromArgb("#3E4451");

            return theme;
        }
    }

    /// <summary>
    /// Creates a Dracula theme inspired by the popular Dracula color scheme.
    /// </summary>
    public static MarkdownTheme Dracula
    {
        get
        {
            var theme = new MarkdownTheme();

            // Dracula palette
            theme.Palette.Background = Color.FromArgb("#282A36");
            theme.Palette.Surface = Color.FromArgb("#44475A");
            theme.Palette.TextPrimary = Color.FromArgb("#F8F8F2");
            theme.Palette.TextSecondary = Color.FromArgb("#6272A4");
            theme.Palette.H1Color = Color.FromArgb("#FF79C6");
            theme.Palette.H2Color = Color.FromArgb("#BD93F9");
            theme.Palette.H3Color = Color.FromArgb("#8BE9FD");
            theme.Palette.HyperlinkColor = Color.FromArgb("#8BE9FD");
            theme.Palette.CodeBlockBackground = Color.FromArgb("#44475A");
            theme.Palette.CodeBlockBorder = Color.FromArgb("#6272A4");
            theme.Palette.CodeBlockText = Color.FromArgb("#50FA7B");
            theme.Palette.BlockQuoteBackground = Color.FromArgb("#44475A");
            theme.Palette.BlockQuoteBorder = Color.FromArgb("#BD93F9");
            theme.Palette.BlockQuoteText = Color.FromArgb("#6272A4");
            theme.Palette.TableHeaderBackground = Color.FromArgb("#44475A");
            theme.Palette.TableHeaderText = Color.FromArgb("#FF79C6");
            theme.Palette.TableRowBackground = Color.FromArgb("#282A36");
            theme.Palette.TableRowText = Color.FromArgb("#F8F8F2");
            theme.Palette.DividerColor = Color.FromArgb("#6272A4");
            theme.Palette.InfoColor = Color.FromArgb("#8BE9FD");
            theme.Palette.WarningColor = Color.FromArgb("#FFB86C");
            theme.Palette.ErrorColor = Color.FromArgb("#FF5555");
            theme.Palette.SuccessColor = Color.FromArgb("#50FA7B");

            // Copy to dark palette (Dracula is primarily a dark theme)
            theme.PaletteDark.Background = theme.Palette.Background;
            theme.PaletteDark.Surface = theme.Palette.Surface;
            theme.PaletteDark.TextPrimary = theme.Palette.TextPrimary;
            theme.PaletteDark.TextSecondary = theme.Palette.TextSecondary;
            theme.PaletteDark.H1Color = theme.Palette.H1Color;
            theme.PaletteDark.H2Color = theme.Palette.H2Color;
            theme.PaletteDark.H3Color = theme.Palette.H3Color;
            theme.PaletteDark.HyperlinkColor = theme.Palette.HyperlinkColor;
            theme.PaletteDark.CodeBlockBackground = theme.Palette.CodeBlockBackground;
            theme.PaletteDark.CodeBlockBorder = theme.Palette.CodeBlockBorder;
            theme.PaletteDark.CodeBlockText = theme.Palette.CodeBlockText;
            theme.PaletteDark.BlockQuoteBackground = theme.Palette.BlockQuoteBackground;
            theme.PaletteDark.BlockQuoteBorder = theme.Palette.BlockQuoteBorder;
            theme.PaletteDark.BlockQuoteText = theme.Palette.BlockQuoteText;
            theme.PaletteDark.TableHeaderBackground = theme.Palette.TableHeaderBackground;
            theme.PaletteDark.TableHeaderText = theme.Palette.TableHeaderText;
            theme.PaletteDark.TableRowBackground = theme.Palette.TableRowBackground;
            theme.PaletteDark.TableRowText = theme.Palette.TableRowText;
            theme.PaletteDark.DividerColor = theme.Palette.DividerColor;

            return theme;
        }
    }

    /// <summary>
    /// Creates a Nord theme inspired by the Nord color palette.
    /// </summary>
    public static MarkdownTheme Nord
    {
        get
        {
            var theme = new MarkdownTheme();

            // Nord light (Snow Storm + Frost)
            theme.Palette.Background = Color.FromArgb("#ECEFF4");
            theme.Palette.Surface = Color.FromArgb("#E5E9F0");
            theme.Palette.TextPrimary = Color.FromArgb("#2E3440");
            theme.Palette.TextSecondary = Color.FromArgb("#4C566A");
            theme.Palette.H1Color = Color.FromArgb("#5E81AC");
            theme.Palette.H2Color = Color.FromArgb("#81A1C1");
            theme.Palette.H3Color = Color.FromArgb("#88C0D0");
            theme.Palette.HyperlinkColor = Color.FromArgb("#5E81AC");
            theme.Palette.CodeBlockBackground = Color.FromArgb("#E5E9F0");
            theme.Palette.CodeBlockBorder = Color.FromArgb("#D8DEE9");
            theme.Palette.CodeBlockText = Color.FromArgb("#A3BE8C");
            theme.Palette.BlockQuoteBackground = Color.FromArgb("#E5E9F0");
            theme.Palette.BlockQuoteBorder = Color.FromArgb("#88C0D0");
            theme.Palette.BlockQuoteText = Color.FromArgb("#4C566A");
            theme.Palette.TableHeaderBackground = Color.FromArgb("#D8DEE9");
            theme.Palette.TableHeaderText = Color.FromArgb("#5E81AC");
            theme.Palette.TableRowBackground = Color.FromArgb("#ECEFF4");
            theme.Palette.TableRowText = Color.FromArgb("#2E3440");
            theme.Palette.DividerColor = Color.FromArgb("#D8DEE9");
            theme.Palette.InfoColor = Color.FromArgb("#5E81AC");
            theme.Palette.WarningColor = Color.FromArgb("#EBCB8B");
            theme.Palette.ErrorColor = Color.FromArgb("#BF616A");
            theme.Palette.SuccessColor = Color.FromArgb("#A3BE8C");

            // Nord dark (Polar Night)
            theme.PaletteDark.Background = Color.FromArgb("#2E3440");
            theme.PaletteDark.Surface = Color.FromArgb("#3B4252");
            theme.PaletteDark.TextPrimary = Color.FromArgb("#ECEFF4");
            theme.PaletteDark.TextSecondary = Color.FromArgb("#D8DEE9");
            theme.PaletteDark.H1Color = Color.FromArgb("#88C0D0");
            theme.PaletteDark.H2Color = Color.FromArgb("#81A1C1");
            theme.PaletteDark.H3Color = Color.FromArgb("#5E81AC");
            theme.PaletteDark.HyperlinkColor = Color.FromArgb("#88C0D0");
            theme.PaletteDark.CodeBlockBackground = Color.FromArgb("#3B4252");
            theme.PaletteDark.CodeBlockBorder = Color.FromArgb("#4C566A");
            theme.PaletteDark.CodeBlockText = Color.FromArgb("#A3BE8C");
            theme.PaletteDark.BlockQuoteBackground = Color.FromArgb("#3B4252");
            theme.PaletteDark.BlockQuoteBorder = Color.FromArgb("#88C0D0");
            theme.PaletteDark.BlockQuoteText = Color.FromArgb("#D8DEE9");
            theme.PaletteDark.TableHeaderBackground = Color.FromArgb("#3B4252");
            theme.PaletteDark.TableHeaderText = Color.FromArgb("#88C0D0");
            theme.PaletteDark.TableRowBackground = Color.FromArgb("#2E3440");
            theme.PaletteDark.TableRowText = Color.FromArgb("#ECEFF4");
            theme.PaletteDark.DividerColor = Color.FromArgb("#4C566A");

            return theme;
        }
    }
}
