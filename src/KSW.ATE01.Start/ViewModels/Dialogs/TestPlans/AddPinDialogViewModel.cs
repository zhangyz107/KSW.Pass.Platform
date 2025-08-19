/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：AddPinDialogViewModel.cs
// 功能描述：添加引脚对话框视图模型
//
// 作者：zhangyingzhong
// 日期：2025/08/14 11:03
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Helpers;
using KSW.Ui;
using System.Collections.ObjectModel;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    /// <summary>
    /// 添加引脚对话框视图模型
    /// </summary>
    public class AddPinDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IPinOverviewBLL _pinOverviewBLL;
        private readonly ISiteInfoBLL _siteInfoBLL;
        private readonly IPinInfoBLL _pinInfoBLL;
        private readonly IPinSiteInfoBLL _pinSiteInfoBLL;
        private string _title;
        private PinOverviewModel _pinOverview;
        private PinInfoModel _pinInfo;
        private ObservableCollection<PinSiteInfoModel> _pinSiteList = new ObservableCollection<PinSiteInfoModel>();
        #endregion

        #region Properties
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// 引脚信息
        /// </summary>
        public PinInfoModel PinInfo
        {
            get => _pinInfo;
            set => SetProperty(ref _pinInfo, value);
        }

        /// <summary>
        /// 引脚站点列表
        /// </summary>
        public ObservableCollection<PinSiteInfoModel> PinSiteList
        {
            get => _pinSiteList;
            set => SetProperty(ref _pinSiteList, value);
        }


        /// <summary>
        /// 引脚类型字典
        /// </summary>
        public Dictionary<PinType, string> PinTypDic { get; } = new Dictionary<PinType, string>()
        {
            { PinType.IO, PinType.IO.Description() },
            { PinType.DPS, PinType.DPS.Description() },
            { PinType.VNA, PinType.VNA.Description() },
        };
        #endregion

        #region Commands
        private AsyncDelegateCommand _oKCommand;
        public AsyncDelegateCommand OKCommand =>
            _oKCommand ?? (_oKCommand = new AsyncDelegateCommand(ExecuteOKCommand, CheckAllSiteChannelName));

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCancelCommand));
        #endregion

        public AddPinDialogViewModel(
            IContainerProvider containerProvider,
            IPinOverviewBLL pinOverviewBLL,
            IPinInfoBLL pinInfoBLL,
            ISiteInfoBLL siteInfoBLL,
            IPinSiteInfoBLL pinSiteInfoBLL) : base(containerProvider)
        {
            _pinOverviewBLL = pinOverviewBLL;
            _pinInfoBLL = pinInfoBLL;
            _siteInfoBLL = siteInfoBLL;
            _pinSiteInfoBLL = pinSiteInfoBLL;
        }

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            Title = L["EditPin"];

            var pinInfo = parameters.GetValue<PinInfoModel>("PinInfoModel");
            if (pinInfo != null)
            {
                _pinOverview = await _pinOverviewBLL?.GetByIdAsync(pinInfo?.PinOverviewId.SafeString());
                var siteInfos = await _siteInfoBLL?.GetSiteInfosFromPinOverviewId(pinInfo?.PinOverviewId.SafeString());

                PinInfo = pinInfo;

                _pinInfo.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(PinInfoModel.PinName))
                        OKCommand.RaiseCanExecuteChanged();
                    if (args.PropertyName == nameof(PinInfoModel.PinType))
                        OKCommand.RaiseCanExecuteChanged();
                };

                if (_pinInfo?.IsNew == true)
                {
                    Title = L["AddPin"];
                    if (!siteInfos.IsEmpty())
                    {
                        var index = 0;
                        foreach (var siteInfo in siteInfos)
                        {
                            var pinSiteInfo = new PinSiteInfoModel()
                            {
                                SiteName = siteInfo.SiteName,
                                SiteInfoId = siteInfo.Id.ToGuid(),
                                PinInfoId = PinInfo.Id.ToGuid(),
                                ChannelName = string.Empty,
                                SortId = index++,
                            };
                            pinSiteInfo.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == nameof(PinSiteInfoModel.ChannelName))
                                    OKCommand.RaiseCanExecuteChanged();
                            };
                            _pinSiteList.Add(pinSiteInfo);
                        }
                    }
                }
                else
                {
                    var pinSiteInfos = await _pinSiteInfoBLL?.GetPinSiteByPinIdAsync(_pinInfo?.Id);
                    if (pinSiteInfos != null)
                    {
                        foreach (var pinSiteInfo in pinSiteInfos)
                        {
                            pinSiteInfo.PropertyChanged += (sender, args) =>
                            {
                                if (args.PropertyName == nameof(PinSiteInfoModel.ChannelName))
                                    OKCommand.RaiseCanExecuteChanged();
                            };
                            _pinSiteList.Add(pinSiteInfo);
                        }
                    }
                }
            }
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        private async Task ExecuteOKCommand()
        {
            var pinInfos = await _pinInfoBLL?.GetPinInfosFromOvewviewIdAsync(_pinOverview.Id);
            if (!pinInfos.IsEmpty() && pinInfos.Any(x => x.PinName.Equals(_pinInfo.PinName)))
            {
                await DialogService.ShowMessageDialog(string.Format(L["FieldAlreadyExists"], L["PinName"]));
                return;
            }

            var channelNameList = PinSiteList.GroupBy(x => x.ChannelName).Where(y => y.Count() > 1);
            if (!channelNameList.IsEmpty())
            {
                var siteNames = string.Join(",", channelNameList.FirstOrDefault()?.Select(x => x.SiteName));
                await DialogService.ShowMessageDialog(string.Format(L["FieldValueSame"], siteNames, L["ChannelName"]));
                return;
            }

            var pinSiteInfos = await _pinSiteInfoBLL?.GetAllPinSiteByOverviewIdAsync(_pinOverview?.Id);
            var channelNames = pinSiteInfos.Select(x => x.ChannelName).ToList();
            var pinSiteInfo = pinSiteInfos.Where(x => channelNames.Any(y => y.Equals(x.ChannelName))).Select(x => x)?.FirstOrDefault();
            if (!pinSiteInfos.IsEmpty() && pinSiteInfo != null)
            {
                await DialogService.ShowMessageDialog(string.Format(L["FieldAlreadyExists"], $"{pinSiteInfo.SiteName}:{L["ChannelName"]}"));
                return;
            }

            var pinInfoId = await _pinInfoBLL?.SaveAsync(PinInfo);
            if (!pinInfoId.IsEmpty())
                await _pinSiteInfoBLL?.SaveAsync(PinSiteList.ToList(), null, null);

            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        private bool CheckAllSiteChannelName()
        {
            var result = true;

            result = _pinInfo?.PinName?.IsEmpty() == false;
            result = _pinInfo?.PinType != null;

            if (!result)
                return result;

            foreach (var item in _pinSiteList)
            {
                if (item.ChannelName.IsEmpty())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private void ExecuteCancelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
