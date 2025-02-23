using System.Threading.Tasks;
using Android.Net.Nsd;
using Graphic;
using Java.Util.Concurrent;
using SkiaSharp;

namespace Symbs;

public partial class MainPage : ContentPage
{
    private SKBitmap? alphabetGraphicImage;

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

    private async Task AlphabetLoaded(Task<SKBitmap> task)
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
    private void AlphabetLoadedSync(SKBitmap result)
    {
        alphabetGraphicImage = result;
        AlphabetGraphic.HeightRequest = (double)result.Height;
        
    }
    private SKRect FontHeight(SKFont font,string text)
    {
        var rect = new SKRect();
        var flt = font.MeasureText(text,out rect);
        return rect;

    }
    private async Task<SKBitmap> LoadAlphabet()
    {
        var x = Enumerable.Range(0,Graphic.CachedGraphic.NumberOfGlyphs)
        .Select(s=> Graphic.CachedGraphic.GetGraphicAtIndex(s))
        .Select(s => (glyph:s.ColorizeBitmap(),title:s.Letter))
        .Select(s => (font:new SKFont(),glyph:s.glyph,title:s.title))
        .Select(s => (font:(Func<SKFont>)(() => { SKFont fnt = s.font; fnt.Size=54 ; return fnt; }),s.glyph,s.title))
        .Select(s => (font:s.font(),s.glyph,s.title))
        .Select(s => (font:s.font,rect:FontHeight(s.font,s.title),s.glyph,s.title))
        .Select(s => (title:(Func<SKBitmap>)(() => {
            var bmp = new SKBitmap((int)s.rect.Width,(int)s.rect.Height);
            using (var can = new SKCanvas(bmp))
            {
                using (var pt = new SKPaint())
                {
                    pt.Color = SKColors.Black;
                    can.DrawRect(new SKRect(0,0,bmp.Width,bmp.Height),pt);
                    pt.Color = SKColors.Turquoise;
                    can.DrawText(s.title,s.rect.Location,s.font,pt);
                    
                }
            }
            return bmp;
        }),s.glyph))
        .Select(s => (title:s.title(),s.glyph))
        .Select(s => );
    }
    private void ShowTextViewButton_Clicked(object sender, EventArgs e)
    {
        if (!TextViewTab.IsVisible)
        {
            AlphabetViewTab.IsVisible = AboutViewTab.IsVisible = ResultsViewTab.IsVisible = ImageViewTab.IsVisible = !(TextViewTab.IsVisible = true);
            ShowTextViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
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
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
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
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
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
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
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
            ShowImageViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowResultsViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAboutViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDarkInactive"];
            ShowAlphabetViewButton.BackgroundColor = (Color)Application.Current!.Resources["PrimaryDark"];
            ShowTextViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowImageViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowResultsViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAboutViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkTextInactive"];
            ShowAlphabetViewButton.TextColor = (Color)Application.Current!.Resources["PrimaryDarkText"];
        }
    }

	
}

