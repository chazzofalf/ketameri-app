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
    public void ShownHandle()
    {
        if (currentText == null || currentText != Text)
        {
            Loader.IsVisible = true;
            Loader.IsRunning = true;
            UpdateImageView()
            .ContinueWith(t => {
                if (t.IsFaulted)
                {
                    PostException(t.Exception);
                }
            });

        }
    }
    bool dirty = false;
    SKBitmap? bitmap;
    private SKSize? last_size;
    private double? scale;
    public bool IsGraphicLoading { get => Loader.IsVisible; set => Loader.IsVisible = Loader.IsRunning = value; }


    private async Task OffBuffer()
	{
		await Task.Yield();
		if (ImageViewerViewer.CanvasSize.IsEmpty || ImageViewerViewer.Height == 0 || ImageViewerViewer.Width == 0 || Text == null) return;
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
        OffBufferLoop()
		.ContinueWith((task) => {if (task.IsFaulted)
		{
			PostException(task.Exception);
		}});
		#else
		var hscale = 1;
        var wscale = 1;
		#endif
		if (last_size == null || currentText == null || currentText != Text || asz.Width != last_size.Value.Width || asz.Height != last_size.Value.Height || bitmap == null)
		{
			await Dispatcher.DispatchAsync(() => {
				
					IsGraphicLoading = true;
				
			});
            var sc = new SymbConvert.SymbConvert();

			bitmap = sc.Translate(Text);
            currentText = Text;
			
			dirty = true;
			scale = hscale;
			last_size = new SKSize((float)bitmap.Width / wscale,(float)(bitmap.Height / hscale));
			
			await Dispatcher.DispatchAsync(() => {
				if (bitmap is SKBitmap about_img_i)
				{
					if ((int)(about_img_i.Height / scale.Value)!= (int)(ImageViewerViewer.Height) )
					{
						ImageViewerViewer.HeightRequest = about_img_i.Height /scale.Value;
						
						
					}
                    if ((int)(about_img_i.Width / scale.Value)!= (int)(ImageViewerViewer.Width) )
                    {
                        ImageViewerViewer.WidthRequest = about_img_i.Width /scale.Value;
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
    private async Task UpdateImageView()
    {
        await Task.Yield();
        currentText = Text;
        var sc = new SymbConvert.SymbConvert();
        bitmap = sc.Translate(currentText);
        
        await Dispatcher.DispatchAsync(() => {
            
            
            ImageViewerViewer.HeightRequest = bitmap.Height / (ImageViewerViewer.Height/ImageViewerViewer.CanvasSize.Height);
            ImageViewerViewer.WidthRequest = bitmap.Width / (ImageViewerViewer.Width / ImageViewerViewer.CanvasSize.Width);
        });
        
        await Dispatcher.DispatchAsync(() => {
            dirty = true;
            
        });
        ImageViewerViewer.InvalidateSurface();
        
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
    private string Text { get => MyMainPage.Text; set => MyMainPage.Text = value; }
    private string? currentText = null;
    private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
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

    private void Load_Clicked(object sender, EventArgs e)
    {
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
    }
}