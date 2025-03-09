using CommunityToolkit.Maui.Storage;
using SkiaSharp;

namespace Symbs;

public partial class ImageViewer : ContentView
{
	public ImageViewer()
	{
		InitializeComponent();
        OffBufferLoop()
		.ContinueWith((task) => {if (task.IsFaulted)
		{
			PostException(task.Exception);
		}});
	}
    private App? ParentApplication => Application.Current as App;
	private void PostException(Exception e)
	{
		ParentApplication?.PostException(e);
	}
    
    bool dirty = false;
    SKBitmap? bitmap;
    private SKSize? last_size;
    private double? scale;
    private double? scale_w;
    public bool IsGraphicLoading { get => Loader.IsVisible; set => Loader.IsVisible = Loader.IsRunning = value; }


    private async Task OffBuffer()
	{
		await Task.Yield();
		if (!IsVisible  || ImageViewerViewer.CanvasSize.IsEmpty || ImageViewerViewer.Height == 0 || ImageViewerViewer.Width == 0 || ParentPage as MainPage == null || (ParentPage as MainPage)!.Text == null) return;
		var text = (ParentPage as MainPage)!.Text;
        var csz = ImageViewerViewer.CanvasSize;
		var asz = new SKSize((float)ImageViewerViewer.Width,(float)ImageViewerViewer.Height);
		
		(var image_height,var image_width,var control_height,var control_width,var height_hint) = (
				csz.Height,
				csz.Width,
				ImageViewerViewer.Height,
				ImageViewerViewer.Width,
				(double)0
			);
		#if IOS
		var hscale = image_height  / control_height;
        var wscale = image_width / control_width;
		#elif ANDROID
		var hscale = image_height  / control_height;
        var wscale = image_width / control_width;
		#else
		var hscale = 1;
        var wscale = 1;
		#endif
		if (last_size == null || currentText == null || currentText != text || asz.Width != last_size.Value.Width || asz.Height != last_size.Value.Height || bitmap == null)
		{
			await Dispatcher.DispatchAsync(() => {
				
					IsGraphicLoading = true;
				
			});
            var sc = new SymbConvert.SymbConvert();

			bitmap = sc.Translate(text,centered:true);
            currentText = text;
			
			dirty = true;
			scale = hscale;
            scale_w = wscale;
			last_size = new SKSize((float)bitmap.Width / (float)wscale,(float)(bitmap.Height / hscale));
			
			await Dispatcher.DispatchAsync(() => {
				if (bitmap is SKBitmap about_img_i)
				{
					if ((int)(about_img_i.Height / scale.Value)!= (int)(ImageViewerViewer.Height) )
					{
						ImageViewerViewer.HeightRequest = about_img_i.Height /scale.Value;
						
						
					}
                    if ((int)(about_img_i.Width / scale_w.Value)!= (int)(ImageViewerViewer.Width) )
                    {
                        ImageViewerViewer.WidthRequest = about_img_i.Width /scale_w.Value;
                    }
				}
				
			});
		}
			await Dispatcher.DispatchAsync(() => ImageViewerViewer.InvalidateSurface());
		//}
	}
    private const int FPS = 1000/60;

    private async Task OffBufferLoop()
	{
		await Task.Yield();
		while (true)
		{
			await Task.Yield();
			await OffBuffer();
			await Task.Delay(dirty ? FPS : 1000);
		}
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
    private MainPage MyMainPage
    {
        get {
            return ParentPage as MainPage ?? throw new Exception("No Main Page");
        }
    }
    
    private string? currentText = null;
    private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        if (!IsVisible) return;
        System.Console.WriteLine($"Draw! {System.DateTime.Now}");
        if (dirty)
        {
            dirty = false;
            if (bitmap is SKBitmap bitmap1)
            {
                e.Surface.Canvas.DrawBitmap(bitmap1,new SKPoint(0,0));
                Loader.IsVisible = Loader.IsRunning = false;
            }
           
        }
        else
        {
            if (bitmap is SKBitmap bitmap1)
            {
                e.Surface.Canvas.DrawBitmap(bitmap1,new SKPoint(0,0));
            }
            
            
        }
    }

    private void SKCanvasView_SizeChanged(object sender, EventArgs e)
    {
    }
	public async Task<SKBitmap?> PickAndShow()
	{
		PickOptions pickOptions = new PickOptions();
		pickOptions.FileTypes = FilePickerFileType.Png;
		try
		{
			var result = await FilePicker.Default.PickAsync();
			if (result != null)
			{
				if (result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
				{
					using var stream = await result.OpenReadAsync();
					var image = SKBitmap.Decode(stream);
					return image;
				}
			}

			return null;
		}
		catch (Exception ex)
		{
			ParentApplication?.PostException(ex);
			// The user canceled or something went wrong
		}

		return null;
	}
    private void Load_Clicked(object sender, EventArgs e)
    {
		PickAndShow()
		.ContinueWith((task) => {
			if (task.IsCompletedSuccessfully && task.Result != null)
			{
				var sc = new SymbConvert.SymbConvert();
				var text = sc.ReverseTranslate(task.Result);
				Dispatcher.Dispatch(() => {
					if (ParentPage is MainPage mp)
					{
						mp.Text = text;
						bitmap = task.Result;
					}
				});
			}
		});
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
		 if (bitmap != null )
        {
            
            var fileSaverResult =  FileSaver.Default.SaveAsync("words.png", bitmap.Encode(SKEncodedImageFormat.Png,100).AsStream());
            
        }
    }
}