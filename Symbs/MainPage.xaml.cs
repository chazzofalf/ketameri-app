namespace Symbs;

public partial class MainPage : ContentPage
{
	

	public MainPage()
	{
		InitializeComponent();
		if (App.Current is App app)
		{
			app.UserAppTheme = AppTheme.Dark;
		}
		
	}

    private void ShowTextViewButton_Clicked(object sender, EventArgs e)
    {
        if (!TextViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = AboutViewTab.IsVisible = ResultsViewTab.IsVisible = ImageViewTab.IsVisible = !(TextViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowImageViewButton_Clicked(object sender, EventArgs e)
    {
        if (!ImageViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = AboutViewTab.IsVisible = ResultsViewTab.IsVisible = TextViewTab.IsVisible = !(ImageViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowResultsViewButton_Clicked(object sender, EventArgs e)
    {
        if (!ResultsViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = AboutViewTab.IsVisible = TextViewTab.IsVisible = ImageViewTab.IsVisible = !(ResultsViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowAboutViewButton_Clicked(object sender, EventArgs e)
    {
        if (!AboutViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = ResultsViewTab.IsVisible = TextViewTab.IsVisible = ImageViewTab.IsVisible = !(AboutViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowAlphabetViewButton_Clicked(object sender, EventArgs e)
    {
        if (!AlphabetViewTab.IsVisible)
        {
            AboutViewTab.IsVisible = ResultsViewTab.IsVisible = TextViewTab.IsVisible = ImageViewTab.IsVisible = !(AlphabetViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
        }
    }

	
}

