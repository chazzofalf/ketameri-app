using SkiaSharp;

namespace BookJoiner;

public static class Extensions
{
    public static void Deconstruct(this SKRect rect,out float x1,out float y1,out float x2,out float y2)
    {
        x1 = rect.Left;
        y1 = rect.Top;
        x2 = rect.Right;
        y2 = rect.Bottom;
    }
}
public class Placement
{        
    public string Page {get;}   
    public bool UseLarge {get;}
    public SKColor FontColor {get;}
    public bool IsCentered {get;}

    public Placement(string page,bool useLarge=true,SKColor? fontColor=null,bool isCentered=false)
    {                
        Page = page;
        UseLarge = useLarge;
        FontColor = fontColor is SKColor fc ? fc : SKColors.Turquoise;
        IsCentered = isCentered;

    }
}

class FullyRealizedPlacement
{
    public SKPoint Location {get;}
    public SKBitmap Page {get;}
    public FullyRealizedPlacement(SKPoint location,SKBitmap page)
    {
        this.Location =location;
        this.Page = page;
    }
}
public class BookJoiner
{
    public SKBitmap Join(Placement[] pages,SKColor? backgroundColor=null,SKBitmap? backgroundImage = null,SKColor? placerColor =null)
    {
        var backgroundColorReal = backgroundColor is SKColor bgc ? bgc : SKColors.Black.WithRed(1);
        var placerColorReal = placerColor is SKColor plc ? plc : SKColors.Red;
        var converter = new SymbConvert.SymbConvert();
        var overlap = (SKRect rect1,SKRect rect2) =>
        {
            (var x1,var y1,var x2,var y2) = rect1;
            (var x3,var y3,var x4,var y4) = rect2;
            if (x1 < x4 && x3 < x2 && y1 < y4 && y3 < y2)
                return true;
            return false;
        };
        
            
        
        var x = Enumerable.Repeat(pages
        .Select(pg => converter.Translate(name:pg.Page,fontColor:pg.FontColor,backgroundColor:backgroundColorReal,backgroundBitmap:backgroundImage,useLarge:pg.UseLarge,centered:pg.IsCentered,isPartOfBook:true,bookPlacerIndicatorColor:placerColor)),1)
        .Select(pgg => pgg.Chunk((int)Math.Ceiling(Math.Sqrt(pgg.Count())))
        .Select(y => y
        .Select(z => z)))
        .First()
        .Select(r => r
        .Aggregate((max_x:0,max_y:0,items:Enumerable.Empty<(int x,SKBitmap bmp)>()),(p,c) => {
            return (max_x:p.max_x+c.Width,int.Max(p.max_y,c.Height),items:p.items.Append((x:p.max_x,bmp:c)));
        }))
        .Select(r => {
            var bmp = new SKBitmap(r.max_x,r.max_y);
            using (var can = new SKCanvas(bmp))
            {
                r.items
                .Aggregate((object?)null,(ign,c) => 
                {
                    can.DrawBitmap(c.bmp,c.x,0);
                    return ign;
                });
            }
            return bmp;

        })
        .Aggregate((max_x:0,max_y:0,imgs:Enumerable.Empty<(int y,SKBitmap bmp)>()),(p,c) => {
            return (max_x:int.Max(p.max_x,c.Width),max_y:p.max_y+c.Height,imgs:p.imgs.Append((y:p.max_y,bmp:c)));
        });
        (var width,var height,var imgs) = x;
        var bmp = new SKBitmap(width,height);
        using (var can = new SKCanvas(bmp))
        {
            imgs.Aggregate((object?)null,(p,c) => {
            can.DrawBitmap(c.bmp,0,c.y);
            return p;
            });
        }
        
        var bmpback = new SKBitmap(width,height);
        using (var can = new SKCanvas(bmpback))
        {
            using (var pt = new SKPaint())
            {
                pt.Color = backgroundColorReal;
                can.DrawRect(new SKRect(0,0,bmpback.Width,bmpback.Height),pt);
                if (backgroundImage != null)
                {
                    var filtered = backgroundImage.Copy();
                    var ops = Enumerable.Range(0,filtered.Height)
                    .Select(r => Enumerable.Range(0,filtered.Width)
                    .Select(c => {
                        filtered.SetPixel(c,r,filtered.GetPixel(c,r).WithRed((byte)(filtered.GetPixel(c,r).Red | 1)));
                        return 1;
                    }).Sum()).Sum();
                    can.DrawBitmap(filtered,new SKPoint(bmpback.Width/2-backgroundImage.Width/2,bmpback.Height/2-bmpback.Height/2));
                }
            }
            
            
        }
        return null; // TODO: You are here. 

    }
}
