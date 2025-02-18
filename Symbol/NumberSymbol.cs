namespace Symbol
{
    class NumberSymbol : Symbol
    {
        private static NumberSymbol[]? _All;
        public  static NumberSymbol[] All => _All = _All ?? GenerateAll();

        public override bool IsLetterSymbol => false;

        private int Orientation { get; }
        private bool Hflipped { get; }
        private bool Vflipped { get; }
        private bool[][] GenerateMatrix()
        {
            return BuildGlyph();
        }

        private bool[][]? _Matrix = null;
        private bool[][] Matrix => _Matrix = _Matrix ?? GenerateMatrix();

        public override int Number {get;}

        public override int NumberOfBits => 0;

        public override bool IsSpace => false;

        private bool[][] BuildGlyph() 
        {
           var core = Enumerable.Range(0,3)
        .Aggregate((number:Number,Symbol:""),(p,ign) => {
            var o = (number:p.number,Symbol:p.Symbol);
            if (p.number % 3 == 0)
            {
                o.Symbol = $"|{o.Symbol}";
            }
            else if (p.number % 3 == 1)
            {
                o.Symbol = $"'{o.Symbol}";
                o.number -= 1;
            }
            else if (p.number % 3 == 2)
            {
                o.Symbol = $",{o.Symbol}";
                o.number += 1;
            }
            o.number /= 3;
            return o;
        },(fin) => {
            return fin.Symbol;
        })
        .Select(ch => ch == '|' ? new [] {true,true} : ch == '\'' ? new [] {true,false} : ch == ',' ? new [] {false,true} : throw new Exception(""))
        .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (RowIndex:b,Row:a.Zip(Enumerable.Range(0,int.MaxValue),(c,d) => (ColumnIndex:d,Column:c))))
        .SelectMany(r => r.Row.Select(c => (RowIndex:r.RowIndex,ColumnIndex:c.ColumnIndex,Item:c.Column)))
        .OrderBy(i => i.ColumnIndex)
        .GroupBy(i => i.ColumnIndex)
        .Select(r => r.OrderBy(c => c.ColumnIndex).Select(c => c.Item).ToArray()).ToArray();

        var glyph = Enumerable.Range(0,9)
        .Select(r => Enumerable.Range(0,9)
        .Select(c => c < 2 || c > 6 || (c - 2) % 2 == 1 ? false :  core[r <= 4 ? 0 : 1][(c - 2)/2])
        .ToArray())
        .ToArray();
        
        return glyph;
        } 
       

        
        protected override bool ReadBitMapInner(int row,int column) => Matrix[row][column];

        private NumberSymbol(int number,int orientation=0,bool hflipped=false,bool vflipped=false) 
        {
            if (number < 0 && number > 13) throw new IndexOutOfRangeException();
            Number = number;
            Orientation = orientation;
            Hflipped = hflipped;
            Vflipped = vflipped;
        }
        private static NumberSymbol[] GenerateAll()
        {
            return Enumerable.Range(0,14)
            .Select(i => new NumberSymbol(i)).ToArray();
            
        }

        public override bool GetBit(int index)
        {
            return false;
        }
    }
}