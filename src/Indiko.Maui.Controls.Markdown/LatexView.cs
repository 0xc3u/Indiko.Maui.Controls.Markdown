using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Indiko.Maui.Controls.Markdown;

public sealed class LatexView : SKCanvasView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(propertyName: nameof(Text),
        returnType: typeof(string), declaringType: typeof(LatexView), propertyChanged: OnLatexChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(propertyName: nameof(FontSize),
        returnType: typeof(float), declaringType: typeof(LatexView), defaultValue: 48f, propertyChanged: OnLatexChanged);
    public float FontSize
    {
        get => (float)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty TextColorProperty =
      BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(LatexView), defaultValue: Colors.Black, propertyChanged: OnLatexChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }


    public static readonly BindableProperty ErrorColorProperty =
     BindableProperty.Create(nameof(ErrorColor), typeof(Color), typeof(LatexView), defaultValue: Colors.Red, propertyChanged: OnLatexChanged);

    public Color ErrorColor
    {
        get => (Color)GetValue(ErrorColorProperty);
        set => SetValue(ErrorColorProperty, value);
    }

    public static readonly BindableProperty HighlightColorProperty =
    BindableProperty.Create(nameof(HighlightColor), typeof(Color), typeof(LatexView), defaultValue: Colors.Transparent, propertyChanged: OnLatexChanged);

    public Color HighlightColor
    {
        get => (Color)GetValue(HighlightColorProperty);
        set => SetValue(HighlightColorProperty, value);
    }

    private static void OnLatexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue)
        {
            var view = bindable as LatexView;
            view?.InvalidateSurface();
            view?.UpdateSize();
        }
    }

    public LatexView()
    {
        PaintSurface += OnPainting;
        SizeChanged += (s, e) => UpdateSize();
    }

    ~LatexView()
    {
        PaintSurface -= OnPainting;
        SizeChanged -= (s, e) => UpdateSize();
    }

	private void UpdateSize()
	{
		if (!string.IsNullOrEmpty(Text))
		{
			var painter = new CSharpMath.SkiaSharp.MathPainter
			{
				LaTeX = Text,
				FontSize = FontSize,
				AntiAlias = true,
				DisplayErrorInline = true,
				TextColor = TextColor?.ToSKColor() ?? SKColors.Black,  // Aggiungi controllo null
				ErrorColor = ErrorColor?.ToSKColor() ?? SKColors.Red,
				HighlightColor = HighlightColor?.ToSKColor() ?? SKColors.Transparent,
				PaintStyle = CSharpMath.Rendering.FrontEnd.PaintStyle.Fill
			};

			var measuredBounds = painter.Measure();
			WidthRequest = (measuredBounds.Width * 0.5);
			HeightRequest = (measuredBounds.Height * 0.5);
		}
	}

	private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
	{
		SKImageInfo info = e.Info;
		SKSurface surface = e.Surface;
		SKCanvas canvas = surface.Canvas;

		canvas.Clear();

		if (string.IsNullOrEmpty(Text))
		{
			return;
		}

		CSharpMath.SkiaSharp.MathPainter painter = new()
		{
			LaTeX = Text,
			FontSize = FontSize,
			AntiAlias = true,
			DisplayErrorInline = true,
			TextColor = TextColor?.ToSKColor() ?? SKColors.Black,  // Aggiungi controllo null
			ErrorColor = ErrorColor?.ToSKColor() ?? SKColors.Red,
			HighlightColor = HighlightColor?.ToSKColor() ?? SKColors.Transparent,
			PaintStyle = CSharpMath.Rendering.FrontEnd.PaintStyle.Fill
		};
		painter.Draw(canvas);
	}
}