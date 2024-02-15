using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RomajiConverter.Core.Models;
using RomajiConverter.WinUI.Enums;

namespace RomajiConverter.WinUI.Controls;

public sealed partial class EditableLabelGroup : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(nameof(Unit),
        typeof(ConvertedUnit),
        typeof(EditableLabelGroup), new PropertyMetadata(null));

    public static readonly DependencyProperty RomajiVisibilityProperty = DependencyProperty.Register(
        nameof(RomajiVisibility),
        typeof(Visibility), typeof(EditableLabelGroup), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty HiraganaVisibilityProperty =
        DependencyProperty.Register(nameof(HiraganaVisibility), typeof(HiraganaVisibility), typeof(EditableLabelGroup),
            new PropertyMetadata(HiraganaVisibility.Collapsed));

    public static readonly DependencyProperty MyFontSizeProperty = DependencyProperty.Register(nameof(MyFontSize),
        typeof(double), typeof(EditableLabelGroup), new PropertyMetadata(14d));

    public static readonly DependencyProperty BorderVisibilitySettingProperty =
        DependencyProperty.Register(nameof(BorderVisibilitySetting), typeof(BorderVisibilitySetting),
            typeof(EditableLabelGroup),
            new PropertyMetadata(BorderVisibilitySetting.Hidden));

    private ReplaceString _selectedHiragana;

    private ReplaceString _selectedRomaji;

    public EditableLabelGroup(ConvertedUnit unit)
    {
        InitializeComponent();
        Unit = unit;
        MyFontSize = 14;
        SelectedRomaji = Unit.ReplaceRomaji[0];
        SelectedHiragana = Unit.ReplaceHiragana[0];
        BorderVisibilitySetting = BorderVisibilitySetting.Highlight;
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

    public BorderVisibilitySetting BorderVisibilitySetting
    {
        get => (BorderVisibilitySetting)GetValue(BorderVisibilitySettingProperty);
        set => SetValue(BorderVisibilitySettingProperty, value);
    }

    public ReplaceString SelectedRomaji
    {
        get => _selectedRomaji;
        set
        {
            if (Equals(value, _selectedRomaji)) return;
            _selectedRomaji = value;
            if (_selectedRomaji.IsSystem)
                SelectedHiragana = Unit.ReplaceHiragana.First(p => p.Id == _selectedRomaji.Id);
            Unit.Romaji = _selectedRomaji.Value;
            OnPropertyChanged();
        }
    }

    public ReplaceString SelectedHiragana
    {
        get => _selectedHiragana;
        set
        {
            if (Equals(value, _selectedHiragana)) return;
            _selectedHiragana = value;
            if (_selectedHiragana.IsSystem)
                SelectedRomaji = Unit.ReplaceRomaji.First(p => p.Id == _selectedHiragana.Id);
            Unit.Hiragana = _selectedHiragana.Value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Destroy()
    {
        RomajiLabel.Destroy();
        HiraganaLabel.Destroy();
        Bindings.StopTracking();
        ClearValue(UnitProperty);
        ClearValue(RomajiVisibilityProperty);
        ClearValue(HiraganaVisibilityProperty);
        ClearValue(MyFontSizeProperty);
    }
}