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
using Microsoft.UI.Xaml.Media.Imaging;
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
/// <see cref="ZoomMode"/> enabled provides pinch / ctrl-mouse-wheel zoom and pan; a double-tap toggles
/// between the fitted scale and a zoomed-in scale. Like the Android/iOS handlers, the image is fitted
/// to the viewport first (fit scale), and <see cref="ZoomImageView.MinZoomScale"/> /
/// <see cref="ZoomImageView.MaxZoomScale"/> are multipliers of that fit scale.
/// </summary>
public sealed class ZoomImageViewHandler : ViewHandler<ZoomImageView, ScrollViewer>
{
    WImage? _image;

    // Base "fits the viewport" scale, computed from the image's natural size and the viewport size.
    float _fitScale = 1f;
    double _fittedWidth = -1;
    double _fittedHeight = -1;

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
        _image.ImageOpened += OnImageOpened;

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
        scroll.SizeChanged += OnSizeChanged;
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
        platformView.SizeChanged -= OnSizeChanged;
        if (_image is not null)
        {
            _image.ImageOpened -= OnImageOpened;
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

        // A different aspect changes how the image should be fitted.
        _fittedWidth = -1;
        _fittedHeight = -1;
        TryFit();
    }

    void UpdateZoomBounds()
    {
        float min = Math.Max(0.1f, _fitScale * (float)VirtualView.MinZoomScale);
        float max = Math.Max(min, _fitScale * (float)VirtualView.MaxZoomScale);
        PlatformView.MinZoomFactor = min;
        PlatformView.MaxZoomFactor = max;
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
                // A new image invalidates the previous fit so the next open re-fits the viewport.
                _fittedWidth = -1;
                _fittedHeight = -1;
                _image.Source = result.Value;
                UpdateAspect();
                TryFit();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving ImageSource: {ex.Message}");
        }
    }

    void OnImageOpened(object sender, RoutedEventArgs e) => TryFit();

    void OnSizeChanged(object sender, SizeChangedEventArgs e) => TryFit();

    void TryFit()
    {
        if (_image is null || _image.Source is null) return;

        double vw = PlatformView.ActualWidth;
        double vh = PlatformView.ActualHeight;
        if (vw <= 0 || vh <= 0) return; // not laid out yet

        // Prefer the decoded bitmap's pixel size; fall back to the laid-out image size
        // (covers vector sources such as SvgImageSource that are not BitmapSources).
        double iw, ih;
        if (_image.Source is BitmapSource bs && bs.PixelWidth > 0 && bs.PixelHeight > 0)
        {
            iw = bs.PixelWidth;
            ih = bs.PixelHeight;
        }
        else
        {
            iw = _image.ActualWidth;
            ih = _image.ActualHeight;
            if (iw <= 0 || ih <= 0) return; // image not measured yet
        }

        float sx = (float)(vw / iw);
        float sy = (float)(vh / ih);
        _fitScale = VirtualView.Aspect == Aspect.AspectFit ? Math.Min(sx, sy) : Math.Max(sx, sy);
        _fitScale = Math.Max(_fitScale, 0.01f);

        UpdateZoomBounds();

        // Snap to the fitted scale only when the viewport size actually changed (first layout or a
        // resize); unrelated updates must not reset a user's current zoom.
        if (Math.Abs(_fittedWidth - vw) > 0.5 || Math.Abs(_fittedHeight - vh) > 0.5)
        {
            _fittedWidth = vw;
            _fittedHeight = vh;
            PlatformView.ChangeView(null, null, _fitScale, disableAnimation: true);
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
