using System;
using Microsoft.UI.Xaml.Data;

namespace RomajiConverter.WinUI.ValueConverters;

public class BoolNegationValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is false;
    }
}