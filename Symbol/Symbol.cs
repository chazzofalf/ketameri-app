

namespace Symbol
{
    
    public abstract class Symbol
    {
        public abstract bool IsSpace {get;}
        public abstract bool IsLetterSymbol {get;}
        protected abstract bool ReadBitMapInner(int row,int col);
        
        private bool[][]? _Matrix = null;
        private bool[][] Matrix => _Matrix = _Matrix ?? GenerateMatrix();
        
    
        private bool[][] GenerateMatrix() =>
        
            
            Enumerable.Range(0,9)
            .Select(r => Enumerable.Range(0,9)
            .Select(c => ReadBitMapInner(r,c)).ToArray()).ToArray();
        

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
