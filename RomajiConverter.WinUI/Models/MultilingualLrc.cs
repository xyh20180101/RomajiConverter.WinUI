using System;

namespace RomajiConverter.WinUI.Models;

public class MultilingualLrc
{
    public MultilingualLrc()
    {
        Time = TimeSpan.Zero;
        JLrc = "";
        CLrc = "";
    }

    /// <summary>
    /// 时间
    /// </summary>
    public TimeSpan Time { get; set; }

    /// <summary>
    /// 日词
    /// </summary>
    public string JLrc { get; set; }

    /// <summary>
    /// 中词
    /// </summary>
    public string CLrc { get; set; }
}