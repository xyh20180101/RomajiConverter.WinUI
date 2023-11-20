using System;
using System.Drawing.Text;
using Windows.ApplicationModel.Resources;
using ABI.Windows.Web.Http;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RomajiConverter.WinUI.Extensions;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using ResourceLoader = Windows.ApplicationModel.Resources.ResourceLoader;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        InitFontFamily();
        VersionTextBlock.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
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

    /// <summary>
    /// 检查更新
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void UpdateButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        var resourceLoader = ResourceLoader.GetForViewIndependentUse();
        try
        {
            UpdateButton.Visibility = Visibility.Collapsed;
            UpdateRing.IsActive = true;
            UpdateRing.Visibility = Visibility.Visible;

            var httpClient = new Windows.Web.Http.HttpClient();
            var cancellationTokenSource = new CancellationTokenSource(10000);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RomajiConverter.WinUI Client");
            var httpResponseMessage =
                await httpClient.GetAsync(
                    new Uri("https://api.github.com/repos/xyh20180101/RomajiConverter.WinUI/releases/latest")).AsTask(cancellationTokenSource.Token);
            var data = JObject.Parse(await httpResponseMessage.Content.ReadAsStringAsync());

            UpdateRing.IsActive = false;
            UpdateRing.Visibility = Visibility.Collapsed;
            UpdateButton.Visibility = Visibility.Visible;

            var lastVersion = new Version(data["tag_name"].ToString());
            if (lastVersion > System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)
            {
                var contentDialog = new ContentDialog
                {
                    Title = resourceLoader.GetString("CheckUpdate"),
                    Content = resourceLoader.GetString("CheckUpdate-New"),
                    CloseButtonText = resourceLoader.GetString("No"),
                    PrimaryButtonText = resourceLoader.GetString("Yes"),
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = Content.XamlRoot
                };

                var result = await contentDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await Windows.System.Launcher.LaunchUriAsync(
                        new Uri("https://github.com/xyh20180101/RomajiConverter.WinUI/releases"));
                }
            }
            else
            {
                await new ContentDialog
                {
                    Title = resourceLoader.GetString("CheckUpdate"),
                    Content = resourceLoader.GetString("CheckUpdate-None"),
                    CloseButtonText = resourceLoader.GetString("Close"),
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = Content.XamlRoot,
                }.ShowAsync();
            }
        }
        catch (Exception exception)
        {
            throw new Exception(resourceLoader.GetString("CheckUpdate-Error"));
        }
        finally
        {
            UpdateRing.IsActive = false;
            UpdateRing.Visibility = Visibility.Collapsed;
            UpdateButton.Visibility = Visibility.Visible;
        }
    }

    private async void RepositorySetting_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/xyh20180101/RomajiConverter.WinUI"));
    }
}