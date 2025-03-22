using SkiaSharp;

namespace Symbs;

public partial class HollowedView : ContentView
{
	public HollowedView()
	{
		InitializeComponent();
	}

    private void Image_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		SKBitmap tileImage = ImageTiler.LoadImage(global::Resources.Resources.WorshipResource);
		SKBitmap tiledImage = ImageTiler.CreateTiledImage(e.RawInfo.Width, e.RawInfo.Height, tileImage);
		var canvas = e.Surface.Canvas;
    	canvas.Clear();
    	canvas.DrawBitmap(tiledImage, new SKRect(0, 0, e.RawInfo.Width, e.RawInfo.Height));
    }
}