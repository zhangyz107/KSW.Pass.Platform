using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Start.ViewModels.TestPlans
{
    /// <summary>
    /// 组信息视图模型
    /// </summary>
    public class GroupInfoSettingViewModel : ViewModelBase
    {
        private int _selectGroup;
        private int _selectPin;
        private DelegateCommand _addGroupCommand;
        private DelegateCommand _removeGroupCommand;
        private DelegateCommand _addPinCommand;
        private DelegateCommand _removePinCommand;

        #region Properties
        /// <summary>
        /// 选中的组
        /// </summary>
        public int SelectGroup
        {
            get => _selectGroup;
            set => SetProperty(ref _selectGroup, value);
        }

        /// <summary>
        /// 选中的引脚
        /// </summary>
        public int SelectPin
        {
            get => _selectPin;
            set => SetProperty(ref _selectPin, value);
        }
        #endregion

        #region Command
        /// <summary>
        /// 添加组命令
        /// </summary>
        public DelegateCommand AddGroupCommand =>
            _addGroupCommand ?? (_addGroupCommand = new DelegateCommand(ExecuteAddGroupCommand));

        /// <summary>
        /// 添加组命令
        /// </summary>
        public DelegateCommand RemoveGroupCommand =>
            _removeGroupCommand ?? (_removeGroupCommand = new DelegateCommand(ExecuteRemoveGroupCommand, () => _selectGroup != null));

        /// <summary>
        /// 添加组命令
        /// </summary>
        public DelegateCommand AddPinCommand =>
            _addPinCommand ?? (_addPinCommand = new DelegateCommand(ExecuteAddPinCommand));

        /// <summary>
        /// 添加组命令
        /// </summary>
        public DelegateCommand RemovePinCommand =>
            _removePinCommand ?? (_removePinCommand = new DelegateCommand(ExecuteRemovePinCommand, () => _selectPin != null));

        #endregion

        public GroupInfoSettingViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {

        }

        private void ExecuteAddGroupCommand()
        {

        }

        private void ExecuteRemoveGroupCommand()
        {

        }

        private void ExecuteAddPinCommand()
        {

        }

        private void ExecuteRemovePinCommand()
        {

        }

    }
}
