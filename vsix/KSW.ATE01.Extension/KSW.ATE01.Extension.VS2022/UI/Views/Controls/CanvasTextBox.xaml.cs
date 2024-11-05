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
    /// CanvasTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class CanvasTextBox : UserControl
    {
        public CanvasTextBox()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CanvasTextBox), new PropertyMetadata(string.Empty));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(CanvasTextBox), new PropertyMetadata(string.Empty));



        public Visibility LabelVisiblity
        {
            get { return (Visibility)GetValue(LabelVisiblityProperty); }
            set { SetValue(LabelVisiblityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelVisiblityProperty =
            DependencyProperty.Register("LabelVisiblity", typeof(Visibility), typeof(CanvasTextBox), new PropertyMetadata(Visibility.Visible));



        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Units.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(string), typeof(CanvasTextBox), new PropertyMetadata(string.Empty));


        public Visibility UnitsVisiblity
        {
            get { return (Visibility)GetValue(UnitsVisiblityProperty); }
            set { SetValue(UnitsVisiblityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitsVisiblityProperty =
            DependencyProperty.Register("UnitsVisiblity", typeof(Visibility), typeof(CanvasTextBox), new PropertyMetadata(Visibility.Visible));


        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(CanvasTextBox), new FrameworkPropertyMetadata(1.0));
    }
}
