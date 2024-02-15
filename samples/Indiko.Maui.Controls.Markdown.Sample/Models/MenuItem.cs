namespace Indiko.Maui.Controls.Markdown.Sample.Models;
public sealed class MenuItem
{
	public string Title { get; }
	public Type ViewType { get; }

	public MenuItem(string title, Type viewType)
	{
		Title = title;
		ViewType = viewType;
	}
}
