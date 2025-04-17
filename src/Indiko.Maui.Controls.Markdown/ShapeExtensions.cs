using Microsoft.Maui.Controls.Shapes;

namespace Indiko.Maui.Controls.Markdown;
public static class ShapeExtensions
{
    public static RoundRectangle WithCornerRadius(this RoundRectangle roundRectangle, float cornerRadius)
    {
      roundRectangle.CornerRadius = new CornerRadius(cornerRadius);
      return roundRectangle;
    }
}
