using SkiaSharp;

namespace Recognizer;
using System.Linq;


public static class Extensions
{
    public static void Deconstruct<T>(this T[] srcArray, out T a0, out T a1, out T a2, out T a3) {
        if (srcArray == null || srcArray.Length < 4)
            throw new ArgumentException(nameof(srcArray));

        a0 = srcArray[0];
        a1 = srcArray[1];
        a2 = srcArray[2];
        a3 = srcArray[3];
    }
}
public class Recognizer
{    
    public Graphic.CachedGraphic Recognize(SKBitmap glyphImage)
    {
        Graphic.CachedGraphic? recognizedGraphic = null;
        
            if (glyphImage.Height != glyphImage.Width) throw new Exception("Ketameri Glyphs are square. This is not!");            
            var borderless = glyphImage;
            var borderless_mini = new SKBitmap(9,9);
            borderless.ScalePixels(borderless_mini,SKSamplingOptions.Default);
            var is_number = (borderless_mini.GetPixel(2,1).Red & 1) == 0;
            if (is_number)
            {
                var num = 0;
                var ops = Enumerable.Range(0,3)
                .Select(s => 2 + 2*s)
                .Select(c => Enumerable.Range(0,2)
                .Select(s => s * 5)
                .Select(r => (borderless_mini.GetPixel(c,r).Red & 1) == 0).ToArray())                
                .Select(cnt => {
                    num *= 3;
                    if (cnt[0] != cnt[1])
                    {
                        if (cnt[0])
                        {
                            num += 1;
                        }
                        else
                        {
                            num -= 1;
                        }
                    }
                    
                    return 1;
                }).Sum();
                recognizedGraphic = Graphic.CachedGraphic.GetNumberGraphicWithNumber(num);               
                

            }
            else
            {
                var indexedBits = Enumerable.Range(0,borderless_mini.Height)
                .Select(r => Enumerable.Range(0,borderless_mini.Width)
                .Select(c => (borderless_mini.GetPixel(c,r).Red & 1) == 0))
                .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (RowIndex:b,Row:
                a.Zip(Enumerable.Range(0,int.MaxValue),(c,d) => (ColumnIndex:d,Column:c))))
                .SelectMany(r => r.Row
                .Select(c => (RowIndex:r.RowIndex,ColumnIndex:c.ColumnIndex,Bit:c.Column)));
                
                
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
                .Select(s => (X:s.X,Y:s.Y))
                .Select(c => indexedBits.Where(b => b.ColumnIndex==c.X && b.RowIndex == c.Y).First())
                .Select(c =>  c.Bit)
                .Select(c => c ? 1 : 0)
                .Aggregate(0,(p,c) => p*2+c);
                recognizedGraphic = Graphic.CachedGraphic.GetLetterGraphicWithNumber(CoordsRaw);
            }
            
        
        if (recognizedGraphic == null) throw new Exception("Couldn't recognize graphic");
        return recognizedGraphic;
        
    }
}
