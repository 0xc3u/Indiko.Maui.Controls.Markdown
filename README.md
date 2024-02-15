![](nuget.png)

# MarkdownView Component for MAUI.NET

The `MarkdownView` component is a versatile and customizable Markdown renderer designed for MAUI.NET applications. It extends `ContentView` and allows developers to display Markdown-formatted text within their MAUI.NET applications, providing a rich text experience. This document outlines the key features, bindable properties, and supported Markdown tags of the `MarkdownView` component.

## Features

- **Customizable Appearance:** Offers extensive support for customizing text appearance, including font size, color, and line break modes for different Markdown elements.
- **Bindable Properties:** Enables dynamic updates and data binding for Markdown content and styling properties.
- **Supported Markdown Elements:** Renders basic Markdown elements, including headers, lists, block quotes, images, and code blocks.
- **Multiple Image Sources:** The `MarkdownView` supports various sources for displaying images within Markdown content, including image URLs, local file paths, and base64 encoded strings. This flexibility allows for a wide range of image content to be seamlessly integrated into your Markdown text.

## Bindable Properties

The `MarkdownView` component offers several bindable properties to customize the Markdown rendering:

- **MarkdownText:** The Markdown-formatted text to be displayed.
- **LineBreakModeText:** Controls the line break mode for regular text content.
- **LineBreakModeHeader:** Controls the line break mode for header elements (H1, H2, H3).
- **H1Color, H2Color, H3Color:** Sets the text color for H1, H2, and H3 headers, respectively.
- **H1FontSize, H2FontSize, H3FontSize:** Sets the font size for H1, H2, and H3 headers, respectively.
- **TextColor:** The default text color for the Markdown content.
- **TextFontSize:** The default font size for the Markdown content.
- **CodeBlockBackgroundColor, CodeBlockBorderColor:** Customize the background and border colors for code blocks.
- **CodeBlockTextColor:** Sets the text color for code blocks.
- **CodeBlockFontSize:** Sets the font size for code blocks.
- **PlaceholderBackgroundColor:** Sets the background color for placeholder elements, such as space between Markdown elements.

## Supported Markdown Tags and Features

The `MarkdownView` supports a subset of Markdown elements and features, suitable for most text formatting needs:

- **Headers (H1, H2, H3):** Marked by `#`, `##`, `###` at the beginning of a line.
- **Unordered Lists:** Created with lines starting with `-` or `*`.
- **Block Quotes:** Denoted by lines starting with `>`.
- **Images:** Uses the Markdown image syntax `![alt text](image_url)`, where the URL can be an http(s) link, a local file path, or a base64 encoded string. This enables the embedding of images from various sources directly within the Markdown content.
- **Code Blocks:** Supports inline code with `` `code` `` and fenced code blocks with triple backticks ```.

### Image Support Details

- **Image URLs:** Direct links to images hosted online.
- **Local File Paths:** Path to an image file stored locally within the application's directories.
- **Base64 Encoded Strings:** A base64 encoded representation of an image, allowing for embedding image data directly within the Markdown string.

To include an image, simply use the standard Markdown image syntax with the desired source type. The `MarkdownView` component will automatically detect and handle the image source accordingly, ensuring that images are rendered correctly regardless of the source type.

```markdown
![Alt text](http://example.com/image.jpg)  // Image URL
![Alt text](image.png)      // Local file
![Alt text](base64,...)     // Base64 encoded string
```

## Customization and Extension

The `MarkdownView` component is designed with customization in mind. Developers can extend or modify it to support additional Markdown elements or to enhance the styling and rendering logic. The use of bindable properties enables seamless integration with MVVM patterns, allowing for dynamic content updates and styling adjustments.

## Usage

To use the `MarkdownView` in your MAUI.NET application, include it in your XAML or C# code and bind or set the `MarkdownText` property to your Markdown-formatted string. Customize the appearance using the available bindable properties to match your application's design.

```xml
xmlns:idk="clr-namespace:Indiko.Maui.Controls.Markdown;assembly=Indiko.Maui.Controls.Markdown"

...

<idk:MarkdownView MarkdownText="{Binding YourMarkdownContent}" />
```

Ensure to include the namespace reference for `MarkdownView` in your XAML or add the component programmatically in your C# code.
