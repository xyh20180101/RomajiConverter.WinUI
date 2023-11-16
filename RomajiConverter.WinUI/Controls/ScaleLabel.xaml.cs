using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI.Controls
{
    public sealed partial class ScaleLabel : UserControl
    {
        public ScaleLabel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty ScaleTextProperty = DependencyProperty.Register("ScaleText", typeof(string), typeof(ScaleLabel), new PropertyMetadata(default(string)));

        [Category("Extension")]
        public string ScaleText
        {
            get => (string)GetValue(ScaleTextProperty);
            set
            {
                SetValue(ScaleTextProperty, value);
                VisualStateManager.GoToState(this, "ScaleTextChanged", true);
                VisualStateManager.GoToState(this, "ScaleTextNormal", true);
            }
        }
    }
}
