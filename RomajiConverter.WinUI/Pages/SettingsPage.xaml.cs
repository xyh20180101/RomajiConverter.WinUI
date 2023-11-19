using System;
using System.Drawing.Text;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using RomajiConverter.WinUI.Extensions;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        InitFontFamily();
    }

    private void BackButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo
        {
            Effect = SlideNavigationTransitionEffect.FromLeft
        });
    }

    /// <summary>
    /// 初始化字体下拉框
    /// </summary>
    private void InitFontFamily()
    {
        foreach (var font in new InstalledFontCollection().Families) FontFamilyComboBox.Items.Add(font.Name);
    }

    /// <summary>
    /// 重置按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ResetButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var resourceLoader = ResourceLoader.GetForViewIndependentUse();
        var contentDialog = new ContentDialog
        {
            Title = resourceLoader.GetString("ResetSettingDialogTitle"),
            Content = resourceLoader.GetString("ResetSettingDialogContent"),
            CloseButtonText = resourceLoader.GetString("ResetSettingDialogCancel"),
            PrimaryButtonText = resourceLoader.GetString("ResetSettingDialogConfirm"),
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = Content.XamlRoot
        };

        var result = await contentDialog.ShowAsync();
        if (result == ContentDialogResult.Primary) App.Config.ResetSetting();
    }

    #region 颜色选取

    private void FontColorTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            FontColorPicker.Color = FontColorTextBox.Text.ToDrawingColor().ToWindowsUIColor();
        }
        finally
        {
            FontColorTextBox.Text = App.Config.FontColor;
        }
    }

    private void BackgroundColorTextBox_OnLostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            BackgroundColorPicker.Color = BackgroundColorTextBox.Text.ToDrawingColor().ToWindowsUIColor();
        }
        finally
        {
            BackgroundColorTextBox.Text = App.Config.BackgroundColor;
        }
    }

    #endregion
}