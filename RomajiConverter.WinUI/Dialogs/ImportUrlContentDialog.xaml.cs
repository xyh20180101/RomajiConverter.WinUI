using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml.Controls;
using RomajiConverter.WinUI.Helpers.LyricsHelpers;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Dialogs;

public sealed partial class ImportUrlContentDialog : ContentDialog, INotifyPropertyChanged
{
    private string _errorText;

    private string _url;

    public ImportUrlContentDialog()
    {
        InitializeComponent();
        Url = string.Empty;
        ErrorText = string.Empty;

        PrimaryButtonClick += OnPrimaryButtonClick;
        Closing += OnClosing;
    }

    public string Url
    {
        get => _url;
        set
        {
            if (value == _url) return;
            _url = value;
            OnPropertyChanged();
        }
    }

    public string ErrorText
    {
        get => _errorText;
        set
        {
            if (value == _errorText) return;
            _errorText = value;
            OnPropertyChanged();
        }
    }

    public List<MultilingualLrc> LrcResult { get; set; } = new();

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void TextBox_OnTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        ErrorText = string.Empty;
    }

    private void OnClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        args.Cancel = args.Result == ContentDialogResult.Primary;
    }

    /*
     * GetLrc方法耗时,导致关闭窗口时LrcResult仍为空
     * 解决方法:禁用PrimaryButton的Close逻辑,手动在OnPrimaryButtonClick方法中关闭窗口
     *
     * 由于Hide无法指定ContentDialogResult,在所有情况下ContentDialogResult都将为None
     * 因此MainPage需要判断LrcResult是否为空,空则不重新渲染歌词
     */
    private async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var url = Url;

        try
        {
            if (url.Contains("music.163.com"))
            {
                var songId = HttpUtility.ParseQueryString(new Uri(url).Query)["id"];

                LrcResult = await CloudMusicLyricsHelper.GetLrc(songId);
            }
            else if (url.Contains("kugou.com"))
            {
                LrcResult = await KuGouMusicLyricsHelper.GetLrc(url);
            }
            else if (url.Contains("y.qq.com"))
            {
                LrcResult = await QQMusicLyricsHelper.GetLrc(url);
            }
            else
            {
                throw new Exception(ResourceLoader.GetForViewIndependentUse().GetString("InvalidUrl"));
            }
        }
        catch (Exception e)
        {
            ErrorText = e.Message;
            return;
        }

        Hide();
    }
}