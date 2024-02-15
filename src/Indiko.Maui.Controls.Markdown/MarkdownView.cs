using Indiko.Maui.Controls.Markdown.Utils;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Indiko.Maui.Controls.Markdown;

public class MarkdownView : ContentView
{
    // MarkdownText property
    public static readonly BindableProperty MarkdownTextProperty =
        BindableProperty.Create(nameof(MarkdownText), typeof(string), typeof(MarkdownView), default(string), propertyChanged: OnMarkdownTextChanged);

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

    // H1Color property
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

    // H2Color property
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

    public static readonly BindableProperty PlaceholderBackgroundColorProperty =
     BindableProperty.Create(nameof(PlaceholderBackgroundColor), typeof(Color), typeof(MarkdownView), Colors.White, propertyChanged: OnMarkdownTextChanged);

    public Color PlaceholderBackgroundColor
    {
        get => (Color)GetValue(PlaceholderBackgroundColorProperty);
        set => SetValue(PlaceholderBackgroundColorProperty, value);
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
            Margin = new Thickness(0,0,0,0),
            Padding = new Thickness(0, 0, 0, 0),
            RowSpacing = 2,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = 5 }, // For bullet points
                new ColumnDefinition { Width = GridLength.Star } // For text
            }
        };

        // Split markdown text by new lines correctly
        var lineSplitSymbole = "\n";

        if (MarkdownText.Contains("\r\n"))
        {
            lineSplitSymbole = "\r\n";
        }
        else if (MarkdownText.Contains("\r"))
        {
            lineSplitSymbole = "\\r";
        }
        else
        {
            lineSplitSymbole = "\\n";
        }

        var lines = MarkdownText.Split(new[] { lineSplitSymbole }, StringSplitOptions.RemoveEmptyEntries);

        int gridRow = 0;
        bool isUnorderedListActive = false;


        foreach (var line in lines.Select(line => line.Trim()))
        {
            // Add a new RowDefinition for the content
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            if (line.StartsWith("# ") || line.StartsWith("## ") || line.StartsWith("### "))
            {
                var label = new Label
                {
                    Text = line[(line.IndexOf(' ') + 1)..].Trim(),
                    TextColor = line.StartsWith("# ") ? H1Color : line.StartsWith("## ") ? H2Color : H3Color,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = line.StartsWith("# ") ? H1FontSize : line.StartsWith("## ") ? H2FontSize : H3FontSize,
                    LineBreakMode = LineBreakModeHeader,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                };
                grid.Children.Add(label);
                Grid.SetColumnSpan(label, 2);
                Grid.SetRow(label, gridRow++);
            }
            else if (line.StartsWith("!["))
            {
                var image = CreateImageBlock(line);

                if(image==null)
                {
                    continue;
                }

                grid.Children.Add(image);
                Grid.SetColumnSpan(image, 2);
                Grid.SetRow(image, gridRow++);
            }
            else if (line.StartsWith(">"))
            {
                var box = new Frame
                {
                    Margin = new Thickness(0),
                    BackgroundColor = CodeBlockBorderColor,
                    BorderColor = CodeBlockBorderColor,
                    CornerRadius = 0,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };

                var blockQuotelabel = new Label
                {
                    FormattedText = CreateFormattedString(line.Substring(1).Trim(), CodeBlockTextColor),
                    LineBreakMode = LineBreakModeText,
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
                        new ColumnDefinition { Width = 5 }, // For bullet points
                        new ColumnDefinition { Width = GridLength.Star } // For text
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
                    BackgroundColor = CodeBlockBackgroundColor,
                    BorderColor = CodeBlockBackgroundColor,
                    Content  = blockQuoteGrid
                };

                grid.Children.Add(blockquote);
                Grid.SetColumnSpan(blockquote, 2);
                Grid.SetRow(blockquote, gridRow++);

               

            }
            else if (line.StartsWith("- ") || line.StartsWith("* "))
            {
                if (!isUnorderedListActive)
                {
                    isUnorderedListActive = true;
                }

                var bulletPoint = new Label
                {
                    Text = "\u2022",
                    FontSize = 14,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(5, 0)
                };

                grid.Children.Add(bulletPoint);
                Grid.SetRow(bulletPoint, gridRow);
                Grid.SetColumn(bulletPoint, 0);

                var listItemText = line.Substring(2); // Remove the "- " or "* " marker
                var formattedString = CreateFormattedString(listItemText, TextColor); // Use a method to handle markdown within the list item

                var listItemLabel = new Label
                {
                    FormattedText = formattedString,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(20, 0, 0, 0) // Indent the list item text
                };

                grid.Children.Add(listItemLabel);
                Grid.SetRow(listItemLabel, gridRow);
                Grid.SetColumn(listItemLabel, 1);

                gridRow++;
            }
            else if (line.StartsWith("```") && line.EndsWith("```"))
            {
                var codeBlock = CreateCodeBlock(line);
                grid.Children.Add(codeBlock);
                Grid.SetRow(codeBlock, gridRow);
                Grid.SetColumnSpan(codeBlock, 2);
                gridRow++;
            }
            else // Regular text
            {
                if (isUnorderedListActive)
                {
                    isUnorderedListActive = false;
                    // Optionally add extra space after a list
                    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    grid.Children.Add(new BoxView { Color = Colors.Transparent });
                    gridRow++;
                }

                // Handle regular text and potential formatting
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
                Grid.SetColumnSpan(label, 2); // Span across both columns for normal text

                gridRow++;

                // After handling an element, add an empty row for space
                AddEmptyRow(grid, ref gridRow);
            }
        }

        Content = grid;
    }

    private void AddEmptyRow(Grid grid, ref int gridRow)
    {
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10) }); // Adjust the space as needed
        var spacer = new BoxView { Color = PlaceholderBackgroundColor };
        grid.Children.Add(spacer);
        Grid.SetColumnSpan(spacer, 2);
        Grid.SetRow(spacer, gridRow++);
    }

    private static Image CreateImageBlock(string line)
    {
        try
        {
            // Image handling
            int startIndex = line.IndexOf('(') + 1;
            int endIndex = line.IndexOf(')', startIndex);
            string imageUrl = line.Substring(startIndex, endIndex - startIndex);

            ImageSource imageSource = default;

            // check if imageUrl is a base64 string
            if (Validations.IsValidBase64String(imageUrl))
            {
                byte[] imageBytes = Convert.FromBase64String(imageUrl);
                imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            else if (Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult))
            {
                imageSource = ImageSource.FromUri(uriResult);
            }
            else
            {
                imageSource = ImageSource.FromFile(imageUrl);
            }

            var image = new Image
            {
                Source = imageSource,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = new Thickness(0, 0, 0, 0)
            };

            return image;
        }catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return default;
    }

    private Frame CreateCodeBlock(string codeText)
    {
        return new Frame
        {
            Padding = new Thickness(10),
            CornerRadius = 4,
            BackgroundColor = CodeBlockBackgroundColor,
            BorderColor = CodeBlockBorderColor,
            Content = new Label
            {
                Text = codeText.Trim('`', ' '),
                FontSize = CodeBlockFontSize,
                FontAutoScalingEnabled = true,
                FontFamily = "Consolas", // Use a monospaced font family
                TextColor = CodeBlockTextColor, // Use a suitable text color for code
                BackgroundColor = Colors.Transparent
            }
        };
    }

    private FormattedString CreateFormattedString(string line, Color textColor)
    {
        // Create a new FormattedString
        var formattedString = new FormattedString();

        // This is a simple parser and will not handle nested or incorrect markdown well.
        // You may want to use or develop a more robust parser for complex markdown.
        var parts = Regex.Split(line, @"(\*\*.*?\*\*|__.*?__|_.*?_|`.*?`)");

        foreach (var part in parts)
        {
            Span span = new Span();

            if (part.StartsWith("**") && part.EndsWith("**"))
            {
                span.Text = part.Trim('*', ' ');
                span.FontAttributes = FontAttributes.Bold;
            }
            else if (part.StartsWith("_") && part.EndsWith("_"))
            {
                span.Text = part.Trim('_', ' ');
                span.FontAttributes = FontAttributes.Italic;
            }
            else if (part.StartsWith("__") && part.EndsWith("__"))
            {
                span.Text = part.Trim('_', ' ');
                span.FontAttributes = FontAttributes.Bold;
                // If you want bold and italic, you can combine flags:
                // span.FontAttributes = FontAttributes.Bold | FontAttributes.Italic;
            }
            else
            {
                span.Text = part;
            }

            // Apply the same text color for all spans here, or customize as needed.
            span.TextColor = textColor;
            span.FontSize = TextFontSize;

            formattedString.Spans.Add(span);
        }

        return formattedString;
    }   
}