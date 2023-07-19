using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dashboard.Utilities.Converters;

public class BoolToFontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool param = (parameter as bool?) ?? System.Convert.ToBoolean((string)parameter);
        if ((value as bool?).GetValueOrDefault() != param)
        {
            return FontWeights.Bold;
        }
        else
        {
            return FontWeights.Regular;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}