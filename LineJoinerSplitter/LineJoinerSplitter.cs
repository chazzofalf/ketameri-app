using SkiaSharp;

namespace LineJoinerSplitter;

public class LineJoinerSplitter
{
    public Graphic.CachedGraphic[] Split(SKBitmap line)
    {
        var rec = new Recognizer.Recognizer();
        var fontsize = line.Height;
        var line_width = line.Height / 9;
        if (line.Width == line_width)
        {
            return Enumerable.Empty<Graphic.CachedGraphic>().ToArray();
        }
        else
        {
            return Enumerable.Range(0,(line.Width+line_width)/(fontsize+line_width))
            .Select(idx => {
                var glyph = new SKBitmap(fontsize,fontsize);
                var canvas = new SKCanvas(glyph);
                canvas.DrawBitmap(line,new SKRect((fontsize+line_width)*idx,0,(fontsize+line_width)*idx+fontsize,fontsize),new SKRect(0,0,fontsize,fontsize));
                return rec.Recognize(glyph);
            }).Reverse().ToArray();
        }
        

        
        
    }
    public SKBitmap Join(Graphic.CachedGraphic[] glyphs,bool useLarge=true,bool useBackground=true,SKColor? color=null,SKColor? backgroundColor=null)
    {
        
        var realBackgroundColor = backgroundColor != null ? backgroundColor.Value : SKColors.Transparent;
        realBackgroundColor = backgroundColor != null ? realBackgroundColor.WithRed((byte)(realBackgroundColor.Red | 1)) : realBackgroundColor;
        if (!glyphs.Any()) {
            var o = new SKBitmap(useLarge ? 6 : 1,useLarge ? 54 : 9,SKColorType.Rgba8888,SKAlphaType.Premul);
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
        var glyphWidth = useLarge ? 54 : 9;
        var padding = glyphs.Count()*lineThickness-lineThickness;
        var width = glyphWidth*glyphs.Length+padding;
        var height = glyphWidth;
        var outx = new SKBitmap(width,height,SKColorType.Rgba8888,SKAlphaType.Premul);
        var can = new SKCanvas(outx);
        var bmps_ops = glyphs
        .Select(g => useLarge ? g.ColorizeBitmap(color,!useLarge) : g.Small)
        .Reverse()  // Protodimic, Ketameri, and other very old texts are read right to left.
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
