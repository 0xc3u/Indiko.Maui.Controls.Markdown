using Indiko.Maui.Controls.Markdown.Sample.ViewModels;

namespace Indiko.Maui.Controls.Markdown.Sample;

public partial class MainPage : ContentPage
{
	MainPageViewModel mainPageViewModel;

	public MainPage(MainPageViewModel mainPageViewModel)
	{
		InitializeComponent();
		this.mainPageViewModel = mainPageViewModel;
		BindingContext = mainPageViewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		mainPageViewModel.OnAppearing(null);
	}

    private void MarkdownView_HyperLinkClicked(object sender, LinkEventArgs e)
    {
		DisplayAlert(Title, e.Url, "OK");
    }
}
