using CommunityToolkit.WinUI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using RomajiConverter.Core.Helpers;
using RomajiConverter.WinUI.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System;
using RomajiConverter.Core.Options;
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class InputPage : Page
{
    public InputPage()
    {
        InitializeComponent();
    }

    public MainPage MainPage { get; set; }

    public EditPage MainEditPage { get; set; }

    public OutputPage MainOutputPage { get; set; }

    private CancellationTokenSource _convertCancellationTokenSource = null;

    /// <summary>
    /// 转换按钮事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ConvertButton_OnClick(object sender, RoutedEventArgs e)
    {
        _convertCancellationTokenSource = new CancellationTokenSource();

        try
        {
            App.ConvertedLineList.Clear();
            MainOutputPage.ClearText();

            MainPage.SetButtonIsEnabled(false);
            StopButton.IsEnabled = true;
            ConvertButton.IsEnabled = false;
            MainEditPage.ShowLoading(true);

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            if (App.Config.IsAIMode)
            {
                var config = App.Config.OpenAIConfigs.FirstOrDefault(p => p.IsSelected);
                if (config is null)
                {
                    var resourceLoader = ResourceLoader.GetForViewIndependentUse();
                    await new ContentDialog
                    {
                        XamlRoot = XamlRoot,
                        Title = resourceLoader.GetString("Tip"),
                        Content = resourceLoader.GetString("OpenAINotConfig"),
                        CloseButtonText = resourceLoader.GetString("Close"),
                        DefaultButton = ContentDialogButton.Close
                    }.ShowAsync();
                    return;
                }
                await RomajiAIHelper.ToRomajiStreamingAsync(App.ConvertedLineList, InputTextBox.Text, new ToRomajiAIOptions
                {
                    IsParticleAsPronunciation = App.Config.IsParticleAsPronunciation,
                    BaseUrl = config.BaseUrl,
                    Model = config.Model,
                    ApiKey = config.ApiKey,
                    Prompt = App.Config.Prompt
                }, _convertCancellationTokenSource.Token);
            }
            else
            {
                var enumerable = RomajiHelper.ToRomaji(InputTextBox.Text, new ToRomajiOptions { IsParticleAsPronunciation = App.Config.IsParticleAsPronunciation });
                using var enumerator = enumerable.GetEnumerator();
                while (!_convertCancellationTokenSource.IsCancellationRequested)
                {
                    if (await Task.Run(() => !enumerator.MoveNext())) break;

                    await dispatcherQueue.EnqueueAsync(() =>
                    {
                        App.ConvertedLineList.Add(enumerator.Current);
                    });
                }
            }
        }
        catch (TaskCanceledException exception) { }
        catch (OperationCanceledException exception) { }
        finally
        {
            MainEditPage.ShowLoading(false);
            ConvertButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            MainPage.SetButtonIsEnabled(true);
        }
    }

    /// <summary>
    /// 滚动事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void InputTextBox_OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var pointer = e.GetCurrentPoint((UIElement)sender);
        if (pointer.PointerDeviceType != PointerDeviceType.Mouse || KeyboardExtension.IsKeyDown(VirtualKey.Control))
        {
            if (pointer.Properties.MouseWheelDelta < 0 &&
                App.Config.InputTextBoxFontSize > 3.047) //14 / Math.Pow(1.1, 16)
                App.Config.InputTextBoxFontSize /= 1.1;
            else if (pointer.Properties.MouseWheelDelta > 0 &&
                     App.Config.InputTextBoxFontSize < 53.1) //14 * Math.Pow(1.1, 14)
                App.Config.InputTextBoxFontSize *= 1.1;
            e.Handled = true;
        }
    }

    /// <summary>
    /// 设置输入文本
    /// </summary>
    /// <param name="str"></param>
    public void SetTextBoxText(string str)
    {
        InputTextBox.Text = str;
    }

    private void StopButton_OnClick(object sender, RoutedEventArgs e)
    {
        _convertCancellationTokenSource?.Cancel();
    }
}