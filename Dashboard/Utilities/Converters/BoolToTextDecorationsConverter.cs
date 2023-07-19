using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Dashboard.Utilities.Converters;

public class BoolToTextDecorationsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool param = Helper.ParseXamlBoolean(parameter);

        return (value as bool?).GetValueOrDefault() != param ? TextDecorations.Strikethrough : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
