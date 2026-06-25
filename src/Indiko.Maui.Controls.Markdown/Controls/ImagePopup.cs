using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace Indiko.Maui.Controls.Markdown.Controls;

/// <summary>
/// Presents a tapped markdown image full-screen in a native modal overlay built around
/// <see cref="ZoomImageView"/>. The overlay supports pinch / double-tap zoom, pan, and a close (✕)
/// button. Presentation uses MAUI modal navigation, so the overlay is rendered by the native
/// modal presenter on each platform.
/// </summary>
internal static class ImagePopup
{
    public static async Task ShowAsync(
        ImageSource source,
        Color backgroundColor,
        Color closeButtonColor,
        double maxZoomScale)
    {
        if (source is null)
            return;

        var navigation = GetNavigation();
        if (navigation is null)
            return;

        try
        {
            var page = BuildPage(source, backgroundColor, closeButtonColor, maxZoomScale, navigation);
            await navigation.PushModalAsync(page, animated: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ImagePopup failed to present: {ex.Message}");
        }
    }

    static ContentPage BuildPage(
        ImageSource source,
        Color backgroundColor,
        Color closeButtonColor,
        double maxZoomScale,
        INavigation navigation)
    {
        var zoom = new ZoomImageView
        {
            ImageSource = source,
            Aspect = Aspect.AspectFit,
            MinZoomScale = 1,
            MaxZoomScale = maxZoomScale <= 1 ? 5 : maxZoomScale,
            ZoomInOnDoubleTap = true,
            ZoomOutOnDoubleTap = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
        };

        var close = new Button
        {
            Text = "✕", // ✕
            FontSize = 22,
            TextColor = closeButtonColor ?? Colors.White,
            BackgroundColor = Colors.Transparent,
            BorderWidth = 0,
            Padding = 0,
            WidthRequest = 48,
            HeightRequest = 48,
            CornerRadius = 24,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 8, 8, 0),
        };

        var grid = new Grid();
        grid.Children.Add(zoom);
        grid.Children.Add(close);

        var page = new ContentPage
        {
            BackgroundColor = backgroundColor ?? Colors.Black,
            Content = grid,
        };

        // Keep the system status/navigation bars hidden during the immersive viewer.
        Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(page, false);

        // On iOS the default modal is a dismissible sheet; force a full-screen presentation so the
        // viewer truly fills the screen.
        page.On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FullScreen);

        async void CloseHandler(object sender, EventArgs e)
        {
            try { await navigation.PopModalAsync(animated: true); }
            catch (Exception ex) { Console.WriteLine($"ImagePopup close failed: {ex.Message}"); }
        }

        close.Clicked += CloseHandler;
        page.Unloaded += (_, _) => close.Clicked -= CloseHandler;

        return page;
    }

    static INavigation GetNavigation()
    {
        var window = Microsoft.Maui.Controls.Application.Current?.Windows?.FirstOrDefault();
        return window?.Page?.Navigation;
    }
}
