using System.Runtime.InteropServices.Swift;
using CommunityToolkit.Maui.Storage;

namespace Symbs;

public partial class TextViewer : ContentView
{
	private bool hasChanged = false;
	public TextViewer()
	{
		InitializeComponent();
		#if WINDOWS
			WinSpacer.IsVisible = true;
		#endif
	}
	private bool IsWindows
	{
		get
		{
			#if WINDOWS
			return true;
			#else
			return false;
			#endif
		}
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
		if (ParentPage is MainPage mp)
		{
			if (IsWindows)
			{
				mp.Text = TextEditor.Text.Replace("\r","\n");
			}
			else
			{
				mp.Text = TextEditor.Text;
			}
			
		}
    }

    private void LoadButton_Clicked(object sender, EventArgs e)
    {
		if (hasChanged)
		{
			ParentPage?.DisplayAlert("Unsaved Changes", "Do you want to load another document and discard current changes?", "Yes", "No")
			.ContinueWith(async (task) => {
				if (task.IsCompletedSuccessfully)
				{
					if (task.Result)
					{
						await Dispatcher.DispatchAsync(() => DoLoad());
					}
					
				}
			});
			
		}
		else
		{
			DoLoad();
		}
    }
	private void DoLoadFinish(string text)
	{
		TextEditor.Text =  text;
		hasChanged = false;
	}
	public bool IsGraphicLoading { get => Loader.IsVisible; set => Loader.IsVisible = Loader.IsRunning = value; }
    private void DoLoad()
    {
		var fp = FilePicker.Default.PickAsync(PickOptions.Default)
		.ContinueWith(async (task) => {
			await Task.Yield();
			if (task.IsCompletedSuccessfully)
			{
				if (task.Result != null)
				{
					
					await Dispatcher.DispatchAsync(() => DoLoadFinish(File.ReadAllText(task.Result.FullPath)));
				}
			}
		});
	}

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
		
		var fileSaverResult =  FileSaver.Default.SaveAsync("message.txt", new MemoryStream(System.Text.Encoding.UTF8.GetBytes(TextEditor.Text)));
    }
		
}