#if ANDROID
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Indiko.Maui.Controls.Markdown.Controls;
using aGraphics = Android.Graphics;
using aViews = Android.Views;
using PointF = Android.Graphics.PointF;

namespace Indiko.Maui.Controls.Markdown.Handlers;

/// <summary>
/// Android handler for <see cref="ZoomImageView"/>. A matrix-driven <see cref="ImageView"/> inside a
/// <see cref="FrameLayout"/> provides pinch-zoom (<see cref="ScaleGestureDetector"/>), pan (touch
/// drag) and double-tap-to-toggle (<see cref="GestureDetector"/>). Touch and gesture work goes
/// through a proxy that keeps only a weak reference to the virtual view.
/// </summary>
public sealed class ZoomImageViewHandler : ViewHandler<ZoomImageView, FrameLayout>
{
    ImageView? _image;
    Proxy? _proxy;

    public static IPropertyMapper<ZoomImageView, ZoomImageViewHandler> PropertyMapper
        = new PropertyMapper<ZoomImageView, ZoomImageViewHandler>(ViewMapper)
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

    public ZoomImageViewHandler() : base(PropertyMapper, CommandMapper) { }

    protected override FrameLayout CreatePlatformView()
    {
        var layout = new FrameLayout(Context);
        _image = new ImageView(Context)
        {
            LayoutParameters = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent)
        };
        _image.SetScaleType(ImageView.ScaleType.Matrix);
        layout.AddView(_image);

        _proxy = new Proxy(layout, _image);
        _proxy.Connect(VirtualView);

        layout.Touch += _proxy.OnTouch;

        return layout;
    }

    protected override void ConnectHandler(FrameLayout platformView)
    {
        base.ConnectHandler(platformView);
        _ = UpdateImageAsync();
        UpdateAspect();
        UpdateZoomBounds();
    }

    protected override void DisconnectHandler(FrameLayout platformView)
    {
        if (_proxy is not null)
        {
            platformView.Touch -= _proxy.OnTouch;
            _proxy.Dispose();
            _proxy = null;
        }

        if (_image is not null)
        {
            _image.ImageMatrix?.Reset();
            _image.SetImageDrawable(null);
            _image = null;
        }

        base.DisconnectHandler(platformView);
    }

    static void MapImageSource(ZoomImageViewHandler h, ZoomImageView v) => _ = h.UpdateImageAsync();
    static void MapAspect(ZoomImageViewHandler h, ZoomImageView v) => h.UpdateAspect();
    static void MapRotation(ZoomImageViewHandler h, ZoomImageView v) => _ = h.UpdateImageAsync();
    static void MapZoomInOnDoubleTap(ZoomImageViewHandler h, ZoomImageView v) { /* handled at tap-time */ }
    static void MapZoomOutOnDoubleTap(ZoomImageViewHandler h, ZoomImageView v) { /* handled at tap-time */ }
    static void MapMinZoomScale(ZoomImageViewHandler h, ZoomImageView v) => h.UpdateZoomBounds();
    static void MapMaxZoomScale(ZoomImageViewHandler h, ZoomImageView v) => h.UpdateZoomBounds();

    async Task UpdateImageAsync()
    {
        if (_image is null || MauiContext is null || _proxy is null) return;

        // Load file-based sources straight from disk (decodes into native memory; avoids the
        // FileImageSource pipeline).
        if (VirtualView.ImageSource is FileImageSource fis && !string.IsNullOrWhiteSpace(fis.File))
        {
            var path = fis.File;
            var degrees = VirtualView.RotationDegrees;
            var bitmap = await Task.Run(() => DecodeRotated(path, degrees));
            if (bitmap is not null)
            {
                _image.SetImageBitmap(bitmap);
                _image.SetScaleType(ImageView.ScaleType.Matrix);
                FitNow();
                return;
            }
        }

        var drawable = await VirtualView.ImageSource.ToDrawableAsync(MauiContext);
        if (drawable is null) return;

        _image.SetImageDrawable(drawable);
        _image.SetScaleType(ImageView.ScaleType.Matrix);

        FitNow();
    }

    void FitNow()
    {
        _proxy?.ResetMatrix(VirtualView.Aspect);
        PlatformView?.Post(() => _proxy?.ResetMatrix(VirtualView.Aspect));
    }

    static Bitmap? DecodeRotated(string path, int degrees)
    {
        Bitmap? bmp = null;
        try
        {
            if (File.Exists(path))
                bmp = BitmapFactory.DecodeFile(path);
        }
        catch { bmp = null; }

        if (bmp is null)
        {
            try
            {
                using var stream = File.OpenRead(path);
                bmp = BitmapFactory.DecodeStream(stream);
            }
            catch { bmp = null; }
        }

        if (bmp is null) return null;

        try { return RotateBitmap(bmp, degrees); }
        catch { return bmp; }
    }

    static Bitmap? RotateBitmap(Bitmap? src, int degrees)
    {
        if (src is null) return null;
        int d = ((degrees % 360) + 360) % 360;
        if (d == 0) return src;

        using var matrix = new Matrix();
        matrix.PostRotate(d);
        var rotated = Bitmap.CreateBitmap(src, 0, 0, src.Width, src.Height, matrix, true);
        if (!ReferenceEquals(rotated, src))
            src.Recycle();
        return rotated;
    }

    void UpdateAspect()
    {
        if (_proxy is null) return;
        _proxy.ResetMatrix(VirtualView.Aspect);
    }

    public override void PlatformArrange(Microsoft.Maui.Graphics.Rect frame)
    {
        base.PlatformArrange(frame);

        // MAUI lays out the FrameLayout itself but doesn't propagate a measure pass to its children,
        // so the MatchParent ImageView stays 0×0 (image invisible). Force measure + layout.
        if (PlatformView is FrameLayout fl && fl.Width > 0 && fl.Height > 0)
        {
            int wSpec = aViews.View.MeasureSpec.MakeMeasureSpec(fl.Width, aViews.MeasureSpecMode.Exactly);
            int hSpec = aViews.View.MeasureSpec.MakeMeasureSpec(fl.Height, aViews.MeasureSpecMode.Exactly);
            fl.Measure(wSpec, hSpec);
            fl.Layout(0, 0, fl.Width, fl.Height);
        }

        _proxy?.ResetMatrix(VirtualView.Aspect);
    }

    void UpdateZoomBounds()
    {
        if (_proxy is null) return;
        _proxy.UpdateZoomBounds(
            (float)VirtualView.MinZoomScale,
            (float)VirtualView.MaxZoomScale,
            VirtualView.ZoomInOnDoubleTap,
            VirtualView.ZoomOutOnDoubleTap);
    }

    // ---- Proxy with weak reference to VirtualView ----
    sealed class Proxy : Java.Lang.Object, IDisposable
    {
        readonly FrameLayout _root;
        readonly ImageView _image;

        readonly Matrix _matrix = new();
        readonly float[] _vals = new float[9];

        ScaleGestureDetector? _scale;
        GestureDetector? _gesture;

        WeakReference<ZoomImageView>? _v;

        float _min = 1f, _max = 5f, _current = 1f;
        bool _allowIn = true, _allowOut = true;

        PointF _last = new();
        bool _dragging;

        aViews.ViewTreeObserver.IOnGlobalLayoutListener? _onceLayout;

        public Proxy(FrameLayout root, ImageView image)
        {
            _root = root;
            _image = image;

            _scale = new ScaleGestureDetector(root.Context, new ScaleListener(this));
            _gesture = new GestureDetector(root.Context, new TapListener(this));

            // one-shot layout listener to ensure we fit once the size is real
            _onceLayout = new OnceLayout(this);
            _root.ViewTreeObserver?.AddOnGlobalLayoutListener(_onceLayout);
        }

        public new void Dispose()
        {
            if (_onceLayout is not null)
            {
                if (_root.ViewTreeObserver?.IsAlive == true)
                    _root.ViewTreeObserver.RemoveOnGlobalLayoutListener(_onceLayout);
                _onceLayout.Dispose();
                _onceLayout = null;
            }

            _gesture?.SetOnDoubleTapListener(null);
            _gesture?.Dispose();
            _gesture = null;

            _scale?.Dispose();
            _scale = null;

            base.Dispose();
        }

        sealed class OnceLayout : Java.Lang.Object, aViews.ViewTreeObserver.IOnGlobalLayoutListener
        {
            readonly WeakReference<Proxy> _p;
            public OnceLayout(Proxy p) => _p = new(p);
            public void OnGlobalLayout()
            {
                if (!_p.TryGetTarget(out var p) || p._root == null) return;

                var vto = p._root.ViewTreeObserver;
                if (vto?.IsAlive == true) vto.RemoveOnGlobalLayoutListener(this);

                if (p._image?.Drawable is not null)
                    p.ResetMatrix(p.V?.Aspect ?? Aspect.AspectFit);
            }
        }

        public void Connect(ZoomImageView v) => _v = new(v);
        public void Disconnect() { _v = null; }

        ZoomImageView? V => _v is { } wr && wr.TryGetTarget(out var x) ? x : null;

        public void UpdateZoomBounds(float min, float max, bool allowIn, bool allowOut)
        {
            _min = min;
            _max = max;
            _allowIn = allowIn;
            _allowOut = allowOut;
            _current = Math.Clamp(_current, _min, _max);
            Apply();
        }

        public void ResetMatrix(Aspect aspect)
        {
            var d = _image.Drawable;
            if (d is null) return;

            int vw = _root.Width > 0 ? _root.Width : _root.MeasuredWidth;
            int vh = _root.Height > 0 ? _root.Height : _root.MeasuredHeight;
            if (vw <= 0 || vh <= 0) return; // wait for next pass

            int dw = d.IntrinsicWidth > 0 ? d.IntrinsicWidth : (d.Bounds.Width() > 0 ? d.Bounds.Width() : vw);
            int dh = d.IntrinsicHeight > 0 ? d.IntrinsicHeight : (d.Bounds.Height() > 0 ? d.Bounds.Height() : vh);

            _matrix.Reset();

            float scale;
            if (aspect == Aspect.AspectFit || aspect == Aspect.AspectFill)
            {
                float sx = (float)vw / dw;
                float sy = (float)vh / dh;
                scale = aspect == Aspect.AspectFit ? Math.Min(sx, sy) : Math.Max(sx, sy);
            }
            else // Fill
            {
                scale = Math.Max((float)vw / dw, (float)vh / dh);
            }

            float tx = (vw - dw * scale) / 2f;
            float ty = (vh - dh * scale) / 2f;

            _matrix.PostScale(scale, scale);
            _matrix.PostTranslate(tx, ty);

            _current = 1f;
            Apply();
        }

        public void OnTouch(object? sender, aViews.View.TouchEventArgs e)
        {
            _scale?.OnTouchEvent(e.Event);
            _gesture?.OnTouchEvent(e.Event);

            if (e.Event.PointerCount == 1 && !(_scale?.IsInProgress ?? false))
            {
                switch (e.Event.Action & MotionEventActions.Mask)
                {
                    case MotionEventActions.Down:
                        _last.Set(e.Event.GetX(), e.Event.GetY());
                        _dragging = true;
                        break;

                    case MotionEventActions.Move:
                        if (_dragging)
                        {
                            float x = e.Event.GetX(), y = e.Event.GetY();
                            _matrix.PostTranslate(x - _last.X, y - _last.Y);
                            Constrain();
                            Apply();
                            _last.Set(x, y);
                        }
                        break;

                    case MotionEventActions.Up:
                    case MotionEventActions.Cancel:
                        _dragging = false;
                        Constrain();
                        Apply();
                        break;
                }
            }

            e.Handled = true;
        }

        public void Zoom(float factor, float focusX, float focusY)
        {
            var target = Math.Clamp(_current * factor, _min, _max);
            factor = target / _current;
            _current = target;

            _matrix.PostScale(factor, factor, focusX, focusY);
            Constrain();
            Apply();
        }

        public void ToggleDoubleTap(float x, float y)
        {
            var goIn = Math.Abs(_current - 1f) < 0.01f;
            if (goIn && !_allowIn) return;
            if (!goIn && !_allowOut) return;

            var target = goIn ? Math.Min(2f, _max) : 1f;
            var factor = target / _current;
            Zoom(factor, x, y);
        }

        void Constrain()
        {
            if (_image.Drawable is null) return;

            _matrix.GetValues(_vals);
            float transX = _vals[Matrix.MtransX];
            float transY = _vals[Matrix.MtransY];
            float scaleX = _vals[Matrix.MscaleX];
            float scaleY = _vals[Matrix.MscaleY];

            var d = _image.Drawable;
            float w = d.IntrinsicWidth * scaleX;
            float h = d.IntrinsicHeight * scaleY;

            int vw = _root.Width;
            int vh = _root.Height;

            float minX = Math.Min(0, vw - w);
            float minY = Math.Min(0, vh - h);
            float maxX = Math.Max(0, vw - w);
            float maxY = Math.Max(0, vh - h);

            transX = Math.Clamp(transX, minX, maxX);
            transY = Math.Clamp(transY, minY, maxY);

            _vals[Matrix.MtransX] = transX;
            _vals[Matrix.MtransY] = transY;
            _matrix.SetValues(_vals);
        }

        void Apply() => _image.ImageMatrix = _matrix;

        sealed class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        {
            readonly Proxy _p;
            public ScaleListener(Proxy p) => _p = p;
            public override bool OnScale(ScaleGestureDetector detector)
            {
                _p.Zoom(detector.ScaleFactor, detector.FocusX, detector.FocusY);
                return true;
            }
        }

        sealed class TapListener : GestureDetector.SimpleOnGestureListener
        {
            readonly Proxy _p;
            public TapListener(Proxy p) => _p = p;

            public override bool OnDoubleTap(MotionEvent e)
            {
                _p.ToggleDoubleTap(e.GetX(), e.GetY());
                return true;
            }
        }
    }
}

// ---- ImageSource -> Android Drawable helper ----
internal static class AndroidImageSourceExtensions
{
    public static async Task<aGraphics.Drawables.Drawable?> ToDrawableAsync(
        this ImageSource? source,
        IMauiContext mauiContext,
        CancellationToken token = default)
    {
        if (source is null) return null;

        var provider = mauiContext.Services.GetRequiredService<IImageSourceServiceProvider>();
        var service = provider.GetImageSourceService(source);

        Context aContext = mauiContext.Context
            ?? throw new System.InvalidOperationException("IMauiContext.Context is null.");

        var result = await service.GetDrawableAsync(source, aContext, token).ConfigureAwait(false);
        return result?.Value;
    }
}
#endif
