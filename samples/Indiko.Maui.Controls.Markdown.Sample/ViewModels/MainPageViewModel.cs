﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Indiko.Maui.Controls.Markdown.Sample.ViewModels;
public partial class MainPageViewModel : BaseViewModel
{
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

    [ObservableProperty]
    string markdownText;

    public override void OnAppearing(object param)
	{

        MarkdownText = @"
# Why .NET MAUI is Awesome
MAUI.net (Multi-platform App UI) is a **modern UI toolkit** from **Microsoft** that enables developers to build **beautiful**, **high-performance**, __cross-platform apps__ from a **single codebase**.

Move now from ~~xamarin~~ to maui.net !

## Single Codebase, Multiple Platforms
With .NET MAUI, you can target for instance

- Android 
- iOS 
- macOS 
- Windows

with the **same code**.

**Why you should learn mobile cross-platform**
1. **Broader Market Reach**
2. **Cost and Time Efficiency**
3. **Consistent User Experience**

This significantly reduces development time and effort, allowing developers to focus on building great features rather than dealing with platform-specific code.

![Maui Bot](dev.png)

## Modern UI Development

> .NET MAUI leverages the latest in UI development, including declarative syntax with XAML or C#, hot reload for rapid UI iteration, and a rich set of controls and layouts that adapt to different screen sizes and devices.

## Performance

Leveraging the power of .NET 6 and beyond, .NET MAUI apps boast high performance.
The framework is optimized for speed and responsiveness, ensuring a smooth user experience across all platforms.

## Extensibility and Ecosystem
With access to the extensive .NET ecosystem, developers can easily integrate a wide range of libraries and tools into their MAUI apps, from databases and authentication services to powerful UI components.

## Developer Productivity

.NET MAUI comes with powerful development tools, including Visual Studio integration, which provides a rich editing, debugging, and deployment experience, further boosting developer productivity.

---

In summary, .NET MAUI offers a unified approach to cross-platform app development, combining ease of use, performance, and extensive ecosystem support, making it an excellent choice for modern app development.

```csharp
for(int n= 0; n<10; n++)
{
    Console.WriteLine(n);
}
```

```csharp
public class SimpleProgram
{
    public static void Main(string[] args)
    {
        string message = ""Hello, World!"";
        Console.WriteLine(message);
        Console.WriteLine(""This is a simple C# program."");
        message = null;
        if (message == null)
            Console.WriteLine(""Message is null!"");
    }
}
```

```int b= 0;```

[More about Maui.net](https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-8.0)

### Table Support
The MarkdownView supports basic tables as well as tables with alignments.

Header 1 | Header 2 | Header 3 
----------|----------|----------
A1 | B1 | C1 
A2 | B2 | C2 
A3 | B3 | C3 

 #### Table with alignments

Left  |  Center |  Right 
:---------|:--------:|---------:
A1 | B1 | C1 
A2 | B2 | C2 
A3 | B3 | C3 

### Math formula Support

$$
$E = mc^2
$$

$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$

$$
\pi r^2
$$

### Lokal SVG Image
![Lokal SVG ](dotnet_bot)

### Web Source SVG Image
![Web Source SVG](https://www.svgrepo.com/show/530402/honor.svg)

### E-Mail Links
Contact us at [support@example.com](support@example.com).
[Link1](https://google.com) | [Link2](https://bing.com)

### Nested lists

- Item 1
  - Subitem 1.1
    - Sub-subitem 1.1.1
  - Subitem 1.2
- Item 2

1. First
   1.1. Sub-first
       1.1.1. Sub-sub-first
2. Second

- Fruits
  1. Apple
  2. Banana
     - Ripe
     - Unripe
- Vegetables

".Trim();



    }
}
