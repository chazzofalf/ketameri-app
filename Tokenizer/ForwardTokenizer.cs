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
            /* The Ketameri writing system possesses a base-14 numerical system

            0 |||
            1 ||'
            2 |',
            3 |'|
            4 |''
            5 ',,
            6 ',|
            7 ','
            8 '|,
            9 '||
            10 '|'
            11 '',
            12 ''|
            13 '''

            For instance the number 2025 is written as '|| |'' '|'
            The number 42 is ||| |'|
            The number 54 is ''| |'| 
            The number 69 is ''' |''

            In reading these numbers, remember that Ketameri is right to left. You should interpret ''| |'| as |'| ''| when trying to go back to arabic by hand.

            Section "A": A little bit of numerological poetry:
            
            Funny tidbit about 42 and 69,
            when the top of a triangle is 42 degrees,
            the two remaining angles,
            if that triangle is to be split into two right triangles,
            The remaining two angles are both 69.

            With 54 it is 63 and 63.
            6 times 9 being supported at the triangles base by 7 times 9.
            How poetically divine. ;-)
            
            The Question to Mr. Douglas' answer of "42." Is "54?" (What does it really mean to be aLIVe?)
            The answer to that is most likely this:
            It is to be. 
            It your purpose to figure out what that means for you.
            I find this to be beautiful and I hope Section A is allowed to stay.
            Because I don't want to have to make it go away.            
             */
            var ketameriNumber   = Enumerable.Empty<BigInteger>();
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
