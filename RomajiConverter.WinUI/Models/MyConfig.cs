using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using RomajiConverter.WinUI.Extensions;

namespace RomajiConverter.WinUI.Models;

public class MyConfig : INotifyPropertyChanged
{
    private bool _isDetailMode;
    private double _inputTextBoxFontSize;
    private double _editPanelFontSize;
    private double _outputTextBoxFontSize;
    private bool _isOpenExplorerAfterSaveImage;
    private string _leftParenthesis;
    private string _rightParenthesis;
    private string _fontFamilyName;
    private int _fontPixelSize;
    private string _fontColor;
    private string _backgroundColor;
    private int _pagePadding;
    private int _textMargin;
    private int _lineMargin;
    private int _linePadding;

    /// <summary>
    /// 默认设置
    /// </summary>
    public MyConfig()
    {
        ResetSetting();
    }

    /// <summary>
    /// 重置为默认设置
    /// </summary>
    public void ResetSetting()
    {
        InputTextBoxFontSize = 14;
        EditPanelFontSize = 14;
        OutputTextBoxFontSize = 14;
        IsOpenExplorerAfterSaveImage = true;
        LeftParenthesis = "(";
        RightParenthesis = ")";

        FontFamilyName = "微软雅黑";
        FontPixelSize = 48;
        FontColor = Color.Black.ToHexString();
        BackgroundColor = Color.White.ToHexString();
        PagePadding = 24;
        TextMargin = 0;
        LineMargin = 48;
        LinePadding = 12;
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region 通用设置

    public bool IsDetailMode
    {
        get => _isDetailMode;
        set
        {
            if (value == _isDetailMode) return;
            _isDetailMode = value;
            OnPropertyChanged();
        }
    }

    public double InputTextBoxFontSize
    {
        get => _inputTextBoxFontSize;
        set
        {
            if (value == _inputTextBoxFontSize) return;
            _inputTextBoxFontSize = value;
            OnPropertyChanged();
        }
    }

    public double EditPanelFontSize
    {
        get => _editPanelFontSize;
        set
        {
            if (value == _editPanelFontSize) return;
            _editPanelFontSize = value;
            OnPropertyChanged();
        }
    }

    public double OutputTextBoxFontSize
    {
        get => _outputTextBoxFontSize;
        set
        {
            if (value == _outputTextBoxFontSize) return;
            _outputTextBoxFontSize = value;
            OnPropertyChanged();
        }
    }

    public bool IsOpenExplorerAfterSaveImage
    {
        get => _isOpenExplorerAfterSaveImage;
        set
        {
            if (value == _isOpenExplorerAfterSaveImage) return;
            _isOpenExplorerAfterSaveImage = value;
            OnPropertyChanged();
        }
    }

    public string LeftParenthesis
    {
        get => _leftParenthesis;
        set
        {
            if (value == _leftParenthesis) return;
            _leftParenthesis = value;
            OnPropertyChanged();
        }
    }

    public string RightParenthesis
    {
        get => _rightParenthesis;
        set
        {
            if (value == _rightParenthesis) return;
            _rightParenthesis = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region 导出图片设置

    public string FontFamilyName
    {
        get => _fontFamilyName;
        set
        {
            if (value == _fontFamilyName) return;
            _fontFamilyName = value;
            OnPropertyChanged();
        }
    }

    public int FontPixelSize
    {
        get => _fontPixelSize;
        set
        {
            if (value == _fontPixelSize) return;
            _fontPixelSize = value;
            OnPropertyChanged();
        }
    }

    public string FontColor
    {
        get => _fontColor;
        set
        {
            if (value == _fontColor) return;
            _fontColor = value;
            OnPropertyChanged();
        }
    }

    public string BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (value == _backgroundColor) return;
            _backgroundColor = value;
            OnPropertyChanged();
        }
    }

    public int PagePadding
    {
        get => _pagePadding;
        set
        {
            if (value == _pagePadding) return;
            _pagePadding = value;
            OnPropertyChanged();
        }
    }

    public int TextMargin
    {
        get => _textMargin;
        set
        {
            if (value == _textMargin) return;
            _textMargin = value;
            OnPropertyChanged();
        }
    }

    public int LineMargin
    {
        get => _lineMargin;
        set
        {
            if (value == _lineMargin) return;
            _lineMargin = value;
            OnPropertyChanged();
        }
    }

    public int LinePadding
    {
        get => _linePadding;
        set
        {
            if (value == _linePadding) return;
            _linePadding = value;
            OnPropertyChanged();
        }
    }

    #endregion
}