#if WINDOWS
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Indiko.Maui.Controls.Markdown.Controls;
// Alias WinUI types that clash with MAUI's implicit global usings
// (Microsoft.Maui.Controls / Microsoft.Maui.Graphics / Microsoft.Maui).
using WImage = Microsoft.UI.Xaml.Controls.Image;
using WStretch = Microsoft.UI.Xaml.Media.Stretch;
using WScrollMode = Microsoft.UI.Xaml.Controls.ScrollMode;
using WScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility;
using WHorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using WVerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;

namespace Indiko.Maui.Controls.Markdown.Handlers;

/// <summary>
/// Windows (WinUI) handler for <see cref="ZoomImageView"/>. A <see cref="ScrollViewer"/> with
/// <see cref="ZoomMode"/> enabled provides pinch / mouse-wheel zoom and pan; a double-tap toggles
/// between the fitted scale and a zoomed-in scale.
/// </summary>
public sealed class ZoomImageViewHandler : ViewHandler<ZoomImageView, ScrollViewer>
{
    WImage? _image;

    public static IPropertyMapper<ZoomImageView, ZoomImageViewHandler> PropertyMapper
        = new PropertyMapper<ZoomImageView, ZoomImageViewHandler>(ViewMapper)
        {
            [nameof(ZoomImageView.ImageSource)] = MapImageSource,
            [nameof(ZoomImageView.Aspect)] = MapAspect,
            [nameof(ZoomImageView.MinZoomScale)] = MapZoomBounds,
            [nameof(ZoomImageView.MaxZoomScale)] = MapZoomBounds,
        };

    public static CommandMapper<ZoomImageView, ZoomImageViewHandler> CommandMapper = new();

    public ZoomImageViewHandler() : base(PropertyMapper, CommandMapper) { }

    protected override ScrollViewer CreatePlatformView()
    {
        _image = new WImage { Stretch = WStretch.Uniform };

        var scroll = new ScrollViewer
        {
            ZoomMode = ZoomMode.Enabled,
            HorizontalScrollMode = WScrollMode.Auto,
            VerticalScrollMode = WScrollMode.Auto,
            HorizontalScrollBarVisibility = WScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = WScrollBarVisibility.Hidden,
            HorizontalContentAlignment = WHorizontalAlignment.Center,
            VerticalContentAlignment = WVerticalAlignment.Center,
            MinZoomFactor = 1f,
            MaxZoomFactor = 5f,
            Content = _image,
        };

        scroll.DoubleTapped += OnDoubleTapped;
        return scroll;
    }

    protected override void ConnectHandler(ScrollViewer platformView)
    {
        base.ConnectHandler(platformView);
        UpdateZoomBounds();
        _ = UpdateImageAsync();
    }

    protected override void DisconnectHandler(ScrollViewer platformView)
    {
        platformView.DoubleTapped -= OnDoubleTapped;
        if (_image is not null)
        {
            _image.Source = null;
            _image = null;
        }
        base.DisconnectHandler(platformView);
    }

    static void MapImageSource(ZoomImageViewHandler h, ZoomImageView v) => _ = h.UpdateImageAsync();
    static void MapAspect(ZoomImageViewHandler h, ZoomImageView v) => h.UpdateAspect();
    static void MapZoomBounds(ZoomImageViewHandler h, ZoomImageView v) => h.UpdateZoomBounds();

    void UpdateAspect()
    {
        if (_image is null) return;
        _image.Stretch = VirtualView.Aspect switch
        {
            Aspect.AspectFill => WStretch.UniformToFill,
            Aspect.Fill => WStretch.Fill,
            _ => WStretch.Uniform
        };
    }

    void UpdateZoomBounds()
    {
        PlatformView.MinZoomFactor = (float)Math.Max(0.1, VirtualView.MinZoomScale);
        PlatformView.MaxZoomFactor = (float)Math.Max(PlatformView.MinZoomFactor, VirtualView.MaxZoomScale);
    }

    async Task UpdateImageAsync()
    {
        if (_image is null || MauiContext is null || VirtualView.ImageSource is null) return;

        try
        {
            var provider = MauiContext.Services.GetRequiredService<IImageSourceServiceProvider>();
            var service = provider.GetRequiredImageSourceService(VirtualView.ImageSource);
            var result = await service.GetImageSourceAsync(VirtualView.ImageSource, 1f, CancellationToken.None)
                .ConfigureAwait(true);

            if (result?.Value is not null && _image is not null)
            {
                _image.Source = result.Value;
                UpdateAspect();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving ImageSource: {ex.Message}");
        }
    }

    void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        var sv = PlatformView;
        bool atFit = Math.Abs(sv.ZoomFactor - sv.MinZoomFactor) < 0.01f;

        if (atFit)
        {
            if (!VirtualView.ZoomInOnDoubleTap) return;
            var target = Math.Min(sv.MinZoomFactor * 2.5f, sv.MaxZoomFactor);
            var pt = e.GetPosition(sv.Content as UIElement);
            sv.ChangeView(pt.X * target - sv.ViewportWidth / 2, pt.Y * target - sv.ViewportHeight / 2, target);
        }
        else
        {
            if (!VirtualView.ZoomOutOnDoubleTap) return;
            sv.ChangeView(0, 0, sv.MinZoomFactor);
        }
    }
}
#endif
