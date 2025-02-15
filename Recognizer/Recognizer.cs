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
    // public IRecognizedOrientation DetermineGlyphOrientation(SKBitmap glyphImage)
    // {
    //     lock (orientationCheckLock)
    //     {

        
    //         if (glyphImage != lastCheckedImage || lastDeterminedOrientation == null)            
    //         {
    //             if (glyphImage.Height != glyphImage.Width) throw new Exception("Ketameri Glyphs are square. This is not!");
    //             SKBitmap mini = new SKBitmap(11,11);
    //             glyphImage.ScalePixels(mini,SKSamplingOptions.Default);
    //             (int top,int bottom,int right,int left) = Enumerable.Range(0,4)
    //             .Select(s => 
    //             Enumerable.Range(0,11)
    //             .Select(c => {
    //                 if (s == 0)
    //                 {
    //                     if ((mini.GetPixel(c,0).Red & 1) == 1)
    //                     {
    //                         return 1;
    //                     }
    //                     else
    //                     {
    //                         return 0;
    //                     }
    //                 }
    //                 else if (s == 1) {
    //                     if ((mini.GetPixel(c,10).Red & 1) == 1)
    //                     {
    //                         return 1;
    //                     }
    //                     else
    //                     {
    //                         return 0;
    //                     }
    //                 }
    //                 else if (s == 2) {
    //                     if ((mini.GetPixel(10,c).Red & 1) == 1)
    //                     {
    //                         return 1;
    //                     }
    //                     else
    //                     {
    //                         return 0;
    //                     }
    //                 }
    //                 else if (s == 3) {
    //                     if ((mini.GetPixel(0,c).Red & 1) == 1)
    //                     {
    //                         return 1;
    //                     }
    //                     else
    //                     {
    //                         return 0;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     return 0;
    //                 }
    //             }).Sum()).ToArray();
    //             (var vflip,var hflip,var rotate) = (false,false,false);
    //             if (top == 2 || top == 3)
    //             {
    //                 rotate = true;
    //                 var temp = 0;
    //                 left = top;
    //                 top = right;
    //                 right = bottom;
    //                 bottom = temp;
                    
    //             }
    //             if (top == 1) {
    //                 vflip = true;
    //             }
    //             if (left == 2)
    //             {
    //                 hflip = true;
    //             }
    //             lastCheckedImage = glyphImage;
    //             lastDeterminedOrientation = new RecognizedOrientation(vflip,hflip,rotate);
                

    //         }
    //     }
    //     return lastDeterminedOrientation;
        
    // }
    public Graphic.Graphic Recognize(SKBitmap glyphImage)
    {
        Graphic.Graphic? recognizedGraphic = null;
        
            if (glyphImage.Height != glyphImage.Width) throw new Exception("Ketameri Glyphs are square. This is not!");
            // var ori = DetermineGlyphOrientation(glyphImage);
            // if (ori.IsHorizontallyFlipped || ori.IsVerticallyFlipped || ori.IsRotated) throw new Exception("Out of proper orientation. Please use DetermineGlyphOrientation(...) and orient the root graphic properly first.");
            
            // var borderless = new SKBitmap(glyphImage.Width*9/11,glyphImage.Height*9/11); // 54 is 9/11 of 66. Never forget. 
            // var c = new SKCanvas(borderless);
            // c.DrawBitmap(glyphImage,new SKRect(glyphImage.Width/11,glyphImage.Height/11,glyphImage.Width*10/11,glyphImage.Height*10/11),new SKRect(0,0,glyphImage.Width*9/11,glyphImage.Height*9/11));
            var borderless = glyphImage;
            var borderless_mini = new SKBitmap(9,9);
            borderless.ScalePixels(borderless_mini,SKSamplingOptions.Default);
            var is_number = (borderless_mini.GetPixel(1,1).Red & 1) == 0;
            if (is_number)
            {
                var num = 0;
                var ops = Enumerable.Range(0,3)
                .Select(s => s *3)
                .Select(r => Enumerable.Range(0,3)
                .Select(s => s * 3)
                .Select(c => (borderless_mini.GetPixel(c,r).Red & 1) == 0))
                .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (RowIndex:b,Row:a
                .Zip(Enumerable.Range(0,int.MaxValue),(c,d) => (ColumnIndex:d,Item:c))))
                .SelectMany(r => r.Row.Select(c => (r.RowIndex,c.ColumnIndex,c.Item)))
                .GroupBy(s => s.ColumnIndex)
                .Select(tr => tr.OrderBy(c => c.RowIndex))
                .Select(r => r
                .Select(c=> c.Item)
                .Where(c=> c)
                .Count())
                .Select(cnt => {
                    num *= 3;
                    if (cnt == 1)
                    {
                        num -= 1;
                    }
                    else if (cnt == 2)
                    {
                        num += 1;
                    }
                    return 1;
                }).Sum();
                recognizedGraphic = Graphic.Graphic.GetNumberGraphicWithNumber(num);               
                

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
                recognizedGraphic = Graphic.Graphic.GetLetterGraphicWithNumber(CoordsRaw);
            }
            
        
        if (recognizedGraphic == null) throw new Exception("Couldn't recognize graphic");
        return recognizedGraphic;
        
    }
}
