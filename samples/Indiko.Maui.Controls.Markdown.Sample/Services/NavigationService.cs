namespace Indiko.Maui.Controls.Markdown.Sample.Services;

public interface INavigationService
{
	Task NavigateToViewAsync(string route, bool animated = true);
	Task NavigateToViewAsync<TModel>(string route, TModel model, bool animated = true);
	Task NavigateToViewAsync(string route, Guid id, bool animated = true);
	Task NavigateBackAsync(bool animated = true);
}

public class NavigationService : INavigationService
{
	public async Task NavigateBackAsync(bool animated = true)
	{
		await Shell.Current.GoToAsync("..", animated);
	}

	public async Task NavigateToViewAsync(string route, bool animated = true)
	{
		await Shell.Current.GoToAsync(route, animated);
	}

	public async Task NavigateToViewAsync<TModel>(string route, TModel model, bool animated = true)
	{
		var type = typeof(TModel);

		await Shell.Current.GoToAsync(route, animated, new Dictionary<string, object> { { type.Name, model } });
	}

	public async Task NavigateToViewAsync(string route, Guid id, bool animated = true)
	{
		await Shell.Current.GoToAsync(route, animated, new Dictionary<string, object> { { "Id", id } });
	}

	public async Task NavigateToViewModalAsync(string route, Guid id, bool animated = true)
	{
		await Shell.Current.GoToAsync(route, animated, new Dictionary<string, object> { { "Id", id } });
	}


}