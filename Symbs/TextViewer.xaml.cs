namespace Symbs;

public partial class TextViewer : ContentPage
{
	private bool hasChanged = false;
	public TextViewer()
	{
		InitializeComponent();
	}
	private void DoNew()
	{
		TextEditor.Text = "";
		hasChanged = false;
		
	}
    private void NewButton_Clicked(object sender, EventArgs e)
    {
		
		if (hasChanged)
		{
			DisplayAlert("Unsaved Changes", "Do you want to load a new document and discard current changes?", "Yes", "No")
			.ContinueWith(async (task) => {
				if (task.IsCompletedSuccessfully)
				{
					if (task.Result)
					{
						await Dispatcher.DispatchAsync(() => DoNew());
					}
					
				}
			});
			
		}
		else
		{
			DoNew();
		}
		
    }

    private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
		hasChanged = true;
    }
}