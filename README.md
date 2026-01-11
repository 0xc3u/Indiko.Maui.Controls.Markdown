![Indiko.Maui.Controls.Markdown](nuget.png)

# Indiko.Maui.Controls.Markdown

The `MarkdownView` control is a flexible component designed for MAUI applications to display and style Markdown content with ease. This control supports various Markdown syntax elements such as headings, blockquotes, code blocks, images, tables, and hyperlinks. It offers extensive customization options via bindable properties and allows you to tailor the appearance of different Markdown elements, making it perfect for integrating rich text content into your app.

![markdownview_screenshots](https://github.com/user-attachments/assets/2485eedb-015d-4ccb-acc2-c19d24ea51d7)

## Build Status
![ci](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/actions/workflows/ci.yml/badge.svg)

## ⚠️ Breaking Changes

### Version 1.5.0 and above

Version 1.5.0 introduces a new **Theming System** that changes how styling is applied to the MarkdownView. If you are upgrading from a version prior to 1.4.0, please note the following changes:

#### New Theme Property
- A new `Theme` property has been added that accepts a `MarkdownTheme` object
- The `UseAppTheme` property enables automatic light/dark mode switching based on system theme

#### New Heading Properties
- Individual `FontFamily` and `FontAttributes` properties have been added for each heading level (H1-H6)
- New properties: `H1FontFamily`, `H1FontAttributes`, `H2FontFamily`, `H2FontAttributes`, etc.
- New heading colors for H4-H6: `H4Color`, `H5Color`, `H6Color`

#### Migration Guide

**Before (< 1.5.0):**
```xml
<idk:MarkdownView 
    H1Color="Blue"
    H1FontSize="24"
    TextFontFace="Arial" />
```

**After (>= 1.5.0) - Using Theme (Recommended):**
```xml
<idk:MarkdownView 
    Theme="{Binding CurrentTheme}"
    UseAppTheme="True" />
```

```csharp
// In your ViewModel or code-behind
CurrentTheme = MarkdownThemeDefaults.GitHub;
```

**After (>= 1.5.0) - Using Individual Properties (Still Supported):**
```xml
<idk:MarkdownView 
    H1Color="Blue"
    H1FontSize="24"
    H1FontFamily="Arial"
    H1FontAttributes="Bold"
    TextFontFace="Arial" />
```

> **Note:** The existing individual styling properties (like `H1Color`, `H1FontSize`, etc.) are still fully supported. The new theming system is an additional feature that provides a more organized way to manage styles. You can continue using individual properties if you prefer.

---

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

## Theming Support

The `MarkdownView` control includes a powerful theming system inspired by MudBlazor's approach. Themes allow you to define a complete visual style for your markdown content, including colors, typography, and spacing, all in one reusable object.

### Theme Structure

A theme consists of three main components:

- **`Palette`**: Color palette for light mode
- **`PaletteDark`**: Color palette for dark mode (optional, falls back to Palette)
- **`Typography`**: Font sizes, font families, line heights, and spacing

### Using Built-in Themes

The library includes several pre-built themes that you can use out of the box:

| Theme | Description |
|-------|-------------|
| `Light` | Default light theme with neutral colors |
| `Dark` | Dark theme optimized for dark backgrounds |
| `GitHub` | GitHub-styled markdown appearance |
| `OneDark` | Atom's One Dark syntax theme |
| `OneLight` | Atom's One Light syntax theme |
| `Dracula` | Popular Dracula color scheme |
| `Nord` | Arctic, north-bluish color palette |
| `Sepia` | Warm, paper-like reading experience |
| `Compact` | Reduced font sizes and spacing |
| `HighContrast` | High contrast for accessibility |
| `DotNetPurple` | .NET brand purple colors |

#### Using a Built-in Theme in C#

```csharp
using Indiko.Maui.Controls.Markdown.Theming;

// Apply a built-in theme
markdownView.Theme = MarkdownThemeDefaults.GitHub;

// Enable automatic light/dark mode switching
markdownView.UseAppTheme = true;
```

#### Using a Built-in Theme in XAML with Binding

```xml
<ContentPage xmlns:idk="clr-namespace:Indiko.Maui.Controls.Markdown;assembly=Indiko.Maui.Controls.Markdown"
             xmlns:theming="clr-namespace:Indiko.Maui.Controls.Markdown.Theming;assembly=Indiko.Maui.Controls.Markdown">
    
    <idk:MarkdownView 
        MarkdownText="{Binding MarkdownText}" 
        Theme="{Binding CurrentTheme}"
        UseAppTheme="True" />
</ContentPage>
```

```csharp
// In your ViewModel
public MarkdownTheme CurrentTheme { get; set; } = MarkdownThemeDefaults.GitHub;

// Switch themes dynamically
public void SwitchToOneDark()
{
    CurrentTheme = MarkdownThemeDefaults.OneDark;
}
```

### Creating Custom Themes

You can create fully custom themes by defining your own palette and typography settings.

#### Custom Theme in C#

```csharp
using Indiko.Maui.Controls.Markdown.Theming;

var customTheme = new MarkdownTheme();

// Customize light palette
customTheme.Palette.TextPrimary = Color.FromArgb("#333333");
customTheme.Palette.H1Color = Color.FromArgb("#1a73e8");
customTheme.Palette.H2Color = Color.FromArgb("#1557b0");
customTheme.Palette.H3Color = Color.FromArgb("#0d47a1");
customTheme.Palette.HyperlinkColor = Color.FromArgb("#1a73e8");
customTheme.Palette.CodeBlockBackground = Color.FromArgb("#f5f5f5");
customTheme.Palette.CodeBlockText = Color.FromArgb("#d32f2f");
customTheme.Palette.BlockQuoteBackground = Color.FromArgb("#e3f2fd");
customTheme.Palette.BlockQuoteBorder = Color.FromArgb("#1a73e8");

// Customize dark palette
customTheme.PaletteDark.TextPrimary = Color.FromArgb("#e0e0e0");
customTheme.PaletteDark.H1Color = Color.FromArgb("#8ab4f8");
customTheme.PaletteDark.H2Color = Color.FromArgb("#669df6");
customTheme.PaletteDark.H3Color = Color.FromArgb("#4285f4");
customTheme.PaletteDark.HyperlinkColor = Color.FromArgb("#8ab4f8");
customTheme.PaletteDark.CodeBlockBackground = Color.FromArgb("#1e1e1e");
customTheme.PaletteDark.CodeBlockText = Color.FromArgb("#f48fb1");

// Customize typography
customTheme.Typography.H1FontSize = 32;
customTheme.Typography.H2FontSize = 26;
customTheme.Typography.H3FontSize = 22;
customTheme.Typography.BodyFontSize = 16;
customTheme.Typography.CodeFontSize = 14;
customTheme.Typography.CodeFontFamily = "Cascadia Code";
customTheme.Typography.LineHeight = 1.6;
customTheme.Typography.ParagraphSpacing = 1.2;

// Apply the theme
markdownView.Theme = customTheme;
```

#### Custom Theme in XAML

You can define themes as resources in XAML for reuse across your application:

```xml
<ContentPage.Resources>
    <theming:MarkdownTheme x:Key="MyCustomTheme">
        <theming:MarkdownTheme.Palette>
            <theming:MarkdownPalette 
                TextPrimary="#333333"
                H1Color="#1a73e8"
                H2Color="#1557b0"
                H3Color="#0d47a1"
                HyperlinkColor="#1a73e8"
                CodeBlockBackground="#f5f5f5"
                CodeBlockBorder="#e0e0e0"
                CodeBlockText="#d32f2f"
                BlockQuoteBackground="#e3f2fd"
                BlockQuoteBorder="#1a73e8"
                BlockQuoteText="#666666"
                TableHeaderBackground="#e8e8e8"
                TableHeaderText="#333333"
                TableRowBackground="White"
                TableRowText="#333333"
                DividerColor="#e0e0e0" />
        </theming:MarkdownTheme.Palette>
        <theming:MarkdownTheme.PaletteDark>
            <theming:MarkdownPaletteDark 
                TextPrimary="#e0e0e0"
                H1Color="#8ab4f8"
                H2Color="#669df6"
                H3Color="#4285f4"
                HyperlinkColor="#8ab4f8"
                CodeBlockBackground="#1e1e1e"
                CodeBlockBorder="#333333"
                CodeBlockText="#f48fb1"
                BlockQuoteBackground="#1e1e1e"
                BlockQuoteBorder="#8ab4f8"
                BlockQuoteText="#9e9e9e" />
        </theming:MarkdownTheme.PaletteDark>
        <theming:MarkdownTheme.Typography>
            <theming:MarkdownTypography 
                H1FontSize="32"
                H2FontSize="26"
                H3FontSize="22"
                BodyFontSize="16"
                CodeFontSize="14"
                LineHeight="1.6"
                CodeFontFamily="Consolas" />
        </theming:MarkdownTheme.Typography>
    </theming:MarkdownTheme>
</ContentPage.Resources>

<idk:MarkdownView 
    MarkdownText="{Binding MarkdownText}" 
    Theme="{StaticResource MyCustomTheme}"
    UseAppTheme="True" />
```

### Palette Properties

The `MarkdownPalette` class contains the following color properties:

| Property | Description |
|----------|-------------|
| `Primary` | Primary accent color |
| `Secondary` | Secondary accent color |
| `Background` | Default background color |
| `Surface` | Surface color for elevated elements |
| `TextPrimary` | Primary text color |
| `TextSecondary` | Secondary text color |
| `TextDisabled` | Disabled text color |
| `H1Color` | Color for H1 headings |
| `H2Color` | Color for H2 headings |
| `H3Color` | Color for H3 headings |
| `H4Color` | Color for H4 headings |
| `H5Color` | Color for H5 headings |
| `H6Color` | Color for H6 headings |
| `HyperlinkColor` | Color for hyperlinks |
| `CodeBlockBackground` | Background color for code blocks |
| `CodeBlockBorder` | Border color for code blocks |
| `CodeBlockText` | Text color for code blocks |
| `BlockQuoteBackground` | Background color for block quotes |
| `BlockQuoteBorder` | Border color for block quotes |
| `BlockQuoteText` | Text color for block quotes |
| `TableHeaderBackground` | Background color for table headers |
| `TableHeaderText` | Text color for table headers |
| `TableRowBackground` | Background color for table rows |
| `TableRowText` | Text color for table rows |
| `TableBorder` | Border color for tables |
| `DividerColor` | Color for horizontal rules |
| `InfoColor` | Color for info alerts |
| `WarningColor` | Color for warning alerts |
| `ErrorColor` | Color for error alerts |
| `SuccessColor` | Color for success alerts |

The palette also provides a helper method:
- `GetHeadingColor(int level)` - Returns the appropriate color for heading levels 1-6

### Typography Properties

The `MarkdownTypography` class contains the following properties:

| Property | Description | Default |
|----------|-------------|---------|
| `DefaultFontFamily` | Default font family for body text | null |
| `HeadingFontFamily` | Default font family for all headings (can be overridden per level) | null |
| `CodeFontFamily` | Font family for code | "Consolas" |
| `BlockQuoteFontFamily` | Font family for block quotes | "Consolas" |
| `H1FontSize` | Font size for H1 | 28 |
| `H1FontFamily` | Font family for H1 (overrides HeadingFontFamily) | null |
| `H1FontAttributes` | Font attributes for H1 (None, Bold, Italic) | Bold |
| `H2FontSize` | Font size for H2 | 24 |
| `H2FontFamily` | Font family for H2 (overrides HeadingFontFamily) | null |
| `H2FontAttributes` | Font attributes for H2 | Bold |
| `H3FontSize` | Font size for H3 | 20 |
| `H3FontFamily` | Font family for H3 (overrides HeadingFontFamily) | null |
| `H3FontAttributes` | Font attributes for H3 | Bold |
| `H4FontSize` | Font size for H4 | 18 |
| `H4FontFamily` | Font family for H4 (overrides HeadingFontFamily) | null |
| `H4FontAttributes` | Font attributes for H4 | Bold |
| `H5FontSize` | Font size for H5 | 16 |
| `H5FontFamily` | Font family for H5 (overrides HeadingFontFamily) | null |
| `H5FontAttributes` | Font attributes for H5 | Bold |
| `H6FontSize` | Font size for H6 | 14 |
| `H6FontFamily` | Font family for H6 (overrides HeadingFontFamily) | null |
| `H6FontAttributes` | Font attributes for H6 | Bold |
| `BodyFontSize` | Font size for body text | 14 |
| `CodeFontSize` | Font size for code | 13 |
| `TableHeaderFontSize` | Font size for table headers | 14 |
| `TableRowFontSize` | Font size for table rows | 13 |
| `LineHeight` | Line height multiplier | 1.5 |
| `HeadingLineHeight` | Line height for headings | 1.3 |
| `ParagraphSpacing` | Spacing between paragraphs | 1.0 |
| `ListItemSpacing` | Spacing between list items | 4 |
| `ListIndent` | Indentation for nested lists | 20 |
| `TextLineBreakMode` | Line break mode for text | WordWrap |
| `HeadingLineBreakMode` | Line break mode for headings | TailTruncation |

### Automatic Light/Dark Mode

Set `UseAppTheme="True"` to automatically switch between `Palette` (light mode) and `PaletteDark` (dark mode) based on the system theme:

```xml
<idk:MarkdownView 
    Theme="{Binding CurrentTheme}"
    UseAppTheme="True" />
```

When the system theme changes, the MarkdownView will automatically re-render with the appropriate palette.

### Cloning and Modifying Themes

You can clone an existing theme and modify it:

```csharp
// Clone a built-in theme and customize it
var myTheme = MarkdownThemeDefaults.GitHub.Clone();
myTheme.Palette.HyperlinkColor = Colors.Orange;
myTheme.Typography.H1FontSize = 36;

markdownView.Theme = myTheme;
```

---

## Bindable Properties

The following is a list of all customizable bindable properties:

### Theme
- **`Theme`**: A `MarkdownTheme` object that defines the complete visual style (colors, typography, spacing).
- **`UseAppTheme`**: When `true`, automatically switches between light and dark palettes based on system theme (default: `false`).

### Headings
Each heading level (H1-H6) supports individual styling with the following properties:

| Level | Color Property | Font Size Property | Font Family Property | Font Attributes Property |
|-------|---------------|-------------------|---------------------|-------------------------|
| H1 | `H1Color` | `H1FontSize` (24) | `H1FontFamily` | `H1FontAttributes` (Bold) |
| H2 | `H2Color` | `H2FontSize` (20) | `H2FontFamily` | `H2FontAttributes` (Bold) |
| H3 | `H3Color` | `H3FontSize` (18) | `H3FontFamily` | `H3FontAttributes` (Bold) |
| H4 | `H4Color` | `H4FontSize` (16) | `H4FontFamily` | `H4FontAttributes` (Bold) |
| H5 | `H5Color` | `H5FontSize` (14) | `H5FontFamily` | `H5FontAttributes` (Bold) |
| H6 | `H6Color` | `H6FontSize` (12) | `H6FontFamily` | `H6FontAttributes` (Bold) |

**FontAttributes Values:**
- `None` - No special formatting
- `Bold` - Bold text
- `Italic` - Italic text

**Example - Custom H1 styling:**
```xml
<idk:MarkdownView 
    H1Color="DarkBlue"
    H1FontSize="32"
    H1FontFamily="Georgia"
    H1FontAttributes="Bold" />
```

**Example - Italic H3 heading:**
```csharp
markdownView.H3FontAttributes = FontAttributes.Italic;
markdownView.H3Color = Colors.Purple;
```

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
- **`DefaultImageWidth`**: The default width for images when only aspect is specified without explicit dimensions (default: `200`).
- **`DefaultImageHeight`**: The default height for images when only aspect is specified without explicit dimensions (default: `200`).

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
    ---   <!-- Horizontal rule -->
    ```

- **Images**: The control supports image URLs, local files, and base64‑encoded images.  
  You can also specify optional `width`, `height`, `aspect`, `horizontal`, and `vertical` attributes using the
  curly‑brace syntax supported by Markdig's Generic Attributes extension.

  - Supported `aspect` values are:
    - `AspectFit` (default) – scales the image to fit while preserving aspect ratio.
    - `AspectFill` – fills the space while preserving aspect ratio (image may be clipped).
    - `Fill` – stretches the image to fill the space (aspect ratio not preserved).

  - Supported `horizontal` values are:
    - `Start` (default) – aligns the image to the start (left in LTR layouts).
    - `Center` – centers the image horizontally.
    - `End` – aligns the image to the end (right in LTR layouts).
    - `Fill` – stretches the image to fill horizontally.

  - Supported `vertical` values are:
    - `Start` – aligns the image to the top.
    - `Center` (default) – centers the image vertically.
    - `End` – aligns the image to the bottom.
    - `Fill` – stretches the image to fill vertically.

    ```markdown
    ![Alt text](http://example.com/image.jpg)                      // image URL
    ![Alt text](image.png)                                         // local file
    ![Alt text](data:image/png;base64,...)                         // base64 string
    ![Alt text](image.png){ width=150 height=75 }                  // specify both width and height (pixels)
    ![Alt text](image.png){ width=200 }                            // specify width only
    ![Alt text](image.png){ height=50 }                            // specify height only
    ![Alt text](image.png){ width=100 height=100 aspect=Fill }     // with custom aspect
    ![Alt text](image.png){ width=100 aspect=AspectFill }          // with aspect only
    ![Alt text](image.png){ width=14 height=14 horizontal=Start vertical=Center }  // with positioning
    ![Alt text](image.png){ width=50 vertical=Start }              // vertical positioning only
    ![Alt text](image.png){ horizontal=Center }                    // center image horizontally
    ```

- **Hyperlinks**: Create hyperlinks using the following format:
    ```markdown
    [Link Text](http://example.com)
    ```

## Image, Hyperlink and E-Mail Handling

### Image Handling
The `MarkdownView` supports various sources for displaying images:
1. **Image URLs**: Fetch and display images from the web.
2. **Local File**: Load images from local resources or assets.
3. **Base64 Encoded String**: Support for images encoded in base64 format.

The `ImageAspect` property allows you to customize how images are displayed within the control.

### Hyperlink Handling
You can respond to hyperlinks in Markdown content using the `LinkCommand` and `OnHyperLinkClicked` event. Hyperlinks are automatically detected and displayed with the color specified by the `HyperlinkColor` property.

When a user taps on a hyperlink:
- The `LinkCommand` is executed, if defined, with the hyperlink URL as the command parameter.
- The `OnHyperLinkClicked` event is triggered, providing the tapped hyperlink URL in the event arguments.    


### Email Link Handling
You can respond to email links in Markdown content using the `EMailCommand` and `OnEmailClicked` event. Email links are automatically detected and displayed with the color specified by the `HyperlinkColor` property. 

When a user taps on an email address:
- The `EMailCommand` is executed, if defined, with the tapped email address as the command parameter.
- The `OnEmailClicked` event is triggered, providing the tapped email address in the event arguments.

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

---

# How to Contribute

Thank you for considering contributing to our project! Please follow these guidelines to ensure a smooth process.

## 1. Work on a Feature Branch

Always create a new branch for your feature or fix. This keeps the main branch clean and makes it easier to manage changes.

```bash
git checkout -b feature/your-feature-name
```

## 2. Start a Pull Request

Once your feature is complete, push your branch to the repository and start a pull request to merge it into the main branch. Ensure all tests pass and your code follows the project's coding standards.

```bash
git push origin feature/your-feature-name
```

Then, create a pull request on GitHub and provide a clear description of your changes.

## 3. Use Semantic Release Prefixes for Commits

When committing your changes, use semantic release prefixes to categorize your commits. This helps in generating automated release notes and versioning.

The commit contains the following structural elements to communicate intent to the consumers of your library:

- **fix:** a commit of the type fix patches a bug in your codebase (this correlates with PATCH in Semantic Versioning).
- **feat:** a commit of the type feat introduces a new feature to the codebase (this correlates with MINOR in Semantic Versioning).
- **BREAKING CHANGE:** a commit that has a footer BREAKING CHANGE:, or appends a ! after the type/scope, introduces a breaking API change (correlating with MAJOR in Semantic Versioning). A BREAKING CHANGE can be part of commits of any type.
- Types other than fix: and feat: are allowed. For example, @commitlint/config-conventional (based on the Angular convention) recommends:
  - **build:** Changes that affect the build system or external dependencies
  - **chore:** Other changes that don't modify src or test files
  - **ci:** Changes to our CI configuration files and scripts
  - **docs:** Documentation only changes
  - **style:** Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
  - **refactor:** A code change that neither fixes a bug nor adds a feature
  - **perf:** A code change that improves performance
  - **test:** Adding missing tests or correcting existing tests

Footers other than BREAKING CHANGE: <description> may be provided and follow a convention similar to git trailer format. Additional types are not mandated by the Conventional Commits specification and have no implicit effect in Semantic Versioning (unless they include a BREAKING CHANGE). A scope may be provided to a commit’s type, to provide additional contextual information and is contained within parenthesis, e.g., feat(parser): add ability to parse arrays.

Example commit messages:

```bash
git commit -m "fix: resolve issue with user authentication"
git commit -m "feat: add new payment gateway integration"
git commit -m "BREAKING CHANGE: update API endpoints"
```

## 4. Write Meaningful Commit Messages

Commit messages should be concise yet descriptive. They should explain the "what" and "why" of your changes.

- **Good Example:** `fix: correct typo in user profile page`
- **Bad Example:** `fixed stuff`

## Additional Tips

- Ensure your code adheres to the project's coding standards and guidelines.
- Include tests for new features or bug fixes.
- Keep your commits atomic; a single commit should represent a single logical change.
- Update the documentation to reflect any new features or changes.

We appreciate your contributions and look forward to your pull requests!

Happy coding!
