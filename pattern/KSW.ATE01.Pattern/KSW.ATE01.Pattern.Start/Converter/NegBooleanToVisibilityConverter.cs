using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KSW.ATE01.Pattern.Start.Converter
{
    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class NegBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value is bool)
            {
                flag = (bool)value;
            }

            return flag ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                return (Visibility)value == Visibility.Collapsed;
            }

            return true;
        }
    }
}
