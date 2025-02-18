using SkiaSharp;

namespace PageJoinerSpliter;

public class PageJoinerSpliter
{
    public SKBitmap Join(SKBitmap[] lines,bool centered,SKColor? background=null,SKBitmap? backgroundimg=null)
    {

        var realBackground = background?.WithRed((byte?)(background?.Red | 1) ?? (byte)0) ?? SKColors.Black.WithRed(1);
        var onBackground = realBackground.WithRed((byte)(realBackground.Red & 0xfe));
        
        
        var x = Enumerable.Repeat(lines.Select(s => 
        {
            var padded = new SKBitmap(s.Width+2,s.Height+2);
            var can = new SKCanvas(padded);
            can.DrawBitmap(s,1,1);
            return padded;
        })
        .Aggregate((MaxWidth:0,TotalHeight:0,Items:Enumerable.Empty<SKBitmap>()),(prev,cur) => {
            return (MaxWidth:int.Max(prev.MaxWidth,cur.Width),TotalHeight:prev.TotalHeight+cur.Height,Items:prev.Items.Append(cur));
        }),1)
        .SelectMany(x => x.Items
        .Select(y => (X:centered ? (x.MaxWidth-y.Width)/2 : (x.MaxWidth-y.Width),Item:y)))
        .Aggregate((CY:0,Items:Enumerable.Empty<(int X, int Y,SKBitmap Item)>()),(p,c) => (CY:p.CY+c.Item.Height,Items:p.Items.Append((X:c.X,Y:p.CY,Item:c.Item))),
        (fin) => fin.Items);
        var width = x.Max(s=> s.Item.Width);
        var height = x.Sum(s => s.Item.Height);

        var outx = new SKBitmap(width+6,height+6);
        var can = new SKCanvas(outx);
        var brush = new SKPaint();
        brush.Color=realBackground;
        can.DrawRect(new SKRect(0,0,outx.Width,outx.Height),brush);
        if (backgroundimg != null)
        {
            var filtered = backgroundimg.Copy();
            var ops = Enumerable.Range(0,filtered.Height)
            .Select(r => Enumerable.Range(0,filtered.Width)
            .Select(c => {
                filtered.SetPixel(c,r,filtered.GetPixel(c,r).WithRed((byte)(filtered.GetPixel(c,r).Red | 1)));
                return 1;
            }).Sum()).Sum();
            can.DrawBitmap(filtered,new SKPoint(outx.Width/2-backgroundimg.Width/2,outx.Height/2-outx.Height/2));
        }
        var ops2 = x.Select(y => {
            outx.SetPixel(3+y.X,3+y.Y,outx.GetPixel(3+y.X,3+y.Y).WithRed((byte)(outx.GetPixel(3+y.X,3+y.Y).Red & 0xfe)));
            outx.SetPixel(3+y.X+y.Item.Width-1,3+y.Y,outx.GetPixel(3+y.X+y.Item.Width-1,3+y.Y).WithRed((byte)(outx.GetPixel(3+y.X+y.Item.Width-1,3+y.Y).Red & 0xfe)));
            outx.SetPixel(3+y.X,3+y.Y+y.Item.Height-1,outx.GetPixel(3+y.X,3+y.Y+y.Item.Height-1).WithRed((byte)(outx.GetPixel(3+y.X,3+y.Y+y.Item.Height-1).Red & 0xfe)));
            outx.SetPixel(3+y.X+y.Item.Width-1,3+y.Y+y.Item.Height-1,outx.GetPixel(3+y.X+y.Item.Width-1,3+y.Y+y.Item.Height-1).WithRed((byte)(outx.GetPixel(3+y.X+y.Item.Width-1,3+y.Y+y.Item.Height-1).Red & 0xfe)));
            return 1;
        }).Sum();
        ops2 = x.Select(y => {
            can.DrawBitmap(y.Item,3+y.X,3+y.Y);
            return 1;
        }).Sum();
        ops2 = new [] {Enumerable.Range(0,outx.Width-2)
        .Select(s => 
        {
            var pix = outx.GetPixel(1+s,1);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(1+s,1,pix);
            return 1;
        }
        ).Sum(),
        Enumerable.Range(0,outx.Width-2)
        .Select(s => 
        {
            var pix = outx.GetPixel(1+s,outx.Height-1-1);
            var red = pix.Red;            
            red = s != outx.Width/2 ? (byte)(red & 0xfe) : (byte)(red | 1);
            pix = pix.WithRed(red);
            outx.SetPixel(1+s,outx.Height-1-1,pix);
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-2)
        .Select(s => {
            var pix = outx.GetPixel(outx.Width-1-1,s+1);
            var red = pix.Red;
            red = s != outx.Height/2 - 1 && s != outx.Height/2 + 1 ? (byte)(red & 0xfe) : (byte)(red | 1);
            pix = pix.WithRed(red);
            outx.SetPixel(outx.Width-1-1,s+1,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-2)
        .Select(s => {
            var pix = outx.GetPixel(1,s+1);
            var red = pix.Red;
            red = s != outx.Height/2 && s != outx.Height/2 - 2 && s != outx.Height/2 + 2 ? (byte)(red & 0xfe) : (byte)(red | 1);
            pix = pix.WithRed(red);
            outx.SetPixel(1,s+1,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Width)
        .Select(s => {
            var pix = outx.GetPixel(s,0);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(s,0,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Width)
        .Select(s => {
            var pix = outx.GetPixel(s,outx.Height-1);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(s,outx.Height-1,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height)
        .Select(s => {
            var pix = outx.GetPixel(0,s);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(0,s,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height)
        .Select(s => {
            var pix = outx.GetPixel(outx.Width-1,s);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(outx.Width-1,s,pix);
            return 1;
        }).Sum(),        
        Enumerable.Range(0,outx.Width-4)
        .Select(s => {
            var pix = outx.GetPixel(2+s,2);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(2+s,2,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Width-4)
        .Select(s => {
            var pix = outx.GetPixel(2+s,outx.Height-1-2);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(2+s,outx.Height-1-2,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-4)
        .Select(s => {
            var pix = outx.GetPixel(2,s+2);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(2,s+2,pix);
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-4)
        .Select(s => {
            var pix = outx.GetPixel(outx.Width-1-2,s+2);
            var red = pix.Red;
            red &= 0xfe;
            pix = pix.WithRed(red);
            outx.SetPixel(outx.Width-1-2,s+2,pix);
            return 1;
        }).Sum(),
        }.Sum();

        return outx;         
    }
}
