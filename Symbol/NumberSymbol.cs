namespace Symbol
{
    public class NumberSymbol : Symbol
    {
        private static NumberSymbol[]? _All;
        public new static NumberSymbol[] All => _All = _All ?? GenerateAll();

        public override bool IsLetterSymbol => false;

        
        private NumberSymbol(int number,int orientation=0,bool hflipped=false,bool vflipped=false) 
        {
            if (number < 0 && number > 13) throw new IndexOutOfRangeException();
        }
        private static NumberSymbol[] GenerateAll()
        {
            return Enumerable.Empty<NumberSymbol>().ToArray();
            
        }
        

    }
}