using SkiaSharp;

namespace Symbs;

class Util
{
    private unsafe bool PixelMatch(byte *ptr,int offset,byte[] pixelTest)
	{
		return ptr[offset+0] == pixelTest[0] &&
		ptr[offset+1] == pixelTest[1] &&
		ptr[offset+2] == pixelTest[2] &&
		ptr[offset+3] == pixelTest[3];
	}
    private SKBitmap? _turquoisePixel;
	private SKBitmap TurquoisePixel => _turquoisePixel = _turquoisePixel ?? GenerateTurquoisePixel();
    private SKBitmap GenerateTurquoisePixel()
    {
        var pixel = new SKBitmap(1,1);
		var turquoise = SKColors.Turquoise;
		var turquoisePaint = new SKPaint() { Color = turquoise};
		var canvas = new SKCanvas(pixel);
		canvas.DrawRect(new SKRect(0,0,pixel.Width,pixel.Height),turquoisePaint);
		return pixel;
    }
    private byte[]? _turquoisePixelBytes;
	private byte[] TurquoisePixelBytes => _turquoisePixelBytes = _turquoisePixelBytes ?? GenerateTurquoisePixelBytes();

    private byte[] GenerateTurquoisePixelBytes()
    {
		byte[] pBytes = new byte[TurquoisePixel.BytesPerPixel];
		
		unsafe
		{
			byte *pixelData = (byte *)TurquoisePixel.GetPixels();
			for (var i=0;i<pBytes.Length;i++)
			{
				pBytes[i] = pixelData[i];
			}
		} 
		return pBytes;
    }
    private unsafe bool IsTurquoise(byte *ptr,int offset)
	{
		return PixelMatch(ptr,offset,TurquoisePixelBytes);
	}
    private void MakeNonTurquoiseTransparent(SKBitmap bitmap)
    {
        var pixels = bitmap.GetPixels();
        unsafe 
        {
            byte *idx = (byte *)pixels;
            var offset = 0;
            var size = bitmap.Width*bitmap.Height*bitmap.BytesPerPixel;
            while (offset < size)
            {

                idx[offset+3] = idx[offset+0] == 64 &&
                idx[offset+1] == 224 &&
                idx[offset+2] == 208 ? (byte)255 : (byte)255;
                idx[offset+3] = IsTurquoise(idx,offset) ? (byte)255 : (byte)0;
                
                offset += 4;
            }
        }
    }
    public SKBitmap MakeTransparency(SKBitmap bitmap)
    {
        var renderCopy = bitmap.Copy();
        MakeNonTurquoiseTransparent(renderCopy);
        return renderCopy;
    }
}
