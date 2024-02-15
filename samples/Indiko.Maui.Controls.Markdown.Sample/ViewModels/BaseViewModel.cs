using CommunityToolkit.Mvvm.ComponentModel;
using Indiko.Maui.Controls.Markdown.Sample.Interfaces;
using Indiko.Maui.Controls.Markdown.Sample.Services;

namespace Indiko.Maui.Controls.Markdown.Sample.ViewModels;

public partial class BaseViewModel : ObservableObject, IViewModel
{
	public INavigationService Navigationservice { get; }

	[ObservableProperty]
	bool isBusy;

	public BaseViewModel(INavigationService navigationService)
	{
		Navigationservice = navigationService;
	}

	public virtual void OnAppearing(object param) { }

	public virtual Task RefreshAsync()
	{
		return Task.CompletedTask;
	}
}
