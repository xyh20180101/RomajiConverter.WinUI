using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Helpers;

public static class GenerateImageHelper
{
    public static Bitmap ToImage(this List<string[][]> list, ImageSetting setting)
    {
        if (list.Any() == false || list[0].Any() == false || list[0][0].Any() == false)
            return new Bitmap(1, 1);

        var fontSize = setting.FontPixelSize;
        var pagePadding = setting.PagePadding;
        var textMargin = setting.TextMargin;
        var lineMargin = setting.LineMargin;
        var linePadding = setting.LinePadding;

        var font = new Font(setting.FontFamilyName, fontSize, GraphicsUnit.Pixel);
        var brush = new SolidBrush(setting.FontColor);
        var background = setting.BackgroundColor;

        //(最长句的渲染长度,该句的单元数)
        var longestLine = list.Select(p => new { MaxLength = p.Sum(q => GetUnitLength(q, font)), UnitCount = p.Length }).MaxBy(p => p.MaxLength);

        //最长句子的渲染长度
        var maxLength = longestLine?.MaxLength ?? 0;
        //最大单元数
        var maxUnitCount = longestLine?.UnitCount ?? 0;
        //图片宽度
        var width = maxLength + maxUnitCount * textMargin + pagePadding * 2;
        //图片高度
        var height = list.Count * (list[0][0].Length * fontSize + linePadding) + list.Count * lineMargin + pagePadding * 2;
        var image = new Bitmap(width, height);

        using var g1 = Graphics.FromImage(image);
        g1.Clear(background);
        g1.InterpolationMode = InterpolationMode.High;
        g1.SmoothingMode = SmoothingMode.HighQuality;
        g1.CompositingQuality = CompositingQuality.HighQuality;
        g1.TextRenderingHint = TextRenderingHint.AntiAlias;
        if (brush.Color.A == 0)
            g1.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

        for (var i = 0; i < list.Count; i++)
        {
            var line = list[i];
            var startX = pagePadding + textMargin;
            foreach (var unit in line)
            {
                var unitLength = GetUnitLength(unit, font);
                var renderXArray = unit.Select(str => startX + GetStringXOffset(str, font, unitLength)).ToArray();
                var renderYArray = unit.Select((str, index) =>
                    pagePadding + (fontSize * unit.Length + linePadding + lineMargin) * i +
                    index * (fontSize + linePadding)).ToArray();
                for (var j = 0; j < unit.Length; j++)
                    g1.DrawString(unit[j], font, brush, new PointF(renderXArray[j], renderYArray[j]));
                startX += unitLength + textMargin;
            }
        }

        return image;
    }

    /// <summary>
    /// 获取单元长度(最长字符串渲染长度)
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="font"></param>
    /// <returns></returns>
    private static int GetUnitLength(string[] unit, Font font)
    {
        using var g = Graphics.FromImage(new Bitmap(1, 1));
        return unit.Any() ? unit.Max(p => (int)g.MeasureString(p, font).Width) : 0;
    }

    /// <summary>
    /// 获取字符串的渲染坐标x轴偏移值
    /// </summary>
    /// <param name="str"></param>
    /// <param name="font"></param>
    /// <param name="unitLength"></param>
    /// <returns></returns>
    private static int GetStringXOffset(string str, Font font, int unitLength)
    {
        using var g = Graphics.FromImage(new Bitmap(1, 1));
        return (unitLength - (int)g.MeasureString(str, font).Width) / 2;
    }

    public class ImageSetting
    {
        public ImageSetting()
        {
        }

        public ImageSetting(MyConfig config)
        {
            FontFamilyName = config.FontFamilyName;
            FontPixelSize = config.FontPixelSize;
            PagePadding = config.PagePadding;
            TextMargin = config.TextMargin;
            LineMargin = config.LineMargin;
            LinePadding = config.LinePadding;
            BackgroundColor = (Color)new ColorConverter().ConvertFromString(config.BackgroundColor);
            FontColor = (Color)new ColorConverter().ConvertFromString(config.FontColor);
        }

        public string FontFamilyName { get; set; }

        public int FontPixelSize { get; set; }

        public int PagePadding { get; set; }

        public int TextMargin { get; set; }

        public int LineMargin { get; set; }

        public int LinePadding { get; set; }

        public Color BackgroundColor { get; set; }

        public Color FontColor { get; set; }
    }
}