using KSW.ATE01.Start.ViewModels;
using KSW.ATE01.Start.ViewModels.Patterns;
using KSW.Ui;
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

namespace KSW.ATE01.Start.Views.Patterns
{
    /// <summary>
    /// PatternEditorView.xaml 的交互逻辑
    /// </summary>
    public partial class PatternToolView : IView
    {
        private readonly IRegionManager _regionManager;

        public PatternToolView(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;

            if (DataContext is PatternToolViewModel vm)
            {
                vm.ClearTabCommand = new DelegateCommand(ExecuteClearTabCommand);
            }
        }

        private void ExecuteClearTabCommand()
        {
            var region = _regionManager.Regions.FirstOrDefault(x => x.Name == RegionNameManagement.PatternEditorContent);
            while (region != null && tabControl.Items.Count > 0)
            {
                var tab = tabControl.Items[0];
                region.Remove(tab);
                tabControl.Items.Remove(tab);
            }
        }
    }
}
