using CommunityToolkit.WinUI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using RomajiConverter.Core.Models;
using RomajiConverter.WinUI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class OutputPage : Page
{
    public OutputPage()
    {
        InitializeComponent();

        SpaceCheckBox.Toggled += ThirdCheckBox_OnToggled;
        NewLineCheckBox.Toggled += ThirdCheckBox_OnToggled;
        TimeCheckBox.Toggled += ThirdCheckBox_OnToggled;
        RomajiCheckBox.Toggled += ThirdCheckBox_OnToggled;
        HiraganaCheckBox.Toggled += ThirdCheckBox_OnToggled;
        JPCheckBox.Toggled += ThirdCheckBox_OnToggled;
        KanjiHiraganaCheckBox.Toggled += ThirdCheckBox_OnToggled;
        CHCheckBox.Toggled += ThirdCheckBox_OnToggled;

        App.ConvertedLineList.CollectionChanged += ConvertedLineListOnCollectionChanged;
    }

    private void ConvertedLineListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (App.Config.IsDetailMode) return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    ((ConvertedLine)e.NewItems[0]).Units.CollectionChanged += UnitsOnCollectionChanged;
                    break;
                }
            case NotifyCollectionChangedAction.Remove:
                {
                    ((ConvertedLine)e.OldItems[0]).Units.CollectionChanged -= UnitsOnCollectionChanged;
                    break;
                }
            case NotifyCollectionChangedAction.Replace:
                {
                    ((ConvertedLine)e.OldItems[0]).Units.CollectionChanged -= UnitsOnCollectionChanged;
                    ((ConvertedLine)e.NewItems[0]).Units.CollectionChanged += UnitsOnCollectionChanged;
                    break;
                }
        }

        RenderText();

        if (App.Config.IsAutoScroll && OutputTextBox.FindDescendant<ScrollViewer>() is { } sv)
        {
            sv.ChangeView(null, sv.ExtentHeight, null, true);
        }
    }

    private void UnitsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        RenderText();

        if (App.Config.IsAutoScroll && OutputTextBox.FindDescendant<ScrollViewer>() is { } sv)
        {
            sv.ChangeView(null, sv.ExtentHeight, null, true);
        }
    }

    public void ClearText()
    {
        OutputTextBox.Text = string.Empty;
    }

    /// <summary>
    /// 显示文本
    /// </summary>
    public void RenderText()
    {
        OutputTextBox.Text = GetResultText();
    }

    /// <summary>
    /// 获取结果文本
    /// </summary>
    /// <returns></returns>
    private string GetResultText()
    {
        string GetString(IEnumerable<string> array)
        {
            return string.Join(SpaceCheckBox.IsOn ? " " : "", array);
        }

        var output = new StringBuilder();

        var time = string.Empty;
        for (var i = 0; i < App.ConvertedLineList.Count; i++)
        {
            var item = App.ConvertedLineList[i];
            if (TimeCheckBox.IsOn)
                time = $"[{item.Time:mm\\:ss\\.fff}]";
            if (RomajiCheckBox.IsOn)
                output.AppendLine(time + GetString(item.Units.Select(p => p.Romaji)));
            if (HiraganaCheckBox.IsOn)
                output.AppendLine(time + GetString(item.Units.Select(p => p.Hiragana)));
            if (JPCheckBox.IsOn)
            {
                if (KanjiHiraganaCheckBox.IsOn)
                {
                    var japanese = item.Japanese;
                    var leftParenthesis = App.Config.LeftParenthesis;
                    var rightParenthesis = App.Config.RightParenthesis;

                    var kanjiUnitList = item.Units.Where(p => p.IsKanji);
                    var replacedIndex = 0;
                    foreach (var kanjiUnit in kanjiUnitList)
                    {
                        var kanjiIndex = japanese.IndexOf(kanjiUnit.Japanese, replacedIndex);
                        var hiraganaIndex = kanjiIndex + kanjiUnit.Japanese.Length;
                        japanese = japanese.Insert(hiraganaIndex,
                            $"{leftParenthesis}{kanjiUnit.Hiragana}{rightParenthesis}");
                        replacedIndex = hiraganaIndex;
                    }

                    output.AppendLine(time + japanese);
                }
                else
                {
                    output.AppendLine(time + item.Japanese);
                }
            }

            if (CHCheckBox.IsOn && !string.IsNullOrWhiteSpace(item.Chinese))
                output.AppendLine(time + item.Chinese);
            if (NewLineCheckBox.IsOn && i < App.ConvertedLineList.Count - 1)
                output.AppendLine();
        }

        if (App.ConvertedLineList.Any())
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

    /// <summary>
    /// 滚动事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OutputTextBox_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var pointer = e.GetCurrentPoint((UIElement)sender);
        if (pointer.PointerDeviceType != PointerDeviceType.Mouse || KeyboardExtension.IsKeyDown(VirtualKey.Control))
        {
            if (pointer.Properties.MouseWheelDelta < 0 && App.Config.OutputTextBoxFontSize > 3.047)
                App.Config.OutputTextBoxFontSize /= 1.1;
            else if (pointer.Properties.MouseWheelDelta > 0 && App.Config.OutputTextBoxFontSize < 53.1)
                App.Config.OutputTextBoxFontSize *= 1.1;
            e.Handled = true;
        }
    }
}