# .NET 10.0 Upgrade Report

## Project target framework modifications

| Project name                                                    | Old Target Frameworks                                                             | New Target Frameworks                                                                       |
|:---------------------------------------------------------------|:--------------------------------------------------------------------------------:|:---------------------------------------------------------------------------------------------:|
| src/Indiko.Maui.Controls.Markdown/Indiko.Maui.Controls.Markdown.csproj | net9.0;net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0 | net9.0;net9.0-android;net9.0-ios;net9.0-maccatalyst;net9.0-windows10.0.19041.0;net10.0-windows |
| samples/Indiko.Maui.Controls.Markdown.Sample/Indiko.Maui.Controls.Markdown.Sample.csproj | net9.0-android;net9.0-ios;net9.0-maccatalyst                    | net9.0-android;net9.0-ios;net9.0-maccatalyst;net10.0-ios;net10.0-android;net10.0-maccatalyst |

## Summary

Your .NET MAUI solution has been successfully upgraded to support .NET 10.0. The multi-targeting setup ensures your components remain compatible with .NET 9.0 while gaining support for the latest .NET 10.0 platform:

- **Library project** now targets .NET 10.0 Windows in addition to existing .NET 9.0 platforms
- **Sample application** now targets .NET 10.0 for iOS, Android, and macCatalyst in addition to existing .NET 9.0 platforms

The upgrade is complete and all projects have been validated successfully.