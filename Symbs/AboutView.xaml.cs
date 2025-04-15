using SkiaSharp;

namespace Symbs;

public partial class AboutView : ContentView
{
	private App? ParentApplication => Application.Current as App;
	private void PostException(Exception e)
	{
		ParentApplication?.PostException(e);
	}
	public AboutView()
	{
		InitializeComponent();
		OffBufferLoop()
		.ContinueWith((task) => {if (task.IsFaulted)
		{
			PostException(task.Exception);
		}});
		#if WINDOWS
			WindowsSpacer.IsVisible = true;
		#endif
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
	bool dirty = true;
	public bool IsGraphicLoading { get => Loader.IsVisible; set => Loader.IsVisible = Loader.IsRunning = value; }
	private async Task OffBuffer()
	{
		await Task.Yield();
		if (AboutGraphic.CanvasSize.IsEmpty || AboutGraphic.Height == 0 || AboutGraphic.Width == 0) return;
		var csz = AboutGraphic.CanvasSize;
		var asz = new SKSize((float)AboutGraphic.Width,(float)AboutGraphic.Height);
		
		(var image_height,var image_width,var control_height,var control_width,var height_hint) = (
				csz.Height,
				csz.Width,
				AboutGraphic.Height,
				AboutGraphic.Width,
				(double)0
			);
		#if IOS
		var hscale = image_height  / control_height;
		#elif ANDROID
		var hscale = image_height  / control_height;
		#else
		var hscale = 1;
		#endif
		if (last_size == null || (int)asz.Width != (int)last_size.Value.Width || (int)asz.Height != (int)last_size.Value.Height || about_img == null)
		{
			await Dispatcher.DispatchAsync(() => {
				
					IsGraphicLoading = true;
				
			});
			var ns = global::Resources.Resources.Names;
			
			var x = global::Resources.Resources.About_text.Split("\n")
			.Select(s => Task.Run(async () => {await Task.Yield(); return s;}))
			.Select(s => Task.Run(async () => {await Task.Yield(); var ss = await s; var sc = new SymbConvert.SymbConvert(); return (English:ss,Ketameri:sc.Translate(ss)); }))
			.Select(s => Task.Run(async () => {await Task.Yield(); var ss = await s; return (EnglishFont:new SKFont() {Size=54,Embolden=true},EnglishBackground:SKColors.Black,EnglishForeground:SKColors.Turquoise,ss.English,ss.Ketameri);}))
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
			
			var mx = Task.Run(async () => (await Task.WhenAll(x))
			.Aggregate((max_width:0,sum_height:0,all:Enumerable.Empty<SKBitmap>()),(s,c) => {
				return (max_width:int.Max(s.max_width,c.Width),sum_height:s.sum_height+c.Height+27,all:s.all.Append(c));
			},(s) => {
				var obmp = new SKBitmap(s.max_width,s.sum_height);
				var ocan = new SKCanvas(obmp);
				var y = 0;
				foreach(var img in s.all)
				{
					ocan.DrawBitmap(img,new SKPoint((obmp.Width-img.Width)/2,y));
					y+=img.Height+27;
				}
				var width = image_width*9/10;
				var ratio = (double)width/(double)obmp.Width;
				var sbmp = new SKBitmap((int)width,(int)(obmp.Height*ratio));
				obmp.ScalePixels(sbmp,SKSamplingOptions.Default);

				return sbmp;
			}));
			
			about_img = await mx;
			dirty = true;
			scale = hscale;
			last_size = new SKSize((float)control_width,(float)(about_img.Height / hscale));
			
			await Dispatcher.DispatchAsync(() => {
				if (about_img is SKBitmap about_img_i)
				{
					if ((int)(about_img_i.Height / scale.Value)!= (int)(AboutGraphic.Height) )
					{
						AboutGraphic.HeightRequest = about_img_i.Height /scale.Value;
						
						
					}
				}
				
			});
		}
			await Dispatcher.DispatchAsync(() => AboutGraphic.InvalidateSurface());
		//}
	}
	private SKSize? last_size;
	private double? scale;
	private SKBitmap? about_img;
	private SKBitmap? _turquoisePixel;
	private SKBitmap TurquoisePixel => _turquoisePixel = _turquoisePixel ?? GenerateTurquoisePixel();
	private byte[]? _turquoisePixelBytes;
	private byte[] TurquoisePixelBytes => _turquoisePixelBytes = _turquoisePixelBytes ?? GenerateTurquoisePixelBytes();

    private byte[] GenerateTurquoisePixelBytes()
    {
		byte[] pBytes = new byte[TurquoisePixel.BytesPerPixel];
		
		unsafe
		{
			byte *pixelData = (byte *)TurquoisePixel.GetPixels();
			for (var i=0;i<pBytes.Length;i++)
			{
				pBytes[i] = pixelData[i];
			}
		} 
		return pBytes;
    }
	private unsafe bool PixelMatch(byte *ptr,int offset,byte[] pixelTest)
	{
		return ptr[offset+0] == pixelTest[0] &&
		ptr[offset+1] == pixelTest[1] &&
		ptr[offset+2] == pixelTest[2] &&
		ptr[offset+3] == pixelTest[3];
	}
	private unsafe bool IsTurquoise(byte *ptr,int offset)
	{
		return PixelMatch(ptr,offset,TurquoisePixelBytes);
	}

    private SKBitmap GenerateTurquoisePixel()
    {
        var pixel = new SKBitmap(1,1);
		var turquoise = SKColors.Turquoise;
		var turquoisePaint = new SKPaint() { Color = turquoise};
		var canvas = new SKCanvas(pixel);
		canvas.DrawRect(new SKRect(0,0,pixel.Width,pixel.Height),turquoisePaint);
		return pixel;
    }

    private void AboutGraphic_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
	{
		if (dirty && about_img != null && scale != null && Microsoft.Maui.Controls.Application.Current is App app)
		{
			
			
			var renderCopy = app.MakeTransparency(about_img);
			
			dirty = false;
			e.Surface.Canvas.DrawBitmap(renderCopy,new SKPoint((e.RawInfo.Width-about_img.Width)/2,0));
			Dispatcher.Dispatch(() => {
				
					IsGraphicLoading = false;
				
			});
				
			
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