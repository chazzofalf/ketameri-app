using System.Numerics;
using System.Runtime.CompilerServices;
using Graphic;
using SkiaSharp;

namespace Tokenizer;

class ForwardTokenizer 
{
    private char[]? _SupportedCharacters = null;
    private string[]? _allGraphicLetters = null;
    private string[] AllGraphicLetters => _allGraphicLetters = _allGraphicLetters ?? Enumerable.Range(0,Graphic.CachedGraphic.NumberOfGlyphs)
    .Select(n => Graphic.CachedGraphic.GetGraphicAtIndex(n))
    .Select(g => g.Letter)
    .OrderBy(g => g)
    .OrderByDescending(g => g.Length)
    .ToArray();


    private char[] SupportedCharacters => _SupportedCharacters = _SupportedCharacters ?? 
    AllGraphicLetters
    .SelectMany(s => s)
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
    
   
    private char[]? _Vowels = null;
    private char[]? _UniversalY = null;
    private char[] Vowels => _Vowels = _Vowels ?? "aeiou".ToArray();
    private char[] UniversalY => _UniversalY = _UniversalY ?? "y".ToArray();
    private char[]? _UpperVowels = null;
    private char[]? _UpperUniversalY = null;
    private char[] UpperVowels => _UpperVowels = _UpperVowels ?? Vowels.Select(char.ToUpper).ToArray();
    private char[] UpperUniversalY => _UpperUniversalY = _UpperUniversalY ?? UniversalY.Select(char.ToUpper).ToArray();
    
    
    
    
    
    

     

    private bool IsSupportedCharacter(char c) => SupportedCharacters.Contains(c);
    private bool IsCapital(char c) => Capitals.Contains(c);
    private bool IsNumber(char c) => Digits.Contains(c);        
    private bool IsVowel(char c) => Vowels.Contains(c) || UpperVowels.Contains(c);
    private bool IsLetter(char c) => Letters.Contains(c);
    private bool IsUniversalY(char c) => UniversalY.Contains(c) || UpperUniversalY.Contains(c);
    private List<char> buffer = new List<char>();
    private List<Graphic.CachedGraphic> cbuffer = new List<Graphic.CachedGraphic>();
    
    private Graphic.CachedGraphic[] ConvertNumberInBuffer()
    {
        var outx = ConvertNumberInObject(buffer);
        buffer.Clear();
        return outx;
    }
    private Graphic.CachedGraphic[]? _SpecialCharacter = null;
    private Graphic.CachedGraphic[] SpecialCharacter => _SpecialCharacter = _SpecialCharacter = 
    
        Enumerable.Repeat(Graphic.CachedGraphic.GetGraphicForSpecial(),1).ToArray();
    
   
    
    private Graphic.CachedGraphic[] ConvertStrangeCharacter(char strange)
    {
        var strange_ord = $"{(int)(ushort)strange}".ToArray();
        return SpecialCharacter.Concat(ConvertNumberInObject(strange_ord))
        .Concat(SpecialCharacter)
        .ToArray();
    }
    private Graphic.CachedGraphic[] ConvertNumberInObject(IEnumerable<char> english)
    {
        if (english == null || english.Count() == 0)
        {
            return Enumerable.Empty<Graphic.CachedGraphic>().ToArray();
        }
        else
        {
            var arabicNumber /* The 0123456789 are arabic numerals. look it up! (Start here: https://www.ahdictionary.com/word/search.html?id=A5414100 if you are truly curious or need your spirit's misplaced upset quelled.) */
            = BigInteger.Parse(string.Join("",english));
            
            var ketameriNumber /* Our ancient Ketameri brothers used a base-14 system with lucky number 13 [They loved magic and superstition way too much!] (3*(3*1))+1)+1) (sometimes style-listically (''' / sky sky sky)). The Favored One's two favorite numbers are ['|][''|] (sky water towards sky sky water) and ['][',,]['|,] (sky water towards sky earth earth towards sky water earth) or 54 and 666. A note on the second favorite number last five bits of the binary representation of that number are the reverse inverse of the first five, this a number of conflict. And man (Homo Sapiens, human beings, male and female) are creatures of absolute conflict (we cannot live with out causing ourselves and each other conflict trouble!) Therefore 666 is the number of man! Solution of numerical riddle the beast of the sea and the beast of the land solved! Now as to what name of what unlucky person that conflicting number represents?...*/
            = Enumerable.Empty<BigInteger>();
            if (arabicNumber == 0)
            {
                ketameriNumber = ketameriNumber.Append(0);
            }
            else
            {
                while (arabicNumber > 0)
                {
                    var modulus = arabicNumber % 14;
                    ketameriNumber = ketameriNumber.Append(modulus); 
                    arabicNumber -= modulus;
                    arabicNumber /= 14;                
                }
            }
            
            ketameriNumber = ketameriNumber.Reverse();
            var x =ketameriNumber.Select(s => $"{s}")
            .Select(Graphic.CachedGraphic.GetGraphicWithLetter)
            .ToArray();
            return x;
        }
    }
    private Graphic.CachedGraphic[] SingleForLetter(char englishChar)
    {
        return Enumerable.Repeat(Graphic.CachedGraphic.GetGraphicWithLetter($"{englishChar}"),1).ToArray();
    }
    public void Put(char englishChar)
    {
        var outx = Enumerable.Empty<Graphic.CachedGraphic>().ToList();
        var last = buffer.Any() ? buffer.Last() : (char?)null;
        
        (
            var last_null,
            var numeric_last,
            var capital_last,
            var vowel_last,  
            var universal_y_last,  
            var letter_last,        
            var numeric_english,  
            var capital_english,
            var vowel_english,     
            var universal_y_english,   
            var letter_english,
            var special_english
        ) = (
                last == null,
                last != null ? IsNumber(last.Value) : false,
                last != null ? IsCapital(last.Value) : false,
                last != null ? IsVowel(last.Value) : false,
                last != null ? IsUniversalY(last.Value) : false,
                last != null ? IsLetter(last.Value) : false,
                IsNumber(englishChar),
                IsCapital(englishChar),
                IsVowel(englishChar),
                IsUniversalY(englishChar),
                IsLetter(englishChar),
                !IsSupportedCharacter(englishChar)
            );
        if (last_null)
        {
            if (numeric_english || letter_english)
            {
                buffer.Add(englishChar);
            }
            else if (!special_english)            
            {                
                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{englishChar}"));
            }
            else
            {
                outx.AddRange(ConvertStrangeCharacter(englishChar));
            }
        }
        else if (numeric_last)
        {
            if (numeric_english)
            {
                buffer.Add(englishChar);
            }
            else if (letter_english)
            {
                outx.AddRange(ConvertNumberInBuffer());
                buffer.Add(englishChar);
            }
            else if (!special_english)
            {
                outx.AddRange(ConvertNumberInBuffer());
                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{englishChar}"));
            }
            else
            {
                outx.AddRange(ConvertNumberInBuffer());
                outx.AddRange(ConvertStrangeCharacter(englishChar));
            }
        }      
        else
        {
            if (numeric_english)
            {
                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));
                buffer.Clear();
                buffer.Add(englishChar);
            }
            else if (letter_english)
            {
                if (capital_last)
                {
                    if (!capital_english)
                    {
                        if (vowel_last || universal_y_last)
                        {
                            if (!vowel_english || universal_y_english)
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                        else
                        {
                            if (vowel_english)
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));     
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                    }
                    else
                    {
                        outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));     
                        buffer.Clear();
                        buffer.Add(englishChar);
                    }
                }
                else
                {
                    if (!capital_english)
                    {
                         if (vowel_last || universal_y_last)
                        {
                            if (!vowel_english || universal_y_english)
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                        else
                        {
                            if (vowel_english)
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));     
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                    }
                    else
                    {
                        outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));     
                        buffer.Clear();
                        buffer.Add(englishChar);
                    }
                }
            }
            else if (!special_english)
            {
                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));  
                buffer.Clear();
                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{englishChar}"));
            }
            else
            {
                outx.Add(Graphic.CachedGraphic.GetGraphicWithLetter($"{last}"));  
                buffer.Clear();
                outx.AddRange(ConvertStrangeCharacter(englishChar));
            }
        }  
        cbuffer.AddRange(outx);
    }
    public Graphic.CachedGraphic[] Finish()
    {
        var outx = Enumerable.Empty<Graphic.CachedGraphic>().ToList();
        var last = buffer.Any() ? buffer.Last() : (char?)null;
        
        (
            var last_null,
            var numeric_last,
            var capital_last,
            var vowel_last, 
            var universal_y_last,   
            var letter_last            
        ) = (
                last == null,
                last != null ? IsNumber(last.Value) : false,
                last != null ? IsCapital(last.Value) : false,
                last != null ? IsVowel(last.Value) : false,
                last != null ? IsUniversalY(last.Value) : false,
                last != null ? IsLetter(last.Value) : false                
            );
        if (numeric_last)
        {
            outx.AddRange(ConvertNumberInBuffer());

        }
        else if (letter_last)
        {
            outx.AddRange(SingleForLetter(last!.Value));
        }
        cbuffer.AddRange(outx);
        var outxx = cbuffer.ToArray();
        cbuffer.Clear();
        buffer.Clear();
        return outxx;
        
    }

    
}
