using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.ATE01.Project.Base.Services.Memory;
using KSW.Helpers;
using KSW.Ui;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.PPMU.Start.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class ShellViewModel : ViewModelBase
    {
        private string _voltage;

        #region Properties
        public string Title { get => "PPMU DTE"; }

        public string Voltage
        {
            get => _voltage;
            set => SetProperty(ref _voltage, value);
        }

        #endregion

        #region Commands
        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        private DelegateCommand _clearAlarmCommand;
        public DelegateCommand ClearAlarmCommand => _clearAlarmCommand ?? (_clearAlarmCommand = new DelegateCommand(ExecuteClearAlarmCommand));

        #endregion

        public ShellViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
            if (!ATE01ShareMemory.OpenShareMemory())
            {
                //开启共享内存
                ATE01ShareMemory.CreateShareMemory();
            }
            var testPlanFilePath = ATE01ShareMemory.TestPlanFilePath;
            var loadTestPlan = ATE01ShareMemory.LoadedTestPlanFilePath;

            ShareMemoryInTestPlan<TestPlanModel> instance = ShareMemoryInTestPlan<TestPlanModel>.Instance;
            if (instance.IsExisting() && instance.Open().Item1)
            {
                var testPlan = instance.ReadObject();
            }
        }


        private void ExecuteRefreshCommand()
        {

        }


        private void ExecuteClearAlarmCommand()
        {

        }
    }
}
