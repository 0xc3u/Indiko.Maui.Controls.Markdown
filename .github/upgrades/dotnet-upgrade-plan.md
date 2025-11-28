# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src/Indiko.Maui.Controls.Markdown/Indiko.Maui.Controls.Markdown.csproj
4. Upgrade samples/Indiko.Maui.Controls.Markdown.Sample/Indiko.Maui.Controls.Markdown.Sample.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

No projects are excluded from this upgrade.

### Project upgrade details

#### src/Indiko.Maui.Controls.Markdown/Indiko.Maui.Controls.Markdown.csproj modifications

Project properties changes:
  - Target frameworks should be changed from `net9.0;net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0` to `net9.0;net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0;net10.0-windows`

#### samples/Indiko.Maui.Controls.Markdown.Sample/Indiko.Maui.Controls.Markdown.Sample.csproj modifications

Project properties changes:
  - Target frameworks should be changed from `net9.0-android;net9.0-ios;net9.0-maccatalyst` to `net9.0-android;net9.0-ios;net9.0-maccatalyst;net10.0-ios;net10.0-android;net10.0-maccatalyst`