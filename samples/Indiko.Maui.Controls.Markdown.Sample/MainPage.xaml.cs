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

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        mainPageViewModel.OnAppearing(null);
    }
	
    private async void MarkdownView_HyperLinkClicked(object sender, LinkEventArgs e)
    {
		await DisplayAlertAsync(Title, e.Url, "OK");
    }

    private async void MarkdownView_OnEmailClicked(object sender, EmailEventArgs e)
    {
        await DisplayAlertAsync(Title, e.Email, "OK");
    }
}
