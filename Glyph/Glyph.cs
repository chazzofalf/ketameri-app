namespace Glyph;

public class Glyph
{
    private static string[]? _Numbers;
    
    private static string[]? _Space;
    private static string[]? _LowerVowels;
    private static string[]? _UpperVowels;
    private static string[]? _LowerConsonants;
    private static string[]? _UpperConsonants;
    private static string[]? _LowerVowelConsonants;
    private static string[]? _UpperVowelConsonants;
    private static string[]? _LowerConsonantVowels;
    private static string[]? _UpperConsonantVowels;
    private static string[]? _Punctuation;
    private static string[]? _Alphabet;
    private static Glyph[]? _Glyphs;
    private Glyph[]? _AllOrientations;

    private static string[] Numbers => _Numbers = _Numbers ?? Enumerable.Range(0,14)
    .Select(s => $"{s}")
    .ToArray();
    private static string[] Space => _Space = _Space ?? Enumerable.Repeat(" ",1)
    .ToArray();
    private static string[] LowerVowels => _LowerVowels = _LowerVowels ??  Enumerable.Repeat("a",1)
    .Append("e")
    .Append("i")
    .Append("o")
    .Append("u")
    .Append("y")
    .ToArray();
    private static string[] UpperVowels => _UpperVowels = _UpperVowels ?? LowerVowels
    .Select(s => s.ToUpperInvariant())
    .ToArray();
    private static string[] LowerConsonants => _LowerConsonants = _LowerConsonants ?? Enumerable.Range(0,char.MaxValue)
    .Select(ch => (char)ch)
    .Select(ch => $"{ch}")
    .Where(ch => char.IsAscii(ch.First()))
    .Where(ch => char.IsLetter(ch.First()))
    .Where(ch => char.IsLower(ch.First()))
    .Where(ch => !LowerVowels.Contains(ch))
    .ToArray();
    private static string[] UpperConsonants => _UpperConsonants = _UpperConsonants ?? LowerConsonants
    .Select(s => s.ToUpper())
    .ToArray();
    private static string[] LowerVowelConsonants => _LowerVowelConsonants = _LowerVowelConsonants ?? LowerVowels
    .SelectMany(v => LowerConsonants
    .Select(c => $"{v}{c}"))
    .ToArray();
    private static string[] UpperVowelConsonants => _UpperVowelConsonants = _UpperConsonants ?? LowerVowelConsonants
    .Select(s => $"{char.ToUpperInvariant(s[0])}{s[1]}")
    .ToArray();
    private static string[] LowerConsonantVowels => _LowerConsonantVowels = _LowerConsonantVowels ?? LowerConsonants
    .SelectMany(c => LowerVowels
    .Select(v => $"{c}{v}"))
    .ToArray();    
    private static string[] UpperConsonantVowels => _UpperConsonantVowels = _UpperConsonantVowels ?? LowerConsonantVowels
    .Select(s => ($"{char.ToUpperInvariant(s[0])}{s[1]}"))
    .ToArray();
    private static string[] Punctuation => _Punctuation = _Punctuation ?? ".!?,'\":;()=<>+-/*^[]{}"
    .Select(s => $"{s}")
    .ToArray();
    private static string[] Alphabet => _Alphabet = _Alphabet ?? 
    Space
    .Concat(Numbers)
    .Concat(UpperVowels)
    .Concat(UpperConsonants)
    .Concat(UpperVowelConsonants)
    .Concat(UpperConsonantVowels)
    .Concat(LowerVowels)
    .Concat(LowerConsonants)
    .Concat(LowerVowelConsonants)
    .Concat(LowerConsonantVowels)
    .Concat(Punctuation)
    .ToArray();
    private static Glyph[] Glyphs => _Glyphs = _Glyphs = Alphabet.Zip(global::Symbol.Symbol.All,(txt,sym) => new Glyph(txt,sym))
    .ToArray();
    public string Letter {get;}
    public Symbol.Symbol Symbol {get;}
    public Glyph(string letter,Symbol.Symbol symbol)
    {
        Letter = letter;
        Symbol = symbol;   
    }
    public  bool Similar(object? obj) 
    {
        return Symbol.Similar(obj);
    }
    public Glyph[] AllOrientations => _AllOrientations = _AllOrientations ??
        Symbol.AllOrientations
        .Select(s=> new Glyph(Letter,s))
        .ToArray();
}
