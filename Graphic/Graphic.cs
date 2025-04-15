using SkiaSharp;

namespace Graphic;
class Graphic
{
    private global::Glyph.Glyph Glyph {get;}
    private static Graphic[]? _AllGraphics;

    public static int NumberOfGlyphs => AllGraphics.Length;
    public static Graphic GetGraphicAtIndex(int index) => AllGraphics[index];
    public static Graphic GetLetterGraphicWithNumber(int number) =>  AllGraphics.Where(s => s.Glyph.Number == number && s.Glyph.IsLetterSymbol).First();
    public static Graphic GetNumberGraphicWithNumber(int number) =>  AllGraphics.Where(s => s.Glyph.Number == number && !s.Glyph.IsLetterSymbol).First();
    
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
        

        return original; 
    }
    public SKBitmap ColorizeBitmap(string hex,bool useSmall=false)
    {
        return ColorizeBitmap(SKColor.Parse(hex),useSmall);
    }

   
}
