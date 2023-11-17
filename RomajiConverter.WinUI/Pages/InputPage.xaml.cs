using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RomajiConverter.WinUI.Helpers;
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
    public sealed partial class InputPage : Page
    {
        public InputPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 转换按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertButton_OnClick(object sender, RoutedEventArgs e)
        {
            App.ConvertedLineList = RomajiHelper.ToRomaji(InputTextBox.Text, AutoVariantCheckBox.IsChecked.Value);

            if (App.Config.IsDetailMode)
                Frame.Navigate(typeof(EditPage), null, new SlideNavigationTransitionInfo
                {
                    Effect = SlideNavigationTransitionEffect.FromRight
                });
            else
                Frame.Navigate(typeof(OutputPage), null, new SlideNavigationTransitionInfo
                {
                    Effect = SlideNavigationTransitionEffect.FromRight
                });
        }

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
    }
}
