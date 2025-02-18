using System.IO.Compression;
using System.Text;
using SkiaSharp;

namespace Graphic;


public class CachedGraphic
{
    private class DataClass
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string Letter {get;set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public bool IsNumber {get;set;}
        public bool IsSpecial {get;set;}
        public bool IsText {get;set;}
        public int Number {get;set;}
        public bool IsLetterSymbol {get;set;}
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public byte[] SmallPNGData {get;set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public byte[] LargePNGData {get;set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public bool[][] Matrix {get;set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
    public string Letter => Data.Letter;
    public bool IsNumber => Data.IsNumber;
    public bool IsSpecial => Data.IsSpecial;
    public bool IsText => Data.IsText;
    public int Number => Data.Number;
    public bool IsLetterSymbol => Data.IsLetterSymbol;
    
    
    
    public bool ReadBitMap(int row,int column)
    {
        return Data.Matrix[row][column];
    }

    private SKBitmap? _SmallTemplate;
    private SKBitmap SmallTemplate => _SmallTemplate = _SmallTemplate ?? SKBitmap.Decode(Data.SmallPNGData);

    public SKBitmap Small => SmallTemplate.Copy();
    private SKBitmap? _LargeTemplate;
    private SKBitmap LargeTemplate => _LargeTemplate = _LargeTemplate ?? SKBitmap.Decode(Data.LargePNGData);
    public SKBitmap Large => LargeTemplate.Copy();
    private static CachedGraphic[]? _All = null;
    private static CachedGraphic[] All => _All = _All ?? Generate();
    public static int NumberOfGlyphs => All.Length;
    public static CachedGraphic  GetGraphicAtIndex(int index) => All[index];
    private static CachedGraphic[]? GenerateFromJSONCache()
    {
        //return null;
        var outx = (DataClass[]?)null;
        try
        {
            using (var fio = Resources.Resources.GlyphResource)
            {
                using (var gzio = new GZipStream(fio,CompressionMode.Decompress))
                {
                    using (var txtio = new StreamReader(gzio,Encoding.UTF8))
                    {
                        var str = txtio.ReadToEnd();
                        outx = System.Text.Json.JsonSerializer.Deserialize<DataClass[]>(str);
                    }
                }
            }
        }
        catch (Exception)
        {

        }
        return outx != null ? outx.Select(s => new CachedGraphic(s)).ToArray() : null;
        
    }
    private static CachedGraphic[]  SaveToCache(CachedGraphic[] toCache)
    {
        if (!Directory.Exists("Cache"))
        {
            Directory.CreateDirectory("Cache");
        }
        //if (!File.Exists("Cache/Glyphs.json.gz"))

        
            using (var fio = File.OpenWrite("Cache/Glyphs.json.gz"))
            {
                using (var gzio = new GZipStream(fio,CompressionLevel.Optimal))
                {
                    using (var txtio = new StreamWriter(gzio,System.Text.Encoding.UTF8))
                    {
                        txtio.Write(System.Text.Json.JsonSerializer.Serialize(toCache.Select(s => s.Data).ToArray()));
                    }
                }
            }
            
            
        
        return toCache;
    }
    private static CachedGraphic[] Regenerate()
    =>
        SaveToCache(Enumerable.Range(0,Graphic.NumberOfGlyphs)
        .Select(s => Graphic.GetGraphicAtIndex(s))
        .Select(s => new CachedGraphic(s))
        .ToArray());
    
    private static CachedGraphic[] Generate()
    {
        return GenerateFromJSONCache() ?? Regenerate();
    }
    private DataClass Data {get;}
    private CachedGraphic(DataClass data)
    {
        Data = data;
    }
    private CachedGraphic(Graphic original)
    {
        Data = new DataClass() {
            Letter = original.Letter,
            IsNumber = original.IsNumber,
            IsSpecial = original.IsSpecial,
            IsText = original.IsText,
            Number = original.Number,
            IsLetterSymbol = original.IsLetterSymbol,
            SmallPNGData = original.Small.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray(),
            LargePNGData = original.Large.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray(),
            Matrix = Enumerable.Range(0,9)
            .Select(r => Enumerable.Range(0,9)
            .Select(c => original.ReadBitMap(r,c)).ToArray()).ToArray()
        };
        
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
