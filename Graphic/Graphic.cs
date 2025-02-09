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


    public Graphic(global::Glyph.Glyph glyph)
    {
        Glyph = glyph;
    }
}
