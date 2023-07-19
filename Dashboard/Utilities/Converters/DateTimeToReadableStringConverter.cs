using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Utilities.Converters;

public class DateTimeToReadableStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        DateTime val = (value as DateTime?).GetValueOrDefault();

        if (val == default) return "";

        return val.Date == DateTime.Today ? val.ToString("h:mm tt") : val.ToReadableDateString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
