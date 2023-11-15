using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using RomajiConverter.WinUI.Extensions;

namespace RomajiConverter.WinUI.Models;

public class MyConfig
{
    /// <summary>
    /// 默认设置
    /// </summary>
    public MyConfig()
    {
        IsDetailMode = false;
        InputTextBoxFontSize = 12;
        EditPanelFontSize = 12;
        OutputTextBoxFontSize = 12;
        IsOpenExplorerAfterSaveImage = true;
        LeftParenthesis = "(";
        RightParenthesis = ")";

        InitImageSetting();
    }

    /// <summary>
    /// 生成图片默认设置
    /// </summary>
    public void InitImageSetting()
    {
        FontFamilyName = "微软雅黑";
        FontPixelSize = 48;
        FontColor = Color.Black.ToHexString();
        BackgroundColor = Color.White.ToHexString();
        PagePadding = 24;
        TextMargin = 0;
        LineMargin = 48;
        LinePadding = 12;
    }

    #region 通用设置

    public bool IsDetailMode { get; set; }

    public double InputTextBoxFontSize { get; set; }

    public double EditPanelFontSize { get; set; }

    public double OutputTextBoxFontSize { get; set; }

    public bool IsOpenExplorerAfterSaveImage { get; set; }

    public string LeftParenthesis { get; set; }

    public string RightParenthesis { get; set; }

    #endregion

    #region 导出图片设置

    public string FontFamilyName { get; set; }

    public int FontPixelSize { get; set; }

    public string FontColor { get; set; }

    public string BackgroundColor { get; set; }

    public int PagePadding { get; set; }

    public int TextMargin { get; set; }

    public int LineMargin { get; set; }

    public int LinePadding { get; set; }

    #endregion
}