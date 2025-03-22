using System;
using System.Globalization;
using System.Windows.Data;

namespace TestySZP.Helpers
{
    public class BoolToCzechTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "Ano" : "Ne";
            return "Ne";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString()?.ToLower() == "ano";
        }
    }
}
