using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using RomajiConverter.WinUI.Enums;
using RomajiConverter.WinUI.Helpers;
using RomajiConverter.WinUI.Models;
using WanaKanaSharp;

namespace RomajiConverter.WinUI.Controls;

public sealed partial class EditableLabelGroup : UserControl, INotifyPropertyChanged
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

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    private int _selectedIndex;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _selectedIndex = value;
            Unit.Romaji = Unit.ReplaceRomaji[SelectedIndex].Value;
            Unit.Hiragana = Unit.ReplaceHiragana[SelectedIndex].Value;
            OnPropertyChanged();
        }
    }
}