
using CommunityToolkit.Maui.Converters;
using Resources;
using SkiaSharp;

namespace Symbs;

public partial class AboutView : ContentView
{
	public AboutView()
	{
		InitializeComponent();

	}
	
	private SKBitmap? about_img;
	private double? hscale;
    private void AboutGraphic_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
		(var image_height,var image_width,var control_height,var control_width,var height_hint) = (
				e.RawInfo.Height,
				e.RawInfo.Width,
				AboutGraphic.Height,
				AboutGraphic.Width,
				(double)0
			);
		if (about_img == null)
		{
			
			
			var x = global::Resources.Resources.About_text.Split("\n")
			.Select(s => Task.Run(async () => {await Task.Yield(); return s;}))
			.Select(s => Task.Run(async () => {await Task.Yield(); var ss = await s; var sc = new SymbConvert.SymbConvert(); return (English:ss,Ketameri:sc.Translate(ss)); }))
			.Select(s => Task.Run(async () => {await Task.Yield(); var ss = await s; return (EnglishFont:new SKFont() {Size=54},EnglishBackground:SKColors.Black,EnglishForeground:SKColors.Turquoise,ss.English,ss.Ketameri);}))
			.Select(s => Task.Run(async () => {await Task.Yield(); var ss = await s; return (ss.EnglishFont,ss.EnglishBackground,ss.EnglishForeground,EnglishRect:FontHeight(ss.EnglishFont,ss.English),ss.English,ss.Ketameri);}))
			.Select(s => Task.Run(async () => {await Task.Yield(); var ss = await s; 
			var engImg = new SKBitmap((int)ss.EnglishRect.Width,(int)ss.EnglishRect.Height);
			var canvas = new SKCanvas(engImg);
			var black = new SKPaint() { Color=ss.EnglishBackground };
			var turquoise = new SKPaint { Color=ss.EnglishForeground};
			canvas.DrawRect(new SKRect(0,0,engImg.Width,engImg.Height),black);
			canvas.DrawText(ss.English,new SKPoint(-ss.EnglishRect.Left,-ss.EnglishRect.Top),ss.EnglishFont,turquoise);
			
			
			return (English:engImg,Ketameri:ss.Ketameri);
			}))
			.Select(s => Task.Run(async () => { await Task.Yield(); var ss = await s;
				var cbmp = new SKBitmap(int.Max(ss.English.Width,ss.Ketameri.Width),ss.English.Height+ss.Ketameri.Height+16);
				var ccan = new SKCanvas(cbmp);
				var black = new SKPaint() { Color = SKColors.Black};
				ccan.DrawRect(new SKRect(0,0,cbmp.Width,cbmp.Height),black);
				ccan.DrawBitmap(ss.English,new SKPoint((cbmp.Width-ss.English.Width)/2,8));
				ccan.DrawBitmap(ss.Ketameri,new SKPoint((cbmp.Width-ss.Ketameri.Width)/2,8+ss.English.Height+8));
				
				return cbmp;
			}));
			// .Select(s => Task.Run(async () => { await Task.Yield(); var ss = await s;
			// 	var width = image_width*9/10;
			// 	var ratio = (double)width/(double)ss.Width;

			// 	var sbmp = new SKBitmap(width,(int)(ss.Height*ratio));
			// 	ss.ScalePixels(sbmp,SKSamplingOptions.Default);
				
			// 	return sbmp;
			// }));
			var mx = Task.Run(async () => (await Task.WhenAll(x))
			.Aggregate((max_width:0,sum_height:0,all:Enumerable.Empty<SKBitmap>()),(s,c) => {
				return (max_width:int.Max(s.max_width,c.Width),sum_height:s.sum_height+c.Height,all:s.all.Append(c));
			},(s) => {
				var obmp = new SKBitmap(s.max_width,s.sum_height);
				var ocan = new SKCanvas(obmp);
				var y = 0;
				foreach(var img in s.all)
				{
					ocan.DrawBitmap(img,new SKPoint((obmp.Width-img.Width)/2,y));
					y+=img.Height;
				}
				var width = image_width*9/10;
				var ratio = (double)width/(double)obmp.Width;
				var sbmp = new SKBitmap(width,(int)(obmp.Height*ratio));
				obmp.ScalePixels(sbmp,SKSamplingOptions.Default);

				return sbmp;
			}));
			about_img = mx.Result;
		}
		#if IOS
        var hscale = image_height  / control_height;
        #elif ANDROID
        var hscale = image_height  / control_height;
        #else
        var hscale = 1;
        #endif
		if (about_img.Height / hscale!= AboutGraphic.Height )
		{
			AboutGraphic.HeightRequest = about_img.Height /hscale;
			
		}		
		else
		{
			e.Surface.Canvas.DrawBitmap(about_img,new SKPoint((e.RawInfo.Width-about_img.Width)/2,0));
		}
    }
	private int index = 0;
	private string GetIndex()
	{
		var idx = $"{index}";
		index += 1;
		while (idx.Length < 10)
		{
			idx = $"0{idx}";
		}
		return idx;
	}
	private object pokeLock = new object();
	
	private void PokeGraphic(SKBitmap graphic)
	{
		lock(pokeLock)
		{
			
			var Path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/DEBUG_IMAGE";
			if (Directory.Exists(Path) && index == 0)
			{
				Directory.Delete(Path,true);
				Directory.CreateDirectory(Path);
			}
			
			var filename=$"{Path}/DBG_{GetIndex()}.png";
			File.WriteAllBytes(filename,graphic.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray());
		}
		
	}
	private SKRect FontHeight(SKFont font,string text)
    {
        var rect = new SKRect();
        var flt = font.MeasureText(text,out rect);
        return rect;

    }
    private void AboutGraphic_SizeChanged(object sender, EventArgs e)
    {
		
		about_img = null;
		AboutGraphic.InvalidateSurface();
    }

}