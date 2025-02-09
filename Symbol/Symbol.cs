using System.Collections.Frozen;

namespace Symbol;

public class Symbol
{
    private static Symbol[]? _All;
    public static Symbol[] All => _All = _All ?? GenerateAll();

    private static Symbol[] GenerateAll()
    {
        var pool = _GetAllSymbols().AsEnumerable();
        var selected = Enumerable.Empty<Symbol>().ToList();
        while (pool.Any())
        {
            var pick = pool.First();
            pool = pool.Skip(1);
            selected.Add(pick);
            pool = pool.Where(s => !pick.Similar(s));
        }
        return selected.ToArray();
    }

    private static Symbol[] _GetAllSymbols() => Enumerable.Range(0,4096)
    .Select(s => Symbol.SymbolForNumber(s))
    .ToArray();
    public static Symbol[] cachedAll = new Symbol[4096];
    private bool[]? bits;
    private bool[] Bits => bits = bits ?? Enumerable.Range(0,12)
    .Select(bi => (Number & (1 << (11-bi))) != 0)
    .ToArray();
    private bool[][]? map;
    private bool[][] Map => map = map ?? Enumerable.Range(0,5)
    .Select(r => Enumerable.Range(0,5)
    .Select(c => {
        if (r == 0 && c == 0)
        {
            return Bits[6] || Bits[7];
        }
        else if (r == 0 && c == 1)
        {
            return Bits[7];
        }
        else if (r == 0 && c == 2)
        {
            return Bits[7] || Bits[8] || Bits[0];
        }
        else if (r == 0 && c == 3)
        {
            return Bits[0];
        }
        else if (r == 0 && c == 4)
        {
            return Bits[0] || Bits[1];
        }
        else if (r == 1 && c == 0)
        {
            return Bits[6];
        }
        else if (r == 1 && c == 1)
        {
            return false;
        }
        else if (r == 1 && c == 2)
        {
            return Bits[8];
        }
        else if (r == 1 && c == 3)
        {
            return false;
        }
        else if (r == 1 && c == 4)
        {
            return Bits[1];
        }
        else if (r == 2 && c == 0)
        {
            return Bits[6] || Bits[11] || Bits[5];
        }
        else if (r == 2 && c == 1)
        {
            return Bits[11];
        }
        else if (r == 2 && c == 2) 
        {
            return Bits[8] || Bits[9] || Bits[10] || Bits[11];
        }
        else if (r == 2 && c == 3) 
        {
            return Bits[9];
        }
        else if (r == 2 && c == 4) 
        {
            return Bits[1] || Bits[9] || Bits[2];
        }
        else if (r == 3 && c == 0) 
        {
            return Bits[5];
        }
        else if (r == 3 && c == 1)
        {
            return false;
        }
        else if (r == 3 && c == 2)
        {
            return Bits[10];
        }
        else if (r == 3 && c == 3)
        {
            return false;
        }
        else if (r == 3 && c == 4)
        {
            return Bits[2];
        }
        else if (r == 4 && c == 0)
        {
            return Bits[5] || Bits[4];
        }
        else if (r == 4 && c == 1)
        {
            return Bits[4];
        }
        else if (r == 4 && c == 2)
        {
            return Bits[4] || Bits[10] || Bits[3];
        }
        else if (r == 4 && c == 3)
        {
            return Bits[3];
        }
        else if (r == 4 && c == 4)
        {
            return Bits[2] || Bits[3];
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }).ToArray()).ToArray();
    public bool ReadBitMap(int row,int column)
    {
        return Map[row][column];
    }
    public int Number { get;}
    public static Symbol SymbolForNumber(int number)
    {
        if (cachedAll[number] == null)
        {
            cachedAll[number] = new Symbol(number);
        }
        return cachedAll[number];
    }
    public Symbol(int number)
    {
        Number = number;
    }
    // *7*0* --> *5*6*
    // 6 8 1 --> 4 B 7
    // *B*9* --> *A*8*
    // 5 A 2 --> 3 9 0
    // *4*3* --> *2*1*
    
    private Symbol VerticalFlip()
    {
        var _oldNum = Number;
        var _newNum =
            (((_oldNum & (1 << (11-0x0))) >> (11-0x0)) << (11-0x3)) |
            (((_oldNum & (1 << (11-0x1))) >> (11-0x1)) << (11-0x2)) |
            (((_oldNum & (1 << (11-0x2))) >> (11-0x2)) << (11-0x1)) |
            (((_oldNum & (1 << (11-0x3))) >> (11-0x3)) << (11-0x0)) |
            (((_oldNum & (1 << (11-0x4))) >> (11-0x4)) << (11-0x7)) |
            (((_oldNum & (1 << (11-0x5))) >> (11-0x5)) << (11-0x6)) |
            (((_oldNum & (1 << (11-0x6))) >> (11-0x6)) << (11-0x5)) |
            (((_oldNum & (1 << (11-0x7))) >> (11-0x7)) << (11-0x4)) |
            (((_oldNum & (1 << (11-0x8))) >> (11-0x8)) << (11-0xA)) |
            (((_oldNum & (1 << (11-0x9))) >> (11-0x9)) << (11-0x9)) |
            (((_oldNum & (1 << (11-0xA))) >> (11-0xA)) << (11-0x8)) |
            (((_oldNum & (1 << (11-0xB))) >> (11-0xB)) << (11-0xB));
        return SymbolForNumber(_newNum);                   

    }
    private Symbol HorizontalFlip()
    {
        var _oldNum = Number;
        var _newNum =
            (((_oldNum & (1 << (11-0x0))) >> (11-0x0)) << (11-0x7)) |
            (((_oldNum & (1 << (11-0x1))) >> (11-0x1)) << (11-0x6)) |
            (((_oldNum & (1 << (11-0x2))) >> (11-0x2)) << (11-0x5)) |
            (((_oldNum & (1 << (11-0x3))) >> (11-0x3)) << (11-0x4)) |
            (((_oldNum & (1 << (11-0x4))) >> (11-0x4)) << (11-0x3)) |
            (((_oldNum & (1 << (11-0x5))) >> (11-0x5)) << (11-0x2)) |
            (((_oldNum & (1 << (11-0x6))) >> (11-0x6)) << (11-0x1)) |
            (((_oldNum & (1 << (11-0x7))) >> (11-0x7)) << (11-0x0)) |
            (((_oldNum & (1 << (11-0x8))) >> (11-0x8)) << (11-0x8)) |
            (((_oldNum & (1 << (11-0x9))) >> (11-0x9)) << (11-0xB)) |
            (((_oldNum & (1 << (11-0xA))) >> (11-0xA)) << (11-0xA)) |
            (((_oldNum & (1 << (11-0xB))) >> (11-0xB)) << (11-0x9));
        return SymbolForNumber(_newNum);

    }
    private Symbol Rotate()
    {
        var _oldNum = Number;
        var _newNum =
            (((_oldNum & (1 << (11-0x0))) >> (11-0x0)) << (11-0x6)) |
            (((_oldNum & (1 << (11-0x1))) >> (11-0x1)) << (11-0x7)) |
            (((_oldNum & (1 << (11-0x2))) >> (11-0x2)) << (11-0x0)) |
            (((_oldNum & (1 << (11-0x3))) >> (11-0x3)) << (11-0x1)) |
            (((_oldNum & (1 << (11-0x4))) >> (11-0x4)) << (11-0x2)) |
            (((_oldNum & (1 << (11-0x5))) >> (11-0x5)) << (11-0x3)) |
            (((_oldNum & (1 << (11-0x6))) >> (11-0x6)) << (11-0x4)) |
            (((_oldNum & (1 << (11-0x7))) >> (11-0x7)) << (11-0x5)) |
            (((_oldNum & (1 << (11-0x8))) >> (11-0x8)) << (11-0xB)) |
            (((_oldNum & (1 << (11-0x9))) >> (11-0x9)) << (11-0x8)) |
            (((_oldNum & (1 << (11-0xA))) >> (11-0xA)) << (11-0x9)) |
            (((_oldNum & (1 << (11-0xB))) >> (11-0xB)) << (11-0xA));
        return SymbolForNumber(_newNum);
    }
    private Symbol Identity()
    {
        var _oldNum = Number;
        var _newNum =
            (((_oldNum & (1 << (11-0x0))) >> (11-0x0)) << (11-0x0)) |
            (((_oldNum & (1 << (11-0x1))) >> (11-0x1)) << (11-0x1)) |
            (((_oldNum & (1 << (11-0x2))) >> (11-0x2)) << (11-0x2)) |
            (((_oldNum & (1 << (11-0x3))) >> (11-0x3)) << (11-0x3)) |
            (((_oldNum & (1 << (11-0x4))) >> (11-0x4)) << (11-0x4)) |
            (((_oldNum & (1 << (11-0x5))) >> (11-0x5)) << (11-0x5)) |
            (((_oldNum & (1 << (11-0x6))) >> (11-0x6)) << (11-0x6)) |
            (((_oldNum & (1 << (11-0x7))) >> (11-0x7)) << (11-0x7)) |
            (((_oldNum & (1 << (11-0x8))) >> (11-0x8)) << (11-0x8)) |
            (((_oldNum & (1 << (11-0x9))) >> (11-0x9)) << (11-0x9)) |
            (((_oldNum & (1 << (11-0xA))) >> (11-0xA)) << (11-0xA)) |
            (((_oldNum & (1 << (11-0xB))) >> (11-0xB)) << (11-0xB));
        return SymbolForNumber(_newNum);

    }
    private Symbol R90()
    {
        return Rotate();
    }
    private Symbol R180()
    {
        return R90().R90();
    }
    private Symbol R270()
    {
        return R180().R90();
    }
    private Symbol VFR90()
    {
        return VerticalFlip().R90();
    }

    private Symbol HFR90()
    {
        return HorizontalFlip().R90();
    }
    private bool ExactlyEqual(Symbol s)
    {
       return Number == s.Number;
    }
    private Symbol[]? _AllOrientations;
    public Symbol[] AllOrientations => _AllOrientations = _AllOrientations ??
        Enumerable.Empty<Symbol>()
        .Append(Identity())
        .Append(R90())
        .Append(R180())
        .Append(R270())
        .Append(VerticalFlip())
        .Append(HorizontalFlip())
        .Append(VFR90())
        .Append(HFR90())
        .ToFrozenSet(new CompareExactly())
        .ToArray();
    class CompareExactly : IEqualityComparer<Symbol>
    {
        public bool Equals(Symbol? x, Symbol? y)
        {
            return  (x == null && y == null) ? true : (x == null || y == null) ? false : 
             x.ExactlyEqual(y);
        }
        public int GetHashCode(Symbol codeh)
        {
            return codeh.GetHashCode();
        }
    }
    
    public  bool Similar(object? obj) 
    {
        var test = false;
        if (obj is Symbol symb1)
        {
            test = Equals_(symb1);
        }
        
        ;
        return test; 
        
        
        
    }
    private bool Equals_(Symbol b)
    {
        var d1 = AllOrientations;
        var d2 = d1.Select(s => s.Number);

        var d3 = d2.Where(s => s == b.Number);
        var d4 = d3.Any();
        
        return d4;
        // .Zip(symb._map.Select(r => r.Select(c => c)), (ar, br) =>
        // ar.Zip(br, (a, b) => a == b))
        // )
        // .Select(s => s.Select(r => r.Where(c => !c).None())
        // .Where(c => !c).None())
        // .Where(c => c)
        // .Any();
        
    }
}
