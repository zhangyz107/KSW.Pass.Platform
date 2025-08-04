using KSW.ATE01.Drawing.Shmoo.ViewModels.Dialogs;
using System.Windows;

namespace KSW.ATE01.Drawing.Shmoo.Views.Dialogs
{
    /// <summary>
    /// HeatmapView.xaml 的交互逻辑
    /// </summary>
    public partial class HeatmapDialogView : Window
    {
        public HeatmapDialogView()
        {
            InitializeComponent();

            var viewModel = new HeatmapDialogViewModel();
            DataContext = viewModel;
        }
    }
}
