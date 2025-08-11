using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Start.ViewModels.Dialogs
{
    public class TestPlanDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Properties
        public string Title => "测试计划";

        public DialogCloseListener RequestClose { get; }
        #endregion

        public TestPlanDialogViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
        }


        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
