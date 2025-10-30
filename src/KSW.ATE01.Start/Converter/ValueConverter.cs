using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            //可以输入“.”或者“,”，原理是使其报错则不对binding的变量赋值
            string result = (value.ToString().EndsWith(".") ? "." : value).ToString();
            result = (result.ToString().EndsWith(",") ? "," : result).ToString();
            //可以输入末尾是0的小数，原理同上
            Regex re = new Regex("^([0-9]{1,}[.,][0-9]*0)$");
            result = re.IsMatch(result) ? "." : result;
            return result;
        }
    }
}
