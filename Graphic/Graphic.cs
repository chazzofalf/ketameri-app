using System.Security.Cryptography.X509Certificates;
using SkiaSharp;

namespace Graphic;

public class CachedGraphic
{
    public string Letter {get;}
    public bool IsNumber {get;}
    public bool IsSpecial {get;}
    public bool IsText {get;}
    public int Number {get;}
    public bool IsLetterSymbol {get;}
    private byte[] SmallPNGData {get;}
    private byte[] LargePNGData {get;}
    
    private bool[][] Matrix {get;}
    public bool ReadBitMap(int row,int column)
    {
        return Matrix[row][column];
    }
    private SKBitmap? _SmallTemplate;
    private SKBitmap SmallTemplate => _SmallTemplate = _SmallTemplate ?? SKBitmap.Decode(SmallPNGData);
    public SKBitmap Small => SmallTemplate.Copy();
    private SKBitmap? _LargeTemplate;
    private SKBitmap LargeTemplate => _LargeTemplate = _LargeTemplate ?? SKBitmap.Decode(LargePNGData);
    public SKBitmap Large => LargeTemplate.Copy();
    private static CachedGraphic[]? _All = null;
    private static CachedGraphic[] All => _All = _All ?? Generate();
    public static int NumberOfGlyphs => All.Length;
    public static CachedGraphic  GetGraphicAtIndex(int index) => All[index];
    private static CachedGraphic[]? GenerateFromJSONCache()
    {
        return null;
    }
    private static CachedGraphic[] Regenerate()
    =>
        Enumerable.Range(0,Graphic.NumberOfGlyphs)
        .Select(s => Graphic.GetGraphicAtIndex(s))
        .Select(s => new CachedGraphic(s))
        .ToArray();
    
    private static CachedGraphic[] Generate()
    {
        return GenerateFromJSONCache() ?? Regenerate();
    }
    private CachedGraphic(Graphic original)
    {
        Letter = original.Letter;
        IsNumber = original.IsNumber;
        IsSpecial = original.IsSpecial;
        IsText = original.IsText;
        Number = original.Number;
        IsLetterSymbol = original.IsLetterSymbol;
        SmallPNGData = original.Small.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray();
        LargePNGData = original.Large.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray();
        Matrix = Enumerable.Range(0,9)
        .Select(r => Enumerable.Range(0,9)
        .Select(c => original.ReadBitMap(r,c)).ToArray()).ToArray();
    }
    public SKBitmap ColorizeBitmap(SKColor? color=null,bool useSmall=false)
    {
        var real_color = color is SKColor colorx ? colorx : SKColors.Turquoise;
        real_color = new SKColor((byte)(real_color.Red & 0xfe),real_color.Green,real_color.Blue,real_color.Alpha);
        var original = useSmall ? Small : Large;
        using (var canvas = new SKCanvas(original))
        {
            float[] invertColorMatrix = {
                -1,  0,  0,  0, 255,
                 0, -1,  0,  0, 255,
                 0,  0, -1,  0, 255,
                 0,  0,  0,  1,   0,
            };
            float[] turquoiseColorMatrix = {
               ((float)real_color.Red)/(255.0f), 0,     0,     0, 0,  // R 
                0,     ((float)real_color.Green)/(255.0f), 0,     0, 0,  // G
                0,     0,     ((float)real_color.Blue)/(255.0f), 0, 0,  // B
                0,     0,     0,     ((float)real_color.Alpha)/(255.0f), 0   // A

            };
            using (var colorFilter = SKColorFilter.CreateColorMatrix(invertColorMatrix))
            {
                using (var turquoiseFilter = SKColorFilter.CreateColorMatrix(turquoiseColorMatrix))
                {
                    using (var paint = new SKPaint())
                    {
                        paint.ColorFilter = colorFilter;
                        canvas.DrawBitmap(original, 0, 0, paint);
                        canvas.Flush();
                        paint.ColorFilter = turquoiseFilter;
                        canvas.DrawBitmap(original,0,0,paint);
                    }
                }
                
            }
        }
        

        return original; // TODO: You're here!
    }
    public SKBitmap ColorizeBitmap(string hex,bool useSmall=false)
    {
        return ColorizeBitmap(SKColor.Parse(hex),useSmall);
    }

    public static CachedGraphic GetLetterGraphicWithNumber(int number) =>  All.Where(s => s.Number == number && s.IsLetterSymbol).First(); //Glyphs.Where(s => s.Number == number).First();
    public static CachedGraphic GetNumberGraphicWithNumber(int number) =>  All.Where(s => s.Number == number && !s.IsLetterSymbol).First(); //Glyphs.Where(s => s.Number == number).First();
    
    public static CachedGraphic GetGraphicWithLetter(string letter) => All.Where(s=> s.Letter == letter).First();
   
    public static CachedGraphic GetGraphicForSpecial() => All.Where(s => s.IsSpecial).First();

}
class Graphic
{
    private global::Glyph.Glyph Glyph {get;}
    private static Graphic[]? _AllGraphics;

    public static int NumberOfGlyphs => AllGraphics.Length;
    public static Graphic GetGraphicAtIndex(int index) => AllGraphics[index];
    public static Graphic GetLetterGraphicWithNumber(int number) =>  AllGraphics.Where(s => s.Glyph.Number == number && s.Glyph.IsLetterSymbol).First(); //Glyphs.Where(s => s.Number == number).First();
    public static Graphic GetNumberGraphicWithNumber(int number) =>  AllGraphics.Where(s => s.Glyph.Number == number && !s.Glyph.IsLetterSymbol).First(); //Glyphs.Where(s => s.Number == number).First();
    
    public static Graphic GetGraphicWithLetter(string letter) => AllGraphics.Where(s=> s.Glyph.Letter == letter).First();
   
    public static Graphic GetGraphicForSpecial() => AllGraphics.Where(s => s.Glyph.IsSpecial).First();

    public string Letter => Glyph.Letter;
    public bool IsNumber => Glyph.IsNumber;
    public bool IsSpecial => Glyph.IsSpecial;
    public bool IsText => Glyph.IsText;
    public int Number => Glyph.Number;
    public bool IsLetterSymbol => Glyph.IsLetterSymbol;
    public bool ReadBitMap(int row,int column)
    {
        return Glyph.ReadBitMap(row,column);
    }


    
    
    

    

    private static Graphic[] AllGraphics => _AllGraphics = _AllGraphics ?? 
    Enumerable.Range(0,global::Glyph.Glyph.NumberOfGlyphs)
    .Select(s => new Graphic(global::Glyph.Glyph.GetGlyphAtIndex(s)))
    .ToArray();
    private SKBitmap _small ;
    private SKBitmap _large;
    public SKBitmap Small 
    {
        get
        {
            return _small.Copy();
        }
    }
    public SKBitmap Large 
    {
        get
        {
            return _large.Copy();
        }
    }
    

    public Graphic(global::Glyph.Glyph glyph)
    {
        Glyph = glyph;
        _small = GenerateSmallBitmap();
        _large = GenerateLargeBitmap();
    }

    private SKBitmap GenerateLargeBitmap()
    {
        var orig_small = Small;
        
        var bmp = new SKBitmap(54,54);
        var opts = new SKSamplingOptions();
        
        orig_small.ScalePixels(bmp,opts);
        
        return bmp;
    }

    private SKBitmap GenerateSmallBitmap()
    {       
        // if (IsLetterSymbol)
        // {
        //     var bmps = new SKBitmap(5,5);
        //     var _ = Enumerable.Range(0,5)
        //     .Select(r => Enumerable.Range(0,5)
        //     .Select(c => 
        //     {
        //         if (ReadBitMap(r,c))
        //         {
        //             bmps.SetPixel(c,r,SKColors.Black);
                    
        //         }
        //         return 1;
        //     }).Sum()).Sum();
        //     var vs_left_orig = new SKBitmap(1,5);
        //     var vs_mid_left_orig = new SKBitmap(1,5);
        //     var vs_mid_orig = new SKBitmap(1,5);
        //     var vs_mid_right_orig = new SKBitmap(1,5);
        //     var vs_right_orig = new SKBitmap(1,5);
        //     var c = new SKCanvas(vs_left_orig);
        //     c.DrawBitmap(bmps,new SKRect(0,0,1,5),new SKRect(0,0,1,5));
        //     c = new SKCanvas(vs_mid_left_orig);
        //     c.DrawBitmap(bmps,new SKRect(1,0,2,5),new SKRect(0,0,1,5));
        //     c = new SKCanvas(vs_mid_orig);
        //     c.DrawBitmap(bmps,new SKRect(2,0,3,5),new SKRect(0,0,1,5));
        //     c = new SKCanvas(vs_mid_right_orig);
        //     c.DrawBitmap(bmps,new SKRect(3,0,4,5),new SKRect(0,0,1,5));
        //     c = new SKCanvas(vs_right_orig);
        //     c.DrawBitmap(bmps,new SKRect(4,0,5,5),new SKRect(0,0,1,5));
            
        
        //     var vs_mid_left = new SKBitmap(3,5);
        //     var vs_mid_right = new SKBitmap(3,5);
        //     vs_mid_left_orig.ScalePixels(vs_mid_left,SKSamplingOptions.Default);
        //     vs_mid_right_orig.ScalePixels(vs_mid_right,SKSamplingOptions.Default);
        //     var stretched_horz = new SKBitmap(9,5);
        //     c = new SKCanvas(stretched_horz);
        //     c.DrawBitmap(vs_left_orig,0,0);
        //     c.DrawBitmap(vs_mid_left,1,0);
        //     c.DrawBitmap(vs_mid_orig,4,0);
        //     c.DrawBitmap(vs_mid_right,5,0);
        //     c.DrawBitmap(vs_right_orig,8,0);
        //     var hs_top_orig = new SKBitmap(9,1);
        //     var hs_mid_top_orig = new SKBitmap(9,1);
        //     var hs_mid_orig = new SKBitmap(9,1);
        //     var hs_mid_bottom_orig = new SKBitmap(9,1);
        //     var hs_bottom_orig = new SKBitmap(9,1);
        //     c = new SKCanvas(hs_top_orig);
        //     c.DrawBitmap(stretched_horz,new SKRect(0,0,9,1),new SKRect(0,0,9,1));
        //     c = new SKCanvas(hs_mid_top_orig);
        //     c.DrawBitmap(stretched_horz,new SKRect(0,1,9,2),new SKRect(0,0,9,1));
        //     c = new SKCanvas(hs_mid_orig);
        //     c.DrawBitmap(stretched_horz,new SKRect(0,2,9,3),new SKRect(0,0,9,1));
        //     c = new SKCanvas(hs_mid_bottom_orig);
        //     c.DrawBitmap(stretched_horz,new SKRect(0,3,9,4),new SKRect(0,0,9,1));
        //     c = new SKCanvas(hs_bottom_orig);
        //     c.DrawBitmap(stretched_horz,new SKRect(0,4,9,5),new SKRect(0,0,9,1));
        //     var hs_mid_top_scaled = new SKBitmap(9,3);
        //     var hs_mid_bottom_scaled = new SKBitmap(9,3);
        //     hs_mid_top_orig.ScalePixels(hs_mid_top_scaled,SKSamplingOptions.Default);
        //     hs_mid_bottom_orig.ScalePixels(hs_mid_bottom_scaled,SKSamplingOptions.Default);
        //     var o = new SKBitmap(9,9);
        //     c = new SKCanvas(o);
        //     c.DrawBitmap(hs_top_orig,0,0);
        //     c.DrawBitmap(hs_mid_top_scaled,0,1);
        //     c.DrawBitmap(hs_mid_orig,0,4);
        //     c.DrawBitmap(hs_mid_bottom_scaled,0,5);
        //     c.DrawBitmap(hs_bottom_orig,0,8);
        //     return o;


            
        // } 
        // else
        // {
            var bmpx = new SKBitmap(9,9);
            var canx = new SKCanvas(bmpx);
            var blackx = new SKPaint();
            blackx.Color = SKColors.Black;        
            canx.Clear();
            var opts = Enumerable.Range(0,9)
            .Select(r => Enumerable.Range(0,9)
            .Select(c => {
                if (ReadBitMap(r,c))
                {
                    bmpx.SetPixel(c,r,SKColors.Black);
                }
                return 1;
            }).Sum()).Sum();
            return bmpx;
        // }
        

        
    }
    public SKBitmap ColorizeBitmap(SKColor? color=null,bool useSmall=false)
    {
        var real_color = color is SKColor colorx ? colorx : SKColors.Turquoise;
        real_color = new SKColor((byte)(real_color.Red & 0xfe),real_color.Green,real_color.Blue,real_color.Alpha);
        var original = useSmall ? Small : Large;
        using (var canvas = new SKCanvas(original))
        {
            float[] invertColorMatrix = {
                -1,  0,  0,  0, 255,
                 0, -1,  0,  0, 255,
                 0,  0, -1,  0, 255,
                 0,  0,  0,  1,   0,
            };
            float[] turquoiseColorMatrix = {
               ((float)real_color.Red)/(255.0f), 0,     0,     0, 0,  // R 
                0,     ((float)real_color.Green)/(255.0f), 0,     0, 0,  // G
                0,     0,     ((float)real_color.Blue)/(255.0f), 0, 0,  // B
                0,     0,     0,     ((float)real_color.Alpha)/(255.0f), 0   // A

            };
            using (var colorFilter = SKColorFilter.CreateColorMatrix(invertColorMatrix))
            {
                using (var turquoiseFilter = SKColorFilter.CreateColorMatrix(turquoiseColorMatrix))
                {
                    using (var paint = new SKPaint())
                    {
                        paint.ColorFilter = colorFilter;
                        canvas.DrawBitmap(original, 0, 0, paint);
                        canvas.Flush();
                        paint.ColorFilter = turquoiseFilter;
                        canvas.DrawBitmap(original,0,0,paint);
                    }
                }
                
            }
        }
        

        return original; // TODO: You're here!
    }
    public SKBitmap ColorizeBitmap(string hex,bool useSmall=false)
    {
        return ColorizeBitmap(SKColor.Parse(hex),useSmall);
    }

   
}
