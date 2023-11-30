using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Newtonsoft.Json;
using RomajiConverter.WinUI.Helpers;
using RomajiConverter.WinUI.Models;
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
        ShowLrc(await CloudMusicHelper.GetLrc(CloudMusicHelper.GetLastSongId()));
    }

    /// <summary>
    /// 显示歌词
    /// </summary>
    /// <param name="lrc"></param>
    private void ShowLrc(List<ReturnLrc> lrc)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in lrc)
        {
            stringBuilder.AppendLine(item.JLrc);
            stringBuilder.AppendLine(item.CLrc);
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

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

        var file = await fileOpenPicker.PickSingleFileAsync();
        if (file != null)
            try
            {
                App.ConvertedLineList =
                    JsonConvert.DeserializeObject<List<ConvertedLine>>(await File.ReadAllTextAsync(file.Path));
                MainEditPage.RenderEditPanel();
            }
            catch (JsonSerializationException exception)
            {
                var resourceLoader = ResourceLoader.GetForViewIndependentUse();
                throw new Exception(resourceLoader.GetString("NotValidLyricsFile"), exception);
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

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileSavePicker, hwnd);

        var file = await fileSavePicker.PickSaveFileAsync();
        if (file != null)
        {
            await FileIO.WriteTextAsync(file,
                JsonConvert.SerializeObject(App.ConvertedLineList, Formatting.Indented));
        }
    }

    /// <summary>
    /// 导出图片按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ConvertPictureButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var fileSavePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        fileSavePicker.FileTypeChoices.Add("png", new List<string> { ".png" });

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(fileSavePicker, hwnd);

        var file = await fileSavePicker.PickSaveFileAsync();
        if (file != null)
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
            if (App.Config.IsOpenExplorerAfterSaveImage)
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

    #endregion
}