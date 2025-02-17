using KSW.ATE01.Application.Events.Projects;
using KSW.Ui;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KSW.ATE01.Start.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : IView
    {
        private readonly IEventAggregator _eventAggregator;

        public ShellView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            eventAggregator.GetEvent<ShellRevealControlEvent>().Subscribe(RevealControl, ThreadOption.UIThread);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            Show();
        }

        private void RevealControl(bool isVisible)
        {
            if (isVisible)
                this.Show();
            else
                this.Hide();
        }

        public void ExecuteMethodBasedOnArgument(string message)
        {
            _eventAggregator.GetEvent<LoadProjectFromArgsEvent>().Publish(message);
        }
    }
}