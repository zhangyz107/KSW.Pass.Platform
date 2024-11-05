using KSW.ATE01.Extension.VS2022.UI.ViewModels;
using System;
using System.Collections.Generic;
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

namespace KSW.ATE01.Extension.VS2022.UI.Views
{
    /// <summary>
    /// DigitalView.xaml 的交互逻辑
    /// </summary>
    public partial class DigitalView : Window
    {
        public DigitalView(DigitalViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
