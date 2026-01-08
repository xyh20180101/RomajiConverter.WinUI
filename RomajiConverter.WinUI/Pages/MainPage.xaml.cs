using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using RomajiConverter.Core.Models;
using RomajiConverter.WinUI.Dialogs;
using RomajiConverter.WinUI.Helpers;
using RomajiConverter.WinUI.Helpers.LyricsHelpers;
using RomajiConverter.WinUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        //提供跨页面操作对象
        MainInputPage.MainPage = this;
        MainInputPage.MainEditPage = MainEditPage;
        MainInputPage.MainOutputPage = MainOutputPage;

        MainEditPage.MainOutputPage = MainOutputPage;
    }

    #region 菜单栏

    /// <summary>
    /// 导入网易云歌词
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ImportCloudMusicButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            ShowLrc(await CloudMusicLyricsHelper.GetLrc(CloudMusicLyricsHelper.GetLastSongId()));
        }
        catch (Exception ex)
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();
            await new ContentDialog
            {
                XamlRoot = Content.XamlRoot,
                Title = resourceLoader.GetString("Exception"),
                Content = ex.Message,
                CloseButtonText = resourceLoader.GetString("Close"),
                DefaultButton = ContentDialogButton.Close
            }.ShowAsync();
        }
    }

    /// <summary>
    /// 通过链接导入歌词
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ImportUrlButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ImportUrlContentDialog
        {
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        var dialogResult = await dialog.ShowAsync();

        if (dialog.LrcResult.Count != 0) ShowLrc(dialog.LrcResult);
    }

    /// <summary>
    /// 显示歌词
    /// </summary>
    /// <param name="lrc"></param>
    private void ShowLrc(List<MultilingualLrc> lrc)
    {
        var stringBuilder = new StringBuilder();

        if (lrc.Select(p => p.CLrc).All(p => p.Length == 0))
            foreach (var item in lrc)
                stringBuilder.AppendLine($"[{item.Time:mm\\:ss\\.fff}]{item.JLrc}");
        else
        {
            foreach (var item in lrc)
            {
                stringBuilder.AppendLine($"[{item.Time:mm\\:ss\\.fff}]{item.JLrc}");
                stringBuilder.AppendLine($"[{item.Time:mm\\:ss\\.fff}]{item.CLrc}");
            }
        }
        MainInputPage.SetTextBoxText(stringBuilder.ToString());
    }

    /// <summary>
    /// 打开按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private async void ReadButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var fileOpenPicker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        fileOpenPicker.FileTypeFilter.Add(".json");
        fileOpenPicker.FileTypeFilter.Add(".lrc");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

        var file = await fileOpenPicker.PickSingleFileAsync();
        if (file != null)
        {
            switch (file.FileType)
            {
                case ".json":
                    try
                    {
                        App.ConvertedLineList = JsonSerializer.Deserialize<ObservableCollection<ConvertedLine>>(await File.ReadAllTextAsync(file.Path));
                    }
                    catch (JsonException exception)
                    {
                        var resourceLoader = ResourceLoader.GetForViewIndependentUse();
                        throw new Exception(resourceLoader.GetString("NotValidLyricsFile"), exception);
                    }
                    break;
                case ".lrc":
                    MainInputPage.SetTextBoxText(await File.ReadAllTextAsync(file.Path));
                    break;
            }
        }
    }

    /// <summary>
    /// 保存按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var fileSavePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        fileSavePicker.FileTypeChoices.Add("json", new List<string> { ".json" });
        fileSavePicker.FileTypeChoices.Add("lrc", new List<string> { ".lrc" });
        fileSavePicker.FileTypeChoices.Add("png", new List<string> { ".png" });

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileSavePicker, hwnd);

        var file = await fileSavePicker.PickSaveFileAsync();
        if (file != null)
        {
            switch (file.FileType)
            {
                case ".json":
                    await FileIO.WriteTextAsync(file,
                        JsonSerializer.Serialize(App.ConvertedLineList, new JsonSerializerOptions
                        {
                            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        }));
                    break;
                case ".lrc":
                    await FileIO.WriteTextAsync(file, MainOutputPage.GetResultText());
                    break;
                case ".png":
                    {
                        var renderData = new List<string[][]>();
                        foreach (var line in App.ConvertedLineList)
                        {
                            var renderLine = new List<string[]>();
                            foreach (var unit in line.Units)
                            {
                                var renderUnit = new List<string>();
                                if (MainEditPage.ToggleSwitchState.Romaji)
                                    renderUnit.Add(unit.Romaji);
                                if (MainEditPage.ToggleSwitchState.Hiragana)
                                {
                                    if (MainEditPage.ToggleSwitchState.IsOnlyShowKanji)
                                        renderUnit.Add(unit.IsKanji ? unit.Hiragana : " ");
                                    else
                                        renderUnit.Add(unit.Hiragana);
                                }

                                renderUnit.Add(unit.Japanese);
                                renderLine.Add(renderUnit.ToArray());
                            }

                            renderData.Add(renderLine.ToArray());
                        }

                        using var image = renderData.ToImage(new GenerateImageHelper.ImageSetting(App.Config));
                        image.Save(file.Path, ImageFormat.Png);
                    }
                    break;
            }
            if (App.Config.IsOpenExplorerAfterSave)
                Process.Start("explorer.exe", $"/select,\"{file.Path}\"");
        }
    }

    /// <summary>
    /// 设置按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void SettingButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        Frame.Navigate(typeof(SettingsPage), null, new SlideNavigationTransitionInfo
        {
            Effect = SlideNavigationTransitionEffect.FromRight
        });
        GC.Collect();
    }

    public void SetButtonIsEnabled(bool isEnabled)
    {
        DetailModeButton.IsEnabled = isEnabled;
    }

    #endregion
}