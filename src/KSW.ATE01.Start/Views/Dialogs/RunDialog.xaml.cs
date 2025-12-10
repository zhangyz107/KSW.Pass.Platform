using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace KSW.ATE01.Start.Views.Dialogs
{
    /// <summary>
    /// RunDialog.xaml 的交互逻辑
    /// </summary>
    public partial class RunDialog : IView
    {
        private double _expandedWidth;

        public RunDialog()
        {
            InitializeComponent();

            this.Loaded += RunDialog_Loaded;
        }

        private void RunDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (!double.IsNaN(this.mainExpander.ActualWidth))
            {
                _expandedWidth = this.mainExpander.ActualWidth;
            }
        }

        private void mainExpander_Collapsed(object sender, RoutedEventArgs e)
        {
            leftColumn.Width = new GridLength(1, GridUnitType.Star);
            rightColumn.Width = new GridLength(1, GridUnitType.Auto);
        }

        private void mainExpander_Expanded(object sender, RoutedEventArgs e)
        {
            leftColumn.Width = new GridLength(1, GridUnitType.Star);
            rightColumn.Width = new GridLength(1, GridUnitType.Star);
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("sdas");
        }
    }
}
