using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;
using RomajiConverter.WinUI.Dialogs;
using RomajiConverter.WinUI.Extensions;
using System;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.Web.Http;
using Microsoft.UI.Composition.SystemBackdrops;
using RomajiConverter.WinUI.Models;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        InitOpenAISettings();
        InitFontFamily();
        InitMicaKindComboBox();
        VersionTextBlock.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    private void BackButton_OnTapped(object sender, TappedRoutedEventArgs e)
    {
        Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo
        {
            Effect = SlideNavigationTransitionEffect.FromLeft
        });
        GC.Collect();
    }

    /// <summary>
    /// 设置OpenAI设置描述
    /// </summary>
    private void InitOpenAISettings()
    {
        OpenAIConfigsSettingExpander.Description =
            App.Config.OpenAIConfigs.FirstOrDefault(p => p.IsSelected)?.Name ?? string.Empty;
    }

    private void InitMicaKindComboBox()
    {
        var colorOptions = Enum.GetValues(typeof(MicaKind)).Cast<MicaKind>().ToList();
        MicaKindComboBox.ItemsSource = colorOptions;
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

            var httpClient = new HttpClient();
            var cancellationTokenSource = new CancellationTokenSource(10000);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "RomajiConverter.WinUI Client");
            var httpResponseMessage =
                await httpClient.GetAsync(
                        new Uri("https://api.github.com/repos/xyh20180101/RomajiConverter.WinUI/releases/latest"))
                    .AsTask(cancellationTokenSource.Token);
            var data = JsonSerializer.Deserialize<JsonObject>(await httpResponseMessage.Content.ReadAsStringAsync());

            UpdateRing.IsActive = false;
            UpdateRing.Visibility = Visibility.Collapsed;
            UpdateButton.Visibility = Visibility.Visible;

            var lastVersion = new Version(data["tag_name"].ToString());
            if (lastVersion > Assembly.GetExecutingAssembly().GetName().Version)
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
                    await Launcher.LaunchUriAsync(
                        new Uri("https://github.com/xyh20180101/RomajiConverter.WinUI/releases"));
            }
            else
            {
                await new ContentDialog
                {
                    Title = resourceLoader.GetString("CheckUpdate"),
                    Content = resourceLoader.GetString("CheckUpdate-None"),
                    CloseButtonText = resourceLoader.GetString("Close"),
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = Content.XamlRoot
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
        await Launcher.LaunchUriAsync(new Uri("https://github.com/xyh20180101/RomajiConverter.WinUI"));
    }

    private void SettingsPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        SettingScrollViewer.ScrollToVerticalOffset(0);
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

    private async void OpenAddOpenAIConfigWindowButton_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new AddOpenAIConfigContentDialog
        {
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        var dialogResult = await dialog.ShowAsync();

        if (dialogResult == ContentDialogResult.Primary)
        {
            App.Config.OpenAIConfigs.Add(dialog.OpenAIConfig);

            if (App.Config.OpenAIConfigs.All(p => !p.IsSelected))
            {
                var first = App.Config.OpenAIConfigs.FirstOrDefault();
                if (first is not null)
                {
                    first.IsSelected = true;
                }
                InitOpenAISettings();
            }
        }
    }

    private void OpenAIConfigSetDefaultButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { CommandParameter: OpenAIConfig config })
        {
            config.IsSelected = true;
            foreach (var item in App.Config.OpenAIConfigs)
            {
                if (item != config)
                    item.IsSelected = false;
            }
            InitOpenAISettings();
        }
    }

    private async void OpenAIConfigEditButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { CommandParameter: OpenAIConfig config })
        {
            var dialog = new AddOpenAIConfigContentDialog
            {
                OpenAIConfig = new OpenAIConfig
                {
                    Name = config.Name,
                    BaseUrl = config.BaseUrl,
                    Model = config.Model,
                    ApiKey = config.ApiKey
                },
                XamlRoot = App.MainWindow.Content.XamlRoot
            };
            var dialogResult = await dialog.ShowAsync();

            if (dialogResult == ContentDialogResult.Primary)
            {
                config.Name = dialog.OpenAIConfig.Name;
                config.BaseUrl = dialog.OpenAIConfig.BaseUrl;
                config.Model = dialog.OpenAIConfig.Model;
                config.ApiKey = dialog.OpenAIConfig.ApiKey;

                InitOpenAISettings();
            }
        }
    }

    private void OpenAIConfigRemoveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { CommandParameter: OpenAIConfig config })
        {
            App.Config.OpenAIConfigs.Remove(config);

            if (App.Config.OpenAIConfigs.All(p => !p.IsSelected))
            {
                var first = App.Config.OpenAIConfigs.FirstOrDefault();
                if (first is not null)
                {
                    first.IsSelected = true;
                }
                InitOpenAISettings();
            }
        }
    }
}