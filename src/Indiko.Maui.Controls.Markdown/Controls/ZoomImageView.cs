namespace Indiko.Maui.Controls.Markdown.Controls;

/// <summary>
/// A zoomable / pannable image view backed by native handlers (iOS/MacCatalyst, Android, Windows).
/// Used by <see cref="MarkdownView"/> to show a tapped image full-screen with pinch-zoom, pan and
/// double-tap-to-toggle. It is a thin, handler-driven control — the zoom mechanics live entirely
/// in the platform handlers so the gestures feel native.
/// </summary>
public sealed class ZoomImageView : View
{
    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(ZoomImageView), null);

    public ImageSource ImageSource
    {
        get => (ImageSource)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly BindableProperty AspectProperty =
        BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(ZoomImageView), Aspect.AspectFit);

    public Aspect Aspect
    {
        get => (Aspect)GetValue(AspectProperty);
        set => SetValue(AspectProperty, value);
    }

    public static readonly BindableProperty ZoomInOnDoubleTapProperty =
        BindableProperty.Create(nameof(ZoomInOnDoubleTap), typeof(bool), typeof(ZoomImageView), true);

    public bool ZoomInOnDoubleTap
    {
        get => (bool)GetValue(ZoomInOnDoubleTapProperty);
        set => SetValue(ZoomInOnDoubleTapProperty, value);
    }

    public static readonly BindableProperty ZoomOutOnDoubleTapProperty =
        BindableProperty.Create(nameof(ZoomOutOnDoubleTap), typeof(bool), typeof(ZoomImageView), true);

    public bool ZoomOutOnDoubleTap
    {
        get => (bool)GetValue(ZoomOutOnDoubleTapProperty);
        set => SetValue(ZoomOutOnDoubleTapProperty, value);
    }

    public static readonly BindableProperty MaxZoomScaleProperty =
        BindableProperty.Create(nameof(MaxZoomScale), typeof(double), typeof(ZoomImageView), 5.0);

    public double MaxZoomScale
    {
        get => (double)GetValue(MaxZoomScaleProperty);
        set => SetValue(MaxZoomScaleProperty, value);
    }

    public static readonly BindableProperty MinZoomScaleProperty =
        BindableProperty.Create(nameof(MinZoomScale), typeof(double), typeof(ZoomImageView), 1.0);

    public double MinZoomScale
    {
        get => (double)GetValue(MinZoomScaleProperty);
        set => SetValue(MinZoomScaleProperty, value);
    }

    // Clockwise display rotation in degrees (0/90/180/270). Defaults to 0 — markdown images are
    // shown in their natural orientation; the property exists for parity with the reference control.
    public static readonly BindableProperty RotationDegreesProperty =
        BindableProperty.Create(nameof(RotationDegrees), typeof(int), typeof(ZoomImageView), 0);

    public int RotationDegrees
    {
        get => (int)GetValue(RotationDegreesProperty);
        set => SetValue(RotationDegreesProperty, value);
    }
}
