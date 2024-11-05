using KSW.ATE01.Extension.VS2022;
using KSW.ATE01.Extension.VS2022.UI.ViewModels;
using KSW.ATE01.Extension.VS2022.UI.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Extension.Shared.Integration.Commands
{
    /// <summary>
    /// Digital DTE命令
    /// </summary>
    internal sealed class DigitalDteCommand : BaseCommand
    {
        private DigitalView _digital;
        private bool _isShow = false;
        private static readonly object lockObject = new object();


        public DigitalDteCommand(ATE01Package package) : base(package, ATE01Guids.GuidATE01MenuSet, ATE01Ids.DigitalDteId)
        {

        }

        public static DigitalDteCommand Instance { get; private set; }

        /// <summary>
        /// Initializes a singleton instance of this command.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static async Task InitializeAsync(ATE01Package package)
        {
            Instance = new DigitalDteCommand(package);
            await Instance.SwitchAsync(on: true);
        }

        /// <summary>
        /// Called to execute the command.
        /// </summary>
        protected override void OnExecute()
        {
            base.OnExecute();

            if (_digital == null || !_isShow)
            {
                lock (lockObject)
                {
                    _isShow = true;
                }
                _digital = new DigitalView(new DigitalViewModel());
                _digital.Closing += Digital_Closing;
                _digital.Show();
            }
            else
                _digital.Activate();
        }

        private void Digital_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (lockObject)
            {
                _isShow = false;
            }
        }

        protected override void OnBeforeQueryStatus()
        {
            Enabled = Package.IDE.Solution.IsOpen && !string.IsNullOrEmpty(Package.IDE.Solution.FileName);
        }
    }
}
