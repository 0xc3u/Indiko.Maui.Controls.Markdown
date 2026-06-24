# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A single-control .NET MAUI library (`Indiko.Maui.Controls.Markdown`) published to NuGet. It renders Markdown into **native MAUI views** (Labels, Grids, Images, BoxViews) — there is no WebView. Markdig parses the text; the control walks the resulting block tree and builds a visual tree by hand.

## Build & release

All projects target **.NET 10** (`net10.0` plus `-android`, `-ios`, `-maccatalyst`, and `-windows` on Windows). The MAUI workload is required: `dotnet workload install maui`.

```bash
# Build the library (this is exactly what CI runs)
dotnet build src/Indiko.Maui.Controls.Markdown.sln -c Release

# Faster inner loop — build a single target framework
dotnet build src/Indiko.Maui.Controls.Markdown/Indiko.Maui.Controls.Markdown.csproj -c Release -f net10.0-android

# Build the sample app (references the library project directly)
dotnet build samples/Indiko.Maui.Controls.Markdown.Sample.sln

# Pack a NuGet package locally
dotnet pack src/Indiko.Maui.Controls.Markdown/Indiko.Maui.Controls.Markdown.csproj -c Release -p:PackageVersion=1.5.0
```

### Tests

`tests/Indiko.Maui.Controls.Markdown.Tests` (xUnit, `net10.0`, `UseMaui=true`) renders markdown through a **real `MarkdownView` instance and asserts on the produced MAUI view tree** — no device or dispatcher needed, because `MarkdownView` builds its `Content` synchronously when `MarkdownText` is set. `MarkdownTestHarness` provides `Render(md)` plus tree-walkers (`Labels`, `Grids`, `Spans`). This is the place to add a regression test or to reproduce a "renders wrong" report (e.g. `EmphasisRenderingTests` proves the control bolds content that a user reported as plain — the real cause being indentation).

```bash
dotnet test tests/Indiko.Maui.Controls.Markdown.Tests
```

The test project is deliberately **not** in `src/Indiko.Maui.Controls.Markdown.sln`, so CI's `dotnet build` and the release pack don't touch it — run it locally. (Image tests inspect only the synchronously-built `Grid`/column structure; the async image load may fault on a background thread without a MainThread, which is harmless to the assertions.)

### Dependency versions (Central Package Management)

Package versions are **centralized** — do not put `Version=` on a `<PackageReference>` in any `.csproj`. Instead:
- `Directory.Packages.props` (repo root) holds every `<PackageVersion>`. Bump a dependency for the whole repo here.
- `Directory.Build.props` (repo root) sets `<MauiVersion>`, which feeds both the explicit `Microsoft.Maui.Controls*` references and MAUI's own implicit references so they stay aligned. Bump MAUI here.

**No SkiaSharp.** The library renders math and SVG entirely through `Microsoft.Maui.Graphics` and has no SkiaSharp dependency of any kind — no `SkiaSharp.Views.Maui.Controls`, `Svg.Skia`, or `CSharpMath.SkiaSharp`. (It was dropped to avoid native binary mismatches — e.g. `SkiaSharp.Views.Maui`'s handler failing to load against a newer MAUI runtime when the platform-SDK band lags the package's.) Math goes through the `CSharpMath.MauiGraphics` project (see *Math & graphics*); SVG through the in-library `Svg/` renderer. If you ever reintroduce SkiaSharp, the old constraint returns: every SkiaSharp-binding package must stay on the same SkiaSharp major.

`Markdig` is intentionally a **bounded range `[1.3.2,2.0.0)`** (not a plain version). `RenderMarkdown` calls many Markdig extension methods whose API can change across majors; an app resolving a higher, incompatible Markdig caused a silent blank render (`MissingMethodException` swallowed). The range makes a Markdig 2.x in a consumer surface as a NuGet restore warning, and `RenderMarkdown`'s catch blocks now raise the public `OnRenderError` event (+ inline error label) instead of swallowing. To adopt Markdig 2.x: widen the range, re-verify every `Use*()` extension call in `RenderMarkdown` still exists, then test.

**Releases are fully automated** via [semantic-release](.releaserc) — do **not** manually bump versions or edit `CHANGELOG.md`. Pushing to `main`/`master` runs the build, then semantic-release computes the next version from conventional-commit messages, tags it, and the tag triggers `release-nuget.yml` to publish. This makes **commit message prefixes load-bearing**: `fix:` → patch, `feat:` → minor, `!`/`BREAKING CHANGE:` → major. `docs:`/`chore:`/`ci:` etc. do not release. `.md`-only pushes are ignored by CI.

## Architecture

### Rendering pipeline (`src/.../MarkdownView.cs`, ~2500 lines — the heart of the library)

`MarkdownText` changing → `RenderMarkdown(string)`:
1. Builds a `MarkdownPipelineBuilder` with a fixed extension set (alerts, math, tables, custom containers, generic attributes, emoji, etc.). **Extension order matters** — see the comments in `RenderMarkdown`; generic attributes must come first, emoji after emphasis.
2. `Markdig.Markdown.Parse` → `MarkdownDocument`.
3. Iterates top-level blocks, dispatching each through `RenderBlock` (a `switch` on Markdig block type) to a dedicated `RenderXxx` method (`RenderParagraph`, `RenderHeading`, `RenderQuote`, `RenderAlertBlock`, `RenderCode`/`RenderCodeBlock`, `RenderTable`, `RenderList`, `RenderFormula`, `RenderCustomContainer`).
4. Each returns a `View`; they're stacked into a `VerticalStackLayout` assigned to `Content`.

Inline content (bold/italic/links/emails/code spans) is rendered into `Span`s on a `FormattedString`. Email/link detection drives the `OnHyperLinkClicked`/`OnEmailClicked` events and `LinkCommand`/`EMailCommand`. Rendering is **defensively wrapped in try/catch at every level** — failures log to `Console` and skip the block rather than throwing, so a malformed element never crashes the host page.

To add support for a new Markdown element: enable the relevant Markdig extension in `RenderMarkdown`, add a `case` to the `RenderBlock` switch, and write a `RenderXxx` method following the existing ones as a blueprint.

### Theming (`src/.../Theming/`)

Two parallel styling APIs coexist and **both must keep working**:
- **Individual bindable properties** (`H1Color`, `CodeBlockFontSize`, …) — the original API, set directly on the control.
- **`Theme` object** (`MarkdownTheme` = `Palette` + `PaletteDark` + `Typography`) — newer, added in 1.5.0. `MarkdownThemeDefaults` is a static factory of built-in themes (`GitHub`, `OneDark`, `Dracula`, `Nord`, etc.).

`ApplyTheme()` is the bridge: it reads the active `MarkdownTheme` and **writes its values into the individual bindable properties**, so the render path only ever reads the individual properties. The `_isApplyingTheme` guard suppresses re-rendering while many properties are set in a batch (one render at the end instead of N). `UseAppTheme=true` subscribes to `Application.Current.RequestedThemeChanged` and swaps `Palette`/`PaletteDark` on system light/dark changes.

> The `samples/src/Indiko.Maui.Controls.Markdown/Theming/` folder is a stray duplicate — the canonical theming source is under `src/`.

### Math & graphics (both on MAUI Graphics, no SkiaSharp)

- **Math** — `LatexView` (`LatexView.cs`) is a `GraphicsView` whose `IDrawable` paints LaTeX via `CSharpMath.MauiGraphics.MathPainter`; `RenderFormula` uses it for `$$…$$` math blocks. `CSharpMath.MauiGraphics` (the separate `src/CSharpMath.MauiGraphics` project) is a SkiaSharp-free port of `CSharpMath.SkiaSharp`: it implements CSharpMath.Rendering's `ICanvas`/`Path` over `Microsoft.Maui.Graphics` `ICanvas`/`PathF`. It builds as plain `net10.0` (abstractions only) and is **bundled into this library's NuGet package** (it's `IsPackable=false`; the `BundleMauiGraphicsBackend` pack target adds its DLL, and `CSharpMath.Rendering` is a direct dependency so it flows to consumers). Gotcha: MAUI's `PathF.Close()` throws on an empty subpath (Skia's was a no-op) — `MauiPath` tracks open-contour state to avoid it.
- **SVG** — the in-library `Svg/` renderer (`SvgImage`, `SvgPathData`, `SvgPaint`) parses SVG and draws onto `Microsoft.Maui.Graphics`; `MarkdownView` calls `SvgImage.RenderToPng` to rasterize remote `.svg` URLs to a PNG `ImageSource`. Rasterization uses the platform `PlatformBitmapExportService`, so that step is behind `#if ANDROID || IOS || MACCATALYST || WINDOWS` (the plain `net10.0` target returns null — it isn't used at runtime). The parser covers paths (full command set incl. arcs), basic shapes, transforms, and hex/rgb/named fills+strokes; **not** gradients, filters, text, or clip/mask. Local `.svg` app resources still go through MAUI's native `MauiImage` pipeline, not this renderer.
- `BuilderExtension.UseMarkdownView()` is now a **no-op** kept for API compatibility (it used to call `UseSkiaSharp()`). Consumers can still call it; nothing breaks if they don't.

## Conventions (from `.github/copilot-instructions.md`)

- Allman braces, 4-space indent, ~120-char lines, `using`s outside the namespace.
- Prefer `Grid` over nested stacks; `Border` not `Frame`; `VerticalStackLayout`/`HorizontalStackLayout` not `StackLayout`.
- On iOS/MacCatalyst, watch for retain-cycle leaks from event handlers on `NSObject` subclasses (the control is `sealed`; prefer `static` handlers or weak-reference proxies — see the copilot instructions for the full pattern).
- Match the style of nearby code rather than introducing new patterns.

Note: `copilot-instructions.md` predates the .NET 10 upgrade and still says ".NET 9" — trust the `.csproj` files (`net10.0`) over that document.
