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
    /// LabelTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class LabelTextBox
    {


        public string TbLabel
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("TbLabel", typeof(string), typeof(LabelTextBox), new PropertyMetadata(string.Empty));


        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(LabelTextBox), new PropertyMetadata(string.Empty));



        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(LabelTextBox), new PropertyMetadata(string.Empty));



        public Visibility UnitVisibility
        {
            get { return (Visibility)GetValue(UnitVisibilityProperty); }
            set { SetValue(UnitVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitVisibilityProperty =
            DependencyProperty.Register("UnitVisibility", typeof(Visibility), typeof(LabelTextBox), new PropertyMetadata(Visibility.Collapsed));



        public LabelTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
