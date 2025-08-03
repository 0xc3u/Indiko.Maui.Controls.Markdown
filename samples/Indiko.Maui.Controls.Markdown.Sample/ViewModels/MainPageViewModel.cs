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

### Data URL Images (Base64)
Testing data URL images with base64 encoding:

3x ![♥](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAEsWlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4KPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iWE1QIENvcmUgNS41LjAiPgogPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgeG1sbnM6ZXhpZj0iaHR0cDovL25zLmFkb2JlLmNvbS9leGlmLzEuMC8iCiAgICB4bWxuczp0aWZmPSJodHRwOi8vbnMuYWRvYmUuY29tL3RpZmYvMS4wLyIKICAgIHhtbG5zOnBob3Rvc2hvcD0iaHR0cDovL25zLmFkb2JlLmNvbS9waG90b3Nob3AvMS4wLyIKICAgIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIKICAgIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIgogICAgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIKICAgZXhpZjpQaXhlbFhEaW1lbnNpb249IjE2IgogICBleGlmOlBpeGVsWURpbWVuc2lvbj0iMTYiCiAgIGV4aWY6Q29sb3JTcGFjZT0iMSIKICAgdGlmZjpJbWFnZVdpZHRoPSIxNiIKICAgdGlmZjpJbWFnZUxlbmd0aD0iMTYiCiAgIHRpZmY6UmVzb2x1dGlvblVuaXQ9IjIiCiAgIHRpZmY6WFJlc29sdXRpb249IjcyLzEiCiAgIHRpZmY6WVJlc29sdXRpb249IjcyLzEiCiAgIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiCiAgIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIKICAgeG1wOk1vZGlmeURhdGU9IjIwMjUtMDgtMDJUMTM6MjY6NDUrMDI6MDAiCiAgIHhtcDpNZXRhZGF0YURhdGU9IjIwMjUtMDgtMDJUMTM6MjY6NDUrMDI6MDAiPgogICA8eG1wTU06SGlzdG9yeT4KICAgIDxyZGY6U2VxPgogICAgIDxyZGY6bGkKICAgICAgc3RFdnQ6YWN0aW9uPSJwcm9kdWNlZCIKICAgICAgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWZmaW5pdHkgUGhvdG8gMiAyLjYuMyIKICAgICAgc3RFdnQ6d2hlbj0iMjAyNS0wOC0wMlQxMzoyNjo0NSswMjowMCIvPgogICAgPC9yZGY6U2VxPgogICA8L3htcE1NOkhpc3Rvcnk+CiAgPC9yZGY6RGVzY3JpcHRpb24+CiA8L3JkZjpSREY+CjwveDp4bXBtZXRhPgo8P3hwYWNrZXQgZW5kPSJyIj8+r7zYUAAAAYFpQ0NQc1JHQiBJRUM2MTk2Ni0yLjEAACiRdZHPK0RRFMc/ZmgmQ6NYSBaTsDJi1MRGmUlDSRqj/Nq8eeaHmh+v92bSZKtspyix8WvBX8BWWStFpGTNltig5zwzNZPMuZ17Pvd77zndey7YIik1bdQPQDqT08OhgGd+YdHjeMGJDRcd1CuqoY3NzExR0z7uqLPijdeqVfvcv+ZaiRkq1DmFR1VNzwlPCE+t5TSLt4Xb1KSyInwq3KfLBYVvLT1a4meLEyX+sliPhINgaxH2JKo4WsVqUk8Ly8vpTqfyavk+1kuaYpm5WYld4p0YhAkRwMMk4wTxM8iIzH68+OiXFTXyB37zp8lKriqzRgGdVRIkydEnal6qxyTGRY/JSFGw+v+3r0Z8yFeq3hSAhifTfOsBxxZ8F03z89A0v4/A/ggXmUp+9gCG30UvVrTufXBvwNllRYvuwPkmtD9oiq78SnZxWzwOryfQvACt19C4VOpZeZ/je4isy1ddwe4e9Mp59/IP7rdnr0lUWjkAAAAJcEhZcwAACxMAAAsTAQCanBgAAADqSURBVDiNzdK9LkRBGMbx365iO9eg0LgCovBRb6JiKpWL4ApIKLgDleqUm7gFIRsKCiGyUWh0G90WEsWZMxnjBFvxNDPvM+/zn3eS4a/VyYuKZfTxhkHgPvoL2MAszgMXXwAVuzjMeBNsx/0ZetnZXuAoASrm8ISZYsJJXHuF/475wHM3Gkst4SZYhsXeRWgAo5amnzTKAdcYTxEe4yYBQv2mgykA+zGTJoBjXP0ifImTpkiASNzE4zfhB2w1t5cTCLxgBXct4Vusxp6kbtkVeMUahpk9xHo8+6ROaTSq6m+7E8vTUH/vf6gPDdkuW1yOeC0AAAAASUVORK5CYII=){ width=16 height=16 }

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
