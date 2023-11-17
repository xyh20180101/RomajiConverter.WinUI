using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace RomajiConverter.WinUI.ValueConverters;

public class IsDetailModeToWidthValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (bool)value ? new GridLength(1.2, GridUnitType.Star) : new GridLength(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}