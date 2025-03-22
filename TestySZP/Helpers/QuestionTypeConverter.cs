using System;
using System.Globalization;
using System.Windows.Data;
using TestySZP.Models;

namespace TestySZP.Helpers
{
    public class QuestionTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(typeof(QuestionType), parameter.ToString()) : null;
        }
    }
}
