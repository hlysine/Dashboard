using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Utilities.Converters;

public class BoolToStarredIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var param = (parameter as bool?) ?? System.Convert.ToBoolean((string)parameter);
        if ((value as bool?).GetValueOrDefault() != param)
        {
            return PackIconKind.Star;
        }
        else
        {
            return PackIconKind.StarOutline;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}