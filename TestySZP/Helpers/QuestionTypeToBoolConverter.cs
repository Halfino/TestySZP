using System;
using System.Globalization;
using System.Windows.Data;
using TestySZP.Models;

namespace TestySZP.Helpers
{
    public class QuestionTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (QuestionType)value == QuestionType.Written;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? QuestionType.Written : QuestionType.MultipleChoice;
        }
    }
}
