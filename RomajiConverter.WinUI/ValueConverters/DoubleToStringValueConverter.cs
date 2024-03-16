using System;
using Microsoft.UI.Xaml.Data;

namespace RomajiConverter.WinUI.ValueConverters;

public class DoubleToStringValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return ((double)value).ToString("F1");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return double.Parse((string)value);
    }
}