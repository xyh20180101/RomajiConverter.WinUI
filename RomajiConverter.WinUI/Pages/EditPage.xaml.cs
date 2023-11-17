using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.WinUI.UI.Controls;
using RomajiConverter.WinUI.Controls;
using RomajiConverter.WinUI.Enums;
using RomajiConverter.WinUI.ValueConverters;
using Microsoft.UI.Xaml.Media.Animation;
using RomajiConverter.WinUI.Extensions;
using Windows.System;
using Microsoft.UI.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditPage : Page
    {
        public EditPage()
        {
            this.InitializeComponent();

            EditRomajiCheckBox.Toggled += EditToggleSwitch_OnToggled;
            EditHiraganaCheckBox.Toggled += EditToggleSwitch_OnToggled;
            IsOnlyShowKanjiCheckBox.Toggled += EditToggleSwitch_OnToggled;
        }

        /// <summary>
        /// ��Ⱦ�༭���
        /// </summary>
        private void RenderEditPanel()
        {
            EditPanel.Children.Clear();

            var fontSizeBinding = new Binding
            {
                Source = App.Config,
                Path = new PropertyPath("EditPanelFontSize"),
                Mode = BindingMode.OneWay
            };

            var separatorMarginBinding = new Binding
            {
                Source = App.Config,
                Path = new PropertyPath("EditPanelFontSize"),
                Mode = BindingMode.OneWay,
                Converter = new FontSizeToMarginValueConverter()
            };

            for (var i = 0; i < App.ConvertedLineList.Count; i++)
            {
                var item = App.ConvertedLineList[i];

                var line = new WrapPanel();
                foreach (var unit in item.Units)
                {
                    var group = new EditableLabelGroup(unit);
                    group.RomajiVisibility = EditRomajiCheckBox.IsOn ? Visibility.Visible : Visibility.Collapsed;
                    group.SetBinding(EditableLabelGroup.MyFontSizeProperty, fontSizeBinding);
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
                if (item.Units.Any() && i < App.ConvertedLineList.Count - 1)
                {
                    var separator = new Grid
                    {
                        Height = 1,
                        Background = new SolidColorBrush(Color.FromArgb(170, 170, 170, 170))
                    };
                    separator.SetBinding(MarginProperty, separatorMarginBinding);
                    EditPanel.Children.Add(separator);
                }
            }
        }

        /// <summary>
        /// �༭������¼�(���ڵ����հ��������ı���ʧ��)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditBorder_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// �༭����ToggleSwitchͨ���¼�
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
        /// �����ı���ť�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertTextButton_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(OutputPage), null, new SlideNavigationTransitionInfo
            {
                Effect = SlideNavigationTransitionEffect.FromRight
            });
        }

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
}