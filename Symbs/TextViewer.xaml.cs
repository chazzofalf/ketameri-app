namespace Symbs;

public partial class TextViewer : ContentView
{
	private bool hasChanged = false;
	public TextViewer()
	{
		InitializeComponent();
	}
	public string Text {
		get => TextEditor.Text;
		set => TextEditor.Text = value;
	}
	private void DoNew()
	{
		TextEditor.Text = "";
		hasChanged = false;
		
	}
	private Page? ParentPage 
	{
		get {
			var view = Parent;
			while (view is not Page && view != null)
			{
				view = view.Parent;
			}
			return view as Page;
		}
	}
    private void NewButton_Clicked(object sender, EventArgs e)
    {
		
		if (hasChanged)
		{
			ParentPage?.DisplayAlert("Unsaved Changes", "Do you want to load a new document and discard current changes?", "Yes", "No")
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