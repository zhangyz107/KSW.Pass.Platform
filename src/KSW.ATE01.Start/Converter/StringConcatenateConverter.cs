using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KSW.ATE01.Start.Converter
{
    public class StringConcatenateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue && parameter is string separator)
            {
                return strValue + separator;
            }

            return value;  // 默认返回原值
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要将拼接的字符串反向转换回来，可以在这里实现
            return value;
        }
    }
}
