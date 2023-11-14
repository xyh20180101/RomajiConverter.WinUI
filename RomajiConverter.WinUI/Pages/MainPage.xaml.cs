using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Newtonsoft.Json;
using RomajiConverter.WinUI.Controls;
using RomajiConverter.WinUI.Enums;
using RomajiConverter.WinUI.Helpers;
using RomajiConverter.WinUI.Models;
using WinRT.Interop;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class MainPage : Page
{
    /// <summary>
    /// 当前的转换结果集合
    /// </summary>
    private List<ConvertedLine> _convertedLineList = new();

    private bool _isDetailMode;

    public MainPage()
    {
        InitializeComponent();

        IsDetailMode = App.Config.IsDetailMode;
        DetailModeButton.IsChecked = IsDetailMode;

        EditRomajiCheckBox.Toggled += EditToggleSwitch_OnToggled;
        EditHiraganaCheckBox.Toggled += EditToggleSwitch_OnToggled;
        IsOnlyShowKanjiCheckBox.Toggled += EditToggleSwitch_OnToggled;

        SpaceCheckBox.Toggled += ThirdCheckBox_OnToggled;
        NewLineCheckBox.Toggled += ThirdCheckBox_OnToggled;
        RomajiCheckBox.Toggled += ThirdCheckBox_OnToggled;
        HiraganaCheckBox.Toggled += ThirdCheckBox_OnToggled;
        JPCheckBox.Toggled += ThirdCheckBox_OnToggled;
        KanjiHiraganaCheckBox.Toggled += ThirdCheckBox_OnToggled;
        CHCheckBox.Toggled += ThirdCheckBox_OnToggled;
    }

    /// <summary>
    /// 是否详细模式
    /// </summary>
    public bool IsDetailMode
    {
        get => _isDetailMode;
        set
        {
            _isDetailMode = value;
            App.Config.IsDetailMode = value;
            SwitchMode(_isDetailMode);
        }
    }

    #region 输入区

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

        InputTextBox.Text = stringBuilder.ToString();
    }

    /// <summary>
    /// 转换按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConvertButton_OnClick(object sender, RoutedEventArgs e)
    {
        _convertedLineList =
            RomajiHelper.ToRomaji(InputTextBox.Text, SpaceCheckBox.IsOn, AutoVariantCheckBox.IsChecked.Value);

        if (IsDetailMode)
            RenderEditPanel();
        else
            OutputTextBox.Text = GetResultText();
    }

    #endregion

    #region 编辑区

    /// <summary>
    /// 渲染编辑面板
    /// </summary>
    private void RenderEditPanel()
    {
        EditPanel.Children.Clear();
        for (var i = 0; i < _convertedLineList.Count; i++)
        {
            var item = _convertedLineList[i];

            var line = new WrapPanel();
            foreach (var unit in item.Units)
            {
                var group = new EditableLabelGroup(unit);
                group.RomajiVisibility = EditRomajiCheckBox.IsOn ? Visibility.Visible : Visibility.Collapsed;
                if (EditHiraganaCheckBox.IsOn)
                {
                    if (IsOnlyShowKanjiCheckBox.IsOn && group.Unit.IsKanji == false)
                        group.HiraganaVisibility = HiraganaVisibility.Collapsed;
                    else
                        group.HiraganaVisibility = HiraganaVisibility.Visible;
                }
                else
                {
                    group.HiraganaVisibility = HiraganaVisibility.Collapsed;
                }

                line.Children.Add(group);
            }

            EditPanel.Children.Add(line);
            if (item.Units.Any() && i < _convertedLineList.Count - 1)
                EditPanel.Children.Add(new Grid
                {
                    Height = 1,
                    Background = new SolidColorBrush(Color.FromArgb(170, 170, 170, 170)),
                    Margin = new Thickness(4, 4, 4, 4)
                });
        }
    }

    /// <summary>
    /// 编辑区点击事件(用于单击空白区后令文本框失焦)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditBorder_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        Focus(FocusState.Programmatic);
    }

    /// <summary>
    /// 编辑区的ToggleSwitch通用事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        var senderName = ((ToggleSwitch)sender).Name;
        foreach (object children in EditPanel.Children)
        {
            WrapPanel wrapPanel;
            if (children.GetType() == typeof(WrapPanel))
                wrapPanel = (WrapPanel)children;
            else
                continue;

            foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
                switch (senderName)
                {
                    case "EditRomajiCheckBox":
                        editableLabelGroup.RomajiVisibility =
                            EditRomajiCheckBox.IsOn ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "EditHiraganaCheckBox":
                        if (EditHiraganaCheckBox.IsOn)
                            if (IsOnlyShowKanjiCheckBox.IsOn && !editableLabelGroup.Unit.IsKanji)
                                editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Hidden;
                            else
                                editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Visible;
                        else
                            editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Collapsed;
                        break;
                    case "IsOnlyShowKanjiCheckBox":
                        if (editableLabelGroup.Unit.IsKanji == false)
                            editableLabelGroup.HiraganaVisibility = IsOnlyShowKanjiCheckBox.IsOn
                                ? HiraganaVisibility.Hidden
                                : HiraganaVisibility.Visible;
                        break;
                }
        }
    }

    /// <summary>
    /// 生成文本按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ConvertTextButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        OutputTextBox.Text = GetResultText();
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
                _convertedLineList =
                    JsonConvert.DeserializeObject<List<ConvertedLine>>(await File.ReadAllTextAsync(file.Path));
                RenderEditPanel();
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
            CachedFileManager.DeferUpdates(file);
            await FileIO.WriteTextAsync(file,
                JsonConvert.SerializeObject(_convertedLineList, Formatting.Indented));
            await CachedFileManager.CompleteUpdatesAsync(file);
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
            foreach (var line in _convertedLineList)
            {
                var renderLine = new List<string[]>();
                foreach (var unit in line.Units)
                {
                    var renderUnit = new List<string>();
                    if (EditRomajiCheckBox.IsOn)
                        renderUnit.Add(unit.Romaji);
                    if (EditHiraganaCheckBox.IsOn)
                    {
                        if (IsOnlyShowKanjiCheckBox.IsOn)
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

    #endregion

    #region 生成文本区

    /// <summary>
    /// 生成文本
    /// </summary>
    /// <returns></returns>
    private string GetResultText()
    {
        string GetString(IEnumerable<string> array)
        {
            return string.Join(SpaceCheckBox.IsOn ? " " : "", array);
        }

        var output = new StringBuilder();
        for (var i = 0; i < _convertedLineList.Count; i++)
        {
            var item = _convertedLineList[i];
            if (RomajiCheckBox.IsOn)
                output.AppendLine(GetString(item.Units.Select(p => p.Romaji)));
            if (HiraganaCheckBox.IsOn)
                output.AppendLine(GetString(item.Units.Select(p => p.Hiragana)));
            if (JPCheckBox.IsOn)
            {
                if (KanjiHiraganaCheckBox.IsOn)
                {
                    var japanese = item.Japanese;
                    var leftParenthesis = "(";
                    var rightParenthesis = ")";

                    var kanjiUnitList = item.Units.Where(p => p.IsKanji);
                    foreach (var kanjiUnit in kanjiUnitList)
                    {
                        var kanjiIndex = japanese.IndexOf(kanjiUnit.Japanese);
                        var hiraganaIndex = kanjiIndex + kanjiUnit.Japanese.Length;
                        japanese = japanese.Insert(hiraganaIndex,
                            $"{leftParenthesis}{kanjiUnit.Hiragana}{rightParenthesis}");
                    }

                    output.AppendLine(japanese);
                }
                else
                {
                    output.AppendLine(item.Japanese);
                }
            }

            if (CHCheckBox.IsOn && !string.IsNullOrWhiteSpace(item.Chinese))
                output.AppendLine(item.Chinese);
            if (NewLineCheckBox.IsOn && i < _convertedLineList.Count - 1)
                output.AppendLine();
        }

        if (_convertedLineList.Any())
            output.Remove(output.Length - Environment.NewLine.Length, Environment.NewLine.Length);
        return output.ToString();
    }

    /// <summary>
    /// 生成文本区的ToggleSwitch通用事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ThirdCheckBox_OnToggled(object sender, RoutedEventArgs e)
    {
        OutputTextBox.Text = GetResultText();
    }

    /// <summary>
    /// 复制按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CopyButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(OutputTextBox.Text);
        Clipboard.SetContent(dataPackage);
    }

    #endregion

    #region 切换详细模式

    private ColumnDefinition _editColumnDefinition = new();

    /// <summary>
    /// 切换详细模式
    /// </summary>
    /// <param name="isDetailMode"></param>
    private void SwitchMode(bool isDetailMode)
    {
        if (isDetailMode)
        {
            if (MainGrid.ColumnDefinitions.Count == 2) MainGrid.ColumnDefinitions.Insert(1, _editColumnDefinition);
            EditGrid.Visibility = Visibility.Visible;
            ReadButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
            ConvertPictureButton.Visibility = Visibility.Visible;
            AppBarSeparator2.Visibility = Visibility.Visible;
        }
        else
        {
            ReadButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
            ConvertPictureButton.Visibility = Visibility.Collapsed;
            AppBarSeparator2.Visibility = Visibility.Collapsed;
            _editColumnDefinition = MainGrid.ColumnDefinitions[1];
            EditGrid.Visibility = Visibility.Collapsed;
            if (MainGrid.ColumnDefinitions.Count == 3) MainGrid.ColumnDefinitions.RemoveAt(1);
        }
    }

    /// <summary>
    /// 切换详细模式按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DetailModeButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        IsDetailMode = DetailModeButton.IsChecked.Value;
    }

    #endregion
}