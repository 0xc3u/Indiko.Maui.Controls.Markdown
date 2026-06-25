![Indiko.Maui.Controls.Markdown](nuget.png)

# Indiko.Maui.Controls.Markdown

The `MarkdownView` control is a flexible component designed for MAUI applications to display and style Markdown content with ease. It renders Markdown into **native MAUI views** (Labels, Grids, Images, BoxViews) — there is **no WebView** — so content scrolls, themes, and behaves like the rest of your app. It supports a rich set of Markdown elements (headings, emphasis, lists, tables, code blocks, blockquotes, links, images, GitHub-style alerts, custom containers, emoji, and LaTeX math) and offers extensive customization through a full theming system and per-element bindable properties.

> As of the latest release the control is **100% SkiaSharp-free** — math and SVG are rendered with `Microsoft.Maui.Graphics`. See [Breaking Changes](#skiasharp-dependency-removed).

![markdownview_screenshots](https://github.com/user-attachments/assets/2485eedb-015d-4ccb-acc2-c19d24ea51d7)

## Features

- 🧱 **Native rendering** — no WebView; every element is a real MAUI view.
- 🔠 **Headings** `H1`–`H6`, **paragraphs**, **bold**/**italic**/**strikethrough**, **inline code**, `==highlight==`, and super/subscript.
- 🔗 **Links** and **email links** with click events *and* commands, plus bare-URL **autolinking**.
- 📋 **Lists** — unordered, ordered, nested, and **task/checkbox** lists (`- [ ]` / `- [x]`).
- 📊 **Tables** with per-column alignment (pipe & grid tables).
- 💬 **Blockquotes** and 🧾 **code blocks** with an optional **copy-to-clipboard** button.
- 🚨 **GitHub-style alerts** — `[!NOTE]`, `[!TIP]`, `[!IMPORTANT]`, `[!WARNING]`, `[!CAUTION]`.
- 📦 **Custom containers** (`::: info … :::`) and 😀 **emoji** shortcodes (`:rocket:`).
- 🧮 **LaTeX math** — block (`$$ … $$`) and inline (`$ … $`), rendered with `Microsoft.Maui.Graphics` (no SkiaSharp).
- 🖼️ **Images** from local files, remote URLs, base64, and **SVG** — with width/height/aspect/alignment attributes.
- 🔍 **Tap-to-zoom image popup** — opt-in full-screen native overlay with pinch/double-tap zoom, pan, and a close button (`AllowImagePopup`).
- 🎨 **Theming system** — 11 built-in themes, light/dark auto-switching, and fully custom themes, *plus* granular per-element bindable properties.
- 🛟 **Render-error event** — failures surface via `OnRenderError` instead of rendering blank.

## Build Status
![ci](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/actions/workflows/ci.yml/badge.svg)

## ⚠️ Breaking Changes

### SkiaSharp dependency removed

`Indiko.Maui.Controls.Markdown` **no longer depends on SkiaSharp** in any form — `SkiaSharp.Views.Maui.Controls`, `Svg.Skia`, and `CSharpMath.SkiaSharp` are gone. Math formulas and SVG images are now rendered entirely with **`Microsoft.Maui.Graphics`**, the cross-platform drawing API built into .NET MAUI.

**Why:** dropping the native SkiaSharp binaries makes the package lighter and avoids native load failures that can occur when a SkiaSharp build targets a newer platform-SDK band than the one installed (for example, the `SkiaSharp.Views.Maui` handler failing to load on iOS/Mac Catalyst).

**What you need to do:** for the common case, **nothing** — the public API of `MarkdownView` is unchanged.

- If you previously called `builder.UseSkiaSharp()` *only* for this control, you can remove it. `builder.UseMarkdownView()` is still available and safe to keep calling — it no longer registers SkiaSharp, but it now registers the handler used by the optional [tap-to-zoom image popup](#tap-to-zoom-image-popup), so keep it if you enable `AllowImagePopup`.
- **SVG rendering** now uses a built-in MAUI Graphics renderer. It supports `path` (full command set including arcs), the basic shapes (`rect`, `circle`, `ellipse`, `line`, `polyline`, `polygon`), transforms, solid `fill`/`stroke` using hex/`rgb()`/`rgba()`/named colors, and **linear & radial gradient fills** (incl. `gradientUnits`, `gradientTransform`, multi-stop, and `href` inheritance). It is a pragmatic subset and does **not** support filters, embedded text, or clip/mask. Two minor approximations: gradient *strokes* fall back to a representative solid color (MAUI's canvas only supports gradient fills), and a radial gradient's focal point (`fx`/`fy`) and `spreadMethod` are ignored.
  - If you display SVGs that rely on any unsupported feature, please [open an issue](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/issues).

---

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

## Getting Started

### 1. (Optional) Register in `MauiProgram.cs`

```csharp
using Indiko.Maui.Controls.Markdown;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseMarkdownView();   // see note below

    return builder.Build();
}
```

> **Note:** `UseMarkdownView()` no longer registers SkiaSharp (the control doesn't need it). It now registers the native handler that powers the [tap-to-zoom image popup](#tap-to-zoom-image-popup) — so **call it if you use `AllowImagePopup`**. If you don't use that feature, the call is optional and safe to keep or remove; math, SVG, and everything else render without any extra setup.

### 2. Add the XAML namespace and the control

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:idk="clr-namespace:Indiko.Maui.Controls.Markdown;assembly=Indiko.Maui.Controls.Markdown"
    x:Class="MyApp.MainPage">

    <ScrollView>
        <idk:MarkdownView MarkdownText="{Binding MarkdownText}" />
    </ScrollView>

</ContentPage>
```

> 💡 `MarkdownView` lays its content out top-to-bottom and does not scroll on its own — wrap it in a `ScrollView` for longer documents.

### 3. …or create it in C#

```csharp
var markdown = new MarkdownView
{
    MarkdownText = "# Hello, Markdown!\n\nThis is **bold** and this is *italic*."
};
```

That's it. Everything else on this page — theming, per-element styling, events, and the supported syntax — is optional customization.

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
| `HighlightColor` | Background for highlighted/marked inline text (`==text==`) |
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
| `InfoColor` | Color for info / `[!NOTE]` alerts |
| `WarningColor` | Color for warning / `[!WARNING]` alerts |
| `ErrorColor` | Color for error / `[!CAUTION]` alerts |
| `SuccessColor` | Color for success / `[!TIP]` alerts |
| `ImportantColor` | Color for `[!IMPORTANT]` alerts |
| `ImagePopupBackground` | Background color of the [tap-to-zoom image popup](#tap-to-zoom-image-popup) overlay (default black) |
| `ImagePopupCloseButton` | Color of the image popup's close (✕) button (default white) |

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

When the system theme changes, the MarkdownView automatically re-renders with the appropriate palette.

> **Behavior:** When `UseAppTheme` is `false` (the default), the theme **always uses the light `Palette`** regardless of the system mode — `PaletteDark` is only consulted when `UseAppTheme="True"` *and* the system is in dark mode. So set `UseAppTheme="True"` (and supply both palettes) if you want dark-mode support.

> **Assigning a built-in theme directly in XAML** uses `x:Static` (the presets are static properties), whereas a theme exposed by your view model is bound normally:
> ```xml
> <!-- preset directly -->
> <idk:MarkdownView Theme="{x:Static theming:MarkdownThemeDefaults.GitHub}" UseAppTheme="True" />
> <!-- or from a view model -->
> <idk:MarkdownView Theme="{Binding CurrentTheme}" UseAppTheme="True" />
> ```

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

These per-element properties let you style the control directly, without a `Theme` object. (When a `Theme` **is** set it overwrites these — see [Theming Support](#theming-support).)

> **Light/dark color variants:** several color properties also expose `…Light` and `…Dark` companions — `TextColorLight`/`TextColorDark`, `H1ColorLight`/`H1ColorDark`, `H2ColorLight`/`H2ColorDark`, `H3ColorLight`/`H3ColorDark`, `HyperlinkColorLight`/`HyperlinkColorDark`, `CodeBlockBackgroundColorLight`/`Dark`, `CodeBlockTextColorLight`/`Dark`, `BlockQuoteBackgroundColorLight`/`Dark`, and `BlockQuoteTextColorLight`/`Dark`. These feed the theming system's light/dark palettes; for plain manual styling just set the base property (e.g. `TextColor`).

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
- **`HighlightColor`**: The background color used for highlighted/marked inline text (`==text==`) (default: `#FFF59D`).
- **`TextHorizontalTextAlignment`**: The horizontal alignment of body (prose) text — paragraphs, list items, and blockquote text. Accepts `Start` (default), `Center`, `End`, or `Justify`. Headings and table cells are not affected (tables keep their per-column alignment).

  ```xml
  <idk:MarkdownView 
      MarkdownText="{Binding MarkdownText}"
      TextHorizontalTextAlignment="Justify" />
  ```

### Line Break Mode
- **`AllowLineBreaksOnHeadlines`**: When `true` (default), headings wrap onto multiple lines (`WordWrap`). When `false`, headings are truncated to a single line using `LineBreakModeHeader`.
- **`LineBreakModeText`**: Line break mode for text (default: `WordWrap`).
- **`LineBreakModeHeader`**: Line break mode for headings when `AllowLineBreaksOnHeadlines` is `false` (default: `TailTruncation`).

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
- **`EnableCodeBlockCopy`**: When `true`, displays a copy button on code blocks (default: `false`).
- **`CodeBlockCopyButtonText`**: The text/emoji for the copy button (default: `📋`).
- **`CodeBlockCopyButtonCopiedText`**: The text/emoji shown after copying (default: `✓`).

### Alert Blocks
- **`AlertInfoColor`**: Color for NOTE alert blocks (default: `#2196F3`).
- **`AlertSuccessColor`**: Color for TIP alert blocks (default: `#4CAF50`).
- **`AlertImportantColor`**: Color for IMPORTANT alert blocks (default: `#9C27B0`).
- **`AlertWarningColor`**: Color for WARNING alert blocks (default: `#FF9800`).
- **`AlertErrorColor`**: Color for CAUTION alert blocks (default: `#F44336`).

### Table
- **`TableHeaderFontSize`**: The font size for table headers (default: `14`).
- **`TableHeaderTextColor`**: The text color for table headers (default: `Black`).
- **`TableHeaderBackgroundColor`**: The background color for table headers (default: `LightGray`).
- **`TableHeaderFontFace`**: The font family for table headers.
- **`TableRowFontSize`**: The font size for table rows (default: `12`).
- **`TableRowTextColor`**: The text color for table rows (default: `Black`).
- **`TableRowBackgroundColor`**: The background color for table rows (default: `White`).
- **`TableRowFontFace`**: The font family for table rows.

### Image
- **`ImageAspect`**: The aspect ratio for images (default: `AspectFit`).
- **`DefaultImageWidth`**: The default width for images when only aspect is specified without explicit dimensions (default: `200`).
- **`DefaultImageHeight`**: The default height for images when only aspect is specified without explicit dimensions (default: `200`).
- **`AllowImagePopup`**: When `true`, tapping a rendered image opens it full-screen in a native, zoomable overlay (default: `false`). See [Tap-to-zoom image popup](#tap-to-zoom-image-popup).
- **`ImagePopupBackgroundColor`**: Background color of the image overlay (default: `Black`).
- **`ImagePopupCloseButtonColor`**: Color of the overlay's close (✕) button (default: `White`).
- **`ImagePopupMaxZoomScale`**: Maximum zoom factor, relative to the fitted size, inside the overlay (default: `5`).

### Links & Email
- **`HyperlinkColor`**: The color for hyperlinks and email links (default: `BlueViolet`).
- **`LinkCommand`**: A command executed when a hyperlink is tapped — the tapped URL is passed as the command parameter.
- **`LinkCommandParameter`**: An optional parameter to pass to `LinkCommand`.
- **`EMailCommand`**: A command executed when an email link is tapped — the email address is passed as the command parameter.
- **`EMailCommandParameter`**: An optional parameter to pass to `EMailCommand`.

See [Events & Commands](#events--commands) for the matching `OnHyperLinkClicked` / `OnEmailClicked` events.

### Horizontal Rule
- **`LineColor`**: The color for horizontal rules (default: `LightGray`).

### Spacing
- **`ParagraphSpacing`**: A multiplier for the spacing between blocks/paragraphs (default: `1.0`).
- **`LineHeightMultiplier`** Modifies the line height multiplier for displayed labels (default: `1.0`). Directly modifies [Label.LineHeight](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.label.lineheight?view=net-maui-8.0).

## Supported Markdown Syntax

- **Headings**: MarkdownView supports headings from `H1` to `H6`.
    ```markdown
    # H1
    ## H2
    ### H3
    ```

- **Bold**: Wrap text with `**` or `__`.
    ```markdown
    **Bold** and __also bold__
    ```

- **Italic**: Wrap text with `*` or `_`.
    ```markdown
    *Italic* and _also italic_
    ```

- **Bold + Italic**: Combine with `***`.
    ```markdown
    ***bold and italic***
    ```

- **Strikethrough, Highlight, Super/Subscript, Inserted**: additional inline styles (Markdig "emphasis extras").
    ```markdown
    ~~strikethrough~~, ==highlighted==, super^script^, sub~script~, ++inserted++
    ```
    - `==highlight==` is drawn with the `HighlightColor` property as its background (default light yellow `#FFF59D`).
    - `^superscript^` / `~subscript~` are **approximated** with a smaller font — MAUI text spans can't offset the baseline.
    - `++inserted++` renders as underlined text.

- **Blockquotes**: Add `>` before a paragraph to create a blockquote.
    ```markdown
    > This is a blockquote.
    ```

- **Alert/Admonition Blocks**: Create GitHub-style alert blocks using the `> [!TYPE]` syntax. These are styled callout boxes for highlighting important information.

    Supported alert types:
    - `NOTE` - Informational notes (ℹ️ blue)
    - `TIP` - Helpful tips and suggestions (💡 green)
    - `IMPORTANT` - Important information (❗ purple)
    - `WARNING` - Warning messages (⚠️ orange)
    - `CAUTION` - Critical warnings (🛑 red)

    ```markdown
    > [!NOTE]
    > This is a note alert. Useful for highlighting information.

    > [!TIP]
    > This is a tip alert. Provides helpful advice.

    > [!IMPORTANT]
    > This is an important alert. Crucial information for users.

    > [!WARNING]
    > This is a warning alert. Content demanding attention.

    > [!CAUTION]
    > This is a caution alert. Negative potential consequences.
    ```

    You can customize the colors for each alert type using the following properties:
    - `AlertInfoColor` - Color for NOTE alerts (default: `#2196F3`)
    - `AlertSuccessColor` - Color for TIP alerts (default: `#4CAF50`)
    - `AlertImportantColor` - Color for IMPORTANT alerts (default: `#9C27B0`)
    - `AlertWarningColor` - Color for WARNING alerts (default: `#FF9800`)
    - `AlertErrorColor` - Color for CAUTION alerts (default: `#F44336`)

- **Code Blocks**: Enclose code in a fenced block (triple backticks). The optional language after the opening fence is accepted but **not** syntax-highlighted — code is shown in a single color (`CodeBlockTextColor`) and word-wrapped. Indented (4-space) code blocks are also supported.

    ````markdown
    ```csharp
    var greeting = "Hello, world!";
    Console.WriteLine(greeting);
    ```
    ````

    **Inline code**: wrap text in single backticks — it's rendered in the code font using the code-block text/background colors.
    ```markdown
    Run the `dotnet build` command, then `dotnet test`.
    ```

    **Copy-to-Clipboard Feature**: You can enable a copy button on code blocks that allows users to copy the code content to the clipboard with a single tap.

    ```xml
    <idk:MarkdownView 
        MarkdownText="{Binding MarkdownText}"
        EnableCodeBlockCopy="True"
        CodeBlockCopyButtonText="📋"
        CodeBlockCopyButtonCopiedText="✓" />
    ```

    Properties:
    - `EnableCodeBlockCopy` - Enable/disable the copy button (default: `false`)
    - `CodeBlockCopyButtonText` - Text/emoji for the copy button (default: `📋`)
    - `CodeBlockCopyButtonCopiedText` - Text/emoji shown after copying (default: `✓`)

- **Lists**: unordered, ordered, **nested**, and **task/checkbox** lists are all supported.
    - Unordered: start lines with `-`, `*`, or `+` (rendered with a `•` bullet).
    - Ordered: start lines with a number followed by a period (the actual number is rendered).
    - Nested: indent sub-items — each level adds indentation.
    - Task lists: `- [ ]` (unchecked) and `- [x]` (checked) render a **read-only** checkbox + label.

    ```markdown
    - Item 1
      - Nested item 1.1
        - Nested item 1.1.1
    - Item 2

    1. First
    2. Second

    - [x] Completed task
    - [ ] Pending task
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

- **Math / LaTeX**: both **block** and **inline** math are typeset as real LaTeX, rendered with `Microsoft.Maui.Graphics` (no SkiaSharp).
    - **Block** math — put a formula on its own lines between `$$` fences:
      ```markdown
      $$
      \int_{-\infty}^{\infty} e^{-x^2}\,dx = \sqrt{\pi}
      $$
      ```
    - **Inline** math — wrap a formula in single `$` within a sentence:
      ```markdown
      The identity $E = mc^2$ relates energy and mass.
      ```
    > Inline math is laid out as a wrapping flow of text + formula segments, so it wraps at segment boundaries rather than word-by-word (a MAUI text run can't break across an embedded view). In **table cells and headings**, inline math falls back to styled raw text rather than a typeset view.

- **Custom Containers**: Wrap content in `:::` fences; the first word after the opening fence selects the outline color.
    ```markdown
    ::: info
    An informational container.
    :::
    ```
    Recognized classes: `info` (blue outline), `warning` (orange), `danger` (red); any other value uses a light-gray outline. The container is drawn as a colored outline around its inner content.

- **Emoji**: Use `:shortcode:` syntax — shortcodes are converted to Unicode emoji.
    ```markdown
    Ship it :rocket: and celebrate :tada:
    ```
    > ASCII smileys (e.g. `:)`) are intentionally **not** converted.

- **Autolinks**: Bare URLs and email addresses are detected automatically and become tappable — no `[...](...)` needed.
    ```markdown
    Visit https://github.com or email me@example.com
    ```

- **Email links**: An explicit `mailto:` link, a link whose target is an email address, or a bare email address all become tappable email links (see [Email Link Handling](#email-link-handling)).
    ```markdown
    [Email me](mailto:support@example.com)
    ```

- **Images**: The control supports image URLs, local files, base64‑encoded images, and **SVG** (any URL ending in `.svg` is rasterized by the built-in MAUI Graphics SVG renderer).
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
    ![Alt text](https://example.com/icon.svg)                      // remote SVG (rasterized)
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

## Images

`MarkdownView` loads images from several sources:

1. **Local file / resource** — `![alt](logo.png)` (loaded via `ImageSource.FromFile`).
2. **Remote URL** — `![alt](https://…/pic.png)` (downloaded over HTTP and cached).
3. **Base64 / data URI** — `![alt](data:image/png;base64,…)`, or a bare base64 string.
4. **SVG** — any URL ending in `.svg` is downloaded and rasterized by the built-in `Microsoft.Maui.Graphics` SVG renderer (no SkiaSharp). See [Breaking Changes](#skiasharp-dependency-removed) for the supported SVG feature set.

Control how images scale with the `ImageAspect` property (or a per-image `aspect=` attribute), and set the fallback size with `DefaultImageWidth` / `DefaultImageHeight`. The full per-image attribute list (`width`, `height`, `aspect`, `horizontal`, `vertical`) is documented under [Supported Markdown Syntax → Images](#supported-markdown-syntax).

> **Base64 gotchas.** If a `data:` image renders blank or shows as literal text, the cause is almost always the data itself, not the control:
> - **The image link must be well-formed** — don't drop the closing `)` before any `{…}` attributes: `![alt](data:image/png;base64,XXXX){width=48}`. A missing `)` makes the whole thing render as plain text.
> - **The base64 must decode to a complete, valid image.** A truncated or corrupt PNG can still have a readable header (so tools report its dimensions) yet fail to decode at render time, showing blank. Verify with e.g. `echo '<base64>' | base64 -d > test.png && sips -g pixelWidth test.png`.

### Tap-to-zoom image popup

Set **`AllowImagePopup="True"`** to make every rendered image tappable. Tapping one opens it **full-screen in a native overlay** — implemented with platform handlers (`UIScrollView` on iOS/Mac Catalyst, a matrix-driven `ImageView` on Android, `ScrollViewer` on Windows), so the gestures feel native:

- **Pinch** to zoom in/out, **drag** to pan.
- **Double-tap** to toggle between fitted and zoomed-in.
- **✕** button (top-right) to close.

It works for every supported image kind (local, remote, base64, SVG). The feature is **off by default**, so existing layouts are unchanged.

```xml
<idk:MarkdownView
    MarkdownText="{Binding MarkdownText}"
    AllowImagePopup="True"
    ImagePopupBackgroundColor="Black"
    ImagePopupCloseButtonColor="White"
    ImagePopupMaxZoomScale="5" />
```

The overlay colors can be set per-instance (as above) **or via a theme** — the palette exposes `ImagePopupBackground` and `ImagePopupCloseButton` (with independent light/dark values through `Palette` / `PaletteDark`), so they follow your theme and auto light/dark switching like every other color.

> **Requires `UseMarkdownView()`.** The popup uses a native handler that is registered by `builder.UseMarkdownView()` in `MauiProgram.cs` (see [Getting Started](#1-optional-register-in-mauiprogramcs)). If you enable `AllowImagePopup` without calling it, tapping an image does nothing.

## Events & Commands

`MarkdownView` raises **events** *and* supports `ICommand` bindings, so you can react to interactions from code-behind or a view model.

| Event | Command | Fires when | Argument passed |
|---|---|---|---|
| `OnHyperLinkClicked` | `LinkCommand` | a hyperlink is tapped | the URL (`LinkEventArgs.Url`) |
| `OnEmailClicked` | `EMailCommand` | an email link is tapped | the address (`EmailEventArgs.Email`) |
| `OnRenderError` | — | markdown fails to render | `MarkdownRenderErrorEventArgs` (`.Exception`, `.Message`) |

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

### Render Error Handling
Rendering failures are no longer swallowed silently. When the control cannot render the markdown — for example, when a different, incompatible **Markdig** version is resolved at runtime than the one the control was built against (which throws `MissingMethodException`/`TypeLoadException` while building the pipeline), or when an individual block fails — the control:

- Raises the **`OnRenderError`** event with a `MarkdownRenderErrorEventArgs` carrying the `Exception` and a human-readable `Message`, so your app can log or react.
- Renders an inline error message instead of blank content. For a Markdig mismatch the message names the Markdig version actually loaded, making it easy to spot.

```csharp
markdownView.OnRenderError += (sender, e) =>
{
    // e.Exception and e.Message describe the failure
    logger.LogError(e.Exception, "Markdown render failed: {Message}", e.Message);
};
```

> **Note on Markdig:** the control calls Markdig extension methods whose API can change between major versions, so its NuGet dependency is bounded to `[1.3.2, 2.0.0)`. If your app pins Markdig outside that range, NuGet surfaces a restore warning rather than letting a silent runtime mismatch occur.

## Notes & Limitations

The control renders a pragmatic subset of Markdown into native views. Knowing these edge cases up front saves surprises:

- **Inline math** (`$ … $`) is typeset as real LaTeX in paragraphs, but wraps at segment boundaries (text around a formula doesn't reflow word-by-word). Inside **table cells / headings** it falls back to styled raw text.
- **Super/subscript** (`^x^` / `~x~`) are approximated with a smaller font — MAUI text spans can't offset the baseline, so they aren't raised/lowered.
- **Images in tables or mixed with inline text** render as the literal placeholder `[Image]`. A **standalone** image (a paragraph that contains only the image) renders fully and scales to the available width. (Inline emphasis, code, links, and the emphasis-extras styles *do* work in those contexts — and inside headings.)
- **Table cells** render only their first paragraph; multi-paragraph cells lose the extra content.
- **No syntax highlighting** in code blocks — the language hint is accepted but code is shown in a single color.
- **Parsed but not rendered:** footnotes, definition lists, citations, abbreviations, footers, and alphabetical/roman list markers are recognized by the parser but produce no special output.

Found something here that should "just work" for your use case? [Open an issue](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/issues) — several of these are candidates for future improvement.

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
