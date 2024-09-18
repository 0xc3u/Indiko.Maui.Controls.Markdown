using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Image = Microsoft.Maui.Controls.Image;

namespace Indiko.Maui.Controls.Markdown;

public class MarkdownView : ContentView
{
    private Dictionary<string, ImageSource> _imageCache = [];

    public delegate void HyperLinkClicked(object sender, LinkEventArgs e);
    public event HyperLinkClicked OnHyperLinkClicked;

    public static readonly BindableProperty MarkdownTextProperty =
        BindableProperty.Create(nameof(MarkdownText), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownTextChanged);

    public string MarkdownText
    {
        get => (string)GetValue(MarkdownTextProperty);
        set => SetValue(MarkdownTextProperty, value);
    }

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

    public static readonly BindableProperty H1FontSizeProperty =
      BindableProperty.Create(nameof(H1FontSize), typeof(double), typeof(MarkdownView), defaultValue: 24d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H1FontSize
    {
        get => (double)GetValue(H1FontSizeProperty);
        set => SetValue(H1FontSizeProperty, value);
    }

    public static readonly BindableProperty H2ColorProperty =
        BindableProperty.Create(nameof(H2Color), typeof(Color), typeof(MarkdownView), Colors.DarkGray, propertyChanged: OnMarkdownTextChanged);

    public Color H2Color
    {
        get => (Color)GetValue(H2ColorProperty);
        set => SetValue(H2ColorProperty, value);
    }

    public static readonly BindableProperty H2FontSizeProperty =
     BindableProperty.Create(nameof(H2FontSize), typeof(double), typeof(MarkdownView), defaultValue: 20d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H2FontSize
    {
        get => (double)GetValue(H2FontSizeProperty);
        set => SetValue(H2FontSizeProperty, value);
    }

    // H3Color property
    public static readonly BindableProperty H3ColorProperty =
        BindableProperty.Create(nameof(H3Color), typeof(Color), typeof(MarkdownView), Colors.Gray, propertyChanged: OnMarkdownTextChanged);

    public Color H3Color
    {
        get => (Color)GetValue(H3ColorProperty);
        set => SetValue(H3ColorProperty, value);
    }

    public static readonly BindableProperty H3FontSizeProperty =
     BindableProperty.Create(nameof(H3FontSize), typeof(double), typeof(MarkdownView), defaultValue: 18d, propertyChanged: OnMarkdownTextChanged);

    [TypeConverter(typeof(FontSizeConverter))]
    public double H3FontSize
    {
        get => (double)GetValue(H3FontSizeProperty);
        set => SetValue(H3FontSizeProperty, value);
    }

    /* ****** Text Styling ******** */

    public static readonly BindableProperty TextColorProperty =
       BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MarkdownView), Colors.Black, propertyChanged: OnMarkdownTextChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
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
       BindableProperty.Create(nameof(CodeBlockBackgroundColor), typeof(Color), typeof(MarkdownView), Colors.LightGray, propertyChanged: OnMarkdownTextChanged);

    public Color CodeBlockBackgroundColor
    {
        get => (Color)GetValue(CodeBlockBackgroundColorProperty);
        set => SetValue(CodeBlockBackgroundColorProperty, value);
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

    /* **************** Image Styling ***********************/

    public static readonly BindableProperty ImageAspectProperty =
       BindableProperty.Create(nameof(ImageAspect), typeof(Aspect), typeof(MarkdownView), defaultValue: Aspect.AspectFit, propertyChanged: OnMarkdownTextChanged);

    public Aspect ImageAspect
    {
        get => (Aspect)GetValue(ImageAspectProperty);
        set => SetValue(ImageAspectProperty, value);
    }

    private static void OnMarkdownTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (MarkdownView)bindable;
        control.RenderMarkdown();
    }
    private void RenderMarkdown()
    {
        if (string.IsNullOrWhiteSpace(MarkdownText))
            return;

        // Clear existing content
        Content = null;

        var grid = new Grid
        {
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(0, 0, 0, 0),
            RowSpacing = 2,
            ColumnSpacing = 0,
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = 15 }, // For bullet points or ordered list numbers
            new ColumnDefinition { Width = GridLength.Star } // For text
        }
        };

        var lines = Regex.Split(MarkdownText, @"\r\n?|\n", RegexOptions.Compiled);
        lines = lines.Where(line => !string.IsNullOrEmpty(line)).ToArray();

        int gridRow = 0;
        bool isUnorderedListActive = false;
        bool isOrderedListActive = false;
        bool currentLineIsBlockQuote = true;
        Label activeCodeBlockLabel = null;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            bool lineBeforeWasBlockQuote = currentLineIsBlockQuote;
            currentLineIsBlockQuote = false;
            if (activeCodeBlockLabel == null)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            if (activeCodeBlockLabel != null)
            {
                HandleActiveCodeBlock(line, ref activeCodeBlockLabel, ref gridRow);
            }
            else if (IsHeadline(line, out int headlineLevel))
            {
                var headlineText = line[(headlineLevel + 1)..].Trim();
                Color textColor = headlineLevel == 1 ? H1Color :
                                  headlineLevel == 2 ? H2Color :
                                  headlineLevel == 3 ? H3Color : TextColor; // Default for h4-h6
                double fontSize = headlineLevel == 1 ? H1FontSize :
                                  headlineLevel == 2 ? H2FontSize :
                                  headlineLevel == 3 ? H3FontSize : TextFontSize; // Default for h4-h6

                var label = new Label
                {
                    Text = headlineText,
                    TextColor = textColor,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = fontSize,
                    FontFamily = TextFontFace,
                    LineBreakMode = LineBreakModeHeader,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                };

                grid.Children.Add(label);
                Grid.SetColumnSpan(label, 2);
                Grid.SetRow(label, gridRow++);
            }
            else if (IsImage(line))
            {
                var image = CreateImageBlock(line);

                if (image == null)
                {
                    continue;
                }

                grid.Children.Add(image);
                Grid.SetColumnSpan(image, 2);
                Grid.SetRow(image, gridRow++);
            }
            else if (IsBlockQuote(line))
            {
                HandleBlockQuote(line, lineBeforeWasBlockQuote, grid, out currentLineIsBlockQuote, ref gridRow);
            }
            else if (IsUnorderedList(line))
            {
                if (!isUnorderedListActive)
                {
                    isUnorderedListActive = true;
                }

                AddBulletPointToGrid(grid, gridRow);
                AddListItemTextToGrid(line[2..], grid, gridRow);

                gridRow++;
            }
            else if (IsOrderedList(line, out int listItemIndex))
            {
                if (!isOrderedListActive)
                {
                    isOrderedListActive = true;
                }

                AddOrderedListItemToGrid(listItemIndex, grid, gridRow);
                AddListItemTextToGrid(line[(listItemIndex.ToString().Length + 2)..], grid, gridRow);

                gridRow++;
            }
            else if (IsCodeBlock(line, out bool isSingleLineCodeBlock))
            {
                HandleSingleLineOrStartOfCodeBlock(line, grid, ref gridRow, isSingleLineCodeBlock, ref activeCodeBlockLabel);
            }
            else if (IsHorizontalRule(line))
            {
                var horizontalLine = new BoxView
                {
                    HeightRequest = 2,
                    Color = LineColor,
                    BackgroundColor = LineColor,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center
                };

                grid.Children.Add(horizontalLine);
                Grid.SetRow(horizontalLine, gridRow);
                Grid.SetColumnSpan(horizontalLine, 2);
                gridRow++;
            }
            else if (IsTable(lines, i, out int tableEndIndex)) // Detect table
            {
                var table = CreateTable(lines, i, tableEndIndex);
                grid.Children.Add(table);
                Grid.SetColumnSpan(table, 2);
                Grid.SetRow(table, gridRow++);
                i = tableEndIndex; // Skip processed lines
            }
            else // Regular text
            {
                if (isUnorderedListActive || isOrderedListActive)
                {
                    isUnorderedListActive = false;
                    isOrderedListActive = false;
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.Children.Add(new BoxView { Color = Colors.Transparent });
                    gridRow++;
                }

                var formattedString = CreateFormattedString(line, TextColor);
                var label = new Label
                {
                    FormattedText = formattedString,
                    LineBreakMode = LineBreakModeText,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Start
                };

                grid.Children.Add(label);
                Grid.SetRow(label, gridRow);
                Grid.SetColumn(label, 0);
                Grid.SetColumnSpan(label, 2);

                gridRow++;

                // never finish with empty superfluous empty line
                if (i != lines.Length - 1) AddEmptyRow(grid, ref gridRow);
            }
        }

        Content = grid;
    }

    private void HandleBlockQuote(string line, bool lineBeforeWasBlockQuote, Grid grid, out bool currentLineIsBlockQuote, ref int gridRow)
    {
        var box = new Frame
        {
            Margin = new Thickness(0),
            BackgroundColor = BlockQuoteBorderColor,
            BorderColor = BlockQuoteBorderColor,
            CornerRadius = 0,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        var blockQuotelabel = new Label
        {
            FormattedText = CreateFormattedString(line[1..].Trim(), BlockQuoteTextColor),
            LineBreakMode = LineBreakModeText,
            FontFamily = BlockQuoteFontFace,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            Padding = new Thickness(5)
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

        blockQuoteGrid.Children.Add(blockQuotelabel);
        Grid.SetRow(blockQuotelabel, 0);
        Grid.SetColumn(blockQuotelabel, 1);

        var blockquote = new Frame
        {
            Padding = new Thickness(0),
            CornerRadius = 0,
            BackgroundColor = BlockQuoteBackgroundColor,
            BorderColor = BlockQuoteBackgroundColor,
            Content = blockQuoteGrid
        };

        if (lineBeforeWasBlockQuote)
        {
            blockquote.Margin = new Thickness(0, -grid.RowSpacing, 0, 0);
        }

        currentLineIsBlockQuote = true;

        grid.Children.Add(blockquote);
        Grid.SetColumnSpan(blockquote, 2);
        Grid.SetRow(blockquote, gridRow++);
    }

    private void HandleSingleLineOrStartOfCodeBlock(string line, Grid grid, ref int gridRow, bool isSingleLineCodeBlock, ref Label activeCodeBlockLabel)
    {
        Frame codeBlock = CreateCodeBlock(line, out Label contentLabel);
        grid.Children.Add(codeBlock);
        Grid.SetRow(codeBlock, gridRow);
        Grid.SetColumnSpan(codeBlock, 2);
        if (isSingleLineCodeBlock)
            gridRow++;
        else
            activeCodeBlockLabel = contentLabel;
    }

    private static void HandleActiveCodeBlock(string line, ref Label activeCodeBlockLabel, ref int gridRow)
    {
        if(IsCodeBlock(line, out bool _))
        {
            activeCodeBlockLabel = null;
            gridRow++;
        }
        else
        {
            activeCodeBlockLabel.Text += (string.IsNullOrWhiteSpace(activeCodeBlockLabel.Text) ? "" : "\n") + line;
        }
    }

    private void AddEmptyRow(Grid grid, ref int gridRow)
    {
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) });
        gridRow++;
    }

    private static bool IsHorizontalRule(string line)
    {
        string compactLine = line.Replace(" ", string.Empty);

        return compactLine.Length >= 3 &&
               (compactLine.All(c => c == '-') || compactLine.All(c => c == '*') || compactLine.All(c => c == '_'));
    }

    private static bool IsHeadline(string line, out int level)
    {
        level = 0;
        line = line.TrimStart();
        while (level < line.Length && line[level] == '#')
        {
            level++;
        }
        bool isHeadline = level > 0 && level < 7 && line.Length > level && line[level] == ' ';

        if (!isHeadline)
        {
            level = 0;
        }
        return isHeadline;
    }

    private static bool IsUnorderedList(string line)
    {
        string trimmedLine = line.TrimStart();

        return trimmedLine.StartsWith("- ") || trimmedLine.StartsWith("* ") || trimmedLine.StartsWith("+ ");
    }

    private static bool IsBlockQuote(string line)
    {
        string trimmedLine = line.TrimStart();

        return trimmedLine.StartsWith('>');
    }

    private static bool IsImage(string line)
    {
        string trimmedLine = line.TrimStart();

        return trimmedLine.StartsWith("![");
    }

    private static bool IsCodeBlock(string line, out bool isSingleLineCodeBlock)
    {
        string trimmedLine = line.Trim();
        isSingleLineCodeBlock = trimmedLine.Count(x => x == '`') >= 6 && trimmedLine.EndsWith("```", StringComparison.Ordinal);

        return trimmedLine.StartsWith("```", StringComparison.Ordinal);
    }

    private static bool IsOrderedList(string line, out int listItemIndex)
    {
        listItemIndex = 0;
        string trimmedLine = line.TrimStart();

        var match = Regex.Match(trimmedLine, @"^(\d+)\. ");
        if (match.Success)
        {
            listItemIndex = int.Parse(match.Groups[1].Value);
            return true;
        }

        return false;
    }

    private static bool IsTable(string[] lines, int currentIndex, out int tableEndIndex)
    {
        tableEndIndex = currentIndex;
        if (!lines[currentIndex].Contains('|'))
            return false;

        for (int i = currentIndex + 1; i < lines.Length; i++)
        {
            if (!lines[i].Contains('|'))
            {
                tableEndIndex = i - 1;
                return true;
            }
        }

        tableEndIndex = lines.Length - 1;
        return true;
    }

    private Image CreateImageBlock(string line)
    {
        int startIndex = line.IndexOf('(') + 1;
        int endIndex = line.IndexOf(')', startIndex);
        string imageUrl = line[startIndex..endIndex];

        var image = new Image
        {
            Aspect = ImageAspect,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            Margin = new Thickness(0),
        };

        LoadImageAsync(imageUrl).ContinueWith(task =>
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                var imageSource = task.Result;
                MainThread.BeginInvokeOnMainThread(() => image.Source = imageSource);
            }
        });

        return image;
    }

    private Frame CreateCodeBlock(string codeText, out Label contentLabel)
    {
        Label content = new()
        {
            Text = codeText.Trim('`', ' '),
            FontSize = CodeBlockFontSize,
            FontAutoScalingEnabled = true,
            FontFamily = CodeBlockFontFace,
            TextColor = CodeBlockTextColor,
            BackgroundColor = Colors.Transparent
        };
        contentLabel = content;
        return new Frame
        {
            Padding = new Thickness(10),
            CornerRadius = 4,
            BackgroundColor = CodeBlockBackgroundColor,
            BorderColor = CodeBlockBorderColor,
            Content = content
        };
    }

    //private FormattedString CreateFormattedString(string line, Color textColor)
    //{
    //    var formattedString = new FormattedString();

    //    var parts = Regex.Split(line, @"(\*\*.*?\*\*|__.*?__|_.*?_|~~.*?~~|`.*?`|\[.*?\]\(.*?\)|\*.*?\*)");

    //    foreach (var part in parts)
    //    {
    //        Span span = new();

    //        if (part.StartsWith("`") && part.EndsWith("`"))
    //        {
    //            span.Text = part.Trim('`');
    //            span.BackgroundColor = CodeBlockBackgroundColor;
    //            span.FontFamily = CodeBlockFontFace;
    //            span.TextColor = CodeBlockTextColor;
    //        }
    //        else if (part.StartsWith("**") && part.EndsWith("**"))
    //        {
    //            span.Text = part.Trim('*', ' ');
    //            span.FontAttributes = FontAttributes.Bold;
    //            span.TextColor = textColor;
    //            span.FontFamily = TextFontFace;
    //        }
    //        else if (part.StartsWith("__") && part.EndsWith("__"))
    //        {
    //            span.Text = part.Trim('_', ' ');
    //            span.FontAttributes = FontAttributes.Bold;
    //            span.TextColor = textColor;
    //            span.FontFamily = TextFontFace;
    //        }
    //        else if (part.StartsWith('_') && part.EndsWith('_'))
    //        {
    //            span.Text = part.Trim('_', ' ');
    //            span.FontAttributes = FontAttributes.Italic;
    //            span.TextColor = textColor;
    //            span.FontFamily = TextFontFace;
    //        }
    //        else if (part.StartsWith('*') && part.EndsWith('*'))
    //        {
    //            span.Text = part.Trim('*', ' ');
    //            span.FontAttributes = FontAttributes.Italic;
    //            span.TextColor = textColor;
    //            span.FontFamily = TextFontFace;
    //        }
    //        else if (part.StartsWith("~~") && part.EndsWith("~~")) // StrikeThrough detection
    //        {
    //            span.Text = part.Trim('~', ' ');
    //            span.TextDecorations = TextDecorations.Strikethrough;
    //            span.TextColor = textColor;
    //            span.FontFamily = TextFontFace;
    //        }
    //        else if (part.StartsWith('[') && part.Contains("](")) // Link detection
    //        {
    //            var linkText = part[1..part.IndexOf(']')];
    //            var linkUrl = part.Substring(part.IndexOf('(') + 1, part.IndexOf(')') - part.IndexOf('(') - 1);

    //            span.Text = linkText;
    //            span.TextColor = HyperlinkColor;
    //            span.TextDecorations = TextDecorations.Underline;
    //            span.FontFamily = TextFontFace;

    //            var linkTapGestureRecognizer = new TapGestureRecognizer();
    //            linkTapGestureRecognizer.Tapped += (_, _) => TriggerHyperLinkClicked(linkUrl);
    //            span.GestureRecognizers.Add(linkTapGestureRecognizer);
    //        }
    //        else
    //        {
    //            span.Text = part;
    //            span.TextColor = textColor;
    //            span.FontFamily = TextFontFace;
    //        }

    //        span.FontSize = TextFontSize;

    //        formattedString.Spans.Add(span);
    //    }

    //    return formattedString;
    //}

    private FormattedString CreateFormattedString(string line, Color textColor)
    {
        var formattedString = new FormattedString();
        var parts = Regex.Split(line, @"(\*\*.*?\*\*|__.*?__|_.*?_|~~.*?~~|`.*?`|\[.*?\]\(.*?\)|\*.*?\*)");

        foreach (var part in parts)
        {
            Span span = new Span();

            if (part.StartsWith("`") && part.EndsWith("`"))
            {
                span.Text = part.Trim('`');
                span.BackgroundColor = CodeBlockBackgroundColor;
                span.FontFamily = CodeBlockFontFace;
                span.TextColor = CodeBlockTextColor;
            }
            else if (part.StartsWith("**") && part.EndsWith("**"))
            {
                var nestedFormatted = CreateFormattedString(part.Trim('*', ' '), textColor);
                foreach (var nestedSpan in nestedFormatted.Spans)
                {
                    nestedSpan.FontAttributes = FontAttributes.Bold;
                    formattedString.Spans.Add(nestedSpan);
                }
                continue;
            }
            else if (part.StartsWith("__") && part.EndsWith("__"))
            {
                span.Text = part.Trim('_', ' ');
                span.FontAttributes = FontAttributes.Bold;
            }
            else if (part.StartsWith('_') && part.EndsWith('_'))
            {
                span.Text = part.Trim('_', ' ');
                span.FontAttributes = FontAttributes.Italic;
            }
            else if (part.StartsWith("~~") && part.EndsWith("~~"))
            {
                span.Text = part.Trim('~');
                span.TextDecorations = TextDecorations.Strikethrough;
            }
            else if (part.StartsWith('[') && part.Contains("](")) // Detect link
            {
                var linkText = part.Substring(1, part.IndexOf(']') - 1);
                var linkUrl = part.Substring(part.IndexOf('(') + 1, part.IndexOf(')') - part.IndexOf('(') - 1);

                span.Text = linkText;
                span.TextColor = HyperlinkColor;
                span.TextDecorations = TextDecorations.Underline;
                var linkTapGestureRecognizer = new TapGestureRecognizer();
                linkTapGestureRecognizer.Tapped += (_, _) => TriggerHyperLinkClicked(linkUrl);
                span.GestureRecognizers.Add(linkTapGestureRecognizer);
            }
            else if (part.StartsWith('*') && part.EndsWith('*'))
            {
                span.Text = part.Trim('*');
                span.FontAttributes = FontAttributes.Italic;
            }
            else
            {
                span.Text = part;
            }

            span.FontSize = TextFontSize;
            span.TextColor = textColor;
            span.FontFamily = TextFontFace;

            formattedString.Spans.Add(span);
        }

        return formattedString;
    }


    private void AddBulletPointToGrid(Grid grid, int gridRow)
    {
        string bulletPointSign = "-";

#if ANDROID
        bulletPointSign = "\u2022";
#endif
#if iOS
        bulletPointSign = "\u2029";
#endif
        var bulletPoint = new Label
        {
            Text = bulletPointSign,
            FontSize = TextFontSize,
            FontFamily = TextFontFace,
            TextColor = TextColor,
            FontAutoScalingEnabled = true,
            VerticalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 0),
            Padding = new Thickness(0,0)
        };

        grid.Children.Add(bulletPoint);
        Grid.SetRow(bulletPoint, gridRow);
        Grid.SetColumn(bulletPoint, 0);
    }

    private void AddOrderedListItemToGrid(int listItemIndex, Grid grid, int gridRow)
    {
        var orderedListItem = new Label
        {
            Text = $"{listItemIndex}.",
            FontSize = TextFontSize,
            FontFamily = TextFontFace,
            TextColor = TextColor,
            FontAutoScalingEnabled = true,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Start,
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0),
            Padding = new Thickness(0)
        };

        grid.Children.Add(orderedListItem);
        Grid.SetRow(orderedListItem, gridRow);
        Grid.SetColumn(orderedListItem, 0);
    }

    private void AddListItemTextToGrid(string listItemText, Grid grid, int gridRow)
    {
        var formattedString = CreateFormattedString(listItemText, TextColor);

        var listItemLabel = new Label
        {
            FormattedText = formattedString,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(15, 0, 0, 0) // Indent the list item text
        };

        grid.Children.Add(listItemLabel);
        Grid.SetRow(listItemLabel, gridRow);
        Grid.SetColumn(listItemLabel, 1);
    }

    private Grid CreateTable(string[] lines, int startIndex, int endIndex)
    {
        var tableGrid = new Grid
        {
            ColumnSpacing = 2,
            RowSpacing = 2,
            BackgroundColor = Colors.Transparent
        };

        var headerCells = lines[startIndex].Split('|').Select(cell => cell.Trim()).ToArray();
        for (int i = 0; i < headerCells.Length; i++)
        {
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int colIndex = 0; colIndex < headerCells.Length; colIndex++)
        {
            var headerLabel = new Label
            {
                Text = headerCells[colIndex],
                FontAttributes = FontAttributes.Bold,
                FontSize = TextFontSize,
                BackgroundColor = CodeBlockBackgroundColor,
                TextColor = CodeBlockTextColor,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(5)
            };
            tableGrid.Children.Add(headerLabel);
            Grid.SetColumn(headerLabel, colIndex);
            Grid.SetRow(headerLabel, 0);
        }

        int rowIndex = 1;
        for (int i = startIndex + 2; i <= endIndex; i++)
        {
            var rowCells = lines[i].Split('|').Select(cell => cell.Trim()).ToArray();
            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int colIndex = 0; colIndex < rowCells.Length; colIndex++)
            {
                var cellLabel = new Label
                {
                    Text = rowCells[colIndex],
                    FontSize = TextFontSize,
                    TextColor = TextColor,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(5)
                };
                tableGrid.Children.Add(cellLabel);
                Grid.SetColumn(cellLabel, colIndex);
                Grid.SetRow(cellLabel, rowIndex);
            }
            rowIndex++;
        }

        return tableGrid;
    }

    internal void TriggerHyperLinkClicked(string url)
    {
        OnHyperLinkClicked?.Invoke(this, new LinkEventArgs { Url = url });

        if (LinkCommand?.CanExecute(url) == true)
        {
            LinkCommand.Execute(url);
        }
    }

    private async Task<ImageSource> LoadImageAsync(string imageUrl)
    {
        ImageSource imageSource;

        try
        {
            if (System.Buffers.Text.Base64.IsValid(imageUrl))
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
                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(uriResult);
                        imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                        if (imageUrl != null) _imageCache[imageUrl] = imageSource;
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
            throw;
        }

        return imageSource ?? ImageSource.FromFile("icon.png");
    }

    ~MarkdownView()
    {
        _imageCache.Clear();
        _imageCache = null;
    }
}