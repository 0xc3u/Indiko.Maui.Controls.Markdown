using CSharpMath.MauiGraphics;

namespace Indiko.Maui.Controls.Markdown;

/// <summary>
/// Renders a LaTeX math formula into a native MAUI view. Backed by <see cref="GraphicsView"/> and
/// the SkiaSharp-free <see cref="CSharpMath.MauiGraphics.MathPainter"/>, so it carries no SkiaSharp
/// dependency. The public API (Text, FontSize, TextColor, ErrorColor, HighlightColor) is unchanged
/// from the previous SKCanvasView-based implementation.
/// </summary>
public sealed class LatexView : GraphicsView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(LatexView), propertyChanged: OnLatexChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(float), typeof(LatexView), defaultValue: 48f, propertyChanged: OnLatexChanged);

    public float FontSize
    {
        get => (float)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(LatexView), defaultValue: Colors.Black, propertyChanged: OnLatexChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty ErrorColorProperty = BindableProperty.Create(
        nameof(ErrorColor), typeof(Color), typeof(LatexView), defaultValue: Colors.Red, propertyChanged: OnLatexChanged);

    public Color ErrorColor
    {
        get => (Color)GetValue(ErrorColorProperty);
        set => SetValue(ErrorColorProperty, value);
    }

    public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
        nameof(HighlightColor), typeof(Color), typeof(LatexView), defaultValue: Colors.Transparent, propertyChanged: OnLatexChanged);

    public Color HighlightColor
    {
        get => (Color)GetValue(HighlightColorProperty);
        set => SetValue(HighlightColorProperty, value);
    }

    public LatexView()
    {
        Drawable = new LatexDrawable(this);
        BackgroundColor = Colors.Transparent;
    }

    private static void OnLatexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (Equals(oldValue, newValue))
        {
            return;
        }

        var view = (LatexView)bindable;
        view.UpdateSize();
        view.Invalidate();
    }

    private void UpdateSize()
    {
        if (string.IsNullOrEmpty(Text))
        {
            WidthRequest = 0;
            HeightRequest = 0;
            return;
        }

        var bounds = CreatePainter().Measure();
        WidthRequest = bounds.Width;
        HeightRequest = bounds.Height;
    }

    private MathPainter CreatePainter() => new()
    {
        LaTeX = Text,
        FontSize = FontSize,
        DisplayErrorInline = true,
        AntiAlias = true,
        PaintStyle = CSharpMath.Rendering.FrontEnd.PaintStyle.Fill,
        TextColor = TextColor ?? Colors.Black,
        ErrorColor = ErrorColor ?? Colors.Red,
        HighlightColor = HighlightColor ?? Colors.Transparent,
    };

    private sealed class LatexDrawable : IDrawable
    {
        private readonly LatexView _view;

        public LatexDrawable(LatexView view) => _view = view;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (string.IsNullOrEmpty(_view.Text))
            {
                return;
            }

            var painter = _view.CreatePainter();

            // Measure() returns a y-up bounding box whose top edge sits at -Ascent. CSharpMath draws
            // with an inverted y-axis (see MathPainter.Draw → DrawCore's Scale(1,-1)), so passing
            // y = Ascent (= -Top) lands the formula's top-left at the canvas origin (0,0).
            var bounds = painter.Measure();
            painter.Draw(canvas, 0f, -bounds.Top);
        }
    }
}
