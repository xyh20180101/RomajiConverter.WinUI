using System.Drawing;

namespace RomajiConverter.WinUI.Extensions;

public static class ColorExtension
{
    public static string ToHexString(this Color color)
    {
        return "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
    }

    public static Color ToDrawingColor(this string hexString)
    {
        return (Color)new ColorConverter().ConvertFromString(hexString);
    }

    public static Windows.UI.Color ToWindowsUIColor(this Color color)
    {
        return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static Color ToDrawingColor(this Windows.UI.Color color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }
}