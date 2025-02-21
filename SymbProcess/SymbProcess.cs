using SkiaSharp;

namespace SymbProcess;

public class SymbProcess
{
    public async Task<SKBitmap> Translate(string name,SKColor? fontColor=null,SKColor? highlightColor=null,SKColor? backgroundColor=null,SKBitmap? backgroundBitmap=null,bool useLarge=true,bool useHighlight=false,bool centered=false)
    {
        await Task.Yield();
        var sc = new SymbConvert.SymbConvert();
        return sc.Translate(name,fontColor,highlightColor,backgroundColor,backgroundBitmap,useLarge,useHighlight,centered);
    }
    public async Task<string> ReverseTranslate(SKBitmap ketameri)
    {
        await Task.Yield();
        var sc = new SymbConvert.SymbConvert();
        return sc.ReverseTranslate(ketameri);
    }
}
