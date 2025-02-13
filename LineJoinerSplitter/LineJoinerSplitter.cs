using SkiaSharp;

namespace LineJoinerSplitter;

public class LineJoinerSplitter
{
    public SKBitmap Join(Graphic.Graphic[] glyphs,bool useLarge=true,bool useBackground=true,SKColor? color=null,SKColor? backgroundColor=null)
    {
        
        var realBackgroundColor = backgroundColor != null ? backgroundColor.Value : SKColors.Black;
        realBackgroundColor = realBackgroundColor.WithRed((byte)(realBackgroundColor.Red & 0xfe));
        if (!glyphs.Any()) {
            var o = new SKBitmap(1,useLarge ? 54 : 9,SKColorType.Rgba8888,SKAlphaType.Premul);
            var c = new SKCanvas(o);
            if (useBackground)
            {
                
                var paint = new SKPaint();
                paint.Color = realBackgroundColor;
                c.DrawRect(new SKRect(0,0,o.Width,o.Height),paint);
            }
            
            
            return o;
        }
        var bmps = glyphs
        .Select(g => useLarge ? g.ColorizeBitmap(color,!useLarge) : g.Small)
        .Select(g => {
            var o = new SKBitmap(g.Width,g.Height,SKColorType.Rgba8888,SKAlphaType.Premul);
            var c = new SKCanvas(o);
            if (useBackground)
            {
                var paint = new SKPaint();
                paint.Color = realBackgroundColor;
                c.DrawRect(new SKRect(0,0,o.Width,o.Height),paint);
            }            
            c.DrawBitmap(g,new SKPoint(0,0));
            return o;
        }).Reverse()
        .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a)); // Protodimic or Ketameri is read right to left.
        var lineThickness = useLarge ? 6 : 1;
        var glyphWidth = useLarge ? 54 : 9;
        var padding = glyphs.Count()*lineThickness-lineThickness;
        var width = glyphWidth*glyphs.Length+padding;
        var height = glyphWidth;
        var outx = new SKBitmap(width,height,SKColorType.Rgba8888,SKAlphaType.Premul);
        return null; // TODO: You are here.

    }
}
