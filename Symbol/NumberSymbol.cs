namespace Symbol
{
    public class NumberSymbol : Symbol
    {
        private static NumberSymbol[]? _All;
        public new static NumberSymbol[] All => _All = _All ?? GenerateAll();

        public override bool IsLetterSymbol => false;

        private int Orientation { get; }
        private bool Hflipped { get; }
        private bool Vflipped { get; }
        private bool[][] GenerateMatrix()
        {
            var outx = Enumerable.Range(0,9)
        .Select(s => Enumerable.Range(0,9)
        .Select(ss => false).ToArray()).ToArray();
            return outx;
        }

        private bool[][]? _Matrix = null;
        private bool[][] Matrix => _Matrix = _Matrix ?? GenerateMatrix();
        public bool GetMatrixElement(int row,int column) => Matrix[row][column];

        private NumberSymbol(int number,int orientation=0,bool hflipped=false,bool vflipped=false) 
        {
            if (number < 0 && number > 13) throw new IndexOutOfRangeException();
            Orientation = orientation;
            Hflipped = hflipped;
            Vflipped = vflipped;
        }
        private static NumberSymbol[] GenerateAll()
        {
            return Enumerable.Empty<NumberSymbol>().ToArray();
            
        }
        

    }
}