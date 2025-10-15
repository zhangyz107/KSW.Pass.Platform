using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KSW.ATE01.Start.Converter
{
    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return null;

            if (decimal.TryParse(value.ToString(), out decimal decimalValue))
                return decimalValue;

            if (double.TryParse(value.ToString(), out double doubleValue))
                return doubleValue;

            if (int.TryParse(value.ToString(), out int intValue))
                return intValue;

            return value;
        }
    }
}
