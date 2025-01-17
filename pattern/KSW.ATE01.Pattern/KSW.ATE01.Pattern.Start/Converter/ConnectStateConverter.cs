using KSW.ATE01.Pattern.Domain.Instruments.Core.Enums;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace KSW.ATE01.Pattern.Start.Converter
{
    public class ConnectStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectStateEnum Cs = (ConnectStateEnum)value;
            BitmapImage bitimage = null;
            switch (Cs)
            {
                case ConnectStateEnum.Connect:
                    bitimage = new BitmapImage(new Uri("/KSW.ATE01.Pattern.Start;component/Resource/Images/tick.png", UriKind.RelativeOrAbsolute));
                    break;
                case ConnectStateEnum.Disconnect:
                    bitimage = new BitmapImage(new Uri("/KSW.ATE01.Pattern.Start;component/Resource/Images/cross.png", UriKind.RelativeOrAbsolute));
                    break;
                case ConnectStateEnum.UnKown:
                    bitimage = new BitmapImage(new Uri("/KSW.ATE01.Pattern.Start;component/Resource/Images/help.png", UriKind.RelativeOrAbsolute));
                    break;
                default:
                    bitimage = new BitmapImage(new Uri("/KSW.ATE01.Pattern.Start;component/Resource/Images/help.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            return bitimage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
