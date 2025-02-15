

namespace Symbol
{
    
    public abstract class Symbol
    {
        public abstract bool IsSpace {get;}
        public abstract bool IsLetterSymbol {get;}
        protected abstract bool ReadBitMapInner(int row,int col);
        
        private bool[][]? _Matrix = null;
        private bool[][] Matrix => _Matrix = _Matrix ?? GenerateMatrix();
        private bool InRange(int v,int a,int b)
    {
        if (b < a)
        {
            return InRange(v,b,a);
        }        
        return v >= a && v <= b;
    }
    private bool InRect(int xv,int yv,int x1,int y1,int x2,int y2)
    {
        return InRange(xv,x1,x2) && InRange(yv,y1,y2);

    }
        private bool[][] GenerateMatrix() =>
        
            Enumerable.Range(0,11)
            .Select(r => Enumerable.Range(0,11)
            .Select(c => {
                if (InRect(c,r,1,1,9,9))
                {
                    return ReadBitMapInner(r-1,c-1);
                }
                else if (IsSpace)
                {
                    return false;
                }
                else if (r == 0)
                {
                    return true;
                }
                else if (r == 10)
                {
                    return c != 5;
                }
                else if (c == 0)
                {
                    return r != 5 && r != 3 && r != 7;
                }
                else if (c == 10)
                {
                    return r != 4 && r != 6;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }).ToArray()).ToArray();
        

        public  bool ReadBitMap(int row,int col) => Matrix[row][col];
        public abstract int Number { get;}
        private static Symbol[]? _All;
        private static Symbol[] All => _All = _All ?? GenerateAll();

        public static int NumberOfSymbols => All.Length;

        public static Symbol GetSymbolAt(int index) => All[index];

        public abstract int NumberOfBits { get;  }

        private static Symbol[] GenerateAll()
        {
            var pool = LetterSymbol.All;
            var numbers = NumberSymbol.All;
            var x = pool.Take(1)
            .OfType<Symbol>().ToArray();
            x = x.Concat(numbers).ToArray();
            x = x.Concat(pool.Skip(1)).ToArray()
            .ToArray();
           
            return x;
        }

        public abstract bool GetBit(int index);
    }
}
