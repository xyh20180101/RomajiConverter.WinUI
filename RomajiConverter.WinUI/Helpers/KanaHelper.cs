using System.Text;

namespace RomajiConverter.WinUI.Helpers;

/// <summary>
/// 此类用于片假、平假互转
/// </summary>
public static class KanaHelper
{
    /// <summary>
    /// 转为片假名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToKatakana(string str)
    {
        var stringBuilder = new StringBuilder();
        foreach (var c in str)
        {
            var bytes = Encoding.Unicode.GetBytes(c.ToString());
            if (bytes.Length == 2 && bytes[1] == 0x30 && bytes[0] >= 0x40 && bytes[0] <= 0x9F)
                stringBuilder.Append(Encoding.Unicode.GetString(new[] { (byte)(bytes[0] + 0x60), bytes[1] }));
            else
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// 转为平假名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToHiragana(string str)
    {
        var stringBuilder = new StringBuilder();
        foreach (var c in str)
        {
            var bytes = Encoding.Unicode.GetBytes(c.ToString());
            if (bytes.Length == 2 && bytes[1] == 0x30 && bytes[0] >= 0xA0 && bytes[0] <= 0xFF)
                stringBuilder.Append(Encoding.Unicode.GetString(new[] { (byte)(bytes[0] - 0x60), bytes[1] }));
            else
                stringBuilder.Append(c);
        }

        return stringBuilder.ToString();
    }
}