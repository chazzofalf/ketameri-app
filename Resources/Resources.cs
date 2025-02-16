namespace Resources;

public class Resources
{
    public static Stream GlyphResource => typeof(Resources).Assembly.GetManifestResourceStream("Resources.Glyphs.json.gz") ?? throw new Exception("Resource Not Found");
}
