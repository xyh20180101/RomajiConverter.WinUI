using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RomajiConverter.WinUI.Enums;
using RomajiConverter.WinUI.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RomajiConverter.WinUI.Controls;

public sealed partial class EditableLabelGroup : UserControl
{
    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(ConvertedUnit),
        typeof(EditableLabelGroup), new PropertyMetadata(null));

    public static readonly DependencyProperty RomajiVisibilityProperty = DependencyProperty.Register("RomajiVisibility",
        typeof(Visibility), typeof(EditableLabelGroup), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty HiraganaVisibilityProperty =
        DependencyProperty.Register("HiraganaVisibility", typeof(HiraganaVisibility), typeof(EditableLabelGroup),
            new PropertyMetadata(HiraganaVisibility.Collapsed));

    public static readonly DependencyProperty MyFontSizeProperty = DependencyProperty.Register("MyFontSize",
        typeof(double), typeof(EditableLabelGroup), new PropertyMetadata(12d));

    public EditableLabelGroup(ConvertedUnit unit)
    {
        InitializeComponent();
        DataContext = this;
        Unit = unit;
        MyFontSize = 14;
    }

    [Category("Extension")]
    public ConvertedUnit Unit
    {
        get => (ConvertedUnit)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    [Category("Extension")]
    public Visibility RomajiVisibility
    {
        get => (Visibility)GetValue(RomajiVisibilityProperty);
        set => SetValue(RomajiVisibilityProperty, value);
    }

    [Category("Extension")]
    public HiraganaVisibility HiraganaVisibility
    {
        get => (HiraganaVisibility)GetValue(HiraganaVisibilityProperty);
        set
        {
            switch (value)
            {
                case HiraganaVisibility.Visible:
                    HiraganaLabel.IsEnabled = true;
                    HiraganaLabel.Opacity = 1;
                    HiraganaLabel.Visibility = Visibility.Visible;
                    break;
                case HiraganaVisibility.Collapsed:
                    HiraganaLabel.IsEnabled = true;
                    HiraganaLabel.Opacity = 1;
                    HiraganaLabel.Visibility = Visibility.Collapsed;
                    break;
                case HiraganaVisibility.Hidden:
                    HiraganaLabel.IsEnabled = false;
                    HiraganaLabel.Opacity = 0;
                    HiraganaLabel.Visibility = Visibility.Visible;
                    break;
            }

            SetValue(HiraganaVisibilityProperty, value);
        }
    }

    [Category("Extension")]
    public double MyFontSize
    {
        get => (double)GetValue(MyFontSizeProperty);
        set => SetValue(MyFontSizeProperty, value);
    }
}