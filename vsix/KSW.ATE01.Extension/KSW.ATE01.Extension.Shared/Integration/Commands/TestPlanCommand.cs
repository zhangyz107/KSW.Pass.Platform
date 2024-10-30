using EnvDTE;
using KSW.ATE01.Extension.Shared.Helpers;
using KSW.ATE01.Extension.Shared.Helpers.Projects;
using KSW.ATE01.Extension.VS2022;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Extension.Shared.Integration.Commands
{
    /// <summary>
    /// Test Plan命令
    /// </summary>
    internal sealed class TestPlanCommand : BaseCommand
    {
        private readonly string _releaseDir = "Release";
        private readonly string _excelExt = ".xlsx";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPlanCommand" /> class.
        /// </summary>
        /// <param name="package">The hosting package.</param>
        public TestPlanCommand(ATE01Package package) : base(package, ATE01Guids.GuidATE01MenuSet, ATE01Ids.TestPlanId)
        {

        }

        /// <summary>
        /// A singleton instance of this command.
        /// </summary>
        public static TestPlanCommand Instance { get; private set; }

        /// <summary>
        /// Initializes a singleton instance of this command.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static async Task InitializeAsync(ATE01Package package)
        {
            Instance = new TestPlanCommand(package);
            await Instance.SwitchAsync(on: true);
        }

        /// <summary>
        /// Called to execute the command.
        /// </summary>
        protected override void OnExecute()
        {
            base.OnExecute();
            var slnFileName = Package.IDE.Solution.FileName;
            var projectDir = Path.GetDirectoryName(slnFileName);
            var projectName = Path.GetFileNameWithoutExtension(slnFileName);
            var configPath = Path.Combine(projectDir, projectName + ProjectHelper.ProjectExt);

            var projectInfo = ProjectHelper.LoadProjectInfo(configPath);
            if (projectInfo != null)
            {
                if (projectInfo.TestPlanType == Enums.TestPlanType.Excel)
                {
                    var testPlanPath = Path.Combine(projectDir, _releaseDir, projectName + _excelExt);
                    if (File.Exists(testPlanPath))
                        System.Diagnostics.Process.Start(testPlanPath);
                }
                else
                {
                    var message = "仅Excel类型的测试计划支持该功能";

                    VsShellUtilities.ShowMessageBox(
                        Package,
                        message,
                        nameof(TestPlanCommand),
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }

        }

        protected override void OnBeforeQueryStatus()
        {
            Enabled = Package.IDE.Solution.IsOpen && !string.IsNullOrEmpty(Package.IDE.Solution.FileName);
        }
    }
}
