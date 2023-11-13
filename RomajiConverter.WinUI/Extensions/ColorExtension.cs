using System;
using System.Collections.Generic;
using System.Text;

namespace RomajiConverter.WinUI.Extensions
{
    public static class ColorExtension
    {
        public static string ToHexString(this System.Drawing.Color color)
        {
            return "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
