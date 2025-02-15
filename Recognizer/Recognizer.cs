using SkiaSharp;

namespace Recognizer;


public class Recognizer
{
    public interface IRecognizedOrientation
    {
        public bool IsVerticallyFlipped {get;}
        public bool IsHorizontallyFlipped {get;}
        public bool IsRotated {get;}
        
        
    }
    private class RecognizedOrientation : IRecognizedOrientation
    {
        
    
        public bool IsVerticallyFlipped {get;}
        public bool IsHorizontallyFlipped {get;}
        public bool IsRotated {get;}
        public RecognizedOrientation(bool vflip,bool hflip,bool rotate)
        {
            IsVerticallyFlipped = vflip;
            IsHorizontallyFlipped = hflip;
            IsRotated = rotate;
        }
    
    }
    private SKBitmap? lastCheckedImage;
    private IRecognizedOrientation? lastDeterminedOrientation;
    public IRecognizedOrientation DetermineGlyphOrientation(SKBitmap glyphImage)
    {
        if (glyphImage == lastCheckedImage && lastDeterminedOrientation != null)
        {
            return lastDeterminedOrientation;
        }
        else
        {
            
        }
        return null;
    }
    public Graphic.Graphic Recognize(SKBitmap glyphImage)
    {
        if (glyphImage.Height != glyphImage.Width) throw new Exception("Ketameri Glyphs are square. This is not!");
        var ori = DetermineGlyphOrientation(glyphImage);
        if (ori.IsHorizontallyFlipped || ori.IsVerticallyFlipped || ori.IsRotated) throw new Exception("Out of proper orientation. Please use DetermineGlyphOrientation(...) and orient the root graphic properly first.");
        
        var borderless = new SKBitmap(glyphImage.Width*9/11,glyphImage.Height*9/11); // 54 is 9/11 of 66. Never forget. 
        var c = new SKCanvas(borderless);
        c.DrawBitmap(glyphImage,new SKRect(glyphImage.Width/11,glyphImage.Height/11,glyphImage.Width*10/11,glyphImage.Height*10/11),new SKRect(0,0,glyphImage.Width*9/11,glyphImage.Height*9/11));


        var indexedBits = Enumerable.Range(0,glyphImage.Height)
        .Select(r => Enumerable.Range(0,glyphImage.Width)
        .Select(c => (glyphImage.GetPixel(c,r).Red & 1) == 0))
        .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (RowIndex:b,Row:
        a.Zip(Enumerable.Range(0,int.MaxValue),(c,d) => (ColumnIndex:d,Column:c))))
        .SelectMany(r => r.Row
        .Select(c => (RowIndex:r.RowIndex,ColumnIndex:c.ColumnIndex,Bit:c.Column)));
        var minSize = 9;
        var coefficent = glyphImage.Height/minSize;
        var CoordsRaw = new [] { (X:6,Y:0),
        (X:8,Y:2),
        (X:8,Y:6),
        (X:6,Y:8),
        (X:2,Y:8),
        (X:0,Y:6),
        (X:0,Y:2),
        (X:2,Y:0),
        (X:4,Y:2),
        (X:6,Y:4),
        (X:4,Y:6),
        (X:2,Y:4)}
        .Select(s => (X:s.X*coefficent+(coefficent/2),Y:s.Y*coefficent+(coefficent/2)))
        .Select(c => indexedBits.Where(b => b.ColumnIndex==c.X && b.RowIndex == c.Y).First())
        .Select(c =>  c.Bit)
        .Select(c => c ? 1 : 0)
        .Aggregate(0,(p,c) => p*2+c);
        return Graphic.Graphic.GetGraphicWithNumber(CoordsRaw);
        
    }
}
