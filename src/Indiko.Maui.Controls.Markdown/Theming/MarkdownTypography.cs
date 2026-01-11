using System.ComponentModel;

namespace Indiko.Maui.Controls.Markdown.Theming;

/// <summary>
/// Defines typography settings for a MarkdownView theme.
/// Contains font sizes, font families, and line heights for all text elements.
/// </summary>
public class MarkdownTypography : INotifyPropertyChanged
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

    #region Font Families

    private string? _defaultFontFamily;
    /// <summary>
    /// Default font family for body text.
    /// </summary>
    public string? DefaultFontFamily
    {
        get => _defaultFontFamily;
        set => SetProperty(ref _defaultFontFamily, value, nameof(DefaultFontFamily));
    }

    private string? _headingFontFamily;
    /// <summary>
    /// Default font family for all headings. Individual heading font families override this if set.
    /// </summary>
    public string? HeadingFontFamily
    {
        get => _headingFontFamily;
        set => SetProperty(ref _headingFontFamily, value, nameof(HeadingFontFamily));
    }

    private string _codeFontFamily = "Consolas";
    /// <summary>
    /// Font family for code blocks and inline code.
    /// </summary>
    public string CodeFontFamily
    {
        get => _codeFontFamily;
        set => SetProperty(ref _codeFontFamily, value, nameof(CodeFontFamily));
    }

    private string _blockQuoteFontFamily = "Consolas";
    /// <summary>
    /// Font family for block quotes.
    /// </summary>
    public string BlockQuoteFontFamily
    {
        get => _blockQuoteFontFamily;
        set => SetProperty(ref _blockQuoteFontFamily, value, nameof(BlockQuoteFontFamily));
    }

    #endregion

    #region H1 Settings

    private double _h1FontSize = 28d;
    /// <summary>
    /// Font size for H1 headings.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double H1FontSize
    {
        get => _h1FontSize;
        set => SetProperty(ref _h1FontSize, value, nameof(H1FontSize));
    }

    private string? _h1FontFamily;
    /// <summary>
    /// Font family for H1 headings. If null, uses HeadingFontFamily or DefaultFontFamily.
    /// </summary>
    public string? H1FontFamily
    {
        get => _h1FontFamily;
        set => SetProperty(ref _h1FontFamily, value, nameof(H1FontFamily));
    }

    private FontAttributes _h1FontAttributes = FontAttributes.Bold;
    /// <summary>
    /// Font attributes (Bold, Italic, None) for H1 headings.
    /// </summary>
    public FontAttributes H1FontAttributes
    {
        get => _h1FontAttributes;
        set => SetProperty(ref _h1FontAttributes, value, nameof(H1FontAttributes));
    }

    #endregion

    #region H2 Settings

    private double _h2FontSize = 24d;
    /// <summary>
    /// Font size for H2 headings.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double H2FontSize
    {
        get => _h2FontSize;
        set => SetProperty(ref _h2FontSize, value, nameof(H2FontSize));
    }

    private string? _h2FontFamily;
    /// <summary>
    /// Font family for H2 headings. If null, uses HeadingFontFamily or DefaultFontFamily.
    /// </summary>
    public string? H2FontFamily
    {
        get => _h2FontFamily;
        set => SetProperty(ref _h2FontFamily, value, nameof(H2FontFamily));
    }

    private FontAttributes _h2FontAttributes = FontAttributes.Bold;
    /// <summary>
    /// Font attributes (Bold, Italic, None) for H2 headings.
    /// </summary>
    public FontAttributes H2FontAttributes
    {
        get => _h2FontAttributes;
        set => SetProperty(ref _h2FontAttributes, value, nameof(H2FontAttributes));
    }

    #endregion

    #region H3 Settings

    private double _h3FontSize = 20d;
    /// <summary>
    /// Font size for H3 headings.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double H3FontSize
    {
        get => _h3FontSize;
        set => SetProperty(ref _h3FontSize, value, nameof(H3FontSize));
    }

    private string? _h3FontFamily;
    /// <summary>
    /// Font family for H3 headings. If null, uses HeadingFontFamily or DefaultFontFamily.
    /// </summary>
    public string? H3FontFamily
    {
        get => _h3FontFamily;
        set => SetProperty(ref _h3FontFamily, value, nameof(H3FontFamily));
    }

    private FontAttributes _h3FontAttributes = FontAttributes.Bold;
    /// <summary>
    /// Font attributes (Bold, Italic, None) for H3 headings.
    /// </summary>
    public FontAttributes H3FontAttributes
    {
        get => _h3FontAttributes;
        set => SetProperty(ref _h3FontAttributes, value, nameof(H3FontAttributes));
    }

    #endregion

    #region H4 Settings

    private double _h4FontSize = 18d;
    /// <summary>
    /// Font size for H4 headings.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double H4FontSize
    {
        get => _h4FontSize;
        set => SetProperty(ref _h4FontSize, value, nameof(H4FontSize));
    }

    private string? _h4FontFamily;
    /// <summary>
    /// Font family for H4 headings. If null, uses HeadingFontFamily or DefaultFontFamily.
    /// </summary>
    public string? H4FontFamily
    {
        get => _h4FontFamily;
        set => SetProperty(ref _h4FontFamily, value, nameof(H4FontFamily));
    }

    private FontAttributes _h4FontAttributes = FontAttributes.Bold;
    /// <summary>
    /// Font attributes (Bold, Italic, None) for H4 headings.
    /// </summary>
    public FontAttributes H4FontAttributes
    {
        get => _h4FontAttributes;
        set => SetProperty(ref _h4FontAttributes, value, nameof(H4FontAttributes));
    }

    #endregion

    #region H5 Settings

    private double _h5FontSize = 16d;
    /// <summary>
    /// Font size for H5 headings.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double H5FontSize
    {
        get => _h5FontSize;
        set => SetProperty(ref _h5FontSize, value, nameof(H5FontSize));
    }

    private string? _h5FontFamily;
    /// <summary>
    /// Font family for H5 headings. If null, uses HeadingFontFamily or DefaultFontFamily.
    /// </summary>
    public string? H5FontFamily
    {
        get => _h5FontFamily;
        set => SetProperty(ref _h5FontFamily, value, nameof(H5FontFamily));
    }

    private FontAttributes _h5FontAttributes = FontAttributes.Bold;
    /// <summary>
    /// Font attributes (Bold, Italic, None) for H5 headings.
    /// </summary>
    public FontAttributes H5FontAttributes
    {
        get => _h5FontAttributes;
        set => SetProperty(ref _h5FontAttributes, value, nameof(H5FontAttributes));
    }

    #endregion

    #region H6 Settings

    private double _h6FontSize = 14d;
    /// <summary>
    /// Font size for H6 headings.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double H6FontSize
    {
        get => _h6FontSize;
        set => SetProperty(ref _h6FontSize, value, nameof(H6FontSize));
    }

    private string? _h6FontFamily;
    /// <summary>
    /// Font family for H6 headings. If null, uses HeadingFontFamily or DefaultFontFamily.
    /// </summary>
    public string? H6FontFamily
    {
        get => _h6FontFamily;
        set => SetProperty(ref _h6FontFamily, value, nameof(H6FontFamily));
    }

    private FontAttributes _h6FontAttributes = FontAttributes.Bold;
    /// <summary>
    /// Font attributes (Bold, Italic, None) for H6 headings.
    /// </summary>
    public FontAttributes H6FontAttributes
    {
        get => _h6FontAttributes;
        set => SetProperty(ref _h6FontAttributes, value, nameof(H6FontAttributes));
    }

    #endregion

    #region Body Font Sizes

    private double _bodyFontSize = 14d;
    /// <summary>
    /// Default font size for body text and paragraphs.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double BodyFontSize
    {
        get => _bodyFontSize;
        set => SetProperty(ref _bodyFontSize, value, nameof(BodyFontSize));
    }

    private double _codeFontSize = 13d;
    /// <summary>
    /// Font size for code blocks.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double CodeFontSize
    {
        get => _codeFontSize;
        set => SetProperty(ref _codeFontSize, value, nameof(CodeFontSize));
    }

    #endregion

    #region Table Font Sizes

    private double _tableHeaderFontSize = 14d;
    /// <summary>
    /// Font size for table headers.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double TableHeaderFontSize
    {
        get => _tableHeaderFontSize;
        set => SetProperty(ref _tableHeaderFontSize, value, nameof(TableHeaderFontSize));
    }

    private double _tableRowFontSize = 13d;
    /// <summary>
    /// Font size for table rows.
    /// </summary>
    [TypeConverter(typeof(FontSizeConverter))]
    public double TableRowFontSize
    {
        get => _tableRowFontSize;
        set => SetProperty(ref _tableRowFontSize, value, nameof(TableRowFontSize));
    }

    #endregion

    #region Line Heights

    private double _lineHeight = 1.5;
    /// <summary>
    /// Line height multiplier for body text.
    /// </summary>
    public double LineHeight
    {
        get => _lineHeight;
        set => SetProperty(ref _lineHeight, value, nameof(LineHeight));
    }

    private double _headingLineHeight = 1.3;
    /// <summary>
    /// Line height multiplier for headings.
    /// </summary>
    public double HeadingLineHeight
    {
        get => _headingLineHeight;
        set => SetProperty(ref _headingLineHeight, value, nameof(HeadingLineHeight));
    }

    #endregion

    #region Spacing

    private double _paragraphSpacing = 1.0;
    /// <summary>
    /// Spacing multiplier between paragraphs.
    /// </summary>
    public double ParagraphSpacing
    {
        get => _paragraphSpacing;
        set => SetProperty(ref _paragraphSpacing, value, nameof(ParagraphSpacing));
    }

    private double _listItemSpacing = 4d;
    /// <summary>
    /// Spacing between list items in pixels.
    /// </summary>
    public double ListItemSpacing
    {
        get => _listItemSpacing;
        set => SetProperty(ref _listItemSpacing, value, nameof(ListItemSpacing));
    }

    private double _listIndent = 20d;
    /// <summary>
    /// Indentation for nested lists in pixels.
    /// </summary>
    public double ListIndent
    {
        get => _listIndent;
        set => SetProperty(ref _listIndent, value, nameof(ListIndent));
    }

    #endregion

    #region Line Break Modes

    private LineBreakMode _textLineBreakMode = LineBreakMode.WordWrap;
    /// <summary>
    /// Line break mode for body text.
    /// </summary>
    public LineBreakMode TextLineBreakMode
    {
        get => _textLineBreakMode;
        set => SetProperty(ref _textLineBreakMode, value, nameof(TextLineBreakMode));
    }

    private LineBreakMode _headingLineBreakMode = LineBreakMode.TailTruncation;
    /// <summary>
    /// Line break mode for headings.
    /// </summary>
    public LineBreakMode HeadingLineBreakMode
    {
        get => _headingLineBreakMode;
        set => SetProperty(ref _headingLineBreakMode, value, nameof(HeadingLineBreakMode));
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Gets the font size for the specified heading level.
    /// </summary>
    /// <param name="level">Heading level (1-6)</param>
    /// <returns>Font size for the heading level</returns>
    public double GetHeadingFontSize(int level)
    {
        return level switch
        {
            1 => H1FontSize,
            2 => H2FontSize,
            3 => H3FontSize,
            4 => H4FontSize,
            5 => H5FontSize,
            6 => H6FontSize,
            _ => H6FontSize
        };
    }

    /// <summary>
    /// Gets the font family for the specified heading level.
    /// Falls back to HeadingFontFamily, then DefaultFontFamily if not set.
    /// </summary>
    /// <param name="level">Heading level (1-6)</param>
    /// <returns>Font family for the heading level</returns>
    public string? GetHeadingFontFamily(int level)
    {
        var specificFamily = level switch
        {
            1 => H1FontFamily,
            2 => H2FontFamily,
            3 => H3FontFamily,
            4 => H4FontFamily,
            5 => H5FontFamily,
            6 => H6FontFamily,
            _ => null
        };

        return specificFamily ?? HeadingFontFamily ?? DefaultFontFamily;
    }

    /// <summary>
    /// Gets the effective font family for headings (legacy method for backward compatibility).
    /// </summary>
    public string? GetHeadingFontFamily()
    {
        return HeadingFontFamily ?? DefaultFontFamily;
    }

    /// <summary>
    /// Gets the font attributes for the specified heading level.
    /// </summary>
    /// <param name="level">Heading level (1-6)</param>
    /// <returns>Font attributes for the heading level</returns>
    public FontAttributes GetHeadingFontAttributes(int level)
    {
        return level switch
        {
            1 => H1FontAttributes,
            2 => H2FontAttributes,
            3 => H3FontAttributes,
            4 => H4FontAttributes,
            5 => H5FontAttributes,
            6 => H6FontAttributes,
            _ => FontAttributes.Bold
        };
    }

    #endregion

    /// <summary>
    /// Creates a deep copy of this typography settings.
    /// </summary>
    public MarkdownTypography Clone()
    {
        return new MarkdownTypography
        {
            DefaultFontFamily = DefaultFontFamily,
            HeadingFontFamily = HeadingFontFamily,
            CodeFontFamily = CodeFontFamily,
            BlockQuoteFontFamily = BlockQuoteFontFamily,
            H1FontSize = H1FontSize,
            H1FontFamily = H1FontFamily,
            H1FontAttributes = H1FontAttributes,
            H2FontSize = H2FontSize,
            H2FontFamily = H2FontFamily,
            H2FontAttributes = H2FontAttributes,
            H3FontSize = H3FontSize,
            H3FontFamily = H3FontFamily,
            H3FontAttributes = H3FontAttributes,
            H4FontSize = H4FontSize,
            H4FontFamily = H4FontFamily,
            H4FontAttributes = H4FontAttributes,
            H5FontSize = H5FontSize,
            H5FontFamily = H5FontFamily,
            H5FontAttributes = H5FontAttributes,
            H6FontSize = H6FontSize,
            H6FontFamily = H6FontFamily,
            H6FontAttributes = H6FontAttributes,
            BodyFontSize = BodyFontSize,
            CodeFontSize = CodeFontSize,
            TableHeaderFontSize = TableHeaderFontSize,
            TableRowFontSize = TableRowFontSize,
            LineHeight = LineHeight,
            HeadingLineHeight = HeadingLineHeight,
            ParagraphSpacing = ParagraphSpacing,
            ListItemSpacing = ListItemSpacing,
            ListIndent = ListIndent,
            TextLineBreakMode = TextLineBreakMode,
            HeadingLineBreakMode = HeadingLineBreakMode
        };
    }
}
