using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokenizer;

public class Tokenizer
{
    private char[]? _SupportedCharacters = null;
    private char[] SupportedCharacters => _SupportedCharacters = _SupportedCharacters ?? 
    Enumerable.Range(0,Graphic.Graphic.NumberOfGlyphs)
    .Select(n => Graphic.Graphic.GetGraphicAtIndex(n))
    .SelectMany(gr => gr.Letter)
    .OrderBy(ch => (int)ch)
    .Distinct()
    .ToArray();

    
    private char[]? _Ascii = null;
    private char[] Ascii => _Ascii = _Ascii ?? 
    SupportedCharacters
    .Where(char.IsAscii)
    .ToArray();
    private char[]? _Digits = null;
    private char[] Digits => _Digits = _Digits ??
    Ascii
    .Where(char.IsDigit)
    .ToArray();
    private char[]? _Letters = null;
    private char[] Letters => _Letters = _Letters ??
    Ascii
    .Where(char.IsLetter)
    .ToArray();
    private char[]? _Capitals = null;
    private char[] Capitals => _Capitals = _Capitals ??
    Letters
    .Where(char.IsUpper)
    .ToArray();
    private char[]? _Lowers = null;
    private char[] Lowers => _Lowers = _Lowers ??
    Letters
    .Where(char.IsLower)
    .ToArray();
    private char[]? _Vowels = null;
    private char[] Vowels => _Vowels = _Vowels ?? "aeiouy".ToArray();
    private char[]? _UpperVowels = null;
    private char[] UpperVowels => _UpperVowels = _UpperConsonants ?? Vowels.Select(char.ToUpper).ToArray();
    private char[]? _Consonants = null;
    private char[] Consonants => _Consonants = _Consonants ?? 
    Lowers
    .Where(ch => !Vowels.Contains(ch))
    .ToArray();
    private char[]? _UpperConsonants = null;
    private char[] UpperConsonants => _UpperConsonants = _UpperConsonants ?? Consonants
    .Select(ch => char.ToUpperInvariant(ch))
    .ToArray();
    private char[]? _Space = null;
    private char[] Space => _Space = _Space ?? " ".ToArray();

    private bool IsSupportedCharacter(char c) => SupportedCharacters.Contains(c);
    private bool IsCapital(char c) => Capitals.Contains(c);
    private bool IsNumber(char c) => Capitals.Contains(c);    
    private bool IsSpace(char c) => Space.Contains(c);
    private bool IsVowel(char c) => Vowels.Contains(c) || UpperVowels.Contains(c);
    private List<char> buffer = new List<char>();
    private bool _IsNumberMode = false;
    private Graphic.Graphic[] ConvertNumberInBuffer()
    {
        var outx = ConvertNumberInObject(buffer);
        buffer.Clear();
        return outx;
    }
    private Graphic.Graphic[] ConvertNumberInObject(IEnumerable<char> english)
    {
        if (english == null || english.Count() == 0)
        {
            return Enumerable.Empty<Graphic.Graphic>().ToArray();
        }
        else
        {
            var arabicNumber /* The 0123456789 are arabic numerals. look it up! (Start here: https://www.ahdictionary.com/word/search.html?id=A5414100 if you are truly curious or need your spirit's misplaced upset quelled.) */
            = BigInteger.Parse(string.Join("",buffer));
            
            var ketameriNumber /* Our ancient Ketameri brothers used a base-14 system with lucky number 13 [They loved magic and superstition way too much!] (3*(3*1))+1)+1) (sometimes style-listically (''' / sky sky sky)). The Favored One's two favorite numbers are ['|][''|] (sky water towards sky sky water) and ['][',,]['|,] (sky water towards sky earth earth towards sky water earth) or 54 and 666. A note on the second favorite number last five bits of the binary representation of that number are the reverse inverse of the first five, this a number of conflict. And man (Homo Sapiens, human beings, male and female) are creatures of absolute conflict (we cannot live with out causing ourselves and each other conflict trouble!) Therefore 666 is the number of man! Solution of numerical riddle the beast of the sea and the beast of the land solved! Now as to what name of what unlucky person that conflicting number represents?...*/
            = Enumerable.Empty<BigInteger>();
            while (arabicNumber > 0)
            {
                var modulus = arabicNumber % 14;
                ketameriNumber = ketameriNumber.Append(modulus); 
                arabicNumber -= modulus;
                arabicNumber /= 14;                
            }
            ketameriNumber = ketameriNumber.Reverse();
            var x =ketameriNumber.Select(s => $"{s}")
            .Select(Graphic.Graphic.GetGraphicWithLetter)
            .ToArray();
            return x;
        }
    }
}
