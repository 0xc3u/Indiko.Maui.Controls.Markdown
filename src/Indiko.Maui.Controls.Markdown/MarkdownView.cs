using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml;
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
            view.RenderMarkdown(text);
    }

    private void RenderMarkdown(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseAlertBlocks()
            .UseAbbreviations()
            .UseEmojiAndSmiley()
            .UseGridTables()
            .UsePipeTables()
            .UseAutoIdentifiers()
            .UseEmphasisExtras()
            .UseDefinitionLists()
            .UseFootnotes()
            .UseListExtras()
            .UseCustomContainers()
            .UseCitations()
            .UseMediaLinks()
            .UseTaskLists()
            .UseEmphasisExtras()
            .UseAutoLinks()
            .UseFooters()
            .UseMathematics()
            .Build();

        MarkdownDocument document = Markdig.Markdown.Parse(markdown, pipeline);

        var layout = new VerticalStackLayout { 
            Margin = 0,
            Padding =0,
            Spacing = (8 * ParagraphSpacing)
        };

        foreach (var block in document)
        {
            if (RenderBlock(block) is View view)
                layout.Children.Add(view);
        }

        Content = layout;
    }

    private View? RenderBlock(Block block)
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


    private View RenderParagraph(ParagraphBlock block)
    {
        if (block.Inline?.FirstChild is LinkInline link && link.IsImage)
        {

            var image = new Image
            {
                Aspect = ImageAspect,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0),
            };

            LoadImageAsync(link.Url).ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    var imageSource = task.Result;
                    MainThread.BeginInvokeOnMainThread(() => image.Source = imageSource);
                }
            });

            return image;

        }

        return new Label
        {
            FormattedText = RenderInlines(block.Inline),
            LineBreakMode = LineBreakMode.WordWrap,
        };
    }

    private View RenderHeading(HeadingBlock block)
    {
        var formatted = new FormattedString();

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
                        FontAttributes = FontAttributes.Bold,
                        TextColor = GetTextColorForBlockLevel(block.Level),
                        FontFamily = TextFontFace
                    });
                }
                else if (inline is EmphasisInline em)
                {
                    var text = string.Concat(em.Select(x => (x as LiteralInline)?.Content.ToString()));
                    formatted.Spans.Add(new Span
                    {
                        Text = text,
                        FontSize = GetFontsizeForBlockLevel(block.Level),
                        FontAttributes = em.DelimiterCount == 2 ? FontAttributes.Bold : FontAttributes.Italic,
                        TextColor = GetTextColorForBlockLevel(block.Level),
                        FontFamily = TextFontFace
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
            LineBreakMode = LineBreakMode.WordWrap,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            LineHeight = LineHeightMultiplier,
        };
    }


    private Color GetTextColorForBlockLevel(int blockLevel)
    {
        if(blockLevel == 1)
            return H1Color;
        else if (blockLevel == 2)
            return H2Color;
        else if (blockLevel == 3)
            return H3Color;
        else
            return H3Color;
    }

    private double GetFontsizeForBlockLevel(int blockLevel)
    {
        if (blockLevel == 1)
            return H1FontSize;
        else if (blockLevel == 2)
            return H2FontSize;
        else if (blockLevel == 3)
            return H3FontSize;
        else
            return H3FontSize;
    }

    private View RenderQuote(QuoteBlock block)
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

    private View RenderCode(FencedCodeBlock block)
    {
        return new Border
        {
            BackgroundColor = CodeBlockBackgroundColor,
            Stroke = new SolidColorBrush(CodeBlockBorderColor),
            Padding = 8,
            StrokeShape = new RoundRectangle().WithCornerRadius(4),
            Content = new Label
            {
                Text = block.Lines.ToString(),
                FontFamily = CodeBlockFontFace,
                TextColor = CodeBlockTextColor,
                FontSize = CodeBlockFontSize,
                LineBreakMode = LineBreakMode.WordWrap,
            }
        };
    }

    private View RenderCodeBlock(CodeBlock block)
    {
        return new Border
        {
            BackgroundColor = CodeBlockBackgroundColor,
            Stroke = new SolidColorBrush(CodeBlockBorderColor),
            Padding = 8,
            StrokeShape = new RoundRectangle().WithCornerRadius(4),
            Content = new Label
            {
                Text = block.Lines.ToString(),
                FontFamily = CodeBlockFontFace,
                TextColor = CodeBlockTextColor,
                FontSize = CodeBlockFontSize,
                LineBreakMode = LineBreakMode.WordWrap,
            }
        };
    }

    private View RenderTable(Table table)
    {
        var grid = new Grid
        {
            ColumnSpacing = 1,
            RowSpacing = 1,
            BackgroundColor = Colors.Gray,
            HorizontalOptions = LayoutOptions.Fill
        };
        grid.HorizontalOptions = LayoutOptions.Fill;


        for (int i = 0; i < table.ColumnDefinitions.Count; i++)
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        int rowIndex = 0;

        foreach (TableRow row in table)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

            for (int colIndex = 0; colIndex < row.Count; colIndex++)
            {
                var cell = row[colIndex] as TableCell;


                var alignment = table.ColumnDefinitions[colIndex].Alignment;

                var horizontalTextAlignment = alignment switch
                {
                    TableColumnAlign.Center => TextAlignment.Center,
                    TableColumnAlign.Right => TextAlignment.End,
                    _ => TextAlignment.Start
                };

                var label = new Label
                {
                    FormattedText = RenderInlines((cell?.FirstOrDefault() as ParagraphBlock)?.Inline),
                    BackgroundColor = row.IsHeader ? TableHeaderBackgroundColor : TableRowBackgroundColor,
                    FontAttributes = row.IsHeader ? FontAttributes.Bold : FontAttributes.None,
                    TextColor = row.IsHeader ? TableHeaderTextColor : TableRowTextColor,
                    FontFamily = row.IsHeader ? TableHeaderFontFace : TableRowFontFace,
                    FontSize = row.IsHeader ? TableHeaderFontSize : TableRowFontSize,
                    Padding = 4,
                    HorizontalTextAlignment = horizontalTextAlignment,
                    LineBreakMode = row.IsHeader ? LineBreakMode.TailTruncation : LineBreakMode.WordWrap,
                };

                label.HorizontalOptions = LayoutOptions.Fill;
                label.VerticalOptions = LayoutOptions.Fill;

                grid.Add(label, colIndex, rowIndex);
            }

            rowIndex++;
        }

        return grid;
    }

    private View RenderFormula(MathBlock mathBlock)
    {
        string formularText  = mathBlock.Lines.ToString();

        var grid = new Grid
        {
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto },
            new ColumnDefinition { Width = GridLength.Auto }
        },
            RowDefinitions =
        {
            new RowDefinition { Height = GridLength.Auto }
        }
        };

        var latexView = new LatexView
        {
            Text = formularText,
            FontSize = (float)TextFontSize * 4,
            TextColor = TextColor,
            HighlightColor = Colors.Transparent,
            ErrorColor = Colors.Red,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(-10, -10)
        };

        return latexView;
    }
    
    private View RenderCustomContainer(CustomContainer container)
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

    private View RenderList(ListBlock listBlock, int nestingLevel = 0)
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
                            var prefix = listBlock.IsOrdered ? $"{item.Order + 1}." : "•";
                            var row = new HorizontalStackLayout
                            {
                                Spacing = 8,
                                Children =
                                {
                                    new Label
                                    {
                                        Text = prefix,
                                        FontAttributes = FontAttributes.Bold,
                                        VerticalOptions = LayoutOptions.Start,
                                        LineBreakMode = LineBreakMode.WordWrap,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    content
                                }
                            };
                            stack.Children.Add(row);
                        }
                    }
                }
            }
        }

        return stack;
    }

    private FormattedString RenderInlines(ContainerInline? inlines)
    {
        var formatted = new FormattedString();

        if (inlines == null) return formatted;

        foreach (var inline in inlines)
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
                    formatted.Spans.Add(new Span { Text = "\n",
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
                    else if (EmailRegex.IsMatch(link.Url)) // Detect email addresses
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
                        TextColor = Colors.DarkOliveGreen, // or bindable
                        FontFamily = TextFontFace,
                        FontSize = TextFontSize
                    });
                    break;
            }
        }

        return formatted;
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
                        if (imageUrl.ToLowerInvariant().EndsWith(".svg"))
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
            throw;
        }

        return imageSource ?? ImageSource.FromFile("icon.png");
    }

    internal void TriggerHyperLinkClicked(string url)
    {
        OnHyperLinkClicked?.Invoke(this, new LinkEventArgs { Url = url });

        if (LinkCommand?.CanExecute(url) == true)
        {
            LinkCommand.Execute(url);
        }
    }

    private void TriggerEmailClicked(string email)
    {
        OnEmailClicked?.Invoke(this, new EmailEventArgs { Email = email });

        if (EMailCommand?.CanExecute(email) == true)
        {
            EMailCommand.Execute(email);
        }
    }

    ~MarkdownView()
    {
        _imageCache.Clear();
        _imageCache = null;
    }
}