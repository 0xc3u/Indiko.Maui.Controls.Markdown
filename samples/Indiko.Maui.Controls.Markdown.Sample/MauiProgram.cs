using CommunityToolkit.Maui;
using Indiko.Maui.Controls.Markdown.Sample.Services;
using Indiko.Maui.Controls.Markdown.Sample.ViewModels;

namespace Indiko.Maui.Controls.Markdown.Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("CamingoCode-Regular.tff", "CamingoCodeRegular");
				fonts.AddFont("CamingoCode-Italic.tff", "CamingoCodeItalic");
				fonts.AddFont("CamingoCode-BoldItalic.tff", "CamingoCodeBoldItalic");
				fonts.AddFont("CamingoCode-Bold.tff", "CamingoCodeBold");
			})
			.UseMauiCommunityToolkit();

        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingletonWithShellRoute<MainPage, MainPageViewModel>(nameof(MainPage));

		return builder.Build();
    }
}