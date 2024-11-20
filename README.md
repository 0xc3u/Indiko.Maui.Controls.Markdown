![Indiko.Maui.Controls.Markdown](nuget.png)

# Indiko.Maui.Controls.Markdown

The `MarkdownView` control is a flexible component designed for MAUI applications to display and style Markdown content with ease. This control supports various Markdown syntax elements such as headings, blockquotes, code blocks, images, tables, and hyperlinks. It offers extensive customization options via bindable properties and allows you to tailor the appearance of different Markdown elements, making it perfect for integrating rich text content into your app.

## Build Status
![ci](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/actions/workflows/ci.yml/badge.svg)

## Installation

You can install the `Indiko.Maui.Controls.Markdown` package via NuGet Package Manager or CLI:

[![NuGet](https://img.shields.io/nuget/v/Indiko.Maui.Controls.Markdown.svg?label=NuGet)](https://www.nuget.org/packages/Indiko.Maui.Controls.Markdown/)

### NuGet Package Manager
```bash
Install-Package Indiko.Maui.Controls.Markdown
```

### .NET CLI
```bash
dotnet add package Indiko.Maui.Controls.Markdown
```

## Bindable Properties

The following is a list of all customizable bindable properties:

### Headings
- **`H1FontSize`**: The font size for H1 headings (default: `24`).
- **`H1Color`**: The color for H1 headings (default: `Black`).
- **`H2FontSize`**: The font size for H2 headings (default: `20`).
- **`H2Color`**: The color for H2 headings (default: `DarkGray`).
- **`H3FontSize`**: The font size for H3 headings (default: `18`).
- **`H3Color`**: The color for H3 headings (default: `Gray`).

### Text Styling
- **`TextFontSize`**: The font size for regular text (default: `12`).
- **`TextColor`**: The color for regular text (default: `Black`).
- **`TextFontFace`**: The font family for regular text.

### Line Break Mode
- **`LineBreakModeText`**: Line break mode for text (default: `WordWrap`).
- **`LineBreakModeHeader`**: Line break mode for headers (default: `TailTruncation`).

### Blockquote
- **`BlockQuoteBackgroundColor`**: The background color for blockquote elements (default: `LightGray`).
- **`BlockQuoteBorderColor`**: The border color for blockquote elements (default: `BlueViolet`).
- **`BlockQuoteTextColor`**: The text color for blockquotes (default: `BlueViolet`).
- **`BlockQuoteFontFace`**: The font family for blockquote text.

### Code Block
- **`CodeBlockBackgroundColor`**: The background color for code blocks (default: `LightGray`).
- **`CodeBlockBorderColor`**: The border color for code blocks (default: `BlueViolet`).
- **`CodeBlockTextColor`**: The text color for code blocks (default: `BlueViolet`).
- **`CodeBlockFontFace`**: The font family for code blocks.
- **`CodeBlockFontSize`**: The font size for code blocks (default: `12`).

### Table
- **`TableHeaderFontSize`**: The font size for table headers (default: `14`).
- **`TableHeaderTextColor`**: The text color for table headers (default: `Black`).
- **`TableHeaderBackgroundColor`**: The background color for table headers (default: `LightGray`).
- **`TableHeaderFontFace`**: The font family for table headers.
- **`TableRowFontSize`**: The font size for table rows (default: `12`).
- **`TableRowTextColor`**: The text color for table rows (default: `Black`).
- **`TableRowFontFace`**: The font family for table rows.

### Image
- **`ImageAspect`**: The aspect ratio for images (default: `AspectFit`).

### Hyperlinks
- **`HyperlinkColor`**: The color for hyperlinks (default: `BlueViolet`).
- **`LinkCommand`**: A command to execute when a hyperlink is clicked.
- **`LinkCommandParameter`**: A parameter to pass when the hyperlink command is executed.

### Horizontal Rule
- **`LineColor`**: The color for horizontal rules (default: `LightGray`).

### Spacing
- **`ParagraphSpacing`**: Modifies the spacing between paragraphs (default: `3`).
- **`LineHeightMultiplier`** Modifies the line height multiplier for displayed labels (default: `1`). Directly modifies [Label.LineHeight](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.label.lineheight?view=net-maui-8.0).

## Supported Markdown Syntax

- **Headings**: MarkdownView supports headings from `H1` to `H6`.
    ```markdown
    # H1
    ## H2
    ### H3
    ```

- **Bold**: Wrap text with `**` or `__` to make it bold.
    ```markdown
    **Bold Text**
    __Bold Text__
    ```

- **Italic**: Wrap text with `*` or `_` to make it italic.
    ```markdown
    *Italic Text*
    _Italic Text_
    ```

- **Strikethrough**: Wrap text with `~~` to create strikethrough text.
    ```markdown
    ~~Strikethrough Text~~
    ```

- **Blockquotes**: Add `>` before a paragraph to create a blockquote.
    ```markdown
    > This is a blockquote.
    ```

- **Code Blocks**: Enclose code in triple backticks (```) for multi-line code blocks, or single backticks for inline code.
    ```markdown
    `Inline code`
    ```
    ```markdown
    ```
    Multi-line code block
    ```
    ```

- **Lists**:
    - Unordered: Use `-`, `*`, or `+` to create an unordered list.
    - Ordered: Use numbers followed by a period to create an ordered list.

    ```markdown
    - Item 1
    - Item 2
    1. Item 1
    2. Item 2
    ```

- **Tables**: Create tables using pipes (`|`) to separate columns. You can also specify text alignment for table columns using colons (`:`).

    - **Left Alignment**: Add a colon on the left side of the dashes.
    - **Center Alignment**: Add a colon on both sides of the dashes.
    - **Right Alignment**: Add a colon on the right side of the dashes.

    ```markdown
    | Header 1      | Header 2      | Header 3      |
    |:------------- |:-------------:| -------------:|
    | Left aligned  | Center aligned| Right aligned |
    | Row 1         | Data 1        | Data 1        |
    | Row 2         | Data 2        | Data 2        |
    ```

    In the above example:
    - Column 1 (Header 1) is left-aligned.
    - Column 2 (Header 2) is center-aligned.
    - Column 3 (Header 3) is right-aligned.

- **Horizontal Rule**: Use `---`, `***`, or `___` for horizontal rules.
    ```markdown
    ---
    ```

- **Images**: The control supports image URLs, local files, and base64 encoded images.
    ```markdown
    ![Alt text](http://example.com/image.jpg) // Image URL
    ![Alt text](image.png) // Local file
    ![Alt text](data:image/png;base64,...) // Base64 encoded string
    ```

- **Hyperlinks**: Create hyperlinks using the following format:
    ```markdown
    [Link Text](http://example.com)
    ```

## Image and Hyperlink Handling

### Image Handling
The `MarkdownView` supports various sources for displaying images:
1. **Image URLs**: Fetch and display images from the web.
2. **Local File**: Load images from local resources or assets.
3. **Base64 Encoded String**: Support for images encoded in base64 format.

The `ImageAspect` property allows you to customize how images are displayed within the control.

### Hyperlink Handling
You can respond to hyperlinks in Markdown content using the `LinkCommand` and `OnHyperLinkClicked` event. Hyperlinks will also display with the color specified by the `HyperlinkColor` property.

## Example Usage

Here's an example of how to use the `MarkdownView` in your XAML:

```xml
<idk:MarkdownView 
    MarkdownText="{Binding MarkdownText}" 
    H1FontSize="20"
    H1Color="{StaticResource Blue100Accent}" 
    H2FontSize="18"
    H2Color="{StaticResource Blue200Accent}"
    H3FontSize="16"
    H3Color="{StaticResource Blue300Accent}"
    CodeBlockBackgroundColor="{StaticResource GrayQuote}"
    CodeBlockTextColor="{StaticResource Gray600}"
    CodeBlockBorderColor="{StaticResource GrayQuoteBorder}"
    CodeBlockFontFace="CamingoCodeRegular"
    CodeBlockFontSize="12"
    BlockQuoteBackgroundColor="{StaticResource Yellow300Accent}"
    BlockQuoteTextColor="{StaticResource Gray600}"
    BlockQuoteBorderColor="{StaticResource Yellow100Accent}"
    BlockQuoteFontFace="CamingoCodeItalic"
    TextFontFace="OpenSans"
    TextFontSize="13"
    TextColor="{StaticResource Black}"
    TableHeaderBackgroundColor="{StaticResource Gray100}"
    TableHeaderFontFace="OpenSans"
    TableHeaderFontSize="13"
    TableHeaderTextColor="{StaticResource Gray900}"
    TableRowFontFace="OpenSans"
    TableRowFontSize="11"
    TableRowTextColor="{StaticResource Gray600}"
    ImageAspect="Fill"
    HyperlinkColor="{StaticResource Blue100Accent}"
    LineColor="{StaticResource GrayQuoteBorder}"
    LinkCommand="{Binding LinkReceivedCommand}"
    OnHyperLinkClicked="MarkdownView_HyperLinkClicked"
    LineHeightMultiplier="1.2"
    ParagraphSpacing=1>
</idk:MarkdownView>
```

Here’s an example of how to use the `MarkdownView` in your c#:

```csharp

 var markdownView = new MarkdownView
        {
            MarkdownText = "# Welcome to MarkdownView\n" +
                          "This is **bold text**, and this is *italic text*.\n\n" +
                          "Here's a blockquote:\n\n" +
                          "> This is a blockquote\n\n" +
                          "Here's a list:\n" +
                          "- Item 1\n" +
                          "- Item 2\n\n" +
                          "Here's a code block:\n\n" +
                          "```\n" +
                          "var code = \"This is a code block\";\n" +
                          "```\n\n" +
                          "Here's a link: [Click here](https://example.com)\n\n" +
                          "Here's an image:\n" +
                          "![Alt text](https://example.com/image.jpg)\n",

            H1FontSize = 24,
            H1Color = Colors.Blue,
            H2FontSize = 20,
            H2Color = Colors.DarkGray,
            H3FontSize = 18,
            H3Color = Colors.Gray,
            TextFontSize = 14,
            TextColor = Colors.Black,
            BlockQuoteBackgroundColor = Colors.LightYellow,
            BlockQuoteTextColor = Colors.Gray,
            BlockQuoteBorderColor = Colors.DarkGray,
            CodeBlockBackgroundColor = Colors.LightGray,
            CodeBlockTextColor = Colors.Purple,
            CodeBlockFontSize = 12,
            CodeBlockFontFace = "Consolas",
            HyperlinkColor = Colors.BlueViolet,
            LineColor = Colors.Gray,
            ImageAspect = Aspect.AspectFit,
            TableHeaderBackgroundColor = Colors.LightGray,
            TableHeaderFontSize = 14,
            TableHeaderTextColor = Colors.Black,
            TableRowFontSize = 12,
            TableRowTextColor = Colors.DarkGray,
            ParagraphSpacing = 1,
            LineHeightMultiplier = 1.2,
        };

        markdownView.OnHyperLinkClicked += (sender, e) =>
        {
            DisplayAlert("Link Clicked", $"You clicked on: {e.Url}", "OK");
        };
```

## Contributing

Contributions to the `MarkdownView` project are very welcome! Whether you want to add new features, improve existing ones, fix bugs, or enhance documentation, your help is highly appreciated.

### How to Contribute

1. **Fork the Repository**: Start by forking the repository on GitHub.
2. **Clone Your Fork**: Clone the forked repository to your local machine.
3. **Create a Branch**: Create a new branch for your feature or bugfix.
    ```bash
    git checkout -b feature/new-feature
    ```
4. **Add Your Changes**: Make your changes or add new features. Be sure to include any necessary tests and documentation updates.
5. **Commit Your Changes**: Use clear and descriptive commit messages.
    ```bash
    git commit -m "Add support for new markdown feature"
    ```
6. **Push the Changes**: Push your branch to GitHub.
    ```bash
    git push origin feature/new-feature
    ```
7. **Create a Pull Request**: Submit a pull request (PR) from your branch to the main repository. Provide a detailed description of what your changes do and how they improve the control.

We encourage contributions that enhance the control, such as:
- Support for additional Markdown syntax.
- New styling properties for customization.
- Performance improvements.
- Bug fixes.

### Guidelines

- Ensure that your code follows good practices and is clean.
- Update the documentation to reflect any new features or changes.

If you have any ideas or questions about contributing, feel free to open an issue on GitHub to discuss it with the maintainers.

We look forward to your contributions!
