using System.ComponentModel;

namespace Indiko.Maui.Controls.Markdown.Theming;

/// <summary>
/// Defines the color palette for a MarkdownView theme.
/// Contains all color properties organized into logical groups similar to MudBlazor's Palette approach.
/// </summary>
public class MarkdownPalette : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, string propertyName)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #region Primary Colors

    private Color _primary = Color.FromArgb("#512BD4");
    /// <summary>
    /// Primary accent color used for hyperlinks and highlights.
    /// </summary>
    public Color Primary
    {
        get => _primary;
        set => SetProperty(ref _primary, value, nameof(Primary));
    }

    private Color _secondary = Color.FromArgb("#DFD8F7");
    /// <summary>
    /// Secondary accent color.
    /// </summary>
    public Color Secondary
    {
        get => _secondary;
        set => SetProperty(ref _secondary, value, nameof(Secondary));
    }

    #endregion

    #region Background Colors

    private Color _background = Colors.White;
    /// <summary>
    /// Default background color.
    /// </summary>
    public Color Background
    {
        get => _background;
        set => SetProperty(ref _background, value, nameof(Background));
    }

    private Color _surface = Color.FromArgb("#F5F5F5");
    /// <summary>
    /// Surface color for cards, code blocks, and elevated elements.
    /// </summary>
    public Color Surface
    {
        get => _surface;
        set => SetProperty(ref _surface, value, nameof(Surface));
    }

    #endregion

    #region Text Colors

    private Color _textPrimary = Color.FromArgb("#1A1A1A");
    /// <summary>
    /// Primary text color for body text and paragraphs.
    /// </summary>
    public Color TextPrimary
    {
        get => _textPrimary;
        set => SetProperty(ref _textPrimary, value, nameof(TextPrimary));
    }

    private Color _textSecondary = Color.FromArgb("#4A4A4A");
    /// <summary>
    /// Secondary text color for less prominent text.
    /// </summary>
    public Color TextSecondary
    {
        get => _textSecondary;
        set => SetProperty(ref _textSecondary, value, nameof(TextSecondary));
    }

    private Color _textDisabled = Color.FromArgb("#9E9E9E");
    /// <summary>
    /// Disabled text color.
    /// </summary>
    public Color TextDisabled
    {
        get => _textDisabled;
        set => SetProperty(ref _textDisabled, value, nameof(TextDisabled));
    }

    #endregion

    #region Heading Colors

    private Color _h1Color = Color.FromArgb("#1A1A1A");
    /// <summary>
    /// Color for H1 headings.
    /// </summary>
    public Color H1Color
    {
        get => _h1Color;
        set => SetProperty(ref _h1Color, value, nameof(H1Color));
    }

    private Color _h2Color = Color.FromArgb("#333333");
    /// <summary>
    /// Color for H2 headings.
    /// </summary>
    public Color H2Color
    {
        get => _h2Color;
        set => SetProperty(ref _h2Color, value, nameof(H2Color));
    }

    private Color _h3Color = Color.FromArgb("#4A4A4A");
    /// <summary>
    /// Color for H3 headings.
    /// </summary>
    public Color H3Color
    {
        get => _h3Color;
        set => SetProperty(ref _h3Color, value, nameof(H3Color));
    }

    private Color _h4Color = Color.FromArgb("#555555");
    /// <summary>
    /// Color for H4 headings.
    /// </summary>
    public Color H4Color
    {
        get => _h4Color;
        set => SetProperty(ref _h4Color, value, nameof(H4Color));
    }

    private Color _h5Color = Color.FromArgb("#666666");
    /// <summary>
    /// Color for H5 headings.
    /// </summary>
    public Color H5Color
    {
        get => _h5Color;
        set => SetProperty(ref _h5Color, value, nameof(H5Color));
    }

    private Color _h6Color = Color.FromArgb("#777777");
    /// <summary>
    /// Color for H6 headings.
    /// </summary>
    public Color H6Color
    {
        get => _h6Color;
        set => SetProperty(ref _h6Color, value, nameof(H6Color));
    }

    /// <summary>
    /// Gets the color for the specified heading level.
    /// </summary>
    /// <param name="level">Heading level (1-6)</param>
    /// <returns>Color for the heading level</returns>
    public Color GetHeadingColor(int level)
    {
        return level switch
        {
            1 => H1Color,
            2 => H2Color,
            3 => H3Color,
            4 => H4Color,
            5 => H5Color,
            6 => H6Color,
            _ => H6Color
        };
    }

    #endregion

    #region Link Colors

    private Color _hyperlinkColor = Color.FromArgb("#512BD4");
    /// <summary>
    /// Color for hyperlinks.
    /// </summary>
    public Color HyperlinkColor
    {
        get => _hyperlinkColor;
        set => SetProperty(ref _hyperlinkColor, value, nameof(HyperlinkColor));
    }

    #endregion

    #region Code Block Colors

    private Color _codeBlockBackground = Color.FromArgb("#F5F5F5");
    /// <summary>
    /// Background color for code blocks.
    /// </summary>
    public Color CodeBlockBackground
    {
        get => _codeBlockBackground;
        set => SetProperty(ref _codeBlockBackground, value, nameof(CodeBlockBackground));
    }

    private Color _codeBlockBorder = Color.FromArgb("#E0E0E0");
    /// <summary>
    /// Border color for code blocks.
    /// </summary>
    public Color CodeBlockBorder
    {
        get => _codeBlockBorder;
        set => SetProperty(ref _codeBlockBorder, value, nameof(CodeBlockBorder));
    }

    private Color _codeBlockText = Color.FromArgb("#D63384");
    /// <summary>
    /// Text color for code blocks.
    /// </summary>
    public Color CodeBlockText
    {
        get => _codeBlockText;
        set => SetProperty(ref _codeBlockText, value, nameof(CodeBlockText));
    }

    #endregion

    #region BlockQuote Colors

    private Color _blockQuoteBackground = Color.FromArgb("#F8F9FA");
    /// <summary>
    /// Background color for block quotes.
    /// </summary>
    public Color BlockQuoteBackground
    {
        get => _blockQuoteBackground;
        set => SetProperty(ref _blockQuoteBackground, value, nameof(BlockQuoteBackground));
    }

    private Color _blockQuoteBorder = Color.FromArgb("#512BD4");
    /// <summary>
    /// Border color for block quotes (the left accent bar).
    /// </summary>
    public Color BlockQuoteBorder
    {
        get => _blockQuoteBorder;
        set => SetProperty(ref _blockQuoteBorder, value, nameof(BlockQuoteBorder));
    }

    private Color _blockQuoteText = Color.FromArgb("#4A4A4A");
    /// <summary>
    /// Text color for block quotes.
    /// </summary>
    public Color BlockQuoteText
    {
        get => _blockQuoteText;
        set => SetProperty(ref _blockQuoteText, value, nameof(BlockQuoteText));
    }

    #endregion

    #region Table Colors

    private Color _tableHeaderBackground = Color.FromArgb("#E8E8E8");
    /// <summary>
    /// Background color for table headers.
    /// </summary>
    public Color TableHeaderBackground
    {
        get => _tableHeaderBackground;
        set => SetProperty(ref _tableHeaderBackground, value, nameof(TableHeaderBackground));
    }

    private Color _tableHeaderText = Color.FromArgb("#1A1A1A");
    /// <summary>
    /// Text color for table headers.
    /// </summary>
    public Color TableHeaderText
    {
        get => _tableHeaderText;
        set => SetProperty(ref _tableHeaderText, value, nameof(TableHeaderText));
    }

    private Color _tableRowBackground = Colors.White;
    /// <summary>
    /// Background color for table rows.
    /// </summary>
    public Color TableRowBackground
    {
        get => _tableRowBackground;
        set => SetProperty(ref _tableRowBackground, value, nameof(TableRowBackground));
    }

    private Color _tableRowText = Color.FromArgb("#1A1A1A");
    /// <summary>
    /// Text color for table rows.
    /// </summary>
    public Color TableRowText
    {
        get => _tableRowText;
        set => SetProperty(ref _tableRowText, value, nameof(TableRowText));
    }

    private Color _tableBorder = Color.FromArgb("#CCCCCC");
    /// <summary>
    /// Border color for table cells.
    /// </summary>
    public Color TableBorder
    {
        get => _tableBorder;
        set => SetProperty(ref _tableBorder, value, nameof(TableBorder));
    }

    #endregion

    #region Divider/Line Colors

    private Color _dividerColor = Color.FromArgb("#E0E0E0");
    /// <summary>
    /// Color for horizontal rules and dividers.
    /// </summary>
    public Color DividerColor
    {
        get => _dividerColor;
        set => SetProperty(ref _dividerColor, value, nameof(DividerColor));
    }

    #endregion

    #region Alert/Container Colors

    private Color _infoColor = Color.FromArgb("#2196F3");
    /// <summary>
    /// Color for info alerts/containers.
    /// </summary>
    public Color InfoColor
    {
        get => _infoColor;
        set => SetProperty(ref _infoColor, value, nameof(InfoColor));
    }

    private Color _warningColor = Color.FromArgb("#FF9800");
    /// <summary>
    /// Color for warning alerts/containers.
    /// </summary>
    public Color WarningColor
    {
        get => _warningColor;
        set => SetProperty(ref _warningColor, value, nameof(WarningColor));
    }

    private Color _errorColor = Color.FromArgb("#F44336");
    /// <summary>
    /// Color for error/danger alerts/containers.
    /// </summary>
    public Color ErrorColor
    {
        get => _errorColor;
        set => SetProperty(ref _errorColor, value, nameof(ErrorColor));
    }

    private Color _successColor = Color.FromArgb("#4CAF50");
    /// <summary>
    /// Color for success alerts/containers.
    /// </summary>
    public Color SuccessColor
    {
        get => _successColor;
        set => SetProperty(ref _successColor, value, nameof(SuccessColor));
    }

    #endregion

    /// <summary>
    /// Creates a deep copy of this palette.
    /// </summary>
    public virtual MarkdownPalette Clone()
    {
        return new MarkdownPalette
        {
            Primary = Primary,
            Secondary = Secondary,
            Background = Background,
            Surface = Surface,
            TextPrimary = TextPrimary,
            TextSecondary = TextSecondary,
            TextDisabled = TextDisabled,
            H1Color = H1Color,
            H2Color = H2Color,
            H3Color = H3Color,
            H4Color = H4Color,
            H5Color = H5Color,
            H6Color = H6Color,
            HyperlinkColor = HyperlinkColor,
            CodeBlockBackground = CodeBlockBackground,
            CodeBlockBorder = CodeBlockBorder,
            CodeBlockText = CodeBlockText,
            BlockQuoteBackground = BlockQuoteBackground,
            BlockQuoteBorder = BlockQuoteBorder,
            BlockQuoteText = BlockQuoteText,
            TableHeaderBackground = TableHeaderBackground,
            TableHeaderText = TableHeaderText,
            TableRowBackground = TableRowBackground,
            TableRowText = TableRowText,
            TableBorder = TableBorder,
            DividerColor = DividerColor,
            InfoColor = InfoColor,
            WarningColor = WarningColor,
            ErrorColor = ErrorColor,
            SuccessColor = SuccessColor
        };
    }
}

/// <summary>
/// Dark theme palette with inverted colors suitable for dark mode.
/// </summary>
public class MarkdownPaletteDark : MarkdownPalette
{
    public MarkdownPaletteDark()
    {
        // Primary Colors - brighter for dark backgrounds
        Primary = Color.FromArgb("#B39DDB");
        Secondary = Color.FromArgb("#7C4DFF");

        // Background Colors - dark variants
        Background = Color.FromArgb("#121212");
        Surface = Color.FromArgb("#1E1E1E");

        // Text Colors - light text on dark background
        TextPrimary = Color.FromArgb("#FFFFFF");
        TextSecondary = Color.FromArgb("#B0B0B0");
        TextDisabled = Color.FromArgb("#666666");

        // Heading Colors
        H1Color = Color.FromArgb("#FFFFFF");
        H2Color = Color.FromArgb("#E0E0E0");
        H3Color = Color.FromArgb("#B0B0B0");
        H4Color = Color.FromArgb("#A0A0A0");
        H5Color = Color.FromArgb("#909090");
        H6Color = Color.FromArgb("#808080");

        // Link Colors
        HyperlinkColor = Color.FromArgb("#B39DDB");

        // Code Block Colors
        CodeBlockBackground = Color.FromArgb("#2D2D2D");
        CodeBlockBorder = Color.FromArgb("#404040");
        CodeBlockText = Color.FromArgb("#F48FB1");

        // BlockQuote Colors
        BlockQuoteBackground = Color.FromArgb("#252525");
        BlockQuoteBorder = Color.FromArgb("#B39DDB");
        BlockQuoteText = Color.FromArgb("#B0B0B0");

        // Table Colors
        TableHeaderBackground = Color.FromArgb("#2D2D2D");
        TableHeaderText = Color.FromArgb("#FFFFFF");
        TableRowBackground = Color.FromArgb("#1E1E1E");
        TableRowText = Color.FromArgb("#E0E0E0");
        TableBorder = Color.FromArgb("#404040");

        // Divider Color
        DividerColor = Color.FromArgb("#404040");

        // Alert Colors - slightly adjusted for dark backgrounds
        InfoColor = Color.FromArgb("#64B5F6");
        WarningColor = Color.FromArgb("#FFB74D");
        ErrorColor = Color.FromArgb("#EF5350");
        SuccessColor = Color.FromArgb("#81C784");
    }

    public override MarkdownPalette Clone()
    {
        var clone = new MarkdownPaletteDark();
        // Copy all properties
        clone.Primary = Primary;
        clone.Secondary = Secondary;
        clone.Background = Background;
        clone.Surface = Surface;
        clone.TextPrimary = TextPrimary;
        clone.TextSecondary = TextSecondary;
        clone.TextDisabled = TextDisabled;
        clone.H1Color = H1Color;
        clone.H2Color = H2Color;
        clone.H3Color = H3Color;
        clone.H4Color = H4Color;
        clone.H5Color = H5Color;
        clone.H6Color = H6Color;
        clone.HyperlinkColor = HyperlinkColor;
        clone.CodeBlockBackground = CodeBlockBackground;
        clone.CodeBlockBorder = CodeBlockBorder;
        clone.CodeBlockText = CodeBlockText;
        clone.BlockQuoteBackground = BlockQuoteBackground;
        clone.BlockQuoteBorder = BlockQuoteBorder;
        clone.BlockQuoteText = BlockQuoteText;
        clone.TableHeaderBackground = TableHeaderBackground;
        clone.TableHeaderText = TableHeaderText;
        clone.TableRowBackground = TableRowBackground;
        clone.TableRowText = TableRowText;
        clone.TableBorder = TableBorder;
        clone.DividerColor = DividerColor;
        clone.InfoColor = InfoColor;
        clone.WarningColor = WarningColor;
        clone.ErrorColor = ErrorColor;
        clone.SuccessColor = SuccessColor;
        return clone;
    }
}
