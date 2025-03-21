using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace Resources;

public class Resources
{
    private static Stream GetResource(string name) =>  typeof(Resources).Assembly.GetManifestResourceStream($"Resources.{name}") ?? throw new Exception($"Resource Not Found: {name}");
    private static string GetString(string name) 
    {
        var text = "";
        using (var rsrc = new StreamReader(GetResource(name),Encoding.UTF8))
        {
            if (rsrc != null)
            {
                text = rsrc.ReadToEnd();
            }
            else
            {
                throw new Exception($"Could not get string from resource: {name}");
            }
        }
        return text;
    }
    public static Stream GlyphResource => GetResource("Glyphs.json.gz"); // All The Ketameri Symbols in a nice file so I don't have to regenerate them laboriously every time run the app. Saves time.
    public static Stream Testing_ScoutTestResource => GetResource("Scout.png"); //My Dog.
    public static string Testing_LetThereBeLight => GetString("lettherebelight.txt"); // A Favorite Verse. 
    public static string Testing_LetThereBeLightNoLines => Testing_LetThereBeLight.Replace("\n"," ");
    public static string Testing_PowerInFaith => GetString("powerinfaith.txt"); // Another Favorite Verse. 
    public static string Testing_PowerInFaithNoLines => Testing_PowerInFaith.Replace("\n"," ");
    public static string About_text => GetString("TextResources.about.txt"); // About Page Message
    public static string[] Names => typeof(Resources).Assembly.GetManifestResourceNames();
}
