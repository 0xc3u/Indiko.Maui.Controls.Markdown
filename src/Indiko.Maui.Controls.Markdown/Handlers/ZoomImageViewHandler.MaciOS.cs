#if IOS || MACCATALYST
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using UIKit;
using Indiko.Maui.Controls.Markdown.Controls;

namespace Indiko.Maui.Controls.Markdown.Handlers;

/// <summary>
/// UIKit handler for <see cref="ZoomImageView"/> (iOS and MacCatalyst). A <see cref="UIScrollView"/>
/// provides the pinch-zoom and pan; a double-tap gesture toggles between the fitted scale and a
/// zoomed-in scale. All native delegate/target work goes through a weak-reference proxy so the
/// handler never participates in a retain cycle.
/// </summary>
public sealed class ZoomImageViewHandler : ViewHandler<ZoomImageView, UIScrollView>
{
    UIImageView? _imageView;
    ZoomProxy? _proxy;                 // strong ref in handler
    UITapGestureRecognizer? _double;   // strong ref in handler

    public static IPropertyMapper<ZoomImageView, ZoomImageViewHandler> PropertyMapper =
        new PropertyMapper<ZoomImageView, ZoomImageViewHandler>(ViewMapper)
        {
            [nameof(ZoomImageView.ImageSource)] = MapImageSource,
            [nameof(ZoomImageView.Aspect)] = MapAspect,
            [nameof(ZoomImageView.RotationDegrees)] = MapRotation,
            [nameof(ZoomImageView.ZoomInOnDoubleTap)] = MapZoomInOnDoubleTap,
            [nameof(ZoomImageView.ZoomOutOnDoubleTap)] = MapZoomOutOnDoubleTap,
            [nameof(ZoomImageView.MinZoomScale)] = MapMinZoomScale,
            [nameof(ZoomImageView.MaxZoomScale)] = MapMaxZoomScale,
        };

    public static CommandMapper<ZoomImageView, ZoomImageViewHandler> CommandMapper = new();

    public ZoomImageViewHandler() : base(PropertyMapper, CommandMapper)
    {
    }

    protected override UIScrollView CreatePlatformView()
    {
        var scroll = new UIScrollView
        {
            ShowsHorizontalScrollIndicator = false,
            ShowsVerticalScrollIndicator = false,
            BouncesZoom = true
        };

        _imageView = new UIImageView
        {
            ContentMode = ToContentMode(VirtualView.Aspect),
            UserInteractionEnabled = true
        };
        scroll.AddSubview(_imageView);

        // Proxy owns NO strong refs to handler; only WEAK refs to VirtualView/native views.
        _proxy = new ZoomProxy();
        _proxy.Connect(VirtualView, scroll, _imageView);

        // Use proxy as UIScrollViewDelegate (avoid handler as delegate to prevent retain cycle)
        scroll.Delegate = _proxy;

        // Gestures with proxy target (no captured closures)
        _double = new UITapGestureRecognizer(_proxy, new ObjCRuntime.Selector("onDoubleTap:"))
        { NumberOfTapsRequired = 2 };

        scroll.AddGestureRecognizer(_double);

        return scroll;
    }

    protected override void ConnectHandler(UIScrollView platformView)
    {
        base.ConnectHandler(platformView);
        _ = UpdateImageAsync();
    }

    protected override void DisconnectHandler(UIScrollView platformView)
    {
        if (_double is not null) { platformView.RemoveGestureRecognizer(_double); _double.Dispose(); _double = null; }

        platformView.Delegate = null; // release native ref to proxy
        _proxy?.Disconnect();
        _proxy = null;

        _imageView?.RemoveFromSuperview();
        _imageView?.Dispose();
        _imageView = null;

        base.DisconnectHandler(platformView);
    }

    async Task UpdateImageAsync()
    {
        if (_imageView is null || MauiContext is null) return;

        var img = await GetImageAsync(MauiContext, VirtualView.ImageSource);
        // The handler may have been disconnected while the (remote) image was loading.
        if (_imageView is null) return;

        img = RotateImage(img, VirtualView.RotationDegrees);

        _imageView.Image = img;
        // The scroll view's zoom scale does the fitting/scaling, so the content mode is a 1:1 fill.
        _imageView.ContentMode = UIViewContentMode.ScaleToFill;
        if (img is not null)
            _imageView.Frame = new CGRect(0, 0, img.Size.Width, img.Size.Height);

        _proxy?.ConfigureZoom();
    }

    // Rotates a UIImage clockwise by the given degrees (0/90/180/270) for display.
    static UIImage? RotateImage(UIImage? src, int degrees)
    {
        if (src is null) return null;
        int d = ((degrees % 360) + 360) % 360;
        if (d == 0) return src;

        var size = src.Size;
        var newSize = (d == 90 || d == 270) ? new CGSize(size.Height, size.Width) : size;

        var format = new UIGraphicsImageRendererFormat { Opaque = false, Scale = src.CurrentScale };
        using var renderer = new UIGraphicsImageRenderer(newSize, format);
        return renderer.CreateImage(ctx =>
        {
            var c = ctx.CGContext;
            c.TranslateCTM(newSize.Width / 2f, newSize.Height / 2f);
            // UIKit's drawing context is y-down, so a positive rotation is clockwise.
            c.RotateCTM((nfloat)(d * Math.PI / 180.0));
            src.Draw(new CGRect(-size.Width / 2f, -size.Height / 2f, size.Width, size.Height));
        });
    }

    static async Task<UIImage?> GetImageAsync(IMauiContext mauiContext, ImageSource? imageSource)
    {
        if (imageSource is null)
            return null;

        // Load file-based sources straight from disk (memory-friendly, and avoids the iOS
        // FileImageSource pipeline that fails to load arbitrary absolute cache paths).
        if (imageSource is FileImageSource fis && !string.IsNullOrWhiteSpace(fis.File) && File.Exists(fis.File))
        {
            return UIImage.FromFile(fis.File);
        }

        try
        {
            // Resolve through the async image-source service so stream/URI-based sources (e.g. a
            // downloaded web image handed over by MarkdownView) actually load — the synchronous
            // path only ever completed for file sources, leaving remote images blank.
            var provider = mauiContext.Services.GetRequiredService<IImageSourceServiceProvider>();
            var service = provider.GetRequiredImageSourceService(imageSource);
            var result = await service.GetImageAsync(imageSource, 1f, CancellationToken.None);
            return result?.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resolving ImageSource: {ex.Message}");
            return null;
        }
    }

    public override void PlatformArrange(Microsoft.Maui.Graphics.Rect rect)
    {
        base.PlatformArrange(rect);
        // Re-fit once the scroll view has its real bounds (image may have loaded before layout).
        _proxy?.ConfigureZoom();
    }

    static UIViewContentMode ToContentMode(Aspect a) => a switch
    {
        Aspect.AspectFill => UIViewContentMode.ScaleAspectFill,
        Aspect.Fill => UIViewContentMode.ScaleToFill,
        _ => UIViewContentMode.ScaleAspectFit
    };

    static void MapImageSource(ZoomImageViewHandler h, ZoomImageView v) => _ = h.UpdateImageAsync();
    static void MapAspect(ZoomImageViewHandler h, ZoomImageView v) => h._proxy?.ConfigureZoom();
    static void MapRotation(ZoomImageViewHandler h, ZoomImageView v) => _ = h.UpdateImageAsync();
    static void MapZoomInOnDoubleTap(ZoomImageViewHandler h, ZoomImageView v) { /* handled in proxy at tap-time */ }
    static void MapZoomOutOnDoubleTap(ZoomImageViewHandler h, ZoomImageView v) { /* handled in proxy at tap-time */ }
    static void MapMinZoomScale(ZoomImageViewHandler h, ZoomImageView v) => h._proxy?.ConfigureZoom();
    static void MapMaxZoomScale(ZoomImageViewHandler h, ZoomImageView v) => h._proxy?.ConfigureZoom();

    // ----------------------------------------------------------------------
    // Proxy: NSObject subclasses are required for delegates/targets on iOS,
    // but we keep ONLY weak references inside to avoid cycles.
    // ----------------------------------------------------------------------
    sealed class ZoomProxy : UIScrollViewDelegate
    {
        WeakReference<ZoomImageView>? _v;
        WeakReference<UIScrollView>? _s;
        WeakReference<UIImageView>? _i;

        public void Connect(ZoomImageView v, UIScrollView s, UIImageView i)
        {
            _v = new(v);
            _s = new(s);
            _i = new(i);
        }

        public void Disconnect()
        {
            _v = null;
            _s = null;
            _i = null;
        }

        ZoomImageView? V => _v is { } wr && wr.TryGetTarget(out var x) ? x : null;
        UIScrollView? S => _s is { } wr && wr.TryGetTarget(out var x) ? x : null;
        UIImageView? I => _i is { } wr && wr.TryGetTarget(out var x) ? x : null;

        // Sizes the image view to the image, fits it to the scroll bounds, and sets fit-relative
        // zoom limits. Min/MaxZoomScale from the control are multipliers of the fit scale.
        public void ConfigureZoom()
        {
            if (S is null || I is null || V is null || I.Image is null) return;

            var bounds = S.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return; // not laid out yet

            var imgSize = I.Image.Size;
            if (imgSize.Width <= 0 || imgSize.Height <= 0) return;

            I.Frame = new CGRect(0, 0, imgSize.Width, imgSize.Height);
            S.ContentSize = imgSize;

            nfloat sx = bounds.Width / imgSize.Width;
            nfloat sy = bounds.Height / imgSize.Height;
            nfloat fit = V.Aspect == Aspect.AspectFill ? (nfloat)Math.Max(sx, sy) : (nfloat)Math.Min(sx, sy);

            S.MinimumZoomScale = fit * (nfloat)V.MinZoomScale;
            S.MaximumZoomScale = fit * (nfloat)V.MaxZoomScale;
            S.SetZoomScale(fit * (nfloat)V.MinZoomScale, false);

            CenterImage();
        }

        // Selector must carry a trailing colon to match the 1-parameter method.
        [Export("onDoubleTap:")]
        public void OnDoubleTap(UITapGestureRecognizer g)
        {
            if (S is null || I is null || V is null) return;

            nfloat fit = S.MinimumZoomScale;          // the fitted (initial) scale
            var current = S.ZoomScale;
            bool atFit = Math.Abs(current - fit) < 0.01f;
            nfloat target = atFit ? (nfloat)Math.Min(fit * 2.5f, S.MaximumZoomScale) : fit;

            // obey flags
            if (target > current && !V.ZoomInOnDoubleTap) return;
            if (target < current && !V.ZoomOutOnDoubleTap) return;

            if (atFit)
            {
                var pt = g.LocationInView(I);
                var w = S.Bounds.Width / target;
                var h = S.Bounds.Height / target;
                S.ZoomToRect(new CGRect(pt.X - (w / 2f), pt.Y - (h / 2f), w, h), true);
            }
            else
            {
                S.SetZoomScale(target, true);
            }
        }

        public override UIView ViewForZoomingInScrollView(UIScrollView scrollView) => I!;
        public override void DidZoom(UIScrollView scrollView) => CenterImage();

        // Centers the (possibly smaller-than-viewport) content using scroll insets.
        public void CenterImage()
        {
            if (S is null || I is null || I.Image is null) return;

            var bounds = S.Bounds;
            var content = S.ContentSize;
            nfloat top = (nfloat)Math.Max(0, (bounds.Height - content.Height) / 2.0);
            nfloat left = (nfloat)Math.Max(0, (bounds.Width - content.Width) / 2.0);
            S.ContentInset = new UIEdgeInsets(top, left, top, left);
        }
    }
}
#endif
