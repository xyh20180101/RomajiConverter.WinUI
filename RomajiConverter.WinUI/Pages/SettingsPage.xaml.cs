using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System.Drawing;
using System.Drawing.Text;
using CommunityToolkit.WinUI.Helpers;
using RomajiConverter.WinUI.Extensions;
using RomajiConverter.WinUI.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI.Pages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
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

    private void InitFontFamily()
    {
        foreach (var font in new InstalledFontCollection().Families)
        {
            FontFamilyComboBox.Items.Add(font.Name);
        }
    }
    private void InitAllConfig(MyConfig config)
    {
        IsOpenExplorerAfterSaveImageToggleSwitch.IsOn = config.IsOpenExplorerAfterSaveImage;
        LeftParenthesisTextBox.Text = config.LeftParenthesis;
        RightParenthesisTextBox.Text = config.RightParenthesis;

        FontFamilyComboBox.SelectedValue = config.FontFamilyName;
        FontPixelSizeSlider.Value = config.FontPixelSize;
        FontColorPicker.Color = config.FontColor.ToDrawingColor().ToWindowsUIColor();
        BackgroundColorPicker.Color = config.BackgroundColor.ToDrawingColor().ToWindowsUIColor();
        PagePaddingSlider.Value = config.PagePadding;
        TextMarginSlider.Value = config.TextMargin;
        LineMarginSlider.Value = config.LineMargin;
        LinePaddingSlider.Value = config.LinePadding;
    }

    private async void ResetButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var contentDialog = new ContentDialog
        {
            Title = "是否恢复默认设置？",
            Content = "现有的设置将不会被保存。",
            CloseButtonText = "取消",
            PrimaryButtonText = "确认",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.Content.XamlRoot
        };

        var result = await contentDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            InitAllConfig(new MyConfig());
        }
    }
}