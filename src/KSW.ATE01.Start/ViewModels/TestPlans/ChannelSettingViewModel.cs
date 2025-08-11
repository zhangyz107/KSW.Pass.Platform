using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Start.ViewModels.TestPlans
{
    public class ChannelSettingViewModel : ViewModelBase
    {
        #region Fields
        private List<int> _siteCountList;
        private int? _siteCount;

        #endregion

        #region Properties 
        /// <summary>
        /// 站点数量列表
        /// </summary>
        public List<int> SiteCountList
        {
            get => _siteCountList;
            set => SetProperty(ref _siteCountList, value);
        }

        /// <summary>
        /// 站点数量
        /// </summary>
        public int? SiteCount
        {
            get => _siteCount;
            set => SetProperty(ref _siteCount, value);
        }
        #endregion

        #region Commands
        private DelegateCommand _addChannelCommand;
        public DelegateCommand AddChannelCommand =>
            _addChannelCommand ?? (_addChannelCommand = new DelegateCommand(ExecuteAddChannelCommand));

        private DelegateCommand _removeChannelCommand;
        public DelegateCommand RemoveChannelCommand =>
            _removeChannelCommand ?? (_removeChannelCommand = new DelegateCommand(ExecuteRemoveChannelCommand));

        #endregion

        public ChannelSettingViewModel(IContainerProvider containerProvider) : base(containerProvider)
        {
            InitList();
        }

        private void InitList()
        {
            _siteCountList = new List<int>();
            for (int i = 0; i < 255; i++)
            {
                _siteCountList.Add(i + 1);
            }
        }

        private void ExecuteAddChannelCommand()
        {
            throw new NotImplementedException();
        }

        private void ExecuteRemoveChannelCommand()
        {
            throw new NotImplementedException();
        }
    }
}
