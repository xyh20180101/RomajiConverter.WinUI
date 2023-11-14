﻿using System.Drawing;

namespace RomajiConverter.WinUI.Extensions;

public static class ColorExtension
{
    public static string ToHexString(this Color color)
    {
        return "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
    }
}