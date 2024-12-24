using KSW.Ui;

namespace KSW.ATE01.Start.Views
{
    /// <summary>
    /// RealTimeTxtDialog.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeTxtView : IView
    {
        public RealTimeTxtView()
        {
            InitializeComponent();
        }

        private void richTB_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Console.WriteLine("abc");
        }
    }
}
