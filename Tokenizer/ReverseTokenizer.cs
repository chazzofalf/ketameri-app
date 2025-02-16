
using System.Numerics;
using System.Security.Claims;

namespace Tokenizer;

class ReverseTokenizer 
{
    private bool _Is_SpecialMode;
    private bool _Is_NumericMode;
    private List<Graphic.CachedGraphic> buffer = new List<Graphic.CachedGraphic>();
    private List<char> cbuffer = new List<char>();
     
    public void Put(Graphic.CachedGraphic token)
    {
        if (_Is_SpecialMode)
        {
            if (token.IsNumber)
            {
                buffer.Add(token);
            }
            else if (token.IsSpecial)
            {
                cbuffer.AddRange(DecodeSymbolFromNumberInBuffer());
                _Is_SpecialMode = false;
            }
            
        }
        else if (_Is_NumericMode)
        {
            if (token.IsNumber)
            {
                buffer.Add(token);
            }
            else
            {
                cbuffer.AddRange(DecodeNumberInBuffer());
                _Is_NumericMode = false;
                Put(token);
            }
        }
        else
        {
            if (token.IsText)
            {
                cbuffer.AddRange(token.Letter.ToArray());
            }
            else if (token.IsNumber)
            {
                _Is_NumericMode = true;
                buffer.Add(token);
            }
            else if (token.IsSpecial)
            {
                _Is_SpecialMode = true;
            }
        }
        
    }

    private BigInteger DecodeNumberInBufferPrestep()
    {
        var number = (BigInteger)0;
        foreach (var token in buffer)
        {
            number *= 14;
            number += int.Parse(token.Letter);
        }
        buffer.Clear();
    
        return number;
    }
    private char[] DecodeNumberInBuffer()
    {
        
        return DecodeNumberInBufferPrestep().ToString().ToArray();
        
    }

    private IEnumerable<char> DecodeSymbolFromNumberInBuffer()
    {
        return Enumerable.Repeat((char)DecodeNumberInBufferPrestep(),1);
    }
    public string Finish()
    {
        var last = buffer.Any() ? buffer.Last() : null;
        var is_numeric = last != null ? last.IsNumber : false;
        if (is_numeric)
        {
            cbuffer.AddRange(DecodeNumberInBuffer());
        }
        var outxx = string.Join("",cbuffer);
        cbuffer.Clear();
        buffer.Clear();
        return outxx;
    }

    
}
