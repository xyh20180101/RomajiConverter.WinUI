using Microsoft.UI.Composition.SystemBackdrops;
using RomajiConverter.WinUI.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace RomajiConverter.WinUI.Models;

public class MyConfig : INotifyPropertyChanged
{
    private bool _isDetailMode;
    private int _windowsWidth;
    private int _windowsHeight;
    private double _inputTextBoxFontSize;
    private double _editPanelFontSize;
    private double _outputTextBoxFontSize;
    private bool _isOpenExplorerAfterSaveImage;
    private string _leftParenthesis;
    private string _rightParenthesis;

    private bool _isParticleAsPronunciation;
    private bool _isAutoScroll;

    private bool _isAIMode;
    private ObservableCollection<OpenAIConfig> _openAIConfigs = [];
    private string _prompt;
    
    private string _fontFamilyName;
    private int _fontPixelSize;
    private string _fontColor;
    private string _backgroundColor;
    private int _pagePadding;
    private int _textMargin;
    private float _wordMargin;
    private int _lineMargin;
    private int _linePadding;

    private MicaKind _micaKind;

    /// <summary>
    /// 默认设置
    /// </summary>
    public MyConfig()
    {
        ResetSetting();
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    /// <summary>
    /// 重置为默认设置
    /// </summary>
    public void ResetSetting()
    {
        WindowWidth = 1400;
        WindowHeight = 800;

        InputTextBoxFontSize = 14;
        EditPanelFontSize = 14;
        OutputTextBoxFontSize = 14;
        IsOpenExplorerAfterSaveImage = true;
        LeftParenthesis = "(";
        RightParenthesis = ")";

        IsParticleAsPronunciation = true;
        IsAutoScroll = true;

        Prompt = string.Empty;

        FontFamilyName = "微软雅黑";
        FontPixelSize = 48;
        FontColor = Color.Black.ToHexString();
        BackgroundColor = Color.White.ToHexString();
        PagePadding = 24;
        TextMargin = 0;
        WordMargin = 0.5f;
        LineMargin = 48;
        LinePadding = 12;

        MicaKind = MicaKind.BaseAlt;
    }

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

    public int WindowWidth
    {
        get => _windowsWidth;
        set
        {
            if (value == _windowsWidth) return;
            _windowsWidth = value;
            OnPropertyChanged();
        }
    }

    public int WindowHeight
    {
        get => _windowsHeight;
        set
        {
            if (value == _windowsHeight) return;
            _windowsHeight = value;
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

    #region 转换设置

    public bool IsParticleAsPronunciation
    {
        get => _isParticleAsPronunciation;
        set
        {
            if (value == _isParticleAsPronunciation) return;
            _isParticleAsPronunciation = value;
            OnPropertyChanged();
        }
    }

    public bool IsAutoScroll
    {
        get => _isAutoScroll;
        set
        {
            if (value == _isAutoScroll) return;
            _isAutoScroll = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region AI设置

    public bool IsAIMode
    {
        get => _isAIMode;
        set
        {
            if (value == _isAIMode) return;
            _isAIMode = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<OpenAIConfig> OpenAIConfigs
    {
        get => _openAIConfigs;
        set
        {
            if (value == _openAIConfigs) return;
            _openAIConfigs = value;
            OnPropertyChanged();
        }
    }

    public string Prompt
    {
        get => _prompt;
        set
        {
            if (value == _prompt) return;
            _prompt = value;
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

    public float WordMargin
    {
        get => _wordMargin;
        set
        {
            if (value.Equals(_wordMargin)) return;
            _wordMargin = value;
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

    public MicaKind MicaKind
    {
        get => _micaKind;
        set
        {
            if (value == _micaKind) return;
            _micaKind = value;
            OnPropertyChanged();
        }
    }
}