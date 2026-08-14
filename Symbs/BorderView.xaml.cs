using System.Reflection;
using Microsoft.Maui.Graphics.Platform;
using SkiaSharp;

namespace Symbs;

public partial class BorderView : ContentView
{

	public BorderView()
	{
		InitializeComponent();
		
	}

    private void Top_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		SKBitmap tileImage = ImageTiler.LoadImage(global::Resources.Resources.BorderResource);
        var scale = e.RawInfo.Width / Top.Width;
        SKBitmap scImage = new SKBitmap((int)(tileImage.Width*scale),(int)(tileImage.Height*scale));
        tileImage.ScalePixels(scImage,SKSamplingOptions.Default);
		SKBitmap tiledImage = ImageTiler.CreateTiledImage(e.RawInfo.Width, e.RawInfo.Height, scImage);
		var canvas = e.Surface.Canvas;
    	canvas.Clear();
    	canvas.DrawBitmap(tiledImage, new SKRect(0, 0, e.RawInfo.Width, e.RawInfo.Height), SKSamplingOptions.Default);
    }

    private void Bottom_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		Top_PaintSurface(sender,e);
    }

    private void Left_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		SKBitmap tileImage = ImageTiler.LoadImage(global::Resources.Resources.VBorderResource);
        var scale = e.RawInfo.Width / Left.Width;
        SKBitmap scImage = new SKBitmap((int)(tileImage.Width*scale),(int)(tileImage.Height*scale));
        tileImage.ScalePixels(scImage,SKSamplingOptions.Default);
		SKBitmap tiledImage = ImageTiler.CreateTiledImage(e.RawInfo.Width, e.RawInfo.Height, scImage);
		var canvas = e.Surface.Canvas;
    	canvas.Clear();
    	canvas.DrawBitmap(tiledImage, new SKRect(0, 0, e.RawInfo.Width, e.RawInfo.Height), SKSamplingOptions.Default);
    }

    private void Right_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		Left_PaintSurface(sender,e);
    }
}
internal class ImageTiler
{
    public static SKBitmap CreateTiledImage(int width, int height, SKBitmap tileImage)
    {
        // Create a new bitmap with the desired dimensions
        var tiledBitmap = new SKBitmap(width, height);
        using (var canvas = new SKCanvas(tiledBitmap))
        {
            // Loop through the width and height of the new image
            for (int y = 0; y < height; y += tileImage.Height)
            {
                for (int x = 0; x < width; x += tileImage.Width)
                {
                    // Draw the tile image at the current position
                    canvas.DrawBitmap(tileImage, x, y, SKSamplingOptions.Default);
                }
            }
        }
        return tiledBitmap;
    }

    public static SKBitmap LoadImage(Stream resource)
    {
        
        
        //var imageSource = ImageSource.FromFile(resourcePath);
        using (var stream = resource)
        {
            return SKBitmap.Decode(stream);
        }
    }
}