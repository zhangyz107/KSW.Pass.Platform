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
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    /// <summary>
    /// 添加引脚对话框视图模型
    /// </summary>
    public class AddPinDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IPinInfoBLL _pinInfoBLL;
        private readonly ISiteInfoBLL _siteInfoBLL;
        private string _title;
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
            IEventAggregator eventAggregator,
            IPinInfoBLL pinInfoBLL,
            ISiteInfoBLL siteInfoBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _pinInfoBLL = pinInfoBLL;
            _siteInfoBLL = siteInfoBLL;
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
                                IsNew = true
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
                    if (_pinInfo?.PinSiteInfos.IsEmpty() == false)
                    {
                        foreach (var pinSiteInfo in _pinInfo?.PinSiteInfos)
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
            if (PinInfo.PinName.IsEmpty())
            {
                var message = $"{L["PinName"]}{L["CanNotBeEmpty"]}";
                SendMessage(message);
                return;
            }

            var channelNameList = PinSiteList.GroupBy(x => x.ChannelName).Where(y => y.Count() > 1);
            if (!channelNameList.IsEmpty())
            {
                var siteNames = string.Join(",", channelNameList.FirstOrDefault()?.Select(x => x.SiteName));
                var message = string.Format(L["FieldValueSame"], siteNames, L["ChannelName"]);
                SendMessage(message);
                return;
            }

            try
            {
                var createPinSiteList = _pinSiteList.Where(x => x.IsNew).ToList();
                var updatePinSiteList = _pinSiteList.Where(x => !x.IsNew).ToList();

                if (_pinInfo?.IsNew == true)
                {
                    _pinInfo.PinSiteInfos = createPinSiteList;
                    await _pinInfoBLL?.CreateAsync(_pinInfo);
                }
                else
                {
                    _pinInfo.PinSiteInfos = updatePinSiteList;
                    await _pinInfoBLL?.UpdateAsync(_pinInfo);
                }

                RaiseRequestClose(new DialogResult(ButtonResult.OK));
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                {
                    Type = UI.WPF.Enums.NotificationType.Error,
                    Content = e.Message,
                });
            }
        }

        private void SendMessage(string message)
        {
            _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
            {
                Type = UI.WPF.Enums.NotificationType.Error,
                Content = message,
            });
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
