using SkiaSharp;

namespace SymbConvert;

public class SymbConvert
{
    public SKBitmap Translate(string name,SKColor? fontColor=null,SKColor? highlightColor=null,SKColor? backgroundColor=null,SKBitmap? backgroundBitmap=null,bool useLarge=true,bool useHighlight=false,bool centered=false)
    {
        var p = new Phonetics.Phonetics();
        var t = new Tokenizer.Tokenizer();
        var l = new LineJoinerSplitter.LineJoinerSplitter();
        var pp = new PageJoinerSpliter.PageJoinerSpliter();
        
        return pp.Join( p.TranslateString(name).Split("\n")
        .Select(line => {
            var ops = line.Select(ch => {
                t.Put(ch);
                return 1;
            }).Sum();
            return t.Finish<Graphic.CachedGraphic[]>();
        })
        .Select(line => l.Join(line,useLarge,useHighlight,fontColor,highlightColor)).ToArray(),centered,backgroundColor,backgroundBitmap);
    }
    public string ReverseTranslate(SKBitmap ketameri)
    {
        var pp = new PageJoinerSpliter.PageJoinerSpliter();
        var l = new LineJoinerSplitter.LineJoinerSplitter();
        var t = new Tokenizer.Tokenizer();
        var p = new Phonetics.Phonetics();
        return p.ReverseTranslateString(string.Join("\n",pp.Split(ketameri)
        .Select(line => l.Split(line))
        .Select(line => {
            var ops = line.Select(ch => {
                t.Put(ch);
                return 1;
            }).Sum();
            return t.Finish<string>();
        })));
    }
}
