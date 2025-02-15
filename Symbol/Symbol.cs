
namespace Symbol
{
    public abstract class Symbol
    {
        public abstract bool IsLetterSymbol {get;}
        public abstract bool ReadBitMap(int row,int col);
        public abstract int Number { get;}
        private static Symbol[]? _All;
        public static Symbol[] All => _All = _All ?? GenerateAll();

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
