using KSW.ATE01.Extension.VS2022;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Extension.Shared.Integration.Commands
{
    /// <summary>
    /// UDB DTE命令
    /// </summary>
    internal sealed class UdbDteCommand : BaseCommand
    {
        public UdbDteCommand(ATE01Package package) : base(package, ATE01Guids.GuidATE01MenuSet, ATE01Ids.UdbDteId)
        {
        }

        /// <summary>
        /// A singleton instance of this command.
        /// </summary>
        public static UdbDteCommand Instance { get; private set; }

        /// <summary>
        /// Initializes a singleton instance of this command.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static async Task InitializeAsync(ATE01Package package)
        {
            Instance = new UdbDteCommand(package);
            await Instance.SwitchAsync(on: true);
        }

        /// <summary>
        /// Called to execute the command.
        /// </summary>
        protected override void OnExecute()
        {
            base.OnExecute();
        }

        protected override void OnBeforeQueryStatus()
        {
            Enabled = Package.IDE.Solution.IsOpen && !string.IsNullOrEmpty(Package.IDE.Solution.FileName);
        }
    }
}
