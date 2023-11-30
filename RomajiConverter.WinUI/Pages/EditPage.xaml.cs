using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using RomajiConverter.WinUI.Controls;
using RomajiConverter.WinUI.Enums;
using RomajiConverter.WinUI.Extensions;
using RomajiConverter.WinUI.ValueConverters;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class EditPage : Page
{
    private static readonly object Lock = new();

    private static readonly Binding FontSizeBinding = new()
    {
        Source = App.Config,
        Path = new PropertyPath("EditPanelFontSize"),
        Mode = BindingMode.OneWay
    };

    private static readonly Binding SeparatorMarginBinding = new()
    {
        Source = App.Config,
        Path = new PropertyPath("EditPanelFontSize"),
        Mode = BindingMode.OneWay,
        Converter = new FontSizeToMarginValueConverter()
    };

    private static readonly SolidColorBrush SeparatorBackground = new(Color.FromArgb(170, 170, 170, 170));

    public EditPage()
    {
        InitializeComponent();

        EditRomajiCheckBox.Toggled += EditToggleSwitch_OnToggled;
        EditHiraganaCheckBox.Toggled += EditToggleSwitch_OnToggled;
        IsOnlyShowKanjiCheckBox.Toggled += EditToggleSwitch_OnToggled;
        BorderVisibilityComboBox.SelectionChanged += BorderVisibilityComboBox_OnSelectionChanged;

        var resourceLoader = ResourceLoader.GetForViewIndependentUse();
        BorderVisibilityComboBox.Items.Add(resourceLoader.GetString("BorderVisibility_Visible"));
        BorderVisibilityComboBox.Items.Add(resourceLoader.GetString("BorderVisibility_Highlight"));
        BorderVisibilityComboBox.Items.Add(resourceLoader.GetString("BorderVisibility_Hidden"));
        BorderVisibilityComboBox.SelectedIndex = 1;
    }

    public OutputPage MainOutputPage { get; set; }

    /// <summary>
    /// ToggleSwitch控件状态
    /// </summary>
    public (bool Romaji, bool Hiragana, bool IsOnlyShowKanji) ToggleSwitchState => (EditRomajiCheckBox.IsOn,
        EditHiraganaCheckBox.IsOn, IsOnlyShowKanjiCheckBox.IsOn);

    /// <summary>
    /// 渲染编辑面板
    /// </summary>
    public void RenderEditPanel()
    {
        lock (Lock)
        {
            foreach (var children in EditPanel.Children)
                if (children.GetType() == typeof(WrapPanel))
                {
                    var wrapPanel = (WrapPanel)children;
                    foreach (var uiElement in wrapPanel.Children)
                    {
                        var editableLabelGroup = (EditableLabelGroup)uiElement;
                        editableLabelGroup.Destroy();
                    }

                    wrapPanel.Children.Clear();
                }
                else if (children.GetType() == typeof(Grid))
                {
                    var grid = (Grid)children;
                    grid.ClearValue(MarginProperty);
                    grid.ClearValue(Panel.BackgroundProperty);
                }

            EditPanel.Children.Clear();
            GC.Collect();

            for (var i = 0; i < App.ConvertedLineList.Count; i++)
            {
                var item = App.ConvertedLineList[i];

                var line = new WrapPanel();
                foreach (var unit in item.Units)
                {
                    var group = new EditableLabelGroup(unit)
                    {
                        RomajiVisibility = EditRomajiCheckBox.IsOn ? Visibility.Visible : Visibility.Collapsed,
                        BorderVisibilitySetting = (BorderVisibilitySetting)BorderVisibilityComboBox.SelectedIndex
                    };
                    group.SetBinding(EditableLabelGroup.MyFontSizeProperty, FontSizeBinding);
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

                if (item.Units.Length != 0 && i < App.ConvertedLineList.Count - 1)
                {
                    var separator = new Grid
                    {
                        Height = 1,
                        Background = SeparatorBackground
                    };
                    separator.SetBinding(MarginProperty, SeparatorMarginBinding);
                    EditPanel.Children.Add(separator);
                }
            }

            EditScrollViewer.ChangeView(0, 0, null, false);
        }
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

            var isLineContainsKanji = wrapPanel.Children.Any(p => ((EditableLabelGroup)p).Unit.IsKanji);

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
                                if (isLineContainsKanji)
                                    editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Hidden;
                                else
                                    editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Collapsed;
                            else
                                editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Visible;
                        else
                            editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Collapsed;
                        break;
                    case "IsOnlyShowKanjiCheckBox":
                        if (EditHiraganaCheckBox.IsOn && editableLabelGroup.Unit.IsKanji == false)
                            if (IsOnlyShowKanjiCheckBox.IsOn)
                                if (isLineContainsKanji)
                                    editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Hidden;
                                else
                                    editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Collapsed;
                            else
                                editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Visible;
                        break;
                }
        }
    }

    /// <summary>
    /// 下拉框选择事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BorderVisibilityComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (object children in EditPanel.Children)
        {
            WrapPanel wrapPanel;
            if (children.GetType() == typeof(WrapPanel))
                wrapPanel = (WrapPanel)children;
            else
                continue;

            foreach (EditableLabelGroup editableLabelGroup in wrapPanel.Children)
            {
                editableLabelGroup.BorderVisibilitySetting = (BorderVisibilitySetting)BorderVisibilityComboBox.SelectedIndex;
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
        MainOutputPage.RenderText();
    }

    /// <summary>
    /// 滚动事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditScrollViewer_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var pointer = e.GetCurrentPoint((UIElement)sender);
        if (pointer.PointerDeviceType != PointerDeviceType.Mouse || KeyboardExtension.IsKeyDown(VirtualKey.Control))
        {
            if (pointer.Properties.MouseWheelDelta < 0 && App.Config.EditPanelFontSize > 3.047)
                App.Config.EditPanelFontSize /= 1.1;
            else if (pointer.Properties.MouseWheelDelta > 0 && App.Config.EditPanelFontSize < 53.1)
                App.Config.EditPanelFontSize *= 1.1;
            e.Handled = true;
        }
    }
}