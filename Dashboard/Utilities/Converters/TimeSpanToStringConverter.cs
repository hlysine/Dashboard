using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Utilities.Converters;

public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var val = (value as TimeSpan?).GetValueOrDefault();
        if (val == default) return "";
        if (val.TotalHours >= 1)
        {
            return val.ToString(@"h\:mm\:ss");
        }
        else
        {
            return val.ToString(@"mm\:ss");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}