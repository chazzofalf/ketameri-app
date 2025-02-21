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
    }

    private void ShowImageViewButton_Clicked(object sender, EventArgs e)
    {
    }

    private void ShowResultsViewButton_Clicked(object sender, EventArgs e)
    {
    }

	
}

