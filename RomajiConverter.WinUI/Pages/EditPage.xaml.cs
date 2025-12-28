using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using RomajiConverter.Core.Models;
using RomajiConverter.WinUI.Controls;
using RomajiConverter.WinUI.Enums;
using RomajiConverter.WinUI.Extensions;
using RomajiConverter.WinUI.ValueConverters;
using System.Collections.Specialized;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI;
using CommunityToolkit.WinUI;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class EditPage : Page
{
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

        App.ConvertedLineList.CollectionChanged += ConvertedLineListOnCollectionChanged;
    }

    public OutputPage MainOutputPage { get; set; }

    /// <summary>
    /// ToggleSwitch控件状态
    /// </summary>
    public (bool Romaji, bool Hiragana, bool IsOnlyShowKanji) ToggleSwitchState => (EditRomajiCheckBox.IsOn,
        EditHiraganaCheckBox.IsOn, IsOnlyShowKanjiCheckBox.IsOn);

    private void ConvertedLineListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!App.Config.IsDetailMode) return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    var lineData = (ConvertedLine)e.NewItems[0];
                    var line = GetLine(lineData);
                    lineData.Units.CollectionChanged += UnitsOnCollectionChanged;

                    EditPanel.Children.Insert(e.NewStartingIndex * 2, line);
                    EditPanel.Children.Insert(e.NewStartingIndex * 2 + 1, GetSeparator());
                    break;
                }
            case NotifyCollectionChangedAction.Remove:
                {
                    ((ConvertedLine)e.OldItems[0]).Units.CollectionChanged -= UnitsOnCollectionChanged;

                    DestroyLine((WrapPanel)EditPanel.Children[e.OldStartingIndex * 2]);
                    DestroySeparator((Grid)EditPanel.Children[e.OldStartingIndex * 2 + 1]);

                    EditPanel.Children.RemoveAt(e.OldStartingIndex * 2);
                    EditPanel.Children.RemoveAt(e.OldStartingIndex * 2 + 1);
                    break;
                }
            case NotifyCollectionChangedAction.Replace:
                {
                    var lineData = (ConvertedLine)e.NewItems[0];
                    var line = GetLine(lineData);

                    ((ConvertedLine)e.OldItems[0]).Units.CollectionChanged -= UnitsOnCollectionChanged;
                    ((ConvertedLine)e.NewItems[0]).Units.CollectionChanged += UnitsOnCollectionChanged;

                    DestroySeparator((Grid)EditPanel.Children[e.OldStartingIndex * 2 + 1]);

                    EditPanel.Children.RemoveAt(e.OldStartingIndex * 2);
                    EditPanel.Children.Insert(e.NewStartingIndex * 2, line);
                    break;
                }
            case NotifyCollectionChangedAction.Move:
                {
                    EditPanel.Children.Move((uint)e.OldStartingIndex * 2, (uint)e.NewStartingIndex * 2);
                    break;
                }
            case NotifyCollectionChangedAction.Reset:
                {
                    foreach (var children in EditPanel.Children)
                        if (children is WrapPanel wrapPanel)
                        {
                            DestroyLine(wrapPanel);
                        }
                        else if (children is Grid grid)
                        {
                            DestroySeparator(grid);
                        }
                    EditPanel.Children.Clear();
                    break;
                }
        }
    }

    private WrapPanel GetLine(ConvertedLine data)
    {
        var wrapPanel = new WrapPanel
        {
            ChildrenTransitions = [new EntranceThemeTransition()]
        };
        foreach (var unitData in data.Units)
            wrapPanel.Children.Add(GetUnit(unitData));
        return wrapPanel;
    }

    private void DestroyLine(WrapPanel wrapPanel)
    {
        foreach (var uiElement in wrapPanel.Children)
        {
            var editableLabelGroup = (EditableLabelGroup)uiElement;
            editableLabelGroup.Destroy();
        }
        wrapPanel.Children.Clear();
    }

    private Grid GetSeparator()
    {
        var separator = new Grid
        {
            ChildrenTransitions = [new EntranceThemeTransition()],
            Height = 1,
            Background = SeparatorBackground
        };
        separator.SetBinding(MarginProperty, SeparatorMarginBinding);
        return separator;
    }

    private void DestroySeparator(Grid grid)
    {
        grid.ClearValue(MarginProperty);
        grid.ClearValue(Panel.BackgroundProperty);
    }

    private void UnitsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                {
                    var unitData = (ConvertedUnit)e.NewItems[0];
                    var line = (WrapPanel)EditPanel.Children[unitData.LineIndex * 2];

                    line.Children.Insert(e.NewStartingIndex, GetUnit(unitData));
                    break;
                }
            case NotifyCollectionChangedAction.Remove:
                {
                    var unitData = (ConvertedUnit)e.OldItems[0];
                    var line = (WrapPanel)EditPanel.Children[unitData.LineIndex * 2];

                    ((EditableLabelGroup)line.Children[e.OldStartingIndex]).Destroy();

                    line.Children.RemoveAt(e.OldStartingIndex);
                    break;
                }
            case NotifyCollectionChangedAction.Replace:
                {
                    var unitData = (ConvertedUnit)e.NewItems[0];
                    var line = (WrapPanel)EditPanel.Children[unitData.LineIndex * 2];

                    ((EditableLabelGroup)line.Children[e.OldStartingIndex]).Destroy();

                    line.Children.Insert(e.NewStartingIndex, GetUnit(unitData));
                    line.Children.RemoveAt(e.OldStartingIndex);
                    break;
                }
            case NotifyCollectionChangedAction.Move:
                {
                    var unit = (ConvertedUnit)e.NewItems[0];
                    var wrapPanel = (WrapPanel)EditPanel.Children[unit.LineIndex * 2];

                    wrapPanel.Children.Move((uint)e.OldStartingIndex * 2, (uint)e.NewStartingIndex * 2);
                    break;
                }
            case NotifyCollectionChangedAction.Reset:
                {
                    if (e.OldItems == null || e.OldItems.Count == 0) break;
                    var unit = (ConvertedUnit)e.OldItems[0];
                    var wrapPanel = (WrapPanel)EditPanel.Children[unit.LineIndex * 2];

                    foreach (var uiElement in wrapPanel.Children)
                    {
                        ((EditableLabelGroup)uiElement).Destroy();
                    }

                    wrapPanel.Children.Clear();
                    break;
                }
        }

        if(App.Config.IsAutoScroll)
            _ = DispatcherQueue.GetForCurrentThread()
                .EnqueueAsync(() =>
                {
                    if (EditScrollViewer.ExtentHeight > EditScrollViewer.ViewportHeight)
                    {
                        EditScrollViewer.ChangeView(
                            horizontalOffset: null,
                            verticalOffset: EditScrollViewer.ExtentHeight,
                            zoomFactor: null,
                            disableAnimation: true);
                    }
                });
    }

    private EditableLabelGroup GetUnit(ConvertedUnit data)
    {
        var group = new EditableLabelGroup(data)
        {
            OpacityTransition = new ScalarTransition(),
            RomajiVisibility = EditRomajiCheckBox.IsOn ? Visibility.Visible : Visibility.Collapsed,
            BorderVisibilitySetting = (BorderVisibilitySetting)BorderVisibilityComboBox.SelectedIndex
        };
        group.SetBinding(EditableLabelGroup.MyFontSizeProperty, FontSizeBinding);
        if (EditHiraganaCheckBox.IsOn)
        {
            if (IsOnlyShowKanjiCheckBox.IsOn && group.Unit.IsKanji == false)
                group.HiraganaVisibility = HiraganaVisibility.Hidden;
            else
                group.HiraganaVisibility = HiraganaVisibility.Visible;
        }
        else
        {
            group.HiraganaVisibility = HiraganaVisibility.Collapsed;
        }
        return group;
    }

    /// <summary>
    /// 编辑区的ToggleSwitch通用事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EditToggleSwitch_OnToggled(object sender, RoutedEventArgs e)
    {
        var senderName = ((ToggleSwitch)sender).Name;
        foreach (var children in EditPanel.Children)
        {
            if (children is not WrapPanel wrapPanel)
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
                        if (EditHiraganaCheckBox.IsOn && !editableLabelGroup.Unit.IsKanji)
                            if (IsOnlyShowKanjiCheckBox.IsOn)
                                editableLabelGroup.HiraganaVisibility = HiraganaVisibility.Hidden;
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
                editableLabelGroup.BorderVisibilitySetting =
                    (BorderVisibilitySetting)BorderVisibilityComboBox.SelectedIndex;
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

    public void ShowLoading(bool isShow)
    {
        if (isShow)
        {
            EditPanel.Children.Insert(EditPanel.Children.Count, new ProgressRing
            {
                Margin = new Thickness(0, 16,0,0)
            });
        }
        else
        {
            if (EditPanel.Children.LastOrDefault() is ProgressRing ring)
            {
                EditPanel.Children.RemoveAt(EditPanel.Children.Count - 1);
            }
        }
    }
}