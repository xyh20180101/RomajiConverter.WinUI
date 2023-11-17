using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace RomajiConverter.WinUI.ValueConverters;

public class FontSizeToMarginValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return new Thickness((int)Math.Floor((double)value / 12) * 4);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}