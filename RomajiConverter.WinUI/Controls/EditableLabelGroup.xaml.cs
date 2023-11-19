using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RomajiConverter.WinUI.Enums;
using RomajiConverter.WinUI.Models;

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
        typeof(double), typeof(EditableLabelGroup), new PropertyMetadata(14d));

    public EditableLabelGroup(ConvertedUnit unit)
    {
        InitializeComponent();
        Unit = unit;
        MyFontSize = 14;
    }

    public ConvertedUnit Unit
    {
        get => (ConvertedUnit)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public Visibility RomajiVisibility
    {
        get => (Visibility)GetValue(RomajiVisibilityProperty);
        set
        {
            switch (value)
            {
                case Visibility.Visible:
                    RomajiLabel.IsEnabled = true;
                    RomajiLabel.Opacity = 1;
                    RomajiLabel.Visibility = Visibility.Visible;
                    break;
                case Visibility.Collapsed:
                    RomajiLabel.IsEnabled = false;
                    RomajiLabel.Opacity = 0;
                    RomajiLabel.Visibility = Visibility.Collapsed;
                    break;
            }

            SetValue(RomajiVisibilityProperty, value);
        }
    }

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
                    HiraganaLabel.IsEnabled = false;
                    HiraganaLabel.Opacity = 0;
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

    public double MyFontSize
    {
        get => (double)GetValue(MyFontSizeProperty);
        set => SetValue(MyFontSizeProperty, value);
    }
}