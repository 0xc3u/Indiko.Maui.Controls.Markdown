using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;
using Indiko.Maui.Controls.Markdown.Theming;
using Markdig;
using Markdig.Extensions.CustomContainers;
using Markdig.Extensions.Mathematics;
using Markdig.Extensions.Tables;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Maui.Controls.Shapes;
using SkiaSharp;
using Svg.Skia;
using Image = Microsoft.Maui.Controls.Image;

namespace Indiko.Maui.Controls.Markdown;

public sealed class MarkdownView : ContentView
{
    private static readonly Regex EmailRegex = new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled);

    public static readonly BindableProperty MarkdownTextProperty =
        BindableProperty.Create(nameof(MarkdownText), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string MarkdownText
    {
        get => (string)GetValue(MarkdownTextProperty);
        set => SetValue(MarkdownTextProperty, value);
    }

    private readonly Thickness _defaultListIndent = new(10, 0, 10, 0);
    private Dictionary<string, ImageSource> _imageCache = [];
    private bool _isApplyingTheme;

    /* **************** Theme Support ***********************/

    public static readonly BindableProperty ThemeProperty =
        BindableProperty.Create(nameof(Theme), typeof(MarkdownTheme), typeof(MarkdownView), 
            propertyChanged: OnThemeChanged);

    /// <summary>
    /// Gets or sets the theme for the MarkdownView. When set, the theme's palette and typography
    /// settings will be applied to the control. Individual property settings will be overridden
    /// by the theme unless Theme is set to null.
    /// </summary>
    public MarkdownTheme Theme
    {
        get => (MarkdownTheme)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly BindableProperty UseAppThemeProperty =
        BindableProperty.Create(nameof(UseAppTheme), typeof(bool), typeof(MarkdownView), false, 
            propertyChanged: OnUseAppThemeChanged);

    /// <summary>
    /// When true, automatically switches between light and dark palettes based on the system theme.
    /// Requires a Theme with both Palette (light) and PaletteDark to be set.
    /// </summary>
    public bool UseAppTheme
    {
        get => (bool)GetValue(UseAppThemeProperty);
        set => SetValue(UseAppThemeProperty, value);
    }

    private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MarkdownView view)
        {
            // Unsubscribe from old theme's property changes
            if (oldValue is MarkdownTheme oldTheme)
            {
                oldTheme.PropertyChanged -= view.OnThemePropertyChanged;
            }

            // Subscribe to new theme's property changes
            if (newValue is MarkdownTheme newTheme)
            {
                newTheme.PropertyChanged += view.OnThemePropertyChanged;
                view.ApplyTheme();
            }
        }
    }

    private static void OnUseAppThemeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MarkdownView view && newValue is bool useAppTheme)
        {
            try
            {
                if (Application.Current == null)
                    return;

                // Always unsubscribe first to avoid duplicate subscriptions
                Application.Current.RequestedThemeChanged -= view.OnSystemThemeChanged;

                if (useAppTheme)
                {
                    Application.Current.RequestedThemeChanged += view.OnSystemThemeChanged;
                    view.ApplyTheme();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting UseAppTheme: {ex.Message}");
            }
        }
    }

    private void OnThemePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        ApplyTheme();
    }

    private void OnSystemThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyTheme();
    }

    /// <summary>
    /// Applies the current theme to all styling properties.
    /// </summary>
    public void ApplyTheme()
    {
        if (Theme == null)
            return;

        _isApplyingTheme = true;
        try
        {
            // Determine which palette to use based on UseAppTheme and current system theme
            var currentAppTheme = UseAppTheme && Application.Current != null
                ? Application.Current.RequestedTheme
                : AppTheme.Light;

            var palette = Theme.GetPalette(currentAppTheme);
            var typography = Theme.Typography;

            // Apply palette colors
            TextColor = palette.TextPrimary;
            H1Color = palette.H1Color;
            H2Color = palette.H2Color;
            H3Color = palette.H3Color;
            H4Color = palette.H4Color;
            H5Color = palette.H5Color;
            H6Color = palette.H6Color;
            HyperlinkColor = palette.HyperlinkColor;
            LineColor = palette.DividerColor;

            // Code block
            CodeBlockBackgroundColor = palette.CodeBlockBackground;
            CodeBlockBorderColor = palette.CodeBlockBorder;
            CodeBlockTextColor = palette.CodeBlockText;

            // Block quote
            BlockQuoteBackgroundColor = palette.BlockQuoteBackground;
            BlockQuoteBorderColor = palette.BlockQuoteBorder;
            BlockQuoteTextColor = palette.BlockQuoteText;

            // Table
            TableHeaderBackgroundColor = palette.TableHeaderBackground;
            TableHeaderTextColor = palette.TableHeaderText;
            TableRowBackgroundColor = palette.TableRowBackground;
            TableRowTextColor = palette.TableRowText;

            // Apply typography settings
            TextFontFace = typography.DefaultFontFamily;
            TextFontSize = typography.BodyFontSize;
            
            // Heading font sizes
            H1FontSize = typography.H1FontSize;
            H2FontSize = typography.H2FontSize;
            H3FontSize = typography.H3FontSize;
            H4FontSize = typography.H4FontSize;
            H5FontSize = typography.H5FontSize;
            H6FontSize = typography.H6FontSize;
            
            // Heading font families
            H1FontFamily = typography.H1FontFamily ?? typography.HeadingFontFamily ?? typography.DefaultFontFamily;
            H2FontFamily = typography.H2FontFamily ?? typography.HeadingFontFamily ?? typography.DefaultFontFamily;
            H3FontFamily = typography.H3FontFamily ?? typography.HeadingFontFamily ?? typography.DefaultFontFamily;
            H4FontFamily = typography.H4FontFamily ?? typography.HeadingFontFamily ?? typography.DefaultFontFamily;
            H5FontFamily = typography.H5FontFamily ?? typography.HeadingFontFamily ?? typography.DefaultFontFamily;
            H6FontFamily = typography.H6FontFamily ?? typography.HeadingFontFamily ?? typography.DefaultFontFamily;
            
            // Heading font attributes
            H1FontAttributes = typography.H1FontAttributes;
            H2FontAttributes = typography.H2FontAttributes;
            H3FontAttributes = typography.H3FontAttributes;
            H4FontAttributes = typography.H4FontAttributes;
            H5FontAttributes = typography.H5FontAttributes;
            H6FontAttributes = typography.H6FontAttributes;
            
            CodeBlockFontSize = typography.CodeFontSize;
            CodeBlockFontFace = typography.CodeFontFamily;
            BlockQuoteFontFace = typography.BlockQuoteFontFamily;
            TableHeaderFontSize = typography.TableHeaderFontSize;
            TableRowFontSize = typography.TableRowFontSize;
            LineHeightMultiplier = typography.LineHeight;
            ParagraphSpacing = typography.ParagraphSpacing;
            LineBreakModeText = typography.TextLineBreakMode;
            LineBreakModeHeader = typography.HeadingLineBreakMode;

            // Image settings from theme
            ImageAspect = Theme.ImageAspect;
            DefaultImageWidth = Theme.DefaultImageWidth;
            DefaultImageHeight = Theme.DefaultImageHeight;
        }
        finally
        {
            _isApplyingTheme = false;
        }

        // Re-render markdown after theme is applied
        if (!string.IsNullOrEmpty(MarkdownText))
        {
            RenderMarkdown(MarkdownText);
        }
    }

    public delegate void HyperLinkClicked(object sender, LinkEventArgs e);
    public event HyperLinkClicked OnHyperLinkClicked;

    public delegate void EmailClickedEventHandler(object sender, EmailEventArgs e);
    public event EmailClickedEventHandler OnEmailClicked;

    public static readonly BindableProperty LineBreakModeTextProperty =
       BindableProperty.Create(nameof(LineBreakModeText), typeof(LineBreakMode), typeof(MarkdownView), LineBreakMode.WordWrap, propertyChanged: OnMarkdownTextChanged);

    public LineBreakMode LineBreakModeText
    {
        get => (LineBreakMode)GetValue(LineBreakModeTextProperty);
        set => SetValue(LineBreakModeTextProperty, value);
    }

    public static readonly BindableProperty LineBreakModeHeaderProperty =
       BindableProperty.Create(nameof(LineBreakModeHeader), typeof(LineBreakMode), typeof(MarkdownView), LineBreakMode.TailTruncation, propertyChanged: OnMarkdownTextChanged);

    public LineBreakMode LineBreakModeHeader
    {
        get => (LineBreakMode)GetValue(LineBreakModeHeaderProperty);
        set => SetValue(LineBreakModeHeaderProperty, value);
    }

    public static readonly BindableProperty H1ColorProperty =
        BindableProperty.Create(nameof(H1Color), typeof(Color), typeof(MarkdownView), Colors.Black, propertyChanged: OnMarkdownTextChanged);

    public Color H1Color
    {
        get => (Color)GetValue(H1ColorProperty);
        set => SetValue(H1ColorProperty, value);
    }

    public static readonly BindableProperty H1ColorLightProperty =
        BindableProperty.Create(nameof(H1ColorLight), typeof(Color), typeof(MarkdownView), Colors.Black);

    public Color H1ColorLight
    {
        get => (Color)GetValue(H1ColorLightProperty);
        set => SetValue(H1ColorLightProperty, value);
    }

    public static readonly BindableProperty H1ColorDarkProperty =
        BindableProperty.Create(nameof(H1ColorDark), typeof(Color), typeof(MarkdownView), Colors.White);

    public Color H1ColorDark
    {
        get => (Color)GetValue(H1ColorDarkProperty);
        set => SetValue(H1ColorDarkProperty, value);
    }

    public static readonly BindableProperty H1FontSizeProperty =
      BindableProperty.Create(nameof(H1FontSize), typeof(double), typeof(MarkdownView), defaultValue: 24d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H1FontSize
    {
        get => (double)GetValue(H1FontSizeProperty);
        set => SetValue(H1FontSizeProperty, value);
    }

    public static readonly BindableProperty H1FontFamilyProperty =
        BindableProperty.Create(nameof(H1FontFamily), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string H1FontFamily
    {
        get => (string)GetValue(H1FontFamilyProperty);
        set => SetValue(H1FontFamilyProperty, value);
    }

    public static readonly BindableProperty H1FontAttributesProperty =
        BindableProperty.Create(nameof(H1FontAttributes), typeof(FontAttributes), typeof(MarkdownView), FontAttributes.Bold, propertyChanged: OnMarkdownTextChanged);

    public FontAttributes H1FontAttributes
    {
        get => (FontAttributes)GetValue(H1FontAttributesProperty);
        set => SetValue(H1FontAttributesProperty, value);
    }

    public static readonly BindableProperty H2ColorProperty =
        BindableProperty.Create(nameof(H2Color), typeof(Color), typeof(MarkdownView), Colors.DarkGray, propertyChanged: OnMarkdownTextChanged);

    public Color H2Color
    {
        get => (Color)GetValue(H2ColorProperty);
        set => SetValue(H2ColorProperty, value);
    }

    public static readonly BindableProperty H2ColorLightProperty =
        BindableProperty.Create(nameof(H2ColorLight), typeof(Color), typeof(MarkdownView), Colors.DarkGray);

    public Color H2ColorLight
    {
        get => (Color)GetValue(H2ColorLightProperty);
        set => SetValue(H2ColorLightProperty, value);
    }

    public static readonly BindableProperty H2ColorDarkProperty =
        BindableProperty.Create(nameof(H2ColorDark), typeof(Color), typeof(MarkdownView), Colors.DarkGray);

    public Color H2ColorDark
    {
        get => (Color)GetValue(H2ColorDarkProperty);
        set => SetValue(H2ColorDarkProperty, value);
    }

    public static readonly BindableProperty H2FontSizeProperty =
     BindableProperty.Create(nameof(H2FontSize), typeof(double), typeof(MarkdownView), defaultValue: 20d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H2FontSize
    {
        get => (double)GetValue(H2FontSizeProperty);
        set => SetValue(H2FontSizeProperty, value);
    }

    public static readonly BindableProperty H2FontFamilyProperty =
        BindableProperty.Create(nameof(H2FontFamily), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string H2FontFamily
    {
        get => (string)GetValue(H2FontFamilyProperty);
        set => SetValue(H2FontFamilyProperty, value);
    }

    public static readonly BindableProperty H2FontAttributesProperty =
        BindableProperty.Create(nameof(H2FontAttributes), typeof(FontAttributes), typeof(MarkdownView), FontAttributes.Bold, propertyChanged: OnMarkdownTextChanged);

    public FontAttributes H2FontAttributes
    {
        get => (FontAttributes)GetValue(H2FontAttributesProperty);
        set => SetValue(H2FontAttributesProperty, value);
    }

    // H3Color property
    public static readonly BindableProperty H3ColorProperty =
        BindableProperty.Create(nameof(H3Color), typeof(Color), typeof(MarkdownView), Colors.Gray, propertyChanged: OnMarkdownTextChanged);

    public Color H3Color
    {
        get => (Color)GetValue(H3ColorProperty);
        set => SetValue(H3ColorProperty, value);
    }

    public static readonly BindableProperty H3ColorLightProperty =
        BindableProperty.Create(nameof(H3ColorLight), typeof(Color), typeof(MarkdownView), Colors.Gray);

    public Color H3ColorLight
    {
        get => (Color)GetValue(H3ColorLightProperty);
        set => SetValue(H3ColorLightProperty, value);
    }

    public static readonly BindableProperty H3ColorDarkProperty =
        BindableProperty.Create(nameof(H3ColorDark), typeof(Color), typeof(MarkdownView), Colors.Gray);

    public Color H3ColorDark
    {
        get => (Color)GetValue(H3ColorDarkProperty);
        set => SetValue(H3ColorDarkProperty, value);
    }

    public static readonly BindableProperty H3FontSizeProperty =
     BindableProperty.Create(nameof(H3FontSize), typeof(double), typeof(MarkdownView), defaultValue: 18d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H3FontSize
    {
        get => (double)GetValue(H3FontSizeProperty);
        set => SetValue(H3FontSizeProperty, value);
    }

    public static readonly BindableProperty H3FontFamilyProperty =
        BindableProperty.Create(nameof(H3FontFamily), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string H3FontFamily
    {
        get => (string)GetValue(H3FontFamilyProperty);
        set => SetValue(H3FontFamilyProperty, value);
    }

    public static readonly BindableProperty H3FontAttributesProperty =
        BindableProperty.Create(nameof(H3FontAttributes), typeof(FontAttributes), typeof(MarkdownView), FontAttributes.Bold, propertyChanged: OnMarkdownTextChanged);

    public FontAttributes H3FontAttributes
    {
        get => (FontAttributes)GetValue(H3FontAttributesProperty);
        set => SetValue(H3FontAttributesProperty, value);
    }

    /* **** H4 Settings **** */

    public static readonly BindableProperty H4ColorProperty =
        BindableProperty.Create(nameof(H4Color), typeof(Color), typeof(MarkdownView), Colors.DimGray, propertyChanged: OnMarkdownTextChanged);

    public Color H4Color
    {
        get => (Color)GetValue(H4ColorProperty);
        set => SetValue(H4ColorProperty, value);
    }

    public static readonly BindableProperty H4FontSizeProperty =
     BindableProperty.Create(nameof(H4FontSize), typeof(double), typeof(MarkdownView), defaultValue: 16d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H4FontSize
    {
        get => (double)GetValue(H4FontSizeProperty);
        set => SetValue(H4FontSizeProperty, value);
    }

    public static readonly BindableProperty H4FontFamilyProperty =
        BindableProperty.Create(nameof(H4FontFamily), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string H4FontFamily
    {
        get => (string)GetValue(H4FontFamilyProperty);
        set => SetValue(H4FontFamilyProperty, value);
    }

    public static readonly BindableProperty H4FontAttributesProperty =
        BindableProperty.Create(nameof(H4FontAttributes), typeof(FontAttributes), typeof(MarkdownView), FontAttributes.Bold, propertyChanged: OnMarkdownTextChanged);

    public FontAttributes H4FontAttributes
    {
        get => (FontAttributes)GetValue(H4FontAttributesProperty);
        set => SetValue(H4FontAttributesProperty, value);
    }

    /* **** H5 Settings **** */

    public static readonly BindableProperty H5ColorProperty =
        BindableProperty.Create(nameof(H5Color), typeof(Color), typeof(MarkdownView), Colors.DimGray, propertyChanged: OnMarkdownTextChanged);

    public Color H5Color
    {
        get => (Color)GetValue(H5ColorProperty);
        set => SetValue(H5ColorProperty, value);
    }

    public static readonly BindableProperty H5FontSizeProperty =
     BindableProperty.Create(nameof(H5FontSize), typeof(double), typeof(MarkdownView), defaultValue: 14d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H5FontSize
    {
        get => (double)GetValue(H5FontSizeProperty);
        set => SetValue(H5FontSizeProperty, value);
    }

    public static readonly BindableProperty H5FontFamilyProperty =
        BindableProperty.Create(nameof(H5FontFamily), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string H5FontFamily
    {
        get => (string)GetValue(H5FontFamilyProperty);
        set => SetValue(H5FontFamilyProperty, value);
    }

    public static readonly BindableProperty H5FontAttributesProperty =
        BindableProperty.Create(nameof(H5FontAttributes), typeof(FontAttributes), typeof(MarkdownView), FontAttributes.Bold, propertyChanged: OnMarkdownTextChanged);

    public FontAttributes H5FontAttributes
    {
        get => (FontAttributes)GetValue(H5FontAttributesProperty);
        set => SetValue(H5FontAttributesProperty, value);
    }

    /* **** H6 Settings **** */

    public static readonly BindableProperty H6ColorProperty =
        BindableProperty.Create(nameof(H6Color), typeof(Color), typeof(MarkdownView), Colors.DimGray, propertyChanged: OnMarkdownTextChanged);

    public Color H6Color
    {
        get => (Color)GetValue(H6ColorProperty);
        set => SetValue(H6ColorProperty, value);
    }

    public static readonly BindableProperty H6FontSizeProperty =
     BindableProperty.Create(nameof(H6FontSize), typeof(double), typeof(MarkdownView), defaultValue: 12d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H6FontSize
    {
        get => (double)GetValue(H6FontSizeProperty);
        set => SetValue(H6FontSizeProperty, value);
    }

    public static readonly BindableProperty H6FontFamilyProperty =
        BindableProperty.Create(nameof(H6FontFamily), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string H6FontFamily
    {
        get => (string)GetValue(H6FontFamilyProperty);
        set => SetValue(H6FontFamilyProperty, value);
    }

    public static readonly BindableProperty H6FontAttributesProperty =
        BindableProperty.Create(nameof(H6FontAttributes), typeof(FontAttributes), typeof(MarkdownView), FontAttributes.Bold, propertyChanged: OnMarkdownTextChanged);

    public FontAttributes H6FontAttributes
    {
        get => (FontAttributes)GetValue(H6FontAttributesProperty);
        set => SetValue(H6FontAttributesProperty, value);
    }

    /* **** Table Header Style ***/

    public static readonly BindableProperty TableHeaderFontSizeProperty =
        BindableProperty.Create(nameof(TableHeaderFontSize), typeof(double), typeof(MarkdownView), defaultValue: 14d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double TableHeaderFontSize
    {
        get => (double)GetValue(TableHeaderFontSizeProperty);
        set => SetValue(TableHeaderFontSizeProperty, value);
    }

    public static readonly BindableProperty TableHeaderTextColorProperty =
      BindableProperty.Create(nameof(TableHeaderTextColor), typeof(Color), typeof(MarkdownView), Colors.Black, propertyChanged: OnMarkdownTextChanged);

    public Color TableHeaderTextColor
    {
        get => (Color)GetValue(TableHeaderTextColorProperty);
        set => SetValue(TableHeaderTextColorProperty, value);
    }

    public static readonly BindableProperty TableHeaderBackgroundColorProperty =
     BindableProperty.Create(nameof(TableHeaderBackgroundColor), typeof(Color), typeof(MarkdownView), Colors.LightGrey, propertyChanged: OnMarkdownTextChanged);

    public Color TableHeaderBackgroundColor
    {
        get => (Color)GetValue(TableHeaderBackgroundColorProperty);
        set => SetValue(TableHeaderBackgroundColorProperty, value);
    }

    public static readonly BindableProperty TableHeaderFontFaceProperty =
        BindableProperty.Create(nameof(TableHeaderFontFace), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string TableHeaderFontFace
    {
        get => (string)GetValue(TableHeaderFontFaceProperty);
        set => SetValue(TableHeaderFontFaceProperty, value);
    }

    /***** Table Row Styling **/

    public static readonly BindableProperty TableRowBackgroundColorProperty =
 BindableProperty.Create(nameof(TableRowBackgroundColor), typeof(Color), typeof(MarkdownView), Colors.White, propertyChanged: OnMarkdownTextChanged);

    public Color TableRowBackgroundColor
    {
        get => (Color)GetValue(TableRowBackgroundColorProperty);
        set => SetValue(TableRowBackgroundColorProperty, value);
    }

    public static readonly BindableProperty TableRowFontFaceProperty =
       BindableProperty.Create(nameof(TableRowFontFace), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string TableRowFontFace
    {
        get => (string)GetValue(TableRowFontFaceProperty);
        set => SetValue(TableRowFontFaceProperty, value);
    }

    public static readonly BindableProperty TableRowTextColorProperty =
     BindableProperty.Create(nameof(TableRowTextColor), typeof(Color), typeof(MarkdownView), Colors.Black, propertyChanged: OnMarkdownTextChanged);

    public Color TableRowTextColor
    {
        get => (Color)GetValue(TableRowTextColorProperty);
        set => SetValue(TableRowTextColorProperty, value);
    }

    public static readonly BindableProperty TableRowFontSizeProperty =
       BindableProperty.Create(nameof(TableRowFontSize), typeof(double), typeof(MarkdownView), defaultValue: 12d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double TableRowFontSize
    {
        get => (double)GetValue(TableRowFontSizeProperty);
        set => SetValue(TableRowFontSizeProperty, value);
    }

    /* ****** Text Styling ******** */

    public static readonly BindableProperty TextColorProperty =
       BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MarkdownView), Colors.Black, propertyChanged: OnMarkdownTextChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

	public static readonly BindableProperty TextColorLightProperty =
	   BindableProperty.Create(nameof(TextColorLight), typeof(Color), typeof(MarkdownView), Colors.Black);

	public Color TextColorLight
	{
		get => (Color)GetValue(TextColorLightProperty);
		set => SetValue(TextColorLightProperty, value);
	}

	public static readonly BindableProperty TextColorDarkProperty =
	   BindableProperty.Create(nameof(TextColorDark), typeof(Color), typeof(MarkdownView), Colors.White);

	public Color TextColorDark
	{
		get => (Color)GetValue(TextColorDarkProperty);
		set => SetValue(TextColorDarkProperty, value);
    }

    public static readonly BindableProperty TextFontSizeProperty =
       BindableProperty.Create(nameof(TextFontSize), typeof(double), typeof(MarkdownView), defaultValue: 12d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double TextFontSize
    {
        get => (double)GetValue(TextFontSizeProperty);
        set => SetValue(TextFontSizeProperty, value);
    }

    public static readonly BindableProperty TextFontFaceProperty =
      BindableProperty.Create(nameof(TextFontFace), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string TextFontFace
    {
        get => (string)GetValue(TextFontFaceProperty);
        set => SetValue(TextFontFaceProperty, value);
    }

    /* ****** Line Block Styling ******** */

    public static readonly BindableProperty LineColorProperty =
    BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(MarkdownView), Colors.LightGray, propertyChanged: OnMarkdownTextChanged);

    public Color LineColor
    {
        get => (Color)GetValue(LineColorProperty);
        set => SetValue(LineColorProperty, value);
    }

    /* ****** Code Block Styling ******** */
    public static readonly BindableProperty CodeBlockBackgroundColorProperty =
       BindableProperty.Create(nameof(CodeBlockBackgroundColor), typeof(Color), typeof(MarkdownView), Colors.White, propertyChanged: OnMarkdownTextChanged);

    public Color CodeBlockBackgroundColor
    {
        get => (Color)GetValue(CodeBlockBackgroundColorProperty);
        set => SetValue(CodeBlockBackgroundColorProperty, value);
    }

	public static readonly BindableProperty CodeBlockBackgroundColorLightProperty =
	   BindableProperty.Create(nameof(CodeBlockBackgroundColorLight), typeof(Color), typeof(MarkdownView), Colors.White);

	public Color CodeBlockBackgroundColorLight
	{
		get => (Color)GetValue(CodeBlockBackgroundColorLightProperty);
		set => SetValue(CodeBlockBackgroundColorLightProperty, value);
	}

	public static readonly BindableProperty CodeBlockBackgroundColorDarkProperty =
	   BindableProperty.Create(nameof(CodeBlockBackgroundColorDark), typeof(Color), typeof(MarkdownView), Colors.White);

	public Color CodeBlockBackgroundColorDark
	{
		get => (Color)GetValue(CodeBlockBackgroundColorDarkProperty);
		set => SetValue(CodeBlockBackgroundColorDarkProperty, value);
	}

	public static readonly BindableProperty CodeBlockBorderColorProperty =
       BindableProperty.Create(nameof(CodeBlockBorderColor), typeof(Color), typeof(MarkdownView), Colors.BlueViolet, propertyChanged: OnMarkdownTextChanged);

    public Color CodeBlockBorderColor
    {
        get => (Color)GetValue(CodeBlockBorderColorProperty);
        set => SetValue(CodeBlockBorderColorProperty, value);
    }

    public static readonly BindableProperty CodeBlockTextColorProperty =
       BindableProperty.Create(nameof(CodeBlockTextColor), typeof(Color), typeof(MarkdownView), Colors.BlueViolet, propertyChanged: OnMarkdownTextChanged);

    public Color CodeBlockTextColor
    {
        get => (Color)GetValue(CodeBlockTextColorProperty);
        set => SetValue(CodeBlockTextColorProperty, value);
    }

	public static readonly BindableProperty CodeBlockTextColorLightProperty =
	   BindableProperty.Create(nameof(CodeBlockTextColorLight), typeof(Color), typeof(MarkdownView), Colors.BlueViolet);

	public Color CodeBlockTextColorLight
	{
		get => (Color)GetValue(CodeBlockTextColorLightProperty);
		set => SetValue(CodeBlockTextColorLightProperty, value);
	}

	public static readonly BindableProperty CodeBlockTextColorDarkProperty =
	   BindableProperty.Create(nameof(CodeBlockTextColorDark), typeof(Color), typeof(MarkdownView), Colors.BlueViolet);

	public Color CodeBlockTextColorDark
	{
		get => (Color)GetValue(CodeBlockTextColorDarkProperty);
		set => SetValue(CodeBlockTextColorDarkProperty, value);
	}

    public static readonly BindableProperty CodeBlockFontSizeProperty =
       BindableProperty.Create(nameof(CodeBlockFontSize), typeof(double), typeof(MarkdownView), defaultValue: 12d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double CodeBlockFontSize
    {
        get => (double)GetValue(CodeBlockFontSizeProperty);
        set => SetValue(CodeBlockFontSizeProperty, value);
    }

    public static readonly BindableProperty CodeBlockFontFaceProperty =
      BindableProperty.Create(nameof(CodeBlockFontFace), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string CodeBlockFontFace
    {
        get => (string)GetValue(CodeBlockFontFaceProperty);
        set => SetValue(CodeBlockFontFaceProperty, value);
    }

    /* ****** BlockQuote Block Styling ******** */

    public static readonly BindableProperty BlockQuoteBackgroundColorProperty =
     BindableProperty.Create(nameof(BlockQuoteBackgroundColor), typeof(Color), typeof(MarkdownView), Colors.LightGray, propertyChanged: OnMarkdownTextChanged);

    public Color BlockQuoteBackgroundColor
    {
        get => (Color)GetValue(BlockQuoteBackgroundColorProperty);
        set => SetValue(BlockQuoteBackgroundColorProperty, value);
    }

	public static readonly BindableProperty BlockQuoteBackgroundColorLightProperty =
	 BindableProperty.Create(nameof(BlockQuoteBackgroundColorLight), typeof(Color), typeof(MarkdownView), Colors.LightGray);

	public Color BlockQuoteBackgroundColorLight
	{
		get => (Color)GetValue(BlockQuoteBackgroundColorLightProperty);
		set => SetValue(BlockQuoteBackgroundColorLightProperty, value);
	}

	public static readonly BindableProperty BlockQuoteBackgroundColorDarkProperty =
	 BindableProperty.Create(nameof(BlockQuoteBackgroundColorDark), typeof(Color), typeof(MarkdownView), Colors.LightGray);

	public Color BlockQuoteBackgroundColorDark
	{
		get => (Color)GetValue(BlockQuoteBackgroundColorDarkProperty);
		set => SetValue(BlockQuoteBackgroundColorDarkProperty, value);
	}

	public static readonly BindableProperty BlockQuoteBorderColorProperty =
      BindableProperty.Create(nameof(BlockQuoteBorderColor), typeof(Color), typeof(MarkdownView), Colors.BlueViolet, propertyChanged: OnMarkdownTextChanged);

    public Color BlockQuoteBorderColor
    {
        get => (Color)GetValue(BlockQuoteBorderColorProperty);
        set => SetValue(BlockQuoteBorderColorProperty, value);
    }

    public static readonly BindableProperty BlockQuoteTextColorProperty =
      BindableProperty.Create(nameof(BlockQuoteTextColor), typeof(Color), typeof(MarkdownView), Colors.BlueViolet, propertyChanged: OnMarkdownTextChanged);

    public Color BlockQuoteTextColor
    {
        get => (Color)GetValue(BlockQuoteTextColorProperty);
        set => SetValue(BlockQuoteTextColorProperty, value);
    }

	public static readonly BindableProperty BlockQuoteTextColorLightProperty =
	 BindableProperty.Create(nameof(BlockQuoteTextColorLight), typeof(Color), typeof(MarkdownView), Colors.BlueViolet);

	public Color BlockQuoteTextColorLight
	{
		get => (Color)GetValue(BlockQuoteTextColorLightProperty);
		set => SetValue(BlockQuoteTextColorLightProperty, value);
	}

	public static readonly BindableProperty BlockQuoteTextColorDarkProperty =
	 BindableProperty.Create(nameof(BlockQuoteTextColorDark), typeof(Color), typeof(MarkdownView), Colors.BlueViolet);

	public Color BlockQuoteTextColorDark
	{
		get => (Color)GetValue(BlockQuoteTextColorDarkProperty);
		set => SetValue(BlockQuoteTextColorDarkProperty, value);
	}

	public static readonly BindableProperty BlockQuoteFontFaceProperty =
     BindableProperty.Create(nameof(BlockQuoteFontFace), typeof(string), typeof(MarkdownView), defaultValue: "Consolas", propertyChanged: OnMarkdownTextChanged);

    public string BlockQuoteFontFace
    {
        get => (string)GetValue(BlockQuoteFontFaceProperty);
        set => SetValue(BlockQuoteFontFaceProperty, value);
    }

    /* ****** Hyplerlink Styling ******** */

    public static readonly BindableProperty HyperlinkColorProperty =
     BindableProperty.Create(nameof(HyperlinkColor), typeof(Color), typeof(MarkdownView), Colors.BlueViolet, propertyChanged: OnMarkdownTextChanged);

    public Color HyperlinkColor
    {
        get => (Color)GetValue(HyperlinkColorProperty);
        set => SetValue(HyperlinkColorProperty, value);
    }

	public static readonly BindableProperty HyperlinkColorLightProperty =
	 BindableProperty.Create(nameof(HyperlinkColorLight), typeof(Color), typeof(MarkdownView), Colors.BlueViolet);

	public Color HyperlinkColorLight
	{
		get => (Color)GetValue(HyperlinkColorLightProperty);
		set => SetValue(HyperlinkColorLightProperty, value);
	}

	public static readonly BindableProperty HyperlinkColorDarkProperty =
	 BindableProperty.Create(nameof(HyperlinkColorDark), typeof(Color), typeof(MarkdownView), Colors.BlueViolet);

	public Color HyperlinkColorDark
	{
		get => (Color)GetValue(HyperlinkColorDarkProperty);
		set => SetValue(HyperlinkColorDarkProperty, value);
	}

    public static readonly BindableProperty LinkCommandProperty =
    BindableProperty.Create(nameof(LinkCommand), typeof(ICommand), typeof(MarkdownView));

    public ICommand LinkCommand
    {
        get => (ICommand)GetValue(LinkCommandProperty);
        set => SetValue(LinkCommandProperty, value);
    }

    public static readonly BindableProperty LinkCommandParameterProperty =
        BindableProperty.Create(nameof(LinkCommandParameter), typeof(object), typeof(MarkdownView));

    public object LinkCommandParameter
    {
        get => GetValue(LinkCommandParameterProperty);
        set => SetValue(LinkCommandParameterProperty, value);
    }

    /* **************** E-Mail Links ************************/

    public static readonly BindableProperty EMailCommandProperty =
   BindableProperty.Create(nameof(EMailCommand), typeof(ICommand), typeof(MarkdownView));

    public ICommand EMailCommand
    {
        get => (ICommand)GetValue(EMailCommandProperty);
        set => SetValue(EMailCommandProperty, value);
    }

    public static readonly BindableProperty EMailCommandParameterProperty =
        BindableProperty.Create(nameof(EMailCommandParameter), typeof(object), typeof(MarkdownView));

    public object EMailCommandParameter
    {
        get => GetValue(EMailCommandParameterProperty);
        set => SetValue(EMailCommandParameterProperty, value);
    }

    /* **************** Image Styling ***********************/

    public static readonly BindableProperty ImageAspectProperty =
       BindableProperty.Create(nameof(ImageAspect), typeof(Aspect), typeof(MarkdownView), defaultValue: Aspect.AspectFit, propertyChanged: OnMarkdownTextChanged);

    public Aspect ImageAspect
    {
        get => (Aspect)GetValue(ImageAspectProperty);
        set => SetValue(ImageAspectProperty, value);
    }

    public static readonly BindableProperty DefaultImageWidthProperty =
       BindableProperty.Create(nameof(DefaultImageWidth), typeof(double), typeof(MarkdownView), defaultValue: 200d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double DefaultImageWidth
    {
        get => (double)GetValue(DefaultImageWidthProperty);
        set => SetValue(DefaultImageWidthProperty, value);
    }

    public static readonly BindableProperty DefaultImageHeightProperty =
       BindableProperty.Create(nameof(DefaultImageHeight), typeof(double), typeof(MarkdownView), defaultValue: 200d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double DefaultImageHeight
    {
        get => (double)GetValue(DefaultImageHeightProperty);
        set => SetValue(DefaultImageHeightProperty, value);
    }

    [Obsolete("This is no longer needed and will be removed in future versions.")]
    public static readonly BindableProperty ListIndentProperty =
    BindableProperty.Create(nameof(ListIndent), typeof(Thickness), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    [Obsolete("This is no longer needed and will be removed in future versions.")]
    public Thickness ListIndent
    {
        get => (Thickness)GetValue(ListIndentProperty);
        set => SetValue(ListIndentProperty, value);
    }

    public static readonly BindableProperty ParagraphSpacingProperty = BindableProperty.Create(nameof(ParagraphSpacing),
        typeof(double), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged, defaultValue: 1.0);

    public double ParagraphSpacing
    {
        get => (double)GetValue(ParagraphSpacingProperty);
        set => SetValue(ParagraphSpacingProperty, value);
    }

    public static readonly BindableProperty LineHeightMultiplierProperty =
        BindableProperty.Create(nameof(LineHeightMultiplier), typeof(double), typeof(MarkdownView),
            propertyChanged: OnMarkdownTextChanged, defaultValue: 1.0);

    public double LineHeightMultiplier
    {
        get => (double)GetValue(LineHeightMultiplierProperty);
        set => SetValue(LineHeightMultiplierProperty, value);
    }

    private static void OnMarkdownTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MarkdownView view && newValue is string text)
        {
            try
            {
                view.RenderMarkdown(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering markdown: {ex.Message}");
            }
        }
    }

    private static void OnStylePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MarkdownView view && !view._isApplyingTheme && !string.IsNullOrEmpty(view.MarkdownText))
        {
            try
            {
                view.RenderMarkdown(view.MarkdownText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering markdown: {ex.Message}");
            }
        }
    }

    private void RenderMarkdown(string markdown)
    {
        // Skip rendering during batch theme updates
        if (_isApplyingTheme)
            return;

        try
        {
            var pipeline = new MarkdownPipelineBuilder()
                // Attributes - must come first to allow other extensions to use them
                .UseGenericAttributes()
                .UseAutoIdentifiers()
                .UseAdvancedExtensions()

                // Block-level extensions
                .UseAlertBlocks()
                .UseAbbreviations()
                .UseDefinitionLists()
                .UseFootnotes()
                .UseFooters()
                .UseCustomContainers()
                .UseCitations()
                .UseMathematics()

                // Table extensions
                .UseGridTables()
                .UsePipeTables()

                // List extensions
                .UseListExtras()
                .UseTaskLists()

                // Code and media
                .UseMediaLinks()

                // Inline-level extensions - order matters here
                .UseAutoLinks()
                .UseEmphasisExtras()

                // Emoji must come after emphasis to avoid * being interpreted as emoji
                .UseEmojiAndSmiley(enableSmileys: false)

                // Encoding
                .UseNonAsciiNoEscape()
                .Build();


            MarkdownDocument document = Markdig.Markdown.Parse(markdown, pipeline);

            var layout = new VerticalStackLayout
            {
                Margin = 0,
                Padding = 0,
                Spacing = (8 * ParagraphSpacing)
            };

            foreach (var block in document)
            {
                try
                {
                    if (RenderBlock(block) is View view)
                        layout.Children.Add(view);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error rendering block: {ex.Message}");
                }
            }

            Content = layout;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in RenderMarkdown: {ex.Message}");
            Content = new Label { Text = "Error rendering markdown content." };
        }
    }

    private View RenderBlock(Block block)
    {
        try
        {
            return block switch
            {
                ParagraphBlock p => RenderParagraph(p),
                HeadingBlock h => RenderHeading(h),
                ListBlock l => RenderList(l),
                QuoteBlock q => RenderQuote(q),
                ThematicBreakBlock => new BoxView { HeightRequest = 1, BackgroundColor = LineColor },
                Table table => RenderTable(table),
                CustomContainer cc => RenderCustomContainer(cc),
                MathBlock m => RenderFormula(m),
                CodeBlock c => c is FencedCodeBlock fenced ? RenderCode(fenced) : RenderCodeBlock(c),
                BlankLineBlock => null,
                _ => null
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering block: {ex.Message}");
            return null;
        }
    }

    private View RenderParagraph(ParagraphBlock block)
    {
        if (block.Inline == null)
            return new Label { LineBreakMode = LineBreakMode.WordWrap };

        // Does this paragraph contain any images?
        bool containsImage = block.Inline.Any(i => i is LinkInline li && li.IsImage);

        if (!containsImage)
        {
            // Only text – return a single label
            return new Label
            {
                FormattedText = RenderInlines(block.Inline),
                LineBreakMode = LineBreakMode.WordWrap
            };
        }

        // Build a one-row grid with a column per segment
        var grid = new Grid { ColumnSpacing = 0, RowSpacing = 0 };
        int columnIndex = 0;

        var textBuffer = new StringBuilder();

        void flushText()
        {
            if (textBuffer.Length == 0) return;

            // Create a column for the buffered text
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var label = new Label
            {
                Text = textBuffer.ToString(),
                FontFamily = TextFontFace,
                FontSize = TextFontSize,
                TextColor = TextColor,
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalOptions = LayoutOptions.Center
            };
            // Place the label in the current column
            grid.Children.Add(label);
            Grid.SetColumn(label, columnIndex);
            Grid.SetRow(label, 0);
            columnIndex++;

            textBuffer.Clear();
        }

        foreach (var inline in block.Inline)
        {
            // If this inline is an image, add the image and flush preceding text
            if (inline is LinkInline link && link.IsImage)
            {
                flushText();

                // Use Auto width for inline images to prevent expansion
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var img = new Image
                {
                    Aspect = ImageAspect,  // Set default aspect from bindable property
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                };

                var attrs = link.TryGetAttributes();
                bool hasExplicitSize = false;
                bool hasCustomHorizontal = false;
                bool hasCustomVertical = false;
                bool hasCustomAspect = false;

                if (attrs != null && attrs.Properties != null)
                {
                    string widthValue = null;
                    string heightValue = null;
                    string aspectValue = null;
                    string horizontalValue = null;
                    string verticalValue = null;

                    foreach (var prop in attrs.Properties)
                    {
                        if (prop.Key.Equals("width", StringComparison.OrdinalIgnoreCase))
                        {
                            widthValue = prop.Value;
                        }
                        else if (prop.Key.Equals("height", StringComparison.OrdinalIgnoreCase))
                        {
                            heightValue = prop.Value;
                        }
                        else if (prop.Key.Equals("aspect", StringComparison.OrdinalIgnoreCase))
                        {
                            aspectValue = prop.Value;
                        }
                        else if (prop.Key.Equals("horizontal", StringComparison.OrdinalIgnoreCase))
                        {
                            horizontalValue = prop.Value;
                        }
                        else if (prop.Key.Equals("vertical", StringComparison.OrdinalIgnoreCase))
                        {
                            verticalValue = prop.Value;
                        }
                    }

                    if (double.TryParse(widthValue, out var w))
                    {
                        img.WidthRequest = w;
                        hasExplicitSize = true;
                    }

                    if (double.TryParse(heightValue, out var h))
                    {
                        img.HeightRequest = h;
                        hasExplicitSize = true;
                    }

                    // Override default aspect if specified
                    if (!string.IsNullOrEmpty(aspectValue) &&
                        Enum.TryParse<Aspect>(aspectValue, ignoreCase: true, out var parsedAspect))
                    {
                        img.Aspect = parsedAspect;
                        hasCustomAspect = true;
                    }

                    // Parse horizontal positioning
                    if (!string.IsNullOrEmpty(horizontalValue))
                    {
                        var parsedHorizontal = ParseLayoutOptions(horizontalValue);
                        if (parsedHorizontal.HasValue)
                        {
                            img.HorizontalOptions = parsedHorizontal.Value;
                            hasCustomHorizontal = true;
                        }
                    }

                    // Parse vertical positioning
                    if (!string.IsNullOrEmpty(verticalValue))
                    {
                        var parsedVertical = ParseLayoutOptions(verticalValue);
                        if (parsedVertical.HasValue)
                        {
                            img.VerticalOptions = parsedVertical.Value;
                            hasCustomVertical = true;
                        }
                    }
                }

                if (hasCustomAspect && !hasExplicitSize)
                {
                    // Use configurable default sizes when only aspect is specified
                    // This allows AspectFill and other aspects to work correctly
                    img.MinimumWidthRequest = DefaultImageWidth;
                    img.MinimumHeightRequest = DefaultImageHeight;
                }

                // For inline images without explicit size and without custom horizontal positioning,
                // let them size naturally at the start
                if (!hasExplicitSize && !hasCustomHorizontal && !hasCustomAspect)
                {
                    img.HorizontalOptions = LayoutOptions.Start;
                }

                // Load the image asynchronously
                LoadImageAsync(link.Url).ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        MainThread.BeginInvokeOnMainThread(() => img.Source = t.Result);
                    }
                    else if (t.IsFaulted)
                    {
                        Console.WriteLine($"Error loading image {link.Url}: {t.Exception?.GetBaseException().Message}");
                    }
                });

                grid.Children.Add(img);
                Grid.SetColumn(img, columnIndex);
                Grid.SetRow(img, 0);
                columnIndex++;
            }
            else if (inline is LiteralInline literal)
            {
                textBuffer.Append(literal.Content.Text.AsSpan(literal.Content.Start, literal.Content.Length));
            }
            else if (inline is EmphasisInline em)
            {
                foreach (var sub in em)
                {
                    if (sub is LiteralInline l)
                        textBuffer.Append(l.Content.Text.AsSpan(l.Content.Start, l.Content.Length));
                }
            }
            else if (inline is LineBreakInline)
            {
                // Preserve line breaks in the buffer
                textBuffer.AppendLine();
            }
        }

        // Flush any text after the last image
        flushText();

        return grid;
    }

    private View RenderHeading(HeadingBlock block)
    {
        try
        {
            var formatted = new FormattedString();
            var headingFontFamily = GetFontFamilyForBlockLevel(block.Level);
            var headingFontAttributes = GetFontAttributesForBlockLevel(block.Level);

            if (block.Inline != null)
            {
                foreach (var inline in block.Inline)
                {
                    if (inline is LiteralInline literal)
                    {
                        formatted.Spans.Add(new Span
                        {
                            Text = literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length),
                            FontSize = GetFontsizeForBlockLevel(block.Level),
                            FontAttributes = headingFontAttributes,
                            TextColor = GetTextColorForBlockLevel(block.Level),
                            FontFamily = headingFontFamily
                        });
                    }
                    else if (inline is EmphasisInline em)
                    {
                        var text = string.Concat(em.Select(x => (x as LiteralInline)?.Content.ToString()));
                        // Combine heading font attributes with emphasis attributes
                        var emphasisAttributes = em.DelimiterCount == 2 ? FontAttributes.Bold : FontAttributes.Italic;
                        var combinedAttributes = headingFontAttributes | emphasisAttributes;
                        
                        formatted.Spans.Add(new Span
                        {
                            Text = text,
                            FontSize = GetFontsizeForBlockLevel(block.Level),
                            FontAttributes = combinedAttributes,
                            TextColor = GetTextColorForBlockLevel(block.Level),
                            FontFamily = headingFontFamily
                        });
                    }
                    else if (inline is LineBreakInline)
                    {
                        formatted.Spans.Add(new Span { Text = "\n" });
                    }
                }
            }

            return new Label
            {
                FormattedText = formatted,
                LineBreakMode = LineBreakModeHeader,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                LineHeight = LineHeightMultiplier,
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering heading: {ex.Message}");
            return new Label { Text = "[Error rendering heading]" };
        }
    }

    private Color GetTextColorForBlockLevel(int blockLevel)
    {
        try
        {
            return blockLevel switch
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting text color for block level: {ex.Message}");
            return Colors.Black;
        }
    }

    private double GetFontsizeForBlockLevel(int blockLevel)
    {
        try
        {
            return blockLevel switch
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting font size for block level: {ex.Message}");
            return 12d;
        }
    }

    private string GetFontFamilyForBlockLevel(int blockLevel)
    {
        try
        {
            var fontFamily = blockLevel switch
            {
                1 => H1FontFamily,
                2 => H2FontFamily,
                3 => H3FontFamily,
                4 => H4FontFamily,
                5 => H5FontFamily,
                6 => H6FontFamily,
                _ => H6FontFamily
            };
            
            // Fall back to TextFontFace if no specific font family is set
            return fontFamily ?? TextFontFace;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting font family for block level: {ex.Message}");
            return null;
        }
    }

    private FontAttributes GetFontAttributesForBlockLevel(int blockLevel)
    {
        try
        {
            return blockLevel switch
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting font attributes for block level: {ex.Message}");
            return FontAttributes.Bold;
        }
    }

    private View RenderQuote(QuoteBlock block)
    {
        try
        {
            var quoteContent = new VerticalStackLayout()
            {
                Margin = 10
            };
            foreach (var subBlock in block)
            {
                if (RenderBlock(subBlock) is View view)
                    quoteContent.Children.Add(view);
            }

            var box = new Border
            {
                Margin = new Thickness(0),
                BackgroundColor = BlockQuoteBorderColor,
                Stroke = new SolidColorBrush(BlockQuoteBorderColor),
                StrokeShape = new RoundRectangle() { CornerRadius = new CornerRadius(4, 0, 4, 0) },
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var blockQuoteGrid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 5 },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };

            blockQuoteGrid.Children.Add(box);
            Grid.SetRow(box, 0);
            Grid.SetColumn(box, 0);

            blockQuoteGrid.Children.Add(quoteContent);
            Grid.SetRow(quoteContent, 0);
            Grid.SetColumn(quoteContent, 1);

            var blockquote = new Border
            {
                Padding = new Thickness(0),
                Stroke = new SolidColorBrush(BlockQuoteBorderColor),
                StrokeShape = new RoundRectangle().WithCornerRadius(4),
                BackgroundColor = BlockQuoteBackgroundColor,
                Content = blockQuoteGrid
            };

            return blockquote;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering quote: {ex.Message}");
            return new Label { Text = "[Error rendering quote]" };
        }
    }

    private View RenderCode(FencedCodeBlock block)
    {
        try
        {
            string codeText = string.Empty;
            if (block?.Lines != null)
            {
                try
                {
                    codeText = block.Lines.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting code block lines to string: {ex.Message}");
                    codeText = "[Error rendering code content]";
                }
            }

            return new Border
            {
                BackgroundColor = CodeBlockBackgroundColor,
                Stroke = new SolidColorBrush(CodeBlockBorderColor),
                Padding = 8,
                StrokeShape = new RoundRectangle().WithCornerRadius(4),
                Content = new Label
                {
                    Text = codeText,
                    FontFamily = CodeBlockFontFace,
                    TextColor = CodeBlockTextColor,
                    FontSize = CodeBlockFontSize,
                    LineBreakMode = LineBreakMode.WordWrap,
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering code block: {ex.Message}");
            return new Label { Text = "[Error rendering code block]" };
        }
    }

    private View RenderCodeBlock(CodeBlock block)
    {
        try
        {
            string codeText = string.Empty;
            if (block?.Lines != null)
            {
                try
                {
                    codeText = block.Lines.ToString() ?? string.Empty;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting code block lines to string: {ex.Message}");
                    codeText = "[Error rendering code content]";
                }
            }

            return new Border
            {
                BackgroundColor = CodeBlockBackgroundColor,
                Stroke = new SolidColorBrush(CodeBlockBorderColor),
                Padding = 8,
                StrokeShape = new RoundRectangle().WithCornerRadius(4),
                Content = new Label
                {
                    Text = codeText,
                    FontFamily = CodeBlockFontFace,
                    TextColor = CodeBlockTextColor,
                    FontSize = CodeBlockFontSize,
                    LineBreakMode = LineBreakMode.WordWrap,
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering code block: {ex.Message}");
            return new Label { Text = "[Error rendering code block]" };
        }
    }

    private View RenderTable(Table table)
    {
        try
        {
            var grid = new Grid
            {
                ColumnSpacing = 1,
                RowSpacing = 1,
                BackgroundColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Fill,
                WidthRequest = -1
            };

            for (int i = 0; i < table.ColumnDefinitions.Count; i++)
            {
                bool hasContent = false;
                foreach (TableRow row in table)
                {
                    if (row.Count > i && row[i] is TableCell cell && cell.Count > 0)
                    {
                        hasContent = true;
                        break;
                    }
                }
                if (hasContent)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                }
            }

            int rowIndex = 0;

            foreach (TableRow row in table)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                for (int colIndex = 0; colIndex < row.Count; colIndex++)
                {
                    var cell = row[colIndex] as TableCell;
                    if (cell == null || cell.Count == 0)
                        continue;

                    var alignment = table.ColumnDefinitions[colIndex].Alignment;

                    var horizontalTextAlignment = alignment switch
                    {
                        TableColumnAlign.Center => TextAlignment.Center,
                        TableColumnAlign.Right => TextAlignment.End,
                        _ => TextAlignment.Start
                    };

                    var label = new Label
                    {
                        FormattedText = RenderInlines((cell.FirstOrDefault() as ParagraphBlock)?.Inline),
                        BackgroundColor = row.IsHeader ? TableHeaderBackgroundColor : TableRowBackgroundColor,
                        FontAttributes = row.IsHeader ? FontAttributes.Bold : FontAttributes.None,
                        TextColor = row.IsHeader ? TableHeaderTextColor : TableRowTextColor,
                        FontFamily = row.IsHeader ? TableHeaderFontFace : TableRowFontFace,
                        FontSize = row.IsHeader ? TableHeaderFontSize : TableRowFontSize,
                        Padding = 4,
                        HorizontalTextAlignment = horizontalTextAlignment,
                        LineBreakMode = row.IsHeader ? LineBreakMode.TailTruncation : LineBreakMode.WordWrap,
                        HorizontalOptions = LayoutOptions.Fill,
                        VerticalOptions = LayoutOptions.Fill
                    };

                    grid.Add(label, colIndex, rowIndex);
                }

                rowIndex++;
            }

            return grid;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering table: {ex.Message}");
            return new Label { Text = "[Error rendering table]" };
        }
    }

	private View RenderFormula(MathBlock mathBlock)
	{
		try
		{
			string formularText = string.Empty;
			if (mathBlock?.Lines != null)
			{
				try
				{
					formularText = mathBlock.Lines.ToString() ?? string.Empty;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error converting math block lines to string: {ex.Message}");
					formularText = "[Error rendering formula]";
				}
			}

			var latexView = new LatexView
			{
				Text = formularText,
				FontSize = (float)TextFontSize * 4,
				HighlightColor = Colors.Transparent,
				ErrorColor = Colors.Red,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center,
				Margin = new Thickness(-10, -10)
			};

			// Add binding for TextColor
			latexView.SetBinding(LatexView.TextColorProperty, new Binding(nameof(TextColor), source: this));

			return latexView;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error rendering formula: {ex.Message}");
			return new Label { Text = "[Error rendering formula]" };
		}
	}

    private View RenderCustomContainer(CustomContainer container)
    {
        try
        {
            var type = container.TryGetAttributes()?.Classes?.FirstOrDefault() ?? "default";

            var color = type switch
            {
                "info" => Colors.LightBlue,
                "warning" => Colors.Orange,
                "danger" => Colors.Red,
                _ => Colors.LightGray
            };

            var inner = new VerticalStackLayout();
            foreach (var child in container)
            {
                if (RenderBlock(child) is View view)
                    inner.Children.Add(view);
            }

            return new Border
            {
                Stroke = color,
                StrokeThickness = 2,
                Padding = new Thickness(10),
                Content = inner
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering custom container: {ex.Message}");
            return new Label { Text = "[Error rendering custom container]" };
        }
    }

    private View RenderList(ListBlock listBlock, int nestingLevel = 0)
    {
        try
        {
            var stack = new VerticalStackLayout
            {
                Padding = new Thickness(nestingLevel * 20, 0, 0, 0),
                Spacing = 4
            };

            foreach (ListItemBlock item in listBlock)
            {
                var isChecklist = item.TryGetAttributes()?.Properties?.FirstOrDefault(p => p.Key == "checked").Value != null;
                var isChecked = isChecklist && item.GetAttributes().Properties.First(p => p.Key == "checked").Value == "true";

                foreach (var subBlock in item)
                {
                    if (subBlock is ListBlock nestedList)
                    {
                        stack.Children.Add(RenderList(nestedList, nestingLevel + 1));
                    }
                    else
                    {
                        var content = RenderBlock(subBlock);

                        if (content != null)
                        {
                            if (isChecklist && content is Label label)
                            {
                                var checkbox = new CheckBox { IsChecked = isChecked, IsEnabled = false };
                                var layout = new HorizontalStackLayout
                                {
                                    Spacing = 8,
                                    Children = { checkbox, label }
                                };
                                stack.Children.Add(layout);
                            }
                            else
                            {
                                var prefix = listBlock.IsOrdered ? $"{item.Order}." : "•";
                                var rowGrid = new Grid
                                {
                                    ColumnDefinitions =
                                    {
                                        new ColumnDefinition { Width = GridLength.Auto },
                                        new ColumnDefinition { Width = GridLength.Star }
                                    },
                                    ColumnSpacing = 8
                                };

                                var prefixLabel = new Label
                                {
                                    Text = prefix,
                                    FontAttributes = FontAttributes.Bold,
                                    VerticalOptions = LayoutOptions.Start,
                                    HorizontalOptions = LayoutOptions.Start
                                };

                                if (content is View contentView)
                                {
                                    contentView.HorizontalOptions = LayoutOptions.Fill;
                                }

                                Grid.SetColumn(prefixLabel, 0);
                                Grid.SetRow(prefixLabel, 0);
                                rowGrid.Children.Add(prefixLabel);

                                Grid.SetColumn(content, 1);
                                Grid.SetRow(content, 0);
                                rowGrid.Children.Add(content);

                                stack.Children.Add(rowGrid);
                            }
                        }
                    }
                }
            }

            return stack;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rendering list: {ex.Message}");
            return new Label { Text = "[Error rendering list]" };
        }
    }

    private FormattedString RenderInlines(ContainerInline inlines)
    {
        var formatted = new FormattedString();

        if (inlines == null) return formatted;

        foreach (var inline in inlines)
        {
            try
            {
                switch (inline)
                {
                    case LiteralInline literal:
                        formatted.Spans.Add(new Span
                        {
                            Text = literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length),
                            FontFamily = TextFontFace,
                            FontSize = TextFontSize,
                            TextColor = TextColor
                        });
                        break;

                    case EmphasisInline em:
                        var text = string.Concat(em.Select(x => (x as LiteralInline)?.Content.ToString()));
                        formatted.Spans.Add(new Span
                        {
                            Text = text,
                            TextDecorations = em.DelimiterChar == '~'
                                ? TextDecorations.Strikethrough
                                : TextDecorations.None,
                            FontAttributes = em.DelimiterChar == '*' && em.DelimiterCount == 2
                                ? FontAttributes.Bold
                                : em.DelimiterChar == '*' && em.DelimiterCount == 1
                                    ? FontAttributes.Italic
                                    : FontAttributes.None,
                            FontFamily = TextFontFace,
                            FontSize = TextFontSize,
                            TextColor = TextColor
                        });
                        break;

                    case LineBreakInline:
                        formatted.Spans.Add(new Span
                        {
                            Text = "\n",
                            FontFamily = TextFontFace,
                            FontSize = TextFontSize,
                            TextColor = TextColor
                        });
                        break;

                    case LinkInline link when !link.IsImage:
                        var linkText = link.FirstChild?.ToString() ?? link.Url;
                        var span = new Span
                        {
                            Text = linkText,
                            TextColor = HyperlinkColor,
                            TextDecorations = TextDecorations.Underline,
                            FontFamily = TextFontFace,
                            FontSize = TextFontSize,
                        };

                        var tap = new TapGestureRecognizer();

                        if (link.Url.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                        {
                            tap.Tapped += (_, __) => TriggerEmailClicked(link.Url.Substring("mailto:".Length));
                        }
                        else if (EmailRegex.IsMatch(link.Url))
                        {
                            var email = EmailRegex.Match(link.Url).Value;
                            tap.Tapped += (_, __) => TriggerEmailClicked(email);
                        }
                        else
                        {
                            tap.Tapped += (_, __) => TriggerHyperLinkClicked(link.Url);
                        }

                        span.GestureRecognizers.Add(tap);
                        formatted.Spans.Add(span);
                        break;
                    case LinkInline image when image.IsImage:
                        formatted.Spans.Add(new Span
                        {
                            Text = "[Image]",
                            FontFamily = TextFontFace,
                            FontSize = TextFontSize,
                            TextColor = TextColor
                        });
                        break;
                    case MathInline math:
                        formatted.Spans.Add(new Span
                        {
                            Text = math.Content.ToString(),
                            FontAttributes = FontAttributes.Italic,
                            TextColor = Colors.DarkOliveGreen,
                            FontFamily = TextFontFace,
                            FontSize = TextFontSize
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering inline: {ex.Message}");
            }
        }

        return formatted;
    }

    private async Task<ImageSource> LoadImageAsync(string imageUrl)
    {
        ImageSource imageSource;

        try
        {
            // Handle data URLs (e.g., data:image/png;base64,iVBORw0...)
            if (imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {

                int idx = imageUrl.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    // extract and clean the base64 portion
                    string base64Data = imageUrl[(idx + "base64,".Length)..]
                                               .Replace("\r", "")
                                               .Replace("\n", "");
                    byte[] bytes = Convert.FromBase64String(base64Data);
                    return ImageSource.FromStream(() => new MemoryStream(bytes));
                }
                else
                {
                    Console.WriteLine($"Data URL missing base64 prefix: {imageUrl}");
                    imageSource = ImageSource.FromFile("icon.png");
                }
            }
            // Handle legacy case where just the base64 string is passed (without data: prefix)
            else if (System.Buffers.Text.Base64.IsValid(imageUrl))
            {
                byte[] imageBytes = Convert.FromBase64String(imageUrl);
                imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            else if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult))
            {

				if (imageUrl != null && _imageCache.TryGetValue(imageUrl, out ImageSource value))
                {
                    return value;
                }
                else
                {
                    try
                    {
                        if (imageUrl.EndsWith(".svg", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var httpClient = new HttpClient();

                            var imageBytes = await httpClient.GetByteArrayAsync(uriResult)
                                .ConfigureAwait(false);
                            if (imageBytes != null)
                            {
                                XmlDocument xmlDocument = new();
                                xmlDocument.LoadXml(Encoding.UTF8.GetString(imageBytes));
                                XmlNodeList commentNodes = xmlDocument.SelectNodes("//comment()");
                                foreach (XmlNode comment in commentNodes)
                                {
                                    comment.ParentNode.RemoveChild(comment);
                                }

                                XmlReader xmlReader = XmlReader.Create(new StringReader(xmlDocument.OuterXml));

                                var svg = new SKSvg();
                                SKPicture svgImage = svg.Load(xmlReader);
                                var image = new SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
                                using (var surface = SKSurface.Create(new SKImageInfo(image.Width, image.Height)))
                                {
                                    var canvas = surface.Canvas;
                                    canvas.Clear(SKColors.Transparent);
                                    canvas.DrawPicture(svg.Picture);
                                    canvas.Flush();
                                    surface.Snapshot().ReadPixels(image.Info, image.GetPixels(), image.RowBytes, 0, 0);
                                }
                                var imageStream = new MemoryStream();
                                image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(imageStream);
                                imageStream.Position = 0;
                                imageSource = ImageSource.FromStream(() => imageStream);
                                if (imageUrl != null) _imageCache[imageUrl] = imageSource;

                                xmlDocument = null;
                                xmlReader.Dispose();
                            }
                            else
                            {
                                Console.WriteLine($"Failed to download image: {imageUrl}");
                                imageSource = default;
                            }
                            httpClient.Dispose();
                        }
                        else
                        {
                            using var httpClient = new HttpClient();
                            var imageBytes = await httpClient.GetByteArrayAsync(uriResult)
                                .ConfigureAwait(false);
                            if (imageBytes != null)
                            {
                                imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                                if (imageUrl != null) _imageCache[imageUrl] = imageSource;
                            }
                            else
                            {
                                imageSource = default;
                                Console.WriteLine($"Failed to download image: {imageUrl}");
                            }
                            httpClient.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error downloading image: {ex.Message}");
                        throw;
                    }
                }
            }
            else
            {
                imageSource = ImageSource.FromFile(imageUrl);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load image: {ex.Message}");
            imageSource = ImageSource.FromFile("icon.png");
        }

        return imageSource ?? ImageSource.FromFile("icon.png");
    }

    private static LayoutOptions? ParseLayoutOptions(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim().ToLowerInvariant() switch
        {
            "start" => LayoutOptions.Start,
            "center" => LayoutOptions.Center,
            "end" => LayoutOptions.End,
            "fill" => LayoutOptions.Fill,
            "fillandexpand" => LayoutOptions.FillAndExpand,
            "startandexpand" => LayoutOptions.StartAndExpand,
            "centerandexpand" => LayoutOptions.CenterAndExpand,
            "endandexpand" => LayoutOptions.EndAndExpand,
            _ => null
        };
    }

    internal void TriggerHyperLinkClicked(string url)
    {
        try
        {
            OnHyperLinkClicked?.Invoke(this, new LinkEventArgs { Url = url });

            if (LinkCommand?.CanExecute(url) == true)
            {
                LinkCommand.Execute(url);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error triggering hyperlink clicked: {ex.Message}");
        }
    }

    private void TriggerEmailClicked(string email)
    {
        try
        {
            OnEmailClicked?.Invoke(this, new EmailEventArgs { Email = email });

            if (EMailCommand?.CanExecute(email) == true)
            {
                EMailCommand.Execute(email);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error triggering email clicked: {ex.Message}");
        }
    }

    ~MarkdownView()
    {
        try
        {
            // Unsubscribe from theme property changes
            if (Theme != null)
            {
                Theme.PropertyChanged -= OnThemePropertyChanged;
            }

            // Unsubscribe from system theme changes
            if (Application.Current != null && UseAppTheme)
            {
                Application.Current.RequestedThemeChanged -= OnSystemThemeChanged;
            }

            _imageCache?.Clear();
            _imageCache = null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in destructor: {ex.Message}");
        }
    }
}