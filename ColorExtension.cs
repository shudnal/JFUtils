using System.ComponentModel;

namespace JFUtils;

[PublicAPI, Description("Thanks to KG utils")]
public static class ColorExtension
{
    public static Color IncreaseColorLight(this Color c)
    {
        Color.RGBToHSV(c, out var h, out var s, out var v);
        v = 1f;
        c = Color.HSVToRGB(h, s, v);
        return c;
    }
    
    public static string IncreaseColorLight(this string color)
    {
        if (!ColorUtility.TryParseHtmlString(color, out var c)) return color;
        Color.RGBToHSV(c, out var h, out var s, out var v);
        v = 1f;
        c = Color.HSVToRGB(h, s, v);
        return "#" + ColorUtility.ToHtmlStringRGB(c);
    }
}