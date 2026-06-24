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

**There is no test project** — despite what `.github/copilot-instructions.md` says about a "tests project," none exists. Verify changes by building and running the sample app.

### Dependency versions (Central Package Management)

Package versions are **centralized** — do not put `Version=` on a `<PackageReference>` in any `.csproj`. Instead:
- `Directory.Packages.props` (repo root) holds every `<PackageVersion>`. Bump a dependency for the whole repo here.
- `Directory.Build.props` (repo root) sets `<MauiVersion>`, which feeds both the explicit `Microsoft.Maui.Controls*` references and MAUI's own implicit references so they stay aligned. Bump MAUI here.

The SkiaSharp-family packages (`SkiaSharp.Views.Maui.Controls`, `Svg.Skia`, `CSharpMath.SkiaSharp`) all bind SkiaSharp and must stay on the **same SkiaSharp major** — currently the **3.x** line, pinned via an explicit `SkiaSharp` `PackageVersion` (`3.119.4`) plus transitive pinning (`CentralPackageTransitivePinningEnabled`). `SkiaSharp.Views.Maui.Controls` deliberately stays on `3.119.x` rather than its `4.x` release: `Svg.Skia`'s latest (`5.1.1`) and `CSharpMath.SkiaSharp` (`0.5.1`) are still built against SkiaSharp 3.x/2.x, so moving Views to 4.x would force SkiaSharp 4.x and break alignment. Don't bump one of these without checking the others' SkiaSharp dependency.

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

### Math & graphics

- `LatexView` (`LatexView.cs`) is an `SKCanvasView` that paints LaTeX via `CSharpMath.SkiaSharp`; `RenderFormula` uses it for `$$…$$` math blocks.
- SkiaSharp also backs SVG image rendering (`Svg.Skia`). Because of this, consumers **must call `builder.UseMarkdownView()`** in `MauiProgram` — `BuilderExtension.UseMarkdownView()` calls `UseSkiaSharp()`, without which math/SVG rendering fails at runtime.

## Conventions (from `.github/copilot-instructions.md`)

- Allman braces, 4-space indent, ~120-char lines, `using`s outside the namespace.
- Prefer `Grid` over nested stacks; `Border` not `Frame`; `VerticalStackLayout`/`HorizontalStackLayout` not `StackLayout`.
- On iOS/MacCatalyst, watch for retain-cycle leaks from event handlers on `NSObject` subclasses (the control is `sealed`; prefer `static` handlers or weak-reference proxies — see the copilot instructions for the full pattern).
- Match the style of nearby code rather than introducing new patterns.

Note: `copilot-instructions.md` predates the .NET 10 upgrade and still says ".NET 9" — trust the `.csproj` files (`net10.0`) over that document.
