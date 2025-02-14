namespace Symbol
{
    public abstract class Symbol
    {
        public abstract bool IsLetterSymbol {get;}
        private static Symbol[]? _All;
        public static Symbol[] All => _All = _All ?? GenerateAll();

        private static Symbol[] GenerateAll()
        {
            var pool = LetterSymbol.All;
            var numbers = NumberSymbol.All;
            return pool.Take(1)
            .OfType<Symbol>()
            .Concat(numbers)
            .Concat(pool.Skip(1))
            .ToArray();
           
            
        }
    }
}
