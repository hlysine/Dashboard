using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Dashboard.Utilities.Converters
{
    public class DateTimeToReadableStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime val = (value as DateTime?).GetValueOrDefault();
            if (val == default) return "";
            if (val.Date == DateTime.Today)
            {
                return val.ToString("h:mm tt");
            }
            else
            {
                return val.ToReadableDateString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
