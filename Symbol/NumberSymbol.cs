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
        .Select(ch => ch == '|' ? new [] {true,true,true} : ch == '\'' ? new [] {true,true,false} : ch == ',' ? new [] {false,false,true} : throw new Exception(""))
        .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (RowIndex:b,Row:a.Zip(Enumerable.Range(0,int.MaxValue),(c,d) => (ColumnIndex:d,Column:c))))
        .SelectMany(r => r.Row.Select(c => (RowIndex:r.RowIndex,ColumnIndex:c.ColumnIndex,Item:c.Column)))
        .OrderBy(i => i.ColumnIndex)
        .GroupBy(i => i.ColumnIndex)
        .Select(r => r.OrderBy(c => c.ColumnIndex).Select(c => c.Item).ToArray()).ToArray();

        var glyph = Enumerable.Range(0,9)
        .Select(r => Enumerable.Range(0,9)
        .Select(c => c % 3 == 2 ? false : core[r/3][c/3]).ToArray()).ToArray();
        // .Select(c => c < 3 || c > 5 || r < 3 || r > 5 ? false : core[r-3][c-3] ).ToArray()).ToArray();
        // var _ = 0;
        // _ = Enumerable.Range(0,9)
        // .Select(i => {
        //     glyph[0][i] = true;
        //     return 1;
        // }).Sum();
        // _ = Enumerable.Range(0,9)
        // .Select(i => {
        //     glyph[i][8] = true;
        //     return 1;
        // }).Sum();
        // _ = Enumerable.Range(0,9)
        // .Select(i => {
        //     glyph[8][i] = true;
        //     return 1;
        // }).Sum();
        // _ = Enumerable.Range(0,9)
        // .Select(i => {
        //     glyph[i][0] = true;
        //     return 1;
        // }).Sum();
        // _ = Enumerable.Range(0,3)
        // .Select(i => {
        //     //glyph[1][3+i] = core[0][i];
        //     glyph[1][3+i] = true;
        //     return 1;
        // }).Sum();
        // _ = Enumerable.Range(0,3)
        // .Select(i => {
        //     //glyph[3+i][7] = core[i][2];
        //     glyph[3+i][7] = true;
        //     return 1;
        // }).Sum();
        // _ = Enumerable.Range(0,3)
        // .Select(i => {
        //     //glyph[7][3+i] = core[2][i];
        //     glyph[7][3+i] = true;
        //     return 1;
        // }).Sum();
        // _ = Enumerable.Range(0,3)
        // .Select(i => {
            
        //     //glyph[3+i][1] = core[i][0];
        //     glyph[3+i][1] = true;
        //     return 1;
        // }).Sum();
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