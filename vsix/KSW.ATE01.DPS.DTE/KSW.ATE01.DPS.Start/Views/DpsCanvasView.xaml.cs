using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KSW.ATE01.DPS.Start.Views
{
    /// <summary>
    /// DpsCanvasView.xaml 的交互逻辑
    /// </summary>
    public partial class DpsCanvasView : UserControl
    {
        public DpsCanvasView()
        {
            InitializeComponent();
        }
    }

    public class SwitchDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double width)
            {
                return width * 0.9;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
