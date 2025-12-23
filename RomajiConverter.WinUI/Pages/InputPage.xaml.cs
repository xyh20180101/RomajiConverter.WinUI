using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using RomajiConverter.Core.Helpers;
using RomajiConverter.WinUI.Extensions;

namespace RomajiConverter.WinUI.Pages;

public sealed partial class InputPage : Page
{
    public InputPage()
    {
        InitializeComponent();
    }

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

            StopButton.IsEnabled = true;
            ConvertButton.IsEnabled = false;
            MainEditPage.ShowLoading(true);

            if (App.Config.IsAIMode)
            {
                var apiKey = App.Config.AISelect switch
                {
                    AIServiceProvider.DeepSeek => App.Config.DeepSeekApiKey,
                    AIServiceProvider.Zhipu => App.Config.ZhipuApiKey,
                    _ => ""
                };
                await RomajiAIHelper.ToRomajiStreaming(App.ConvertedLineList, InputTextBox.Text, App.Config.AISelect,
                    apiKey, _convertCancellationTokenSource.Token);
            }
            else
            {
                await RomajiHelper.ToRomaji(App.ConvertedLineList, InputTextBox.Text);
            }
        }
        catch (TaskCanceledException exception) { }
        catch (OperationCanceledException exception) { }
        finally
        {
            MainEditPage.ShowLoading(false);
            ConvertButton.IsEnabled = true;
            StopButton.IsEnabled = false;
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