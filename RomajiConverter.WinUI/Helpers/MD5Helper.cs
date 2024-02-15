using System.Security.Cryptography;
using System.Text;

namespace RomajiConverter.WinUI.Helpers;

public static class MD5Helper
{
    public static string GetMD5(string str)
    {
        using var md5 = MD5.Create();
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

        var stringBuilder = new StringBuilder();

        foreach (var d in data) stringBuilder.Append(d.ToString("x2"));
        return stringBuilder.ToString();
    }
}