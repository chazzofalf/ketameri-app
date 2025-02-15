using SkiaSharp;

namespace LineJoinerSplitter;

public class LineJoinerSplitter
{
    public SKBitmap Join(Graphic.Graphic[] glyphs,bool useLarge=true,bool useBackground=true,SKColor? color=null,SKColor? backgroundColor=null)
    {
        
        var realBackgroundColor = backgroundColor != null ? backgroundColor.Value : SKColors.Black;
        realBackgroundColor = realBackgroundColor.WithRed((byte)(realBackgroundColor.Red | 1));
        if (!glyphs.Any()) {
            var o = new SKBitmap(1,useLarge ? 66 : 11,SKColorType.Rgba8888,SKAlphaType.Premul);
            var c = new SKCanvas(o);
            if (useBackground)
            {
                
                var paint = new SKPaint();
                paint.Color = realBackgroundColor;
                c.DrawRect(new SKRect(0,0,o.Width,o.Height),paint);
            }
            
            
            return o;
        }
        var lineThickness = useLarge ? 6 : 1;
        var glyphWidth = useLarge ? 66 : 11;
        var padding = glyphs.Count()*lineThickness-lineThickness;
        var width = glyphWidth*glyphs.Length+padding;
        var height = glyphWidth;
        var outx = new SKBitmap(width,height,SKColorType.Rgba8888,SKAlphaType.Premul);
        var can = new SKCanvas(outx);
        var bmps_ops = glyphs
        .Select(g => useLarge ? g.ColorizeBitmap(color,!useLarge) : g.Small)
        .Reverse()  // Protodimic or Ketameri is read right to left.
        .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a))
        .Select(s => {
            can.DrawBitmap(s.Item,s.Index*(glyphWidth+lineThickness),0);
            return 1;
        })
        .Sum();
        if (useBackground)
        {
            var orig_outx = outx;
            outx = new SKBitmap(orig_outx.Width,orig_outx.Height,SKColorType.Rgba8888,SKAlphaType.Premul);
            var cano = new SKCanvas(outx);
            var paint = new SKPaint();
            paint.Color = realBackgroundColor;
            cano.DrawRect(new SKRect(0,0,outx.Width,outx.Height),paint);
            cano.DrawBitmap(orig_outx,new SKPoint(0,0));
        }
        

        return outx;

    }
}
