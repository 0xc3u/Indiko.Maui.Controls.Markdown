using CommunityToolkit.Mvvm.ComponentModel;
using Indiko.Maui.Controls.Markdown.Sample.Interfaces;

namespace Indiko.Maui.Controls.Markdown.Sample.ViewModels;

public partial class BaseViewModel : ObservableObject, IViewModel
{
	[ObservableProperty]
	bool isBusy;

	public virtual void OnAppearing(object param) { }

	public virtual Task RefreshAsync()
	{
		return Task.CompletedTask;
	}
}
