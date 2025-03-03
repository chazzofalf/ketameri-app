using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Storage;
using Graphic;
using Microsoft.Maui.Layouts;
using Microsoft.VisualBasic;
using SkiaSharp;

namespace Symbs;

public partial class MainPage : ContentPage
{
    private SKBitmap[]? alphabetGraphicImages;
    private SKBitmap? alphabetGraphic;
    

    public MainPage()
	{
		InitializeComponent();
        
		if (App.Current is App app)
		{
			app.UserAppTheme = AppTheme.Dark;
		}
		_ = Task.Run(LoadAlphabet)
        .ContinueWith(AlphabetLoaded);
	}

    private async Task AlphabetLoaded(Task<SKBitmap[]> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            await Dispatcher.DispatchAsync(() => AlphabetLoadedSync(task.Result));
        }
        else if (task.IsFaulted)
        {
            await Dispatcher.DispatchAsync(() => AlphabetLoadedException(task.Exception));
        }
    }
    private void OnException(string reason,AggregateException aggregateException)
    {
        var excflat = aggregateException.Flatten();
        System.Console.Error.WriteLine(reason);
        foreach (var exec in excflat.InnerExceptions)
        {
            System.Console.Error.WriteLine(exec.GetType().FullName);
            System.Console.Error.WriteLine(exec.Message);
            System.Console.Error.WriteLine(exec.StackTrace);
            System.Console.Error.WriteLine();
        }
        System.Console.Error.WriteLine(reason);
    }
    private void AlphabetLoadedException(AggregateException aggregateException)
    {
        OnException("Exception on Loading Alphabet Graphic",aggregateException);
    }
    private void AlphabetLoadedSync(SKBitmap[] result)
    {
        alphabetGraphicImages = result;
        AlphabetGraphic.InvalidateSurface();
        //AlphabetGraphic.HeightRequest = (double)result.Height;
        
    }
    private SKRect FontHeight(SKFont font,string text)
    {
        var rect = new SKRect();
        var flt = font.MeasureText(text,out rect);
        return rect;

    }
    private async Task<SKBitmap[]> LoadAlphabet()
    {
        await Task.Yield();
        var pho = new Phonetics.Phonetics();
        var x = Enumerable.Range(0,Graphic.CachedGraphic.NumberOfGlyphs)
        .Select(s=> Graphic.CachedGraphic.GetGraphicAtIndex(s))
        .Select(s => (glyph:s.ColorizeBitmap(),title:s.Letter,transliteration:$"({pho.ReverseTranslateString(s.Letter)})"))
        .Select(s => (font:new SKFont(), s.glyph, s.title,s.transliteration))
        .Select(s => (font:(Func<SKFont>)(() => { SKFont fnt = s.font; fnt.Size=54 ; return fnt; }),s.glyph,s.title,s.transliteration))
        .Select(s => (font:s.font(),s.glyph,s.title,s.transliteration))
        .Select(s => {
            return (font:s.font,rect:FontHeight(s.font,s.title),transliterationRect:FontHeight(s.font,s.transliteration),s.glyph,s.title,s.transliteration);
            }
        )
        .Select(s => (font:s.font,rect:s.rect.Height == 0 || s.rect.Width == 0 ? new SKRect(0,0,1,1) : s.rect,transliterationRect:s.transliterationRect.Height == 0 || s.transliterationRect.Width == 0 ? new SKRect(0,0,1,1) : s.transliterationRect,s.glyph,s.title,s.transliteration))
        //.Select(s => (font:s.font,rect:new SKRect(0,0,s.rect.Width,s.rect.Height),s.glyph,s.title))
        .Select(s => (title:(Func<SKBitmap>)(() => {
            var bmp = new SKBitmap((int)s.rect.Width,(int)s.rect.Height);
            using (var can = new SKCanvas(bmp))
            {
                using (var pt = new SKPaint())
                {
                    pt.Color = SKColors.Black;
                    can.DrawRect(new SKRect(0,0,bmp.Width,bmp.Height),pt);
                    pt.Color = SKColors.Turquoise;
                    can.DrawText(s.title,new SKPoint(-s.rect.Left,-s.rect.Top),s.font,pt);                    
                }
            }
            return bmp;
        }),
        transliteration:(Func<SKBitmap>)(() => {
            var bmp = new SKBitmap((int)s.transliterationRect.Width,(int)s.transliterationRect.Height);
            using (var can = new SKCanvas(bmp))
            {
                using (var pt = new SKPaint())
                {
                    pt.Color = SKColors.Black;
                    can.DrawRect(new SKRect(0,0,bmp.Width,bmp.Height),pt);
                    pt.Color = SKColors.Turquoise;
                    can.DrawText(s.transliteration,new SKPoint(-s.transliterationRect.Left,-s.transliterationRect.Top),s.font,pt);                    
                }
            }
            return bmp;
        }),s.glyph))
        .Select(s => (title:s.title(),transliteration:s.transliteration(),s.glyph))
        .Select(s => (title:(Func<SKBitmap>)(() => {
            var bmp = new SKBitmap(int.Max(s.title.Width,s.title.Height),int.Max(s.title.Width,s.title.Height));
            using (var can = new SKCanvas(bmp))
            {
                can.DrawBitmap(s.title,new SKPoint((bmp.Width-s.title.Width)/2,(bmp.Height-s.title.Height)/2));

            }
            return bmp;
        }),
        transliteration:(Func<SKBitmap>)(() => {
            var bmp = new SKBitmap(int.Max(s.transliteration.Width,s.transliteration.Height),int.Max(s.transliteration.Width,s.transliteration.Height));
            using (var can = new SKCanvas(bmp))
            {
                can.DrawBitmap(s.transliteration,new SKPoint((bmp.Width-s.transliteration.Width)/2,(bmp.Height-s.transliteration.Height)/2));

            }
            return bmp;
        }),s.glyph))
        .Select(s => (title:s.title(),transliteration:s.transliteration(),s.glyph))
        .Select(s => (title:(Func<SKBitmap>)(() => {
            var bmp = new SKBitmap(54,54);
            s.title.ScalePixels(bmp,SKSamplingOptions.Default);
            return bmp;
        })
        ,transliteration:(Func<SKBitmap>)(() => {
            var bmp = new SKBitmap(27,27);
            s.transliteration.ScalePixels(bmp,SKSamplingOptions.Default);
            return bmp;
        }),s.glyph))
        .Select(s => (title:s.title(),transliteration:s.transliteration(),s.glyph))
        .Select(s => (Func<SKBitmap>)(() => {
            var bmp = new SKBitmap(54,175);
            using (var can = new SKCanvas(bmp))
            {
                can.DrawBitmap(s.glyph,new SKPoint(0,0));
                can.DrawBitmap(s.title,new SKPoint(0,54+27));
                can.DrawBitmap(s.transliteration,new SKPoint(13,54+27+54+13));
            }
            return bmp;
        }))
        .Select(s => s())
        .Select(s => {
            var bordered = new SKBitmap(s.Width+12,s.Height+12);
            using (var can = new SKCanvas(bordered))
            {
                using (var p = new SKPaint())
                {
                    p.Color = SKColors.Turquoise;
                    can.DrawRect(new SKRect(0,0,bordered.Width,bordered.Height),p);
                    p.Color = SKColors.Black;
                    can.DrawRect(new SKRect(1,1,bordered.Width-1,bordered.Height-1),p);
                    can.DrawBitmap(s,6,6);
                }
            }
            return bordered;
        });        
        /*.Aggregate((MaxWidth:0,SumHeight:0,Rerun:Enumerable.Empty<SKBitmap>()),(state,current) => 
            (MaxWidth:int.Max(current.Width,state.MaxWidth),state.SumHeight+current.Height + (state.SumHeight > 0 ? 54 :0),state.Rerun.Append(current)),(fin) =>
            {
                var bmp = new SKBitmap(fin.MaxWidth,fin.SumHeight);
                var y = 0;
                using (var can = new SKCanvas(bmp))
                {
                    foreach (var img in fin.Rerun)
                    {
                        can.DrawBitmap(img,new SKPoint(0,y));
                    y += 189;
                    }
                }
                return bmp;
            }
        );*/
        return x.ToArray();
        
    }
    private void ShowTextViewButton_Clicked(object sender, EventArgs e)
    {
        if (!TextViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = AboutViewTab.IsVisible = ResultsViewTab.IsVisible = ImageViewTab.IsVisible = !(TextViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            // ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            // ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowImageViewButton_Clicked(object sender, EventArgs e)
    {
        if (!ImageViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = AboutViewTab.IsVisible = ResultsViewTab.IsVisible = TextViewTab.IsVisible = !(ImageViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            // ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            // ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowResultsViewButton_Clicked(object sender, EventArgs e)
    {
        if (!ResultsViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = AboutViewTab.IsVisible = TextViewTab.IsVisible = ImageViewTab.IsVisible = !(ResultsViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            // ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            // ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowAboutViewButton_Clicked(object sender, EventArgs e)
    {
        if (!AboutViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = ResultsViewTab.IsVisible = TextViewTab.IsVisible = ImageViewTab.IsVisible = !(AboutViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            // ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            // ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
        }
    }

    private void ShowAlphabetViewButton_Clicked(object sender, EventArgs e)
    {
        if (!AlphabetViewTab.IsVisible)
        {
            AboutViewTab.IsVisible = ResultsViewTab.IsVisible = TextViewTab.IsVisible = ImageViewTab.IsVisible = !(AlphabetViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            // ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            // ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
        }
    }
    private SKBitmap? AlphabetGridHeaderGraphic(int realHeight,int realWidth,double controlHeight,double controlWidth, out double heightHint)
    {
        var headerText = "Ketameri Alphabet";        
        var subHeaderText = "(English equivalents are in parenthesis, the bigger letter values are Ketameri transliterations of those symbols)";
        var headerFont = new SKFont() { Size=54};
        var subHeaderFont = new SKFont() { Size=54};
        var headerRect = FontHeight(headerFont,headerText);
        var subHeaderRect = FontHeight(subHeaderFont,subHeaderText);
        var headerGraphic = new SKBitmap((int)headerRect.Width,(int)headerRect.Height);
        var subHeaderGraphic = new SKBitmap((int)subHeaderRect.Width,(int)subHeaderRect.Height);
        var black = new SKPaint() { Color=SKColors.Black};
        var turquoise = new SKPaint() { Color=SKColors.Turquoise};
        var headerCanvas = new SKCanvas(headerGraphic);
        var subHeaderCanvas = new SKCanvas(subHeaderGraphic);
        
            
        headerCanvas.DrawRect(new SKRect(0,0,headerGraphic.Width,headerGraphic.Height),black);
    
    
        headerCanvas.DrawText(headerText,new SKPoint(-headerRect.Left,-headerRect.Top),headerFont,turquoise);
        

    
        subHeaderCanvas.DrawRect(new SKRect(0,0,subHeaderGraphic.Width,subHeaderGraphic.Height),black);
    
    
        subHeaderCanvas.DrawText(subHeaderText,new SKPoint(-subHeaderRect.Left,-subHeaderRect.Top),headerFont,turquoise);
                
        
        var hscale = realHeight  / controlHeight;
        var width = realWidth-16;
        var maxLineWidth = width*2/3;
        var headerScale = (double)maxLineWidth/(double)headerGraphic.Width;
        var scaledHeader = new SKBitmap((int)(headerGraphic.Width*headerScale),(int)(headerGraphic.Height*headerScale));
        headerGraphic.ScalePixels(scaledHeader,SKSamplingOptions.Default);
        var subHeaderScale = (double)maxLineWidth/(double)subHeaderGraphic.Width;
        var scaledSubheader = new SKBitmap((int)(subHeaderGraphic.Width*subHeaderScale),(int)(subHeaderGraphic.Height*subHeaderScale));
        subHeaderGraphic.ScalePixels(scaledSubheader,SKSamplingOptions.Default);
        heightHint = (8+scaledHeader.Height+27+scaledSubheader.Height+27);
        var output = new SKBitmap(width,(int)heightHint);
        var outputCanvas = new SKCanvas(output);
        
        
        outputCanvas.DrawRect(new SKRect(0,0,output.Width,output.Height),black);
            
        outputCanvas.DrawBitmap(scaledHeader,new SKPoint((realWidth-scaledHeader.Width)/2,8));
        outputCanvas.DrawBitmap(scaledSubheader,new SKPoint((realWidth-scaledSubheader.Width)/2,8+scaledHeader.Height+27));
        
        return output;
    }
    private SKBitmap? AlphabetGraphicFull(int realHeight,int realWidth,double controlHeight,double controlWidth,out double heightHint)
    {
        var heightHintHeader = (double)0;
        var heightHintGrid = (double)0;
        var header = AlphabetGridHeaderGraphic(realHeight,realWidth,controlHeight,controlWidth,out heightHintHeader);
        var grid = AlphabetGridGraphic(realHeight,realWidth,controlHeight,controlWidth,out heightHintGrid);
        var outx = new SKBitmap(realWidth,(int)(heightHintHeader+heightHintGrid));
        using (var black = new SKPaint() { Color = SKColors.Black})
        {
            using (var blanker = new SKCanvas(outx))
            {
                blanker.DrawRect(new SKRect(0,0,outx.Width,outx.Height),black);
                blanker.DrawBitmap(header,0,0);
                blanker.DrawBitmap(grid,0,(int)heightHintHeader);
            }
        }
        heightHint = heightHintHeader+heightHintGrid;
        return outx;
    }
    private SKBitmap? AlphabetGridGraphic(int realHeight,int realWidth,double controlHeight,double controlWidth,out double heightHint)
    {
        
        if (alphabetGraphicImages is SKBitmap[] input)
        {
            var hscale = realHeight  / controlHeight;
            var width = realWidth-16;
            var seg_width = input.Select(s => s.Width).Max();
            var seg_height = input.Select(s => s.Height).Max();
            var segs_per_width = width /seg_width;
            int height = realHeight-16;
            var segs_per_height = Math.Ceiling((double)((double)input.Length/(double)segs_per_width));
            var desired_height = (int)(seg_height*segs_per_height+27*(segs_per_height-1)) + 16;
            height += 16;
            width += 16;
            heightHint = desired_height;
            var bmp = new SKBitmap(width,(int)heightHint);
            var idx = 0;
            using (var can = new SKCanvas(bmp))
            {
                using (var paint = new SKPaint())
                {
                    paint.Color = SKColors.Black;
                    can.DrawRect(new SKRect(0,0,bmp.Width,bmp.Height),paint);
                    foreach (var img in input)
                    {
                        var x = idx % segs_per_width;
                        var y = idx / segs_per_width;
                        var ix = (x * seg_width) +8;
                        var iy = (y * (seg_height+27)) +8;
                        can.DrawBitmap(img,new SKPoint(ix,(int)iy));
                        idx += 1;
                    }
                }
                
            }
            return bmp;
        }
        else
        {
            heightHint = controlHeight;
            return null;
        }
    }
    
    private void AlphabetGraphic_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {        
        if (alphabetGraphicImages is SKBitmap[] input)
        {
            var newHeight = (double)0;
            var outtemp = AlphabetGraphicFull(e.Info.Height,e.Info.Width,AlphabetGraphic.Height,AlphabetGraphic.Width,out newHeight);      
            
            if (newHeight > e.Info.Height)
            {
                AlphabetGraphic.HeightRequest = newHeight;
                alphabetGraphic = outtemp;
            }
            else
            {
                
                e.Surface.Canvas.DrawBitmap(alphabetGraphic,new SKPoint(0,0));
            }
            
            
            
        }
        
        
    }

    private void AlphabetGraphic_SizeChanged(object sender, EventArgs e)
    {
        AlphabetGraphic.InvalidateSurface();
    }
    
    private void SaveAlphabet_Clicked(object sender, EventArgs e)
    {
         if (alphabetGraphic != null)
        {
            var fileSaverResult =  FileSaver.Default.SaveAsync("alphabet.png", alphabetGraphic.Encode(SKEncodedImageFormat.Png,100).AsStream());
            
        }
           
            
        
        
         
    }

	
}

