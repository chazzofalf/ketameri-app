using SkiaSharp;

namespace BookJoiner;

public class Placement
{
    public bool IsAbsolute {get;}
    public int? PreviousElementIndex {get;}
    public string Page {get;}
    public SKPoint Location {get;}
    public bool UseLarge {get;}
    public SKColor FontColor {get;}
    public bool IsCentered {get;}

    public Placement(string page,SKPoint location,bool isAbsolute,int? previousElementIndex=null,bool useLarge=true,SKColor? fontColor=null,bool isCentered=false)
    {
        IsAbsolute = isAbsolute;
        PreviousElementIndex = previousElementIndex;
        Page = page;
        Location = location;
        UseLarge = useLarge;
        FontColor = fontColor is SKColor fc ? fc : SKColors.Turquoise;
        IsCentered = isCentered;

    }
}
class PartiallyRealizedPlacement
{
    public bool IsAbsolute {get;}
    public int? PreviousElementIndex {get;}
    public SKPoint Location {get;}
    public SKBitmap Page {get;}
    public PartiallyRealizedPlacement(bool isAbsolute,int? previousElementIndex,SKPoint location,SKBitmap page)
    {
        IsAbsolute = isAbsolute;
        PreviousElementIndex = previousElementIndex;
        Location = location;
        Page = page;
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
        var x = pages
        .Select(pg => new PartiallyRealizedPlacement(isAbsolute:pg.IsAbsolute,previousElementIndex:pg.PreviousElementIndex,location:pg.Location,converter.Translate(name:pg.Page,fontColor:pg.FontColor,backgroundColor:backgroundColorReal,backgroundBitmap:backgroundImage,useLarge:pg.UseLarge,centered:pg.IsCentered,isPartOfBook:true,bookPlacerIndicatorColor:placerColor)));

        return null; // TODO: You are here. 

    }
}
