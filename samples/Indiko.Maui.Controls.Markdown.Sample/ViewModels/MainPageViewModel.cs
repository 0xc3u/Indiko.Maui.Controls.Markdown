using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Indiko.Maui.Controls.Markdown.Theming;

namespace Indiko.Maui.Controls.Markdown.Sample.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    [ObservableProperty]
    string markdownText;

    [ObservableProperty]
    MarkdownTheme currentTheme;

    public MainPageViewModel()
    {
        // Start with GitHub theme
        CurrentTheme = MarkdownThemeDefaults.GitHub;
    }

	[RelayCommand]
    private void OnLinkReceived(object link)
    {
        Console.WriteLine(string.Concat("LINK RECEIVED = '",link, "'"));
    }

    [RelayCommand]
    private void OnEMailReceived(object email)
    {
        Console.WriteLine(string.Concat("E-MAIL RECEIVED = '", email, "'"));
    }

    [RelayCommand]
    private void OnThemeChanged()
    {
        Console.WriteLine("Request change theme");
        AppTheme appTheme = App.Current.UserAppTheme;
        if (appTheme == AppTheme.Unspecified)
            appTheme = Application.Current.PlatformAppTheme;
        Application.Current.UserAppTheme = appTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
    }

    [RelayCommand]
    private void SetGitHubTheme()
    {
        CurrentTheme = MarkdownThemeDefaults.GitHub;
        Console.WriteLine("Switched to GitHub theme");
    }

    [RelayCommand]
    private void SetSepiaTheme()
    {
        CurrentTheme = MarkdownThemeDefaults.Sepia;
        Console.WriteLine("Switched to Sepia theme");
    }

    [RelayCommand]
    private void SetCompactTheme()
    {
        CurrentTheme = MarkdownThemeDefaults.Compact;
        Console.WriteLine("Switched to Compact theme");
    }

    [RelayCommand]
    private void SetOneDarkTheme()
    {
        CurrentTheme = MarkdownThemeDefaults.OneDark;
        Console.WriteLine("Switched to One Dark theme");
    }

    [RelayCommand]
    private void SetOneLightTheme()
    {
        CurrentTheme = MarkdownThemeDefaults.OneLight;
        Console.WriteLine("Switched to One Light theme");
    }

    [RelayCommand]
    private void SetDraculaTheme()
    {
        CurrentTheme = MarkdownThemeDefaults.Dracula;
        Console.WriteLine("Switched to Dracula theme");
    }

    [RelayCommand]
    private void SetNordTheme()
    {
        CurrentTheme = MarkdownThemeDefaults.Nord;
        Console.WriteLine("Switched to Nord theme");
    }

    public override void OnAppearing(object param)
	{

        MarkdownText = @"
# Indiko.Maui.Controls.Markdown

A complete **feature showcase**. Every element below is rendered into _native MAUI views_ (there is no WebView), and styled by the active theme. Use the buttons under this view to switch themes live. :tada:

---

## 1. Headings

# Heading level 1
## Heading level 2
### Heading level 3
#### Heading level 4
##### Heading level 5
###### Heading level 6

---

## 2. Text formatting

This paragraph mixes inline styles: **bold** (`**`), __also bold__ (`__`), *italic* (`*`), _also italic_ (`_`), and ***bold + italic*** (`***`).

More inline styles: ~~strikethrough~~, ==highlighted== (`==`), E = mc^2^ (superscript `^`), H~2~O (subscript `~`), and ++inserted++ (`++`).

Inline code sits in a sentence like `var answer = 42;` — and emoji shortcodes render too: :rocket: :fire: :heart: :coffee: :100:

This line ends with a hard break
and continues on the next line.

---

## 3. Links & email

- Inline link: [.NET MAUI documentation](https://learn.microsoft.com/dotnet/maui/)
- Formatted link label: [**bold** _italic_ link](https://dotnet.microsoft.com)
- Bare URL autolink: https://github.com/0xc3u/Indiko.Maui.Controls.Markdown
- Email link: [support@example.com](mailto:support@example.com)
- Bare email autolink: hello@example.com

---

## 4. Lists

**Unordered**

- First item
- Second item
  - Nested item
    - Deeper nested item
- Third item

**Ordered**

1. Step one
2. Step two
3. Step three

**Task list**

- [x] Remove the SkiaSharp dependency
- [x] Render math & SVG with MAUI Graphics
- [ ] Take over the world

---

## 5. Blockquotes

> Blockquotes are great for callouts and quotations.
> They can span multiple lines and contain **formatting** and `inline code`.

---

## 6. Alert blocks

> [!NOTE]
> Useful information that users should take into account.

> [!TIP]
> Helpful advice for doing things better.

> [!IMPORTANT]
> Key information users need to succeed.

> [!WARNING]
> Critical content demanding immediate attention.

> [!CAUTION]
> Warns about risks or negative outcomes.

---

## 7. Custom containers

::: info
An **info** container — drawn with a blue outline.
:::

::: warning
A **warning** container — drawn with an orange outline.
:::

::: danger
A **danger** container — drawn with a red outline.
:::

---

## 8. Code

Inline code: run `dotnet build` then `dotnet test`.

A fenced block (set `EnableCodeBlockCopy=""True""` on the control to show a 📋 copy button):

```csharp
public static class Greeter
{
    public static void Main()
    {
        string message = ""Hello, .NET MAUI!"";
        Console.WriteLine(message);
    }
}
```

---

## 9. Tables

| Feature   | Supported | Notes            |
|-----------|-----------|------------------|
| Headings  | Yes       | H1–H6            |
| Tables    | Yes       | with alignment   |
| Math      | Yes       | block & inline   |

**Column alignment** (`:---` left, `:--:` center, `---:` right):

| Left | Center | Right |
|:-----|:------:|------:|
| a    | b      | c     |
| dd   | ee     | ff    |

---

## 10. Math (LaTeX)

Inline math flows within text — the identity $E = mc^2$ relates energy and mass, and $a^2 + b^2 = c^2$ is the Pythagorean theorem.

Block math is centered on its own line:

$$
\int_{-\infty}^{\infty} e^{-x^2}\,dx = \sqrt{\pi}
$$

$$
\frac{n!}{k!(n-k)!} = \binom{n}{k}
$$

---

## 11. Images

**Local image** (from app resources):

![Dev bot](dev.png){aspect=AspectFill}

**Local SVG** — rendered by the built-in MAUI Graphics SVG renderer:

![.NET bot](dotnet_bot){aspect=AspectFit width=120}

**Remote SVG** (downloaded and rasterized):

![Honor badge](https://www.svgrepo.com/show/530402/honor.svg){width=120 horizontal=Center}

**Remote raster image** — scales down to fit the available width:

![Remote](https://picsum.photos/1200/800)

**Sized & centered** `{width=200 horizontal=Center}`:

![Centered](https://picsum.photos/800/500){width=200 horizontal=Center}

**Inline icon** in a sentence ![icon](clipboard.png){width=16 height=16} stays in the text flow.

**Base64 (data URI)** — an image decoded from an inline base64 string:

![Square](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAARElEQVR42u3PQREAAAQAMH2k0V0fKvi622MBFpU9n4WAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAwNUCrlbILdN94G4AAAAASUVORK5CYII=){width=48 height=48}

---

## 12. Horizontal rule

Content above the rule.

---

Content below the rule.

**That's every supported feature — happy authoring!** :clap:
".Trim();

    }
}
