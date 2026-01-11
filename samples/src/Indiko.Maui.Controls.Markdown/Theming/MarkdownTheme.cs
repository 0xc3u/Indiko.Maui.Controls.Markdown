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
}
