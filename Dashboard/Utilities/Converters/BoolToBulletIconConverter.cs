using MaterialDesignThemes.Wpf;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Utilities.Converters;

public class BoolToBulletIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool param = Helper.ParseXamlBoolean(parameter);

        return (value as bool?).GetValueOrDefault() != param ? PackIconKind.Tick : PackIconKind.SquareSmall;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
