using System.Numerics;
using System.Runtime.CompilerServices;
using Graphic;
using SkiaSharp;

namespace Tokenizer;

class ForwardTokenizer 
{
    private char[]? _SupportedCharacters = null;
    private string[]? _allGraphicLetters = null;
    private string[] AllGraphicLetters => _allGraphicLetters = _allGraphicLetters ?? Enumerable.Range(0,Graphic.Graphic.NumberOfGlyphs)
    .Select(n => Graphic.Graphic.GetGraphicAtIndex(n))
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
    private char[] Vowels => _Vowels = _Vowels ?? "aeiouy".ToArray();
    private char[]? _UpperVowels = null;
    private char[] UpperVowels => _UpperVowels = _UpperVowels ?? Vowels.Select(char.ToUpper).ToArray();
    
    
    
    
    
    

     

    private bool IsSupportedCharacter(char c) => SupportedCharacters.Contains(c);
    private bool IsCapital(char c) => Capitals.Contains(c);
    private bool IsNumber(char c) => Digits.Contains(c);        
    private bool IsVowel(char c) => Vowels.Contains(c) || UpperVowels.Contains(c);
    private bool IsLetter(char c) => Letters.Contains(c);
    private List<char> buffer = new List<char>();
    private List<Graphic.Graphic> cbuffer = new List<Graphic.Graphic>();
    
    private Graphic.Graphic[] ConvertNumberInBuffer()
    {
        var outx = ConvertNumberInObject(buffer);
        buffer.Clear();
        return outx;
    }
    private Graphic.Graphic[]? _SpecialCharacter = null;
    private Graphic.Graphic[] SpecialCharacter => _SpecialCharacter = _SpecialCharacter = 
    
        Enumerable.Repeat(Graphic.Graphic.GetGraphicForSpecial(),1).ToArray();
    
   
    
    private Graphic.Graphic[] ConvertStrangeCharacter(char strange)
    {
        var strange_ord = $"{(int)(ushort)strange}".ToArray();
        return SpecialCharacter.Concat(ConvertNumberInObject(strange_ord))
        .Concat(SpecialCharacter)
        .ToArray();
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
            = BigInteger.Parse(string.Join("",english));
            
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
    private Graphic.Graphic[] SingleForLetter(char englishChar)
    {
        return Enumerable.Repeat(Graphic.Graphic.GetGraphicWithLetter($"{englishChar}"),1).ToArray();
    }
    public void Put(char englishChar)
    {
        var outx = Enumerable.Empty<Graphic.Graphic>().ToList();
        var last = buffer.Any() ? buffer.Last() : (char?)null;
        
        (
            var last_null,
            var numeric_last,
            var capital_last,
            var vowel_last,    
            var letter_last,        
            var numeric_english,  
            var capital_english,
            var vowel_english,          
            var letter_english,
            var special_english
        ) = (
                last == null,
                last != null ? IsNumber(last.Value) : false,
                last != null ? IsCapital(last.Value) : false,
                last != null ? IsVowel(last.Value) : false,
                last != null ? IsLetter(last.Value) : false,
                IsNumber(englishChar),
                IsCapital(englishChar),
                IsVowel(englishChar),
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
                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{englishChar}"));
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
                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{englishChar}"));
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
                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));
                buffer.Clear();
                buffer.Add(englishChar);
            }
            else if (letter_english)
            {
                if (capital_last)
                {
                    if (!capital_english)
                    {
                        if (vowel_last)
                        {
                            if (!vowel_english)
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                        else
                        {
                            if (vowel_english)
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));     
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                    }
                    else
                    {
                        outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));     
                        buffer.Clear();
                        buffer.Add(englishChar);
                    }
                }
                else
                {
                    if (!capital_english)
                    {
                         if (vowel_last)
                        {
                            if (!vowel_english)
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                        else
                        {
                            if (vowel_english)
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}{englishChar}"));
                                buffer.Clear();
                            }
                            else
                            {
                                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));     
                                buffer.Clear();
                                buffer.Add(englishChar);
                            }
                        }
                    }
                    else
                    {
                        outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));     
                        buffer.Clear();
                        buffer.Add(englishChar);
                    }
                }
            }
            else if (!special_english)
            {
                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));  
                buffer.Clear();
                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{englishChar}"));
            }
            else
            {
                outx.Add(Graphic.Graphic.GetGraphicWithLetter($"{last}"));  
                buffer.Clear();
                outx.AddRange(ConvertStrangeCharacter(englishChar));
            }
        }  
        cbuffer.AddRange(outx);
    }
    public Graphic.Graphic[] Finish()
    {
        var outx = Enumerable.Empty<Graphic.Graphic>().ToList();
        var last = buffer.Any() ? buffer.Last() : (char?)null;
        
        (
            var last_null,
            var numeric_last,
            var capital_last,
            var vowel_last,    
            var letter_last            
        ) = (
                last == null,
                last != null ? IsNumber(last.Value) : false,
                last != null ? IsCapital(last.Value) : false,
                last != null ? IsVowel(last.Value) : false,
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
