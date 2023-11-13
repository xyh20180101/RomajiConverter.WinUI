using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RomajiConverter.WinUI.Models;
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
    public sealed partial class EditableLabelGroup : UserControl
    {
        public EditableLabelGroup(ConvertedUnit unit)
        {
            InitializeComponent();
            DataContext = this;
            Unit = unit;
            MyFontSize = 18;
        }

        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(ConvertedUnit), typeof(EditableLabelGroup),new PropertyMetadata(""));

        [Category("Extension")]
        public ConvertedUnit Unit
        {
            get => (ConvertedUnit)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        public static readonly DependencyProperty RomajiVisibilityProperty = DependencyProperty.Register("RomajiVisibility", typeof(Visibility), typeof(EditableLabelGroup), new PropertyMetadata(""));

        [Category("Extension")]
        public Visibility RomajiVisibility
        {
            get => (Visibility)GetValue(RomajiVisibilityProperty);
            set => SetValue(RomajiVisibilityProperty, value);
        }

        public static readonly DependencyProperty HiraganaVisibilityProperty = DependencyProperty.Register("HiraganaVisibility", typeof(Visibility), typeof(EditableLabelGroup), new PropertyMetadata(""));

        [Category("Extension")]
        public Visibility HiraganaVisibility
        {
            get => (Visibility)GetValue(HiraganaVisibilityProperty);
            set => SetValue(HiraganaVisibilityProperty, value);
        }

        public static readonly DependencyProperty MyFontSizeProperty = DependencyProperty.Register("MyFontSize", typeof(double), typeof(EditableLabelGroup), new PropertyMetadata(12d));

        [Category("Extension")]
        public double MyFontSize
        {
            get => (double)GetValue(MyFontSizeProperty);
            set => SetValue(MyFontSizeProperty, value);
        }
    }
}
