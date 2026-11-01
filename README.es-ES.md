![Indiko.Maui.Controls.Markdown](nuget.png)

# Indiko.Maui.Controls.Markdown

El control `MarkdownView` es un componente flexible diseñado para aplicaciones MAUI para mostrar y dar estilo a contenido Markdown con facilidad. Renderiza Markdown en **vistas nativas de MAUI** (Labels, Grids, Images, BoxViews) — **no utiliza WebView** — por lo que el contenido se desplaza, se tematiza y se comporta como el resto de su aplicación. Soporta un conjunto rico de elementos Markdown (encabezados, énfasis, listas, tablas, bloques de código, citas, enlaces, imágenes, alertas estilo GitHub, contenedores personalizados, emojis y matemáticas LaTeX) y ofrece una personalización extensa a través de un sistema de tematización completo y propiedades vinculables por elemento.

> A partir de la última versión, el control está **100% libre de SkiaSharp** — las matemáticas y el SVG se renderizan con `Microsoft.Maui.Graphics`. Consulte [Cambios Importantes](#cambios-importantes).

![markdownview_screenshots](https://github.com/user-attachments/assets/2485eedb-015d-4ccb-acc2-c19d24ea51d7)

## Características

- 🧱 **Renderizado nativo** — sin WebView; cada elemento es una vista real de MAUI.
- 🔠 **Encabezados** `H1`–`H6`, **párrafos**, **negrita**/**cursiva**/**tachado**, **código en línea**, `==resaltado==` y superíndice/subíndice.
- 🔗 **Enlaces** y **enlaces de correo electrónico** con eventos de clic *y* comandos, además de **autovínculos** de URLs simples.
- 📋 **Listas** — desordenadas, ordenadas, anidadas y listas de **tareas/casillas de verificación** (`- [ ]` / `- [x]`).
- 📊 **Tablas** con alineación por columna (tablas de tuberías y cuadrícula).
- 💬 **Citas** y 🧾 **bloques de código** con un botón opcional de **copiar al portapapeles**.
- 🚨 **Alertas estilo GitHub** — `[!NOTE]`, `[!TIP]`, `[!IMPORTANT]`, `[!WARNING]`, `[!CAUTION]`.
- 📦 **Contenedores personalizados** (`::: info … :::`) y 😀 **shortcodes de emoji** (`:rocket:`).
- 🧮 **Matemáticas LaTeX** — en bloque (`$$ … $$`) y en línea (`$ … $`), renderizadas con `Microsoft.Maui.Graphics` (sin SkiaSharp).
- 🖼️ **Imágenes** desde archivos locales, URLs remotas, base64 y **SVG** — con atributos de ancho/alto/aspecto/alineación.
- 🔍 **Ventana emergente de imagen al tocar para hacer zoom** — superposición nativa de pantalla completa opcional con zoom mediante pinza/doble toque, desplazamiento y un botón de cierre (`AllowImagePopup`).
- 🎨 **Sistema de tematización** — 11 temas integrados, cambio automático luz/oscuro y temas totalmente personalizados, *además* de propiedades vinculables granulares por elemento.
- 🛟 **Evento de error de renderizado** — los fallos se notifican a través de `OnRenderError` en lugar de renderizar el contenido en blanco.

## Estado de Compilación
![ci](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/actions/workflows/ci.yml/badge.svg)

## ⚠️ Cambios Importantes

### Se eliminó la dependencia de SkiaSharp

`Indiko.Maui.Controls.Markdown` **ya no depende de SkiaSharp** en ninguna forma — `SkiaSharp.Views.Maui.Controls`, `Svg.Skia` y `CSharpMath.SkiaSharp` han sido eliminados. Las fórmulas matemáticas y las imágenes SVG ahora se renderizan enteramente con **`Microsoft.Maui.Graphics`**, la API de dibujo multiplataforma integrada en .NET MAUI.

**Por qué:** eliminar los binarios nativos de SkiaSharp hace que el paquete sea más ligero y evita fallos de carga nativa que pueden ocurrir cuando una compilación de SkiaSharp apunta a una banda de SDK de plataforma más nueva que la instalada (por ejemplo, el manejador `SkiaSharp.Views.Maui` que falla al cargar en iOS/Mac Catalyst).

**Qué debe hacer:** para el caso común, **nada** — la API pública de `MarkdownView` no ha cambiado.

- Si anteriormente llamaba a `builder.UseSkiaSharp()` *solo* para este control, puede eliminarlo. `builder.UseMarkdownView()` sigue estando disponible y es seguro seguir llamándolo — ya no registra SkiaSharp, pero ahora registra el manejador utilizado por la opcional [ventana emergente de imagen al tocar para hacer zoom](#ventana-emergente-de-imagen-al-tocar-para-hacer-zoom), así que manténgalo si habilita `AllowImagePopup`.
- **El renderizado SVG** ahora utiliza un renderizador de MAUI Graphics integrado. Soporta `path` (conjunto completo de comandos incluyendo arcos), las formas básicas (`rect`, `circle`, `ellipse`, `line`, `polyline`, `polygon`), transformaciones, `fill`/`stroke` sólidos usando colores hex/`rgb()`/`rgba()`/nombrados, y **rellenos de degradado lineal y radial** (incluyendo `gradientUnits`, `gradientTransform`, multi-stop y herencia de `href`). Es un subconjunto pragmático y **no** soporta filtros, texto incrustado o clip/mask. Dos aproximaciones menores: los *trazos* de degradado vuelven a un color sólido representativo (el canvas de MAUI solo soporta rellenos de degradado), y el punto focal de un degradado radial (`fx`/`fy`) y el `spreadMethod` son ignorados.
  - Si muestra SVGs que dependan de cualquier función no soportada, por favor [abra un problema](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/issues).

---

### Versión 1.5.0 y superiores

La versión 1.5.0 introduce un nuevo **Sistema de Tematización** que cambia la forma en que se aplica el estilo al MarkdownView. Si está actualizando desde una versión anterior a la 1.4.0, tenga en cuenta los siguientes cambios:

#### Nueva Propiedad Theme
- Se ha añadido una nueva propiedad `Theme` que acepta un objeto `MarkdownTheme`.
- La propiedad `UseAppTheme` permite el cambio automático de modo claro/oscuro basado en el tema del sistema.

#### Nuevas Propiedades de Encabezado
- Se han añadido propiedades individuales de `FontFamily` y `FontAttributes` para cada nivel de encabezado (H1-H6).
- Nuevas propiedades: `H1FontFamily`, `H1FontAttributes`, `H2FontFamily`, `H2FontAttributes`, etc.
- Nuevos colores de encabezado para H4-H6: `H4Color`, `H5Color`, `H6Color`.

#### Guía de Migración

**Antes (< 1.5.0):**
```xml
<idk:MarkdownView 
    H1Color="Blue"
    H1FontSize="24"
    TextFontFace="Arial" />
```

**Después (>= 1.5.0) - Usando Tema (Recomendado):**
```xml
<idk:MarkdownView 
    Theme="{Binding CurrentTheme}"
    UseAppTheme="True" />
```

```csharp
// En su ViewModel o code-behind
CurrentTheme = MarkdownThemeDefaults.GitHub;
```

**Después (>= 1.5.0) - Usando Propiedades Individuales (Sigue siendo compatible):**
```xml
<idk:MarkdownView 
    H1Color="Blue"
    H1FontSize="24"
    H1FontFamily="Arial"
    H1FontAttributes="Bold"
    TextFontFace="Arial" />
```

> **Nota:** Las propiedades de estilo individuales existentes (como `H1Color`, `H1FontSize`, etc.) siguen siendo totalmente compatibles. El nuevo sistema de tematización es una característica adicional que proporciona una forma más organizada de gestionar los estilos. Puede seguir utilizando las propiedades individuales si lo prefiere.

---

## Instalación

Puede instalar el paquete `Indiko.Maui.Controls.Markdown` a través del Administrador de Paquetes NuGet o la CLI:

[![NuGet](https://img.shields.io/nuget/v/Indiko.Maui.Controls.Markdown.svg?label=NuGet)](https://www.nuget.org/packages/Indiko.Maui.Controls.Markdown/)

### Administrador de Paquetes NuGet
```bash
Install-Package Indiko.Maui.Controls.Markdown
```

### .NET CLI
```bash
dotnet add package Indiko.Maui.Controls.Markdown
```

## Primeros Pasos

### 1. (Opcional) Registrar en `MauiProgram.cs`

```csharp
using Indiko.Maui.Controls.Markdown;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseMarkdownView();   // ver nota abajo

    return builder.Build();
}
```

> **Nota:** `UseMarkdownView()` ya no registra SkiaSharp (el control no lo necesita). Ahora registra el manejador nativo que impulsa la [ventana emergente de imagen al tocar para hacer zoom](#ventana-emergente-de-imagen-al-tocar-para-hacer-zoom) — así que **llámelo si usa `AllowImagePopup`**. Si no usa esa función, la llamada es opcional y segura de mantener o eliminar; las matemáticas, el SVG y todo lo demás se renderizan sin ninguna configuración adicional.

### 2. Agregar el espacio de nombres XAML y el control

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

> 💡 `MarkdownView` organiza su contenido de arriba hacia abajo y no se desplaza por sí solo — envuélvalo en un `ScrollView` para documentos largos.

### 3. …o crearlo en C#

```csharp
var markdown = new MarkdownView
{
    MarkdownText = "# ¡Hola, Markdown!\n\nEsto es **negrita** y esto es *cursiva*."
};
```

Eso es todo. Todo lo demás en esta página — tematización, estilo por elemento, eventos y la sintaxis soportada — es personalización opcional.

## Soporte de Tematización

El control `MarkdownView` incluye un potente sistema de tematización inspirado en el enfoque de MudBlazor. Los temas le permiten definir un estilo visual completo para su contenido markdown, incluyendo colores, tipografía y espaciado, todo en un solo objeto reutilizable.

### Estructura del Tema

Un tema consta de tres componentes principales:

- **`Palette`**: Paleta de colores para el modo claro.
- **`PaletteDark`**: Paleta de colores para el modo oscuro (opcional, vuelve a la Palette si no existe).
- **`Typography`**: Tamaños de fuente, familias de fuente, alturas de línea y espaciado.

### Uso de Temas Integrados

La biblioteca incluye varios temas predefinidos que puede usar directamente:

| Tema | Descripción |
|-------|-------------|
| `Light` | Tema claro predeterminado con colores neutros |
| `Dark` | Tema oscuro optimizado para fondos oscuros |
| `GitHub` | Apariencia de markdown al estilo de GitHub |
| `OneDark` | Tema de sintaxis One Dark de Atom |
| `OneLight` | Tema de sintaxis One Light de Atom |
| `Dracula` | Popular esquema de colores Dracula |
| `Nord` | Paleta de colores ártica, azulada |
| `Sepia` | Experiencia de lectura cálida, similar al papel |
| `Compact` | Tamaños de fuente y espaciado reducidos |
| `HighContrast` | Alto contraste para accesibilidad |
| `DotNetPurple` | Colores púrpura de la marca .NET |

#### Uso de un Tema Integrado en C#

```csharp
using Indiko.Maui.Controls.Markdown.Theming;

// Aplicar un tema integrado
markdownView.Theme = MarkdownThemeDefaults.GitHub;

// Habilitar el cambio automático de modo claro/oscuro
markdownView.UseAppTheme = true;
```

#### Uso de un Tema Integrado en XAML con Binding

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
// En su ViewModel
public MarkdownTheme CurrentTheme { get; set; } = MarkdownThemeDefaults.GitHub;

// Cambiar temas dinámicamente
public void SwitchToOneDark()
{
    CurrentTheme = MarkdownThemeDefaults.OneDark;
}
```

### Creación de Temas Personalizados

Puede crear temas totalmente personalizados definiendo su propia paleta y configuración de tipografía.

#### Tema Personalizado en C#

```csharp
using Indiko.Maui.Controls.Markdown.Theming;

var customTheme = new MarkdownTheme();

// Personalizar paleta clara
customTheme.Palette.TextPrimary = Color.FromArgb("#333333");
customTheme.Palette.H1Color = Color.FromArgb("#1a73e8");
customTheme.Palette.H2Color = Color.FromArgb("#1557b0");
customTheme.Palette.H3Color = Color.FromArgb("#0d47a1");
customTheme.Palette.HyperlinkColor = Color.FromArgb("#1a73e8");
customTheme.Palette.CodeBlockBackground = Color.FromArgb("#f5f5f5");
customTheme.Palette.CodeBlockText = Color.FromArgb("#d32f2f");
customTheme.Palette.BlockQuoteBackground = Color.FromArgb("#e3f2fd");
customTheme.Palette.BlockQuoteBorder = Color.FromArgb("#1a73e8");

// Personalizar paleta oscura
customTheme.PaletteDark.TextPrimary = Color.FromArgb("#e0e0e0");
customTheme.PaletteDark.H1Color = Color.FromArgb("#8ab4f8");
customTheme.PaletteDark.H2Color = Color.FromArgb("#669df6");
customTheme.PaletteDark.H3Color = Color.FromArgb("#4285f4");
customTheme.PaletteDark.HyperlinkColor = Color.FromArgb("#8ab4f8");
customTheme.PaletteDark.CodeBlockBackground = Color.FromArgb("#1e1e1e");
customTheme.PaletteDark.CodeBlockText = Color.FromArgb("#f48fb1");

// Personalizar tipografía
customTheme.Typography.H1FontSize = 32;
customTheme.Typography.H2FontSize = 26;
customTheme.Typography.H3FontSize = 22;
customTheme.Typography.BodyFontSize = 16;
customTheme.Typography.CodeFontSize = 14;
customTheme.Typography.CodeFontFamily = "Cascadia Code";
customTheme.Typography.LineHeight = 1.6;
customTheme.Typography.ParagraphSpacing = 1.2;

// Aplicar el tema
markdownView.Theme = customTheme;
```

#### Tema Personalizado en XAML

Puede definir los temas como recursos en XAML para reutilizarlos en toda su aplicación:

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

### Propiedades de Palette

La clase `MarkdownPalette` contiene las siguientes propiedades de color:

| Propiedad | Descripción |
|----------|-------------|
| `Primary` | Color de acento primario |
| `Secondary` | Color de acento secundario |
| `Background` | Color de fondo predeterminado |
| `Surface` | Color de superficie para elementos elevados |
| `TextPrimary` | Color de texto primario |
| `TextSecondary` | Color de texto secundario |
| `TextDisabled` | Color de texto deshabilitado |
| `HighlightColor` | Fondo para texto resaltado/marcado en línea (`==texto==`) |
| `H1Color` | Color para encabezados H1 |
| `H2Color` | Color para encabezados H2 |
| `H3Color` | Color para encabezados H3 |
| `H4Color` | Color para encabezados H4 |
| `H5Color` | Color para encabezados H5 |
| `H6Color` | Color para encabezados H6 |
| `HyperlinkColor` | Color para hipervínculos |
| `CodeBlockBackground` | Color de fondo para bloques de código |
| `CodeBlockBorder` | Color de borde para bloques de código |
| `CodeBlockText` | Color de texto para bloques de código |
| `BlockQuoteBackground` | Color de fondo para citas |
| `BlockQuoteBorder` | Color de borde para citas |
| `BlockQuoteText` | Color de texto para citas |
| `TableHeaderBackground` | Color de fondo para encabezados de tabla |
| `TableHeaderText` | Color de texto para encabezados de tabla |
| `TableRowBackground` | Color de fondo para filas de tabla |
| `TableRowText` | Color de texto para filas de tabla |
| `TableBorder` | Color de borde para tablas |
| `DividerColor` | Color para reglas horizontales |
| `InfoColor` | Color para alertas de info / `[!NOTE]` |
| `WarningColor` | Color para alertas de advertencia / `[!WARNING]` |
| `ErrorColor` | Color para alertas de error / `[!CAUTION]` |
| `SuccessColor` | Color para alertas de éxito / `[!TIP]` |
| `ImportantColor` | Color para alertas `[!IMPORTANT]` |
| `ImagePopupBackground` | Color de fondo de la superposición de [ventana emergente de imagen al tocar para hacer zoom](#ventana-emergente-de-imagen-al-tocar-para-hacer-zoom) (negro por defecto) |
| `ImagePopupCloseButton` | Color del botón de cerrar (✕) de la ventana emergente de imagen (blanco por defecto) |

La paleta también proporciona un método auxiliar:
- `GetHeadingColor(int level)` - Devuelve el color apropiado para los niveles de encabezado 1-6.

### Propiedades de Typography

La clase `MarkdownTypography` contiene las siguientes propiedades:

| Propiedad | Descripción | Predeterminado |
|----------|-------------|---------|
| `DefaultFontFamily` | Familia de fuente predeterminada para el texto del cuerpo | null |
| `HeadingFontFamily` | Familia de fuente predeterminada para todos los encabezados (puede anularse por nivel) | null |
| `CodeFontFamily` | Familia de fuente para código | "Consolas" |
| `BlockQuoteFontFamily` | Familia de fuente para citas | "Consolas" |
| `H1FontSize` | Tamaño de fuente para H1 | 28 |
| `H1FontFamily` | Familia de fuente para H1 (anula HeadingFontFamily) | null |
| `H1FontAttributes` | Atributos de fuente para H1 (None, Bold, Italic) | Bold |
| `H2FontSize` | Tamaño de fuente para H2 | 24 |
| `H2FontFamily` | Familia de fuente para H2 (anula HeadingFontFamily) | null |
| `H2FontAttributes` | Atributos de fuente para H2 | Bold |
| `H3FontSize` | Tamaño de fuente para H3 | 20 |
| `H3FontFamily` | Familia de fuente para H3 (anula HeadingFontFamily) | null |
| `H3FontAttributes` | Atributos de fuente para H3 | Bold |
| `H4FontSize` | Tamaño de fuente para H4 | 18 |
| `H4FontFamily` | Familia de fuente para H4 (anula HeadingFontFamily) | null |
| `H4FontAttributes` | Atributos de fuente para H4 | Bold |
| `H5FontSize` | Tamaño de fuente para H5 | 16 |
| `H5FontFamily` | Familia de fuente para H5 (anula HeadingFontFamily) | null |
| `H5FontAttributes` | Atributos de fuente para H5 | Bold |
| `H6FontSize` | Tamaño de fuente para H6 | 14 |
| `H6FontFamily` | Familia de fuente para H6 (anula HeadingFontFamily) | null |
| `H6FontAttributes` | Atributos de fuente para H6 | Bold |
| `BodyFontSize` | Tamaño de fuente para el texto del cuerpo | 14 |
| `CodeFontSize` | Tamaño de fuente para código | 13 |
| `TableHeaderFontSize` | Tamaño de fuente para encabezados de tabla | 14 |
| `TableRowFontSize` | Tamaño de fuente para filas de tabla | 13 |
| `LineHeight` | Multiplicador de altura de línea | 1.5 |
| `HeadingLineHeight` | Altura de línea para encabezados | 1.3 |
| `ParagraphSpacing` | Espaciado entre párrafos | 1.0 |
| `ListItemSpacing` | Espaciado entre elementos de lista | 4 |
| `ListIndent` | Sangría para listas anidadas | 20 |
| `TextLineBreakMode` | Modo de salto de línea para texto | WordWrap |
| `HeadingLineBreakMode` | Modo de salto de línea para encabezados | TailTruncation |

### Modo Claro/Oscuro Automático

Establezca `UseAppTheme="True"` para cambiar automáticamente entre `Palette` (modo claro) y `PaletteDark` (modo oscuro) basándose en el tema del sistema:

```xml
<idk:MarkdownView 
    Theme="{Binding CurrentTheme}"
    UseAppTheme="True" />
```

Cuando el tema del sistema cambia, el MarkdownView se renderiza automáticamente con la paleta apropiada.

> **Comportamiento:** Cuando `UseAppTheme` es `false` (predeterminado), el tema **siempre usa la `Palette` clara** independientemente del modo del sistema — `PaletteDark` solo se consulta cuando `UseAppTheme="True"` *y* el sistema está en modo oscuro. Por lo tanto, establezca `UseAppTheme="True"` (y proporcione ambas paletas) si desea soporte para modo oscuro.

> **Asignar un tema integrado directamente en XAML** utiliza `x:Static` (los ajustes predefinidos son propiedades estáticas), mientras que un tema expuesto por su view model se vincula normalmente:
> ```xml
> <!-- ajuste directo -->
> <idk:MarkdownView Theme="{x:Static theming:MarkdownThemeDefaults.GitHub}" UseAppTheme="True" />
> <!-- o desde un view model -->
> <idk:MarkdownView Theme="{Binding CurrentTheme}" UseAppTheme="True" />
> ```

### Clonar y Modificar Temas

Puede clonar un tema existente y modificarlo:

```csharp
// Clonar un tema integrado y personalizarlo
var myTheme = MarkdownThemeDefaults.GitHub.Clone();
myTheme.Palette.HyperlinkColor = Colors.Orange;
myTheme.Typography.H1FontSize = 36;

markdownView.Theme = myTheme;
```

---

## Propiedades Vinculables

Estas propiedades por elemento le permiten dar estilo al control directamente, sin un objeto `Theme`. (Cuando **se** establece un `Theme`, este anula estas propiedades — consulte [Soporte de Tematización](#soporte-de-tematización)).

> **Variantes de color claro/oscuro:** varias propiedades de color también exponen acompañantes `…Light` y `…Dark` — `TextColorLight`/`TextColorDark`, `H1ColorLight`/`H1ColorDark`, `H2ColorLight`/`H2ColorDark`, `H3ColorLight`/`H3ColorDark`, `HyperlinkColorLight`/`HyperlinkColorDark`, `CodeBlockBackgroundColorLight`/`Dark`, `CodeBlockTextColorLight`/`Dark`, `BlockQuoteBackgroundColorLight`/`Dark` y `BlockQuoteTextColorLight`/`Dark`. Estas alimentan las paletas clara/oscura del sistema de tematización; para un estilo manual simple, solo establezca la propiedad base (por ejemplo, `TextColor`).

### Tema
- **`Theme`**: Un objeto `MarkdownTheme` que define el estilo visual completo (colores, tipografía, espaciado).
- **`UseAppTheme`**: Cuando es `true`, cambia automáticamente entre paletas clara y oscura basándose en el tema del sistema (predeterminado: `false`).

### Encabezados
Cada nivel de encabezado (H1-H6) admite estilo individual con las siguientes propiedades:

| Nivel | Propiedad de Color | Propiedad de Tamaño de Fuente | Propiedad de Familia de Fuente | Propiedad de Atributos de Fuente |
|-------|---------------|-------------------|---------------------|-------------------------|
| H1 | `H1Color` | `H1FontSize` (24) | `H1FontFamily` | `H1FontAttributes` (Bold) |
| H2 | `H2Color` | `H2FontSize` (20) | `H2FontFamily` | `H2FontAttributes` (Bold) |
| H3 | `H3Color` | `H3FontSize` (18) | `H3FontFamily` | `H3FontAttributes` (Bold) |
| H4 | `H4Color` | `H4FontSize` (16) | `H4FontFamily` | `H4FontAttributes` (Bold) |
| H5 | `H5Color` | `H5FontSize` (14) | `H5FontFamily` | `H5FontAttributes` (Bold) |
| H6 | `H6Color` | `H6FontSize` (12) | `H6FontFamily` | `H6FontAttributes` (Bold) |

**Valores de FontAttributes:**
- `None` - Sin formato especial
- `Bold` - Texto en negrita
- `Italic` - Texto en cursiva

**Ejemplo - Estilo personalizado de H1:**
```xml
<idk:MarkdownView 
    H1Color="DarkBlue"
    H1FontSize="32"
    H1FontFamily="Georgia"
    H1FontAttributes="Bold" />
```

**Ejemplo - Encabezado H3 en cursiva:**
```csharp
markdownView.H3FontAttributes = FontAttributes.Italic;
markdownView.H3Color = Colors.Purple;
```

### Estilo de Texto
- **`TextFontSize`**: El tamaño de fuente para el texto regular (predeterminado: `12`).
- **`TextColor`**: El color para el texto regular (predeterminado: `Black`).
- **`TextFontFace`**: La familia de fuente para el texto regular.
- **`HighlightColor`**: El color de fondo utilizado para el texto resaltado/marcado en línea (`==texto==`) (predeterminado: `#FFF59D`).
- **`TextHorizontalTextAlignment`**: La alineación horizontal del texto del cuerpo (prosa) — párrafos, elementos de lista y texto de citas. Acepta `Start` (predeterminado), `Center`, `End` o `Justify`. Los encabezados y las celdas de tabla no se ven afectados (las tablas mantienen su alineación por columna).

  ```xml
  <idk:MarkdownView 
      MarkdownText="{Binding MarkdownText}"
      TextHorizontalTextAlignment="Justify" />
  ```

### Modo de Salto de Línea
- **`AllowLineBreaksOnHeadlines`**: Cuando es `true` (predeterminado), los encabezados se ajustan en varias líneas (`WordWrap`). Cuando es `false`, los encabezados se truncan a una sola línea usando `LineBreakModeHeader`.
- **`LineBreakModeText`**: Modo de salto de línea para el texto (predeterminado: `WordWrap`).
- **`LineBreakModeHeader`**: Modo de salto de línea para encabezados cuando `AllowLineBreaksOnHeadlines` es `false` (predeterminado: `TailTruncation`).

### Cita (Blockquote)
- **`BlockQuoteBackgroundColor`**: El color de fondo para los elementos de cita (predeterminado: `LightGray`).
- **`BlockQuoteBorderColor`**: El color del borde para los elementos de cita (predeterminado: `BlueViolet`).
- **`BlockQuoteTextColor`**: El color del texto para las citas (predeterminado: `BlueViolet`).
- **`BlockQuoteFontFace`**: La familia de fuente para el texto de la cita.

### Bloque de Código
- **`CodeBlockBackgroundColor`**: El color de fondo para los bloques de código (predeterminado: `LightGray`).
- **`CodeBlockBorderColor`**: El color del borde para los bloques de código (predeterminado: `BlueViolet`).
- **`CodeBlockTextColor`**: El color del texto para los bloques de código (predeterminado: `BlueViolet`).
- **`CodeBlockFontFace`**: La familia de fuente para los bloques de código.
- **`CodeBlockFontSize`**: El tamaño de fuente para los bloques de código (predeterminado: `12`).
- **`EnableCodeBlockCopy`**: Cuando es `true`, muestra un botón de copiar en los bloques de código (predeterminado: `false`).
- **`CodeBlockCopyButtonText`**: El texto/emoji para el botón de copiar (predeterminado: `📋`).
- **`CodeBlockCopyButtonCopiedText`**: El texto/emoji mostrado después de copiar (predeterminado: `✓`).

### Bloques de Alerta
- **`AlertInfoColor`**: Color para los bloques de alerta NOTE (predeterminado: `#2196F3`).
- **`AlertSuccessColor`**: Color para los bloques de alerta TIP (predeterminado: `#4CAF50`).
- **`AlertImportantColor`**: Color para los bloques de alerta IMPORTANT (predeterminado: `#9C27B0`).
- **`AlertWarningColor`**: Color para los bloques de alerta WARNING (predeterminado: `#FF9800`).
- **`AlertErrorColor`**: Color para los bloques de alerta CAUTION (predeterminado: `#F44336`).

### Tabla
- **`TableHeaderFontSize`**: El tamaño de fuente para los encabezados de tabla (predeterminado: `14`).
- **`TableHeaderTextColor`**: El color del texto para los encabezados de tabla (predeterminado: `Black`).
- **`TableHeaderBackgroundColor`**: El color de fondo para los encabezados de tabla (predeterminado: `LightGray`).
- **`TableHeaderFontFace`**: La familia de fuente para los encabezados de tabla.
- **`TableRowFontSize`**: El tamaño de fuente para las filas de tabla (predeterminado: `12`).
- **`TableRowTextColor`**: El color del texto para las filas de tabla (predeterminado: `Black`).
- **`TableRowBackgroundColor`**: El color de fondo para las filas de tabla (predeterminado: `White`).
- **`TableRowFontFace`**: La familia de fuente para las filas de tabla.

### Imagen
- **`ImageAspect`**: La relación de aspecto para las imágenes (predeterminado: `AspectFit`).
- **`DefaultImageWidth`**: El ancho predeterminado para las imágenes cuando solo se especifica el aspecto sin dimensiones explícitas (predeterminado: `200`).
- **`DefaultImageHeight`**: El alto predeterminado para las imágenes cuando solo se especifica el aspecto sin dimensiones explícitas (predeterminado: `200`).
- **`AllowImagePopup`**: Cuando es `true`, tocar una imagen renderizada la abre en pantalla completa en una superposición nativa con zoom (predeterminado: `false`). Consulte [Ventana emergente de imagen al tocar para hacer zoom](#ventana-emergente-de-imagen-al-tocar-para-hacer-zoom).
- **`ImagePopupBackgroundColor`**: Color de fondo de la superposición de imagen (predeterminado: `Black`).
- **`ImagePopupCloseButtonColor`**: Color del botón de cerrar (✕) de la superposición (predeterminado: `White`).
- **`ImagePopupMaxZoomScale`**: Factor de zoom máximo, relativo al tamaño ajustado, dentro de la superposición (predeterminado: `5`).

### Enlaces y Correo
- **`HyperlinkColor`**: El color para los hipervínculos y enlaces de correo electrónico (predeterminado: `BlueViolet`).
- **`LinkCommand`**: Un comando ejecutado cuando se toca un hipervínculo — la URL tocada se pasa como parámetro del comando.
- **`LinkCommandParameter`**: Un parámetro opcional para pasar al `LinkCommand`.
- **`EMailCommand`**: Un comando ejecutado cuando se toca un enlace de correo electrónico — la dirección de correo se pasa como parámetro del comando.
- **`EMailCommandParameter`**: Un parámetro opcional para pasar al `EMailCommand`.

Consulte [Eventos y Comandos](#eventos--comandos) para los eventos correspondientes `OnHyperLinkClicked` / `OnEmailClicked`.

### Regla Horizontal
- **`LineColor`**: El color para las reglas horizontales (predeterminado: `LightGray`).

### Espaciado
- **`ParagraphSpacing`**: Un multiplicador para el espaciado entre bloques/párrafos (predeterminado: `1.0`).
- **`LineHeightMultiplier`**: Modifica el multiplicador de altura de línea para las etiquetas mostradas (predeterminado: `1.0`). Modifica directamente [Label.LineHeight](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.label.lineheight?view=net-maui-8.0).

## Sintaxis de Markdown Soportada

- **Encabezados**: MarkdownView soporta encabezados desde `H1` hasta `H6`.
    ```markdown
    # H1
    ## H2
    ### H3
    ```

- **Negrita**: Envuelva el texto con `**` o `__`.
    ```markdown
    **Negrita** y __también negrita__
    ```

- **Cursiva**: Envuelva el texto con `*` o `_`.
    ```markdown
    *Cursiva* y _también cursiva_
    ```

- **Negrita + Cursiva**: Combine con `***`.
    ```markdown
    ***negrita y cursiva***
    ```

- **Tachado, Resaltado, Superíndice/Subíndice, Insertado**: estilos adicionales en línea (extras de énfasis de Markdig).
    ```markdown
    ~~tachado~~, ==resaltado==, super^índice^, sub~índice~, ++insertado++
    ```
    - `==resaltado==` se dibuja con la propiedad `HighlightColor` como fondo (amarillo claro predeterminado `#FFF59D`).
    - `^superíndice^` / `~subíndice~` se **aproximan** con una fuente más pequeña — los tramos de texto de MAUI no pueden desplazar la línea base.
    - `++insertado++` se renderiza como texto subrayado.

- **Citas**: Agregue `>` antes de un párrafo para crear una cita.
    ```markdown
    > Esto es una cita.
    ```

- **Bloques de Alerta/Admonición**: Cree bloques de alerta al estilo de GitHub usando la sintaxis `> [!TIPO]`. Son cuadros de llamada estilizados para resaltar información importante.

    Tipos de alerta soportados:
    - `NOTE` - Notas informativas (ℹ️ azul)
    - `TIP` - Consejos y sugerencias útiles (💡 verde)
    - `IMPORTANT` - Información importante (❗ púrpura)
    - `WARNING` - Mensajes de advertencia (⚠️ naranja)
    - `CAUTION` - Advertencias críticas (🛑 rojo)

    ```markdown
    > [!NOTE]
    > Esto es una alerta de nota. Útil para resaltar información.

    > [!TIP]
    > Esto es una alerta de consejo. Proporciona sugerencias útiles.

    > [!IMPORTANT]
    > Esto es una alerta importante. Información crucial para los usuarios.

    > [!WARNING]
    > Esto es una alerta de advertencia. Contenido que requiere atención.

    > [!CAUTION]
    > Esto es una alerta de precaución. Posibles consecuencias negativas.
    ```

    Puede personalizar los colores para cada tipo de alerta utilizando las siguientes propiedades:
    - `AlertInfoColor` - Color para alertas NOTE (predeterminado: `#2196F3`)
    - `AlertSuccessColor` - Color para alertas TIP (predeterminado: `#4CAF50`)
    - `AlertImportantColor` - Color para alertas IMPORTANT (predeterminado: `#9C27B0`)
    - `AlertWarningColor` - Color para alertas WARNING (predeterminado: `#FF9800`)
    - `AlertErrorColor` - Color para alertas CAUTION (predeterminado: `#F44336`)

- **Bloques de Código**: Encierre el código en un bloque delimitado (triples comillas invertidas). El idioma opcional después de la valla de apertura es aceptado pero **no** tiene resaltado de sintaxis — el código se muestra en un solo color (`CodeBlockTextColor`) y con ajuste de línea. También se admiten bloques de código indentados (4 espacios).

    ````markdown
    ```csharp
    var greeting = "Hello, world!";
    Console.WriteLine(greeting);
    ```
    ````

    **Código en línea**: envuelva el texto en comillas invertidas simples — se renderiza en la fuente de código utilizando los colores de texto/fondo del bloque de código.
    ```markdown
    Ejecute el comando `dotnet build`, luego `dotnet test`.
    ```

    **Función de Copiar al Portapapeles**: puede habilitar un botón de copia en los bloques de código que permite a los usuarios copiar el contenido del código al portapapeles con un solo toque.

    ```xml
    <idk:MarkdownView 
        MarkdownText="{Binding MarkdownText}"
        EnableCodeBlockCopy="True"
        CodeBlockCopyButtonText="📋"
        CodeBlockCopyButtonCopiedText="✓" />
    ```

    Propiedades:
    - `EnableCodeBlockCopy` - Habilitar/deshabilitar el botón de copia (predeterminado: `false`)
    - `CodeBlockCopyButtonText` - Texto/emoji para el botón de copia (predeterminado: `📋`)
    - `CodeBlockCopyButtonCopiedText` - Texto/emoji mostrado después de copiar (predeterminado: `✓`)

- **Listas**: se admiten listas desordenadas, ordenadas, **anidadas** y de **tareas/casillas de verificación**.
    - Desordenadas: comience las líneas con `-`, `*` o `+` (renderizadas con un punto `•`).
    - Ordenadas: comience las líneas con un número seguido de un punto (se renderiza el número real).
    - Anidadas: idente los subelementos — cada nivel añade sangría.
    - Listas de tareas: `- [ ]` (sin marcar) y `- [x]` (marcada) renderizan una casilla de verificación de **solo lectura** + etiqueta.

    ```markdown
    - Elemento 1
      - Elemento anidado 1.1
        - Elemento anidado 1.1.1
    - Elemento 2

    1. Primero
    2. Segundo

    - [x] Tarea completada
    - [ ] Tarea pendiente
    ```

- **Tablas**: cree tablas usando tuberías (`|`) para separar las columnas. También puede especificar la alineación del texto para las columnas de la tabla usando dos puntos (`:`).

    - **Alineación Izquierda**: agregue dos puntos en el lado izquierdo de los guiones.
    - **Alineación Central**: agregue dos puntos en ambos lados de los guiones.
    - **Alineación Derecha**: agregue dos puntos en el lado derecho de los guiones.

    ```markdown
    | Encabezado 1  | Encabezado 2  | Encabezado 3  |
    |:------------- |:-------------:| -------------:|
    | Alineado Izq | Alineado Cent | Alineado Der  |
    | Fila 1        | Dato 1        | Dato 1        |
    | Fila 2        | Dato 2        | Dato 2        |
    ```

    En el ejemplo anterior:
    - La columna 1 (Encabezado 1) está alineada a la izquierda.
    - La columna 2 (Encabezado 2) está alineada al centro.
    - La columna 3 (Encabezado 3) está alineada a la derecha.

- **Regla Horizontal**: use `---`, `***` o `___` para reglas horizontales.
    ```markdown
    ---   <!-- Regla horizontal -->
    ```

- **Matemáticas / LaTeX**: tanto las matemáticas **en bloque** como las **en línea** se componen como LaTeX real, renderizado con `Microsoft.Maui.Graphics` (sin SkiaSharp).
    - **Bloque** — ponga una fórmula en sus propias líneas entre vallas `$$`:
      ```markdown
      $$
      \int_{-\infty}^{\infty} e^{-x^2}\,dx = \sqrt{\pi}
      $$
      ```
    - **En línea** — envuelva una fórmula en un solo `$` dentro de una oración:
      ```markdown
      La identidad $E = mc^2$ relaciona la energía y la masa.
      ```
    > Las matemáticas en línea se organizan como un flujo envolvente de texto + segmentos de fórmula, por lo que se ajustan en los límites de los segmentos en lugar de palabra por palabra (un tramo de texto de MAUI no puede romperse a través de una vista incrustada). En **celdas de tabla y encabezados**, las matemáticas en línea vuelven al texto bruto estilizado en lugar de una vista compuesta.

- **Contenedores Personalizados**: envuelva el contenido en vallas `:::`; la primera palabra después de la valla de apertura selecciona el color del contorno.
    ```markdown
    ::: info
    Un contenedor informativo.
    :::
    ```
    Clases reconocidas: `info` (contorno azul), `warning` (naranja), `danger` (rojo); cualquier otro valor usa un contorno gris claro. El contenedor se dibuja como un contorno coloreado alrededor de su contenido interno.

- **Emoji**: use la sintaxis `:shortcode:` — los shortcodes se convierten en emoji Unicode.
    ```markdown
    Envíalo :rocket: y celebra :tada:
    ```
    > Las caritas ASCII (por ejemplo, `:)`) **no** se convierten intencionalmente.

- **Autovínculos**: las URLs simples y las direcciones de correo electrónico se detectan automáticamente y se vuelven tocables — no es necesario `[...](...)`.
    ```markdown
    Visite https://github.com o envíeme un correo a me@example.com
    ```

- **Enlaces de correo electrónico**: un enlace explícito `mailto:`, un enlace cuyo destino sea una dirección de correo electrónico, o una dirección de correo simple se convierten en enlaces de correo tocables (consulte [Manejo de Enlaces de Correo](#manejo-de-enlaces-de-correo)).
    ```markdown
    [Envíeme un correo](mailto:support@example.com)
    ```

- **Imágenes**: el control soporta URLs de imágenes, archivos locales, imágenes codificadas en base64 y **SVG** (cualquier URL que termine en `.svg` es rasterizada por el renderizador SVG integrado de MAUI Graphics).
  También puede especificar atributos opcionales de `width`, `height`, `aspect`, `horizontal` y `vertical` utilizando la sintaxis de llaves soportada por la extensión Generic Attributes de Markdig.

  - Los valores de `aspect` soportados son:
    - `AspectFit` (predeterminado) – escala la imagen para que quepa preservando la relación de aspecto.
    - `AspectFill` – llena el espacio preservando la relación de aspecto (la imagen puede quedar recortada).
    - `Fill` – estira la imagen para llenar el espacio (no se preserva la relación de aspecto).

  - Los valores de `horizontal` soportados son:
    - `Start` (predeterminado) – alinea la imagen al inicio (izquierda en diseños LTR).
    - `Center` – centra la imagen horizontalmente.
    - `End` – alinea la imagen al final (derecha en diseños LTR).
    - `Fill` – estira la imagen para llenar horizontalmente.

  - Los valores de `vertical` soportados son:
    - `Start` – alinea la imagen arriba.
    - `Center` (predeterminado) – centra la imagen verticalmente.
    - `End` – alinea la imagen abajo.
    - `Fill` – estira la imagen para llenar verticalmente.

    ```markdown
    ![Texto alt](http://example.com/image.jpg)                      // URL de imagen
    ![Texto alt](image.png)                                         // archivo local
    ![Texto alt](https://example.com/icon.svg)                      // SVG remoto (rasterizado)
    ![Texto alt](data:image/png;base64,...)                         // cadena base64
    ![Texto alt](image.png){ width=150 height=75 }                  // especificar ancho y alto (píxeles)
    ![Texto alt](image.png){ width=200 }                            // especificar solo ancho
    ![Texto alt](image.png){ height=50 }                            // especificar solo alto
    ![Texto alt](image.png){ width=100 height=100 aspect=Fill }     // con aspecto personalizado
    ![Texto alt](image.png){ width=100 aspect=AspectFill }          // con solo aspecto
    ![Texto alt](image.png){ width=14 height=14 horizontal=Start vertical=Center }  // con posicionamiento
    ![Texto alt](image.png){ width=50 vertical=Start }              // solo posicionamiento vertical
    ![Texto alt](image.png){ horizontal=Center }                    // centrar imagen horizontalmente
    ```

- **Hipervínculos**: cree hipervínculos usando el siguiente formato:
    ```markdown
    [Texto del enlace](http://example.com)
    ```

## Imágenes

`MarkdownView` carga imágenes desde varias fuentes:

1. **Archivo local / recurso** — `![alt](logo.png)` (cargado vía `ImageSource.FromFile`).
2. **URL remota** — `![alt](https://…/pic.png)` (descargada por HTTP y almacenada en caché).
3. **Base64 / URI de datos** — `![alt](data:image/png;base64,…)`, o una cadena base64 simple.
4. **SVG** — cualquier URL que termine en `.svg` es descargada y rasterizada por el renderizador SVG integrado de `Microsoft.Maui.Graphics` (sin SkiaSharp). Consulte [Cambios Importantes](#cambios-importantes) para el conjunto de características de SVG soportadas.

Controle cómo se escalan las imágenes con la propiedad `ImageAspect` (o un atributo `aspect=` por imagen), y establezca el tamaño de respaldo con `DefaultImageWidth` / `DefaultImageHeight`. La lista completa de atributos por imagen (`width`, `height`, `aspect`, `horizontal`, `vertical`) está documentada en [Sintaxis de Markdown Soportada → Imágenes](#sintaxis-de-markdown-soportada).

> **Advertencias sobre Base64.** Si una imagen `data:` se renderiza en blanco o se muestra como texto literal, la causa es casi siempre los datos mismos, no el control:
> - **El enlace de la imagen debe estar bien formado** — no olvide el `)` de cierre antes de cualquier atributo `{…}`: `![alt](data:image/png;base64,XXXX){width=48}`. Un `)` faltante hace que todo se renderice como texto plano.
> - **El base64 debe decodificarse en una imagen completa y válida.** Un PNG truncado o corrupto puede seguir teniendo un encabezado legible (por lo que las herramientas informan sus dimensiones), pero puede fallar al decodificarse en el renderizado, mostrándose en blanco. Verifique con, por ejemplo, `echo '<base64>' | base64 -d > test.png && sips -g pixelWidth test.png`.

### Ventana emergente de imagen al tocar para hacer zoom

Establezca **`AllowImagePopup="True"`** para que cada imagen renderizada sea tocable. Al tocar una, se abre **en pantalla completa en una superposición nativa** — implementada con manejadores de plataforma (`UIScrollView` en iOS/Mac Catalyst, un `ImageView` basado en matriz en Android, `ScrollViewer` en Windows), por lo que los gestos se sienten nativos:

- **Pellizcar** para acercar/alejar, **arrastrar** para desplazar.
- **Doble toque** para alternar entre el tamaño ajustado y el zoom.
- Botón **✕** (arriba a la derecha) para cerrar.

Funciona para cada tipo de imagen soportado (local, remota, base64, SVG). La función está **desactivada por defecto**, por lo que los diseños existentes no cambian.

```xml
<idk:MarkdownView
    MarkdownText="{Binding MarkdownText}"
    AllowImagePopup="True"
    ImagePopupBackgroundColor="Black"
    ImagePopupCloseButtonColor="White"
    ImagePopupMaxZoomScale="5" />
```

Los colores de la superposición se pueden establecer por instancia (como arriba) **o mediante un tema** — la paleta expone `ImagePopupBackground` e `ImagePopupCloseButton` (con valores independientes claro/oscuro a través de `Palette` / `PaletteDark`), por lo que siguen su tema y el cambio automático claro/oscuro como cualquier otro color.

> **Requiere `UseMarkdownView()`.** La ventana emergente utiliza un manejador nativo que es registrado por `builder.UseMarkdownView()` en `MauiProgram.cs` (consulte [Primeros Pasos](#1-opcional-registrar-en-mauiprogramcs)). Si habilita `AllowImagePopup` sin llamarlo, al tocar una imagen no pasará nada.

## Eventos y Comandos

`MarkdownView` dispara **eventos** *y* soporta vinculaciones de `ICommand`, por lo que puede reaccionar a las interacciones desde el código subyacente o un view model.

| Evento | Comando | Se dispara cuando | Argumento pasado |
|---|---|---|---|
| `OnHyperLinkClicked` | `LinkCommand` | se toca un hipervínculo | la URL (`LinkEventArgs.Url`) |
| `OnEmailClicked` | `EMailCommand` | se toca un enlace de correo | la dirección (`EmailEventArgs.Email`) |
| `OnRenderError` | — | el markdown falla al renderizarse | `MarkdownRenderErrorEventArgs` (`.Exception`, `.Message`) |

### Manejo de Hipervínculos
Puede responder a los hipervínculos en el contenido de Markdown usando el `LinkCommand` y el evento `OnHyperLinkClicked`. Los hipervínculos se detectan automáticamente y se muestran con el color especificado por la propiedad `HyperlinkColor`.

Cuando un usuario toca un hipervínculo:
- Se ejecuta el `LinkCommand`, si está definido, con la URL del hipervínculo como parámetro del comando.
- Se dispara el evento `OnHyperLinkClicked`, proporcionando la URL del hipervínculo tocado en los argumentos del evento.

### Manejo de Enlaces de Correo
Puede responder a los enlaces de correo electrónico en el contenido de Markdown usando el `EMailCommand` y el evento `OnEmailClicked`. Los enlaces de correo se detectan automáticamente y se muestran con el color especificado por la propiedad `HyperlinkColor`. 

Cuando un usuario toca una dirección de correo electrónico:
- Se ejecuta el `EMailCommand`, si está definido, con la dirección de correo tocada como parámetro del comando.
- Se dispara el evento `OnEmailClicked`, proporcionando la dirección de correo tocada en los argumentos del evento.

### Manejo de Errores de Renderizado
Los fallos de renderizado ya no se ignoran silenciosamente. Cuando el control no puede renderizar el markdown — por ejemplo, cuando se resuelve en tiempo de ejecución una versión de **Markdig** diferente e incompatible a la que el control fue construido (lo que lanza `MissingMethodException`/`TypeLoadException` mientras se construye la tubería), o cuando un bloque individual falla — el control:

- Dispara el evento **`OnRenderError`** con un `MarkdownRenderErrorEventArgs` que lleva la `Exception` y un `Message` legible para humanos, para que su aplicación pueda registrarlo o reaccionar.
- Renderiza un mensaje de error en línea en lugar de contenido en blanco. Para un desajuste de Markdig, el mensaje indica la versión de Markdig realmente cargada, facilitando su detección.

```csharp
markdownView.OnRenderError += (sender, e) =>
{
    // e.Exception y e.Message describen el fallo
    logger.LogError(e.Exception, "Markdown render failed: {Message}", e.Message);
};
```

> **Nota sobre Markdig:** el control llama a métodos de extensión de Markdig cuya API puede cambiar entre versiones mayores, por lo que su dependencia de NuGet está limitada a `[1.3.2, 2.0.0)`. Si su aplicación fija Markdig fuera de ese rango, NuGet muestra una advertencia de restauración en lugar de permitir que ocurra un desajuste silencioso en tiempo de ejecución.

## Notas y Limitaciones

El control renderiza un subconjunto pragmático de Markdown en vistas nativas. Conocer estos casos límite de antemano evita sorpresas:

- **Matemáticas en línea** (`$ … $`) se componen como LaTeX real en los párrafos, pero se ajustan en los límites de los segmentos (el texto alrededor de una fórmula no fluye palabra por palabra). Dentro de **celdas de tabla / encabezados**, vuelve al texto bruto estilizado.
- **Superíndice/subíndice** (`^x^` / `~x~`) se aproximan con una fuente más pequeña — los tramos de texto de MAUI no pueden desplazar la línea base, por lo que no se elevan/bajan.
- **Imágenes en tablas o mezcladas con texto en línea** se renderizan como el marcador literal `[Image]`. Una imagen **independiente** (un párrafo que contiene solo la imagen) se renderiza completamente y se escala al ancho disponible. (El énfasis en línea, el código, los enlaces y los estilos de extras de énfasis *sí* funcionan en esos contextos — y dentro de los encabezados).
- **Las celdas de tabla** renderizan solo su primer párrafo; las celdas de varios párrafos pierden el contenido extra.
- **Sin resaltado de sintaxis** en los bloques de código — el indicativo del idioma es aceptado, pero el código se muestra en un solo color.
- **Analizado pero no renderizado:** las notas al pie, las listas de definición, las citas, las abreviaturas, los pies de página y los marcadores de lista alfabéticos/romanos son reconocidos por el analizador pero no producen una salida especial.

¿Encontró algo aquí que debería "simplemente funcionar" para su caso de uso? [Abra un problema](https://github.com/0xc3u/Indiko.Maui.Controls.Markdown/issues) — varios de estos son candidatos para mejoras futuras.

## Ejemplo de Uso

Aquí tiene un ejemplo de cómo usar el `MarkdownView` en su XAML:

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

Aquí tiene un ejemplo de cómo usar el `MarkdownView` en su c#:

```csharp

 var markdownView = new MarkdownView
        {
            MarkdownText = "# Bienvenido a MarkdownView\n" +
                          "Esto es **texto en negrita**, y esto es *texto en cursiva*.\n\n" +
                          "Aquí hay una cita:\n\n" +
                          "> Esto es una cita\n\n" +
                          "Aquí hay una lista:\n" +
                          "- Elemento 1\n" +
                          "- Elemento 2\n\n" +
                          "Aquí hay un bloque de código:\n\n" +
                          "```\n" +
                          "var code = \"Esto es un bloque de código\";\n" +
                          "```\n\n" +
                          "Aquí hay un enlace: [Haga clic aquí](https://example.com)\n\n" +
                          "Aquí hay una imagen:\n" +
                          "![Texto alt](https://example.com/image.jpg)\n",

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
            DisplayAlert("Enlace tocado", $"Hizo clic en: {e.Url}", "OK");
        };
```

## Contribuciones

¡Las contribuciones al proyecto `MarkdownView` son muy bienvenidas! Ya sea que desee agregar nuevas funciones, mejorar las existentes, corregir errores o mejorar la documentación, su ayuda es muy apreciada.

---

# Cómo Contribuir

¡Gracias por considerar contribuir a nuestro proyecto! Siga estas pautas para asegurar un proceso fluido.

## 1. Trabaje en una Rama de Funcionalidad (Feature Branch)

Cree siempre una nueva rama para su funcionalidad o corrección. Esto mantiene la rama principal limpia y facilita la gestión de los cambios.

```bash
git checkout -b feature/nombre-de-su-funcionalidad
```

## 2. Inicie un Pull Request

Una vez que su funcionalidad esté completa, envíe su rama al repositorio e inicie un pull request para fusionarla con la rama principal. Asegúrese de que todas las pruebas pasen y de que su código siga los estándares de codificación del proyecto.

```bash
git push origin feature/nombre-de-su-funcionalidad
```

Luego, cree un pull request en GitHub y proporcione una descripción clara de sus cambios.

## 3. Use Prefijos de Semantic Release para los Commits

Al confirmar sus cambios, use prefijos de semantic release para categorizar sus commits. Esto ayuda a generar notas de lanzamiento y versionado automatizados.

El commit contiene los siguientes elementos estructurales para comunicar la intención a los consumidores de su biblioteca:

- **fix:** un commit de tipo fix corrige un error en su código base (esto se correlaciona con PATCH en Semantic Versioning).
- **feat:** un commit de tipo feat introduce una nueva funcionalidad al código base (esto se correlaciona con MINOR en Semantic Versioning).
- **BREAKING CHANGE:** un commit que tiene un pie de página BREAKING CHANGE:, o añade un ! después del tipo/alcance, introduce un cambio disruptivo en la API (correlacionándose con MAJOR en Semantic Versioning). Un BREAKING CHANGE puede ser parte de commits de cualquier tipo.
- Se permiten tipos distintos de fix: y feat:. Por ejemplo, @commitlint/config-conventional (basado en la convención de Angular) recomienda:
  - **build:** Cambios que afectan al sistema de compilación o dependencias externas.
  - **chore:** Otros cambios que no modifican los archivos src o test.
  - **ci:** Cambios en nuestros archivos de configuración y scripts de CI.
  - **docs:** Cambios solo en la documentación.
  - **style:** Cambios que no afectan el significado del código (espacios en blanco, formato, puntos y coma faltantes, etc.).
  - **refactor:** Un cambio de código que ni corrige un error ni añade una funcionalidad.
  - **perf:** Un cambio de código que mejora el rendimiento.
  - **test:** Agregar pruebas faltantes o corregir pruebas existentes.

Se pueden proporcionar pies de página distintos de BREAKING CHANGE: \<descripción\> y seguir una convención similar al formato de remolque de git. Los tipos adicionales no son obligatorios por la especificación de Conventional Commits y no tienen un efecto implícito en Semantic Versioning (a menos que incluyan un BREAKING CHANGE). Se puede proporcionar un alcance al tipo de un commit para brindar información contextual adicional y se contiene entre paréntesis, por ejemplo, feat(parser): add ability to parse arrays.

Ejemplos de mensajes de commit:

```bash
git commit -m "fix: resolve issue with user authentication"
git commit -m "feat: add new payment gateway integration"
git commit -m "BREAKING CHANGE: update API endpoints"
```

## 4. Escriba Mensajes de Commit Significativos

Los mensajes de commit deben ser concisos pero descriptivos. Deben explicar el "qué" y el "por qué" de sus cambios.

- **Buen ejemplo:** `fix: correct typo in user profile page`
- **Mal ejemplo:** `fixed stuff`

## Consejos Adicionales

- Asegúrese de que su código cumpla con los estándares y pautas de codificación del proyecto.
- Incluya pruebas para nuevas funcionalidades o correcciones de errores.
- Mantenga sus commits atómicos; un solo commit debe representar un solo cambio lógico.
- Actualice la documentación para reflejar cualquier nueva funcionalidad o cambio.

¡Agradecemos sus contribuciones y esperamos sus pull requests!

¡Feliz programación!
