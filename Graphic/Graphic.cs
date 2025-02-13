using System.Security.Cryptography.X509Certificates;
using SkiaSharp;

namespace Graphic;

public class Graphic
{
    private global::Glyph.Glyph Glyph {get;}
    private static Graphic[]? _AllGraphics;

    public static int NumberOfGlyphs => AllGraphics.Length;
    public static Graphic GetGraphicAtIndex(int index) => AllGraphics[index];
    public static Graphic GetGraphicWithNumber(int number) =>  AllGraphics.Where(s => s.Glyph.Number == number).First(); //Glyphs.Where(s => s.Number == number).First();
    public static bool HasGraphicWithNumber(int number) => AllGraphics.Where(s => s.Glyph.Number == number).Any();
    public static Graphic GetGraphicWithLetter(string letter) => AllGraphics.Where(s=> s.Glyph.Letter == letter).First();
    public static bool HasGraphicWithLetter(string letter) => AllGraphics.Where(s=> s.Glyph.Letter == letter).Any();
    public static Graphic GetGraphicForSpecial() => AllGraphics.Where(s => s.Glyph.IsSpecial).First();

    public string Letter => Glyph.Letter;
    public bool IsNumber => Glyph.IsNumber;
    public bool IsSpecial => Glyph.IsSpecial;
    public bool IsText => Glyph.IsText;
    public int Number => Glyph.Number;
    public bool ReadBitMap(int row,int column)
    {
        return Glyph.ReadBitMap(row,column);
    }


    private Graphic[]? _AllOrientations;
    public int NumberOfOrientations => AllOrientations.Length;
    public Graphic OrientationAtIndex(int index) => AllOrientations[index];
    private Graphic[] AllOrientations => _AllOrientations = _AllOrientations ??
        Enumerable.Range(0,Glyph.NumberOfOrientations)
        .Select(s => Glyph.OrientationAtIndex(s))
        .Select(s => new Graphic(s))
        .ToArray();

    public bool Similar(object? obj) 
    {
        return Glyph.Similar(obj);
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
        var bmp = new SKBitmap(9,9);
        var can = new SKCanvas(bmp);
        var black = new SKPaint();
        black.Color = SKColors.Black;        
        can.Clear();
        foreach (var idx in Enumerable.Range(0,NumberOfBits))
        {
            if (GetBit(idx))
            {
                if (idx == 0)
                {
                    can.DrawLine(new SKPoint(4,0),new SKPoint(9,0),black);
                }
                else if (idx == 1)
                {
                    can.DrawLine(new SKPoint(8,0),new SKPoint(8,4),black);
                }
                else if (idx == 2)
                {
                    can.DrawLine(new SKPoint(8,4),new SKPoint(8,9),black);
                }
                else if (idx == 3)
                {
                    can.DrawLine(new SKPoint(4,8),new SKPoint(9,8),black);
                }
                else if (idx == 4)
                {
                    can.DrawLine(new SKPoint(0,8),new SKPoint(4,8),black);
                }
                else if (idx == 5)
                {
                    can.DrawLine(new SKPoint(0,4),new SKPoint(0,9),black);
                }
                else if (idx == 6)
                {
                    can.DrawLine(new SKPoint(0,0),new SKPoint(0,4),black);                    
                }
                else if (idx == 7)
                {
                    can.DrawLine(new SKPoint(0,0),new SKPoint(4,0),black);
                }
                else if (idx == 8)
                {
                    can.DrawLine(new SKPoint(4,0),new SKPoint(4,4),black);                    
                }
                else if (idx == 9)
                {
                    can.DrawLine(new SKPoint(4,4),new SKPoint(9,4),black);
                }
                else if (idx == 10)
                {
                    can.DrawLine(new SKPoint(4,4),new SKPoint(4,9),black);
                }
                else if (idx == 11)
                {
                    can.DrawLine(new SKPoint(0,4),new SKPoint(4,4),black);
                }
            }            
        }
        return bmp;
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

    public int NumberOfBits => Glyph.NumberOfBits;
    public bool GetBit(int index) => Glyph.GetBit(index);
}
