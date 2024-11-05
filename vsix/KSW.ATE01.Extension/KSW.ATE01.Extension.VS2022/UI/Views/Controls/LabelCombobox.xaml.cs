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

namespace KSW.ATE01.Extension.VS2022.UI.Views.Controls
{
    /// <summary>
    /// LabelCombobox.xaml 的交互逻辑
    /// </summary>
    public partial class LabelCombobox : UserControl
    {
        public string CbLabel
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("CbLabel", typeof(string), typeof(LabelTextBox), new PropertyMetadata(string.Empty));

        public LabelCombobox()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
