using System.Drawing;
using System.Runtime.Serialization;
using SkiaSharp;

namespace PageJoinerSpliter;

enum Orientation
{
    None = 0,
    Rotate = 1,    
    Flip_Horizontally = Rotate << 1,
    Flip_Vertically = Flip_Horizontally << 1
}
public class PageJoinerSpliter
{
    public SKBitmap Join(SKBitmap[] lines,bool centered,SKColor? background=null,SKBitmap? backgroundimg=null,bool isPartOfBook=false,SKColor? bookPlacerIndicatorColor=null)
    {
        
        var realBackground = !isPartOfBook ? background?.WithRed((byte?)(background?.Red | 1) ?? (byte)0) ?? SKColors.Black.WithRed(1) : SKColors.Transparent;
        var onBackground = !isPartOfBook ? realBackground.WithRed((byte)(realBackground.Red & 0xfe)) : SKColors.Transparent;
        var bookPlacerIndicatorColorReal = isPartOfBook ? bookPlacerIndicatorColor is SKColor idc ? idc  : SKColors.Red : SKColors.Transparent;
        
        
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
        
        var outx = new SKBitmap(width+8,height+8);
        var can = new SKCanvas(outx);
        var brush = new SKPaint();        
        brush.Color=realBackground;
        if (!isPartOfBook)
        {
            can.DrawRect(new SKRect(0,0,outx.Width,outx.Height),brush);    
        }
        
        if (backgroundimg != null && !isPartOfBook)
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
            outx.SetPixel(4+y.X,4+y.Y,!isPartOfBook ? outx.GetPixel(4+y.X,4+y.Y).WithRed((byte)(outx.GetPixel(4+y.X,4+y.Y).Red & 0xfe)) : bookPlacerIndicatorColorReal );
            outx.SetPixel(4+y.X+y.Item.Width-1,4+y.Y,!isPartOfBook ? outx.GetPixel(4+y.X+y.Item.Width-1,4+y.Y).WithRed((byte)(outx.GetPixel(4+y.X+y.Item.Width-1,4+y.Y).Red & 0xfe)) : bookPlacerIndicatorColorReal);
            outx.SetPixel(4+y.X,4+y.Y+y.Item.Height-1,!isPartOfBook ? outx.GetPixel(4+y.X,4+y.Y+y.Item.Height-1).WithRed((byte)(outx.GetPixel(4+y.X,4+y.Y+y.Item.Height-1).Red & 0xfe)) : bookPlacerIndicatorColorReal);
            outx.SetPixel(4+y.X+y.Item.Width-1,4+y.Y+y.Item.Height-1,!isPartOfBook ? outx.GetPixel(4+y.X+y.Item.Width-1,4+y.Y+y.Item.Height-1).WithRed((byte)(outx.GetPixel(4+y.X+y.Item.Width-1,4+y.Y+y.Item.Height-1).Red & 0xfe)) : bookPlacerIndicatorColorReal);
            return 1;
        }).Sum();
        ops2 = x.Select(y => {
            can.DrawBitmap(y.Item,4+y.X,4+y.Y);
            return 1;
        }).Sum();        
        ops2 = new [] {Enumerable.Range(0,outx.Width-4)
        .Select(s => 
        {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(2+s,2);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(2+s,2,pix);
            }
            else
            {
                outx.SetPixel(2+s,2,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }
        ).Sum(),
        Enumerable.Range(0,outx.Width-4)
        .Select(s => 
        {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(2+s,outx.Height-1-2);
                var red = pix.Red;            
                red = s != outx.Width/2 ? (byte)(red & 0xfe) : (byte)(red | 1);
                pix = pix.WithRed(red);
                outx.SetPixel(2+s,outx.Height-1-2,pix);
            }
            else
            {
                outx.SetPixel(2+s,outx.Height-1-2,bookPlacerIndicatorColorReal);
            }
            
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-4)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(outx.Width-1-2,s+2);
                var red = pix.Red;
                red = s != outx.Height/2 - 1 && s != outx.Height/2 + 1 ? (byte)(red & 0xfe) : (byte)(red | 1);
                pix = pix.WithRed(red);
                outx.SetPixel(outx.Width-1-2,s+2,pix);
            }
            else
            {
                outx.SetPixel(outx.Width-1-2,s+2,bookPlacerIndicatorColorReal);
            }
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-4)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(2,s+2);
                var red = pix.Red;
                red = s != outx.Height/2 && s != outx.Height/2 - 2 && s != outx.Height/2 + 2 ? (byte)(red & 0xfe) : (byte)(red | 1);
                pix = pix.WithRed(red);
                outx.SetPixel(2,s+2,pix);
            }            
            else
            {
                outx.SetPixel(2,s+2,bookPlacerIndicatorColorReal);
            }
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Width-2)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(s+1,1);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(s+1,1,pix);
            }
            else
            {
                outx.SetPixel(s+1,1,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Width-2)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(s+1,outx.Height-2);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(s+1,outx.Height-2,pix);
            }
            else
            {
                outx.SetPixel(s+1,outx.Height-2,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-2)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(1,s+1);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(1,s+1,pix);
            }
            else
            {
                outx.SetPixel(1,s+1,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-2)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(outx.Width-1-1,s+1);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(outx.Width-1-1,s+1,pix);
            }
            else
            {
                outx.SetPixel(outx.Width-1-1,s+1,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),        
        Enumerable.Range(0,outx.Width-6)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(3+s,3);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(3+s,3,pix);
            }
            else
            {
                outx.SetPixel(3+s,3,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Width-6)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(3+s,outx.Height-1-3);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(3+s,outx.Height-1-3,pix);
            }
            else
            {
                outx.SetPixel(3+s,outx.Height-1-3,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-6)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(3,s+3);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(3,s+3,pix);
            }
            else
            {
                outx.SetPixel(3,s+3,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),
        Enumerable.Range(0,outx.Height-6)
        .Select(s => {
            if (!isPartOfBook)
            {
                var pix = outx.GetPixel(outx.Width-1-3,s+3);
                var red = pix.Red;
                red &= 0xfe;
                pix = pix.WithRed(red);
                outx.SetPixel(outx.Width-1-3,s+3,pix);
            }
            else
            {
                outx.SetPixel(outx.Width-1-3,s+3,bookPlacerIndicatorColorReal);
            }
            
            return 1;
        }).Sum(),
        }.Sum();

        return outx;         
    }
    class SplitState
    {
        public int? Top {get;set;}
        public int? Left {get;set;}
        public int? Bottom {get;set;}
        public int? Right {get;set;}
        public List<SKRect> Rectangles {get;} = new List<SKRect>();
    }
    public SKBitmap[] Split(SKBitmap page)
    {
        var debordered = new SKBitmap(page.Width-4,page.Height-4);
        using (var canvas = new SKCanvas(debordered))
        {
            canvas.DrawBitmap(page,new SKRect(2,2,page.Width-2,page.Height-2),new SKRect(0,0,page.Width-4,page.Height-4));
        }        
        (var top,var bottom,var right,var left) = new [] {
            Enumerable.Range(0,debordered.Width)
            .Select(s => debordered.GetPixel(s,0))
            .Select(s => s.Red)
            .Select(s => s & 1)            
            .Sum(),
            Enumerable.Range(0,debordered.Width)
            .Select(s => debordered.GetPixel(s,debordered.Height-1))
            .Select(s => s.Red)
            .Select(s => s & 1)            
            .Sum(),
            Enumerable.Range(0,debordered.Height)
            .Select(s => debordered.GetPixel(debordered.Width-1,s))
            .Select(s => s.Red)
            .Select(s => s & 1)            
            .Sum(),
            Enumerable.Range(0,debordered.Height)
            .Select(s => debordered.GetPixel(0,s))
            .Select(s => s.Red)
            .Select(s => s & 1)            
            .Sum()
        };
        var temp = 0;
        var orientations = Orientation.None;
        if (top == 2 || top == 3)
        {
            temp = top;
            top = right;
            right = bottom;
            bottom = left;
            left = temp;
            orientations |= Orientation.Rotate;
        }
        if (left == 2)
        {
            temp = left;
            left = right;
            right = temp;
            orientations |= Orientation.Flip_Horizontally;
        }
        if (top == 1)
        {
            temp = top;
            top = bottom;
            bottom = temp;
            orientations |= Orientation.Flip_Vertically;
        }
        if ((orientations & Orientation.Rotate) == Orientation.Rotate)
        {
            debordered = Rotate(debordered);
        }
        if ((orientations & Orientation.Flip_Horizontally) == Orientation.Flip_Horizontally)
        {
            debordered = HFlip(debordered);
        }
        if ((orientations & Orientation.Flip_Vertically) == Orientation.Flip_Vertically)
        {
            debordered = VFlip(debordered);
        }
        var stripped = new SKBitmap(debordered.Width-4,debordered.Height-4);
        using (var canvas = new SKCanvas(stripped))
        {
            canvas.DrawBitmap(debordered,new SKRect(2,2,debordered.Width-2,debordered.Height-2),new SKRect(0,0,debordered.Width-4,debordered.Height-4));
        }        
        return Enumerable.Range(0,stripped.Height)
        .SelectMany(r => Enumerable.Range(0,stripped.Width)
        .Select(c => (Row:r,Column:c)))
        .Aggregate(new SplitState(),(state,current) => {
            if ((stripped.GetPixel(current.Column,current.Row).Red & 1) == 0)
            {
                if (state.Top == null)
                {
                    (state.Top,state.Left) = (current.Row,current.Column);
                }
                else if (state.Right == null && current.Row == state.Top)
                {
                    state.Right = current.Column;
                }
                else if (state.Right == null)
                {
                    throw new Exception("Open Ended Rectangle");
                }
                else if (state.Bottom == null && current.Column == state.Left)
                {
                    state.Bottom = current.Row;
                }
                else if (state.Bottom != null && state.Left != null && state.Right == current.Column)
                {
                    state.Rectangles.Add(new SKRect((float)state.Left,(float)state.Top,(float)state.Right,(float)state.Bottom));
                    (state.Left,state.Top,state.Right,state.Bottom) = Wipe();
                }
            }
            return state;
        },(state) => {
            return state.Rectangles;
        })
        .Select(r => new SKRect(r.Left+1,r.Top+1,r.Right,r.Bottom))
        .Select(r => {
            var segment = new SKBitmap((int)r.Width,(int)r.Height);
            using (var canvas = new SKCanvas(segment))
            {
                canvas.DrawBitmap(stripped,new SKRect(r.Left,r.Top,r.Right,r.Bottom),new SKRect(0,0,r.Width,r.Height));                
            }
            return segment;
        }).ToArray();

    }
    private (int? left,int? top, int? right, int? bottom) Wipe()
    {
        return (null,null,null,null);
    }
    
    private static SKBitmap HFlip(SKBitmap bmp)
    {
        // Create a bitmap (to return)
        var flipped = new SKBitmap(bmp.Width, bmp.Height, bmp.Info.ColorType, bmp.Info.AlphaType);

        // Create a canvas to draw into the bitmap
        using var canvas = new SKCanvas(flipped);

        // Set a transform matrix which moves the bitmap to the right,
        // and then "scales" it by -1, which just flips the pixels
        // horizontally
        canvas.Translate(bmp.Width, 0);
        canvas.Scale(-1, 1);
        canvas.DrawBitmap(bmp, 0, 0);
        return flipped;
    }
    private static SKBitmap VFlip(SKBitmap bmp)
    {
        // Create a bitmap (to return)
        var flipped = new SKBitmap(bmp.Width, bmp.Height, bmp.Info.ColorType, bmp.Info.AlphaType);

        // Create a canvas to draw into the bitmap
        using var canvas = new SKCanvas(flipped);

        // Set a transform matrix which moves the bitmap to the right,
        // and then "scales" it by -1, which just flips the pixels
        // horizontally
        canvas.Translate(0, bmp.Height);
        canvas.Scale(1, -1);
        canvas.DrawBitmap(bmp, 0, 0);
        return flipped;
    }
    private static SKBitmap Rotate(SKBitmap bitmap)
    {
        
        var rotated = new SKBitmap(bitmap.Height, bitmap.Width);

        using (var surface = new SKCanvas(rotated))
        {
            surface.Translate(rotated.Width, 0);
            surface.RotateDegrees(90);
            surface.DrawBitmap(bitmap, 0, 0);
        }

        return rotated;
        
    }
}
