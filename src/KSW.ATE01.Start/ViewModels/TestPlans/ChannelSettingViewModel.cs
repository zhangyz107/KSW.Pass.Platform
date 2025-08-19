using KSW.ATE01.Application.BLLs.Abstractions.Projects;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events.Projects;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Start.Views.Dialogs.TestPlans;
using KSW.Helpers;
using KSW.Ui;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.ATE01.Start.ViewModels.TestPlans
{
    public class ChannelSettingViewModel : ViewModelBase, INavigationAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IProjectBLL _projectBLL;
        private readonly IPinOverviewBLL _pinOverviewBLL;
        private readonly ISiteInfoBLL _siteInfoBLL;
        private readonly IPinInfoBLL _pinInfoBLL;
        private List<int> _siteCountList;
        private string _title;
        private ProjectInfoModel _projectInfo;
        private PinOverviewModel _pinOverview;
        private PinInfoModel _pinInfo;
        private ObservableCollection<PinInfoModel> _pinList = new ObservableCollection<PinInfoModel>();

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
        /// 标题
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// 项目信息
        /// </summary>
        public ProjectInfoModel ProjectInfo
        {
            get => _projectInfo;
            set => SetProperty(ref _projectInfo, value);
        }

        /// <summary>
        /// 引脚概览信息
        /// </summary>
        public PinOverviewModel PinOverview
        {
            get => _pinOverview;
            set => SetProperty(ref _pinOverview, value);
        }

        /// <summary>
        /// 站点数量
        /// </summary>
        public bool CanEdit => _pinOverview != null && PinList.IsEmpty();

        /// <summary>
        /// 引脚信息列表
        /// </summary>
        public ObservableCollection<PinInfoModel> PinList
        {
            get => _pinList;
            set => SetProperty(ref _pinList, value);
        }

        /// <summary>
        /// 当前引脚信息
        /// </summary>
        public PinInfoModel PinInfo
        {
            get => _pinInfo;
            set
            {
                if (SetProperty(ref _pinInfo, value))
                {
                    RemovePinCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region Commands
        private AsyncDelegateCommand _sureSiteCountCommand;
        public AsyncDelegateCommand SureSiteCountCommand =>
            _sureSiteCountCommand ?? (_sureSiteCountCommand = new AsyncDelegateCommand(ExecuteSureSiteCountCommand, () => CanEdit));

        private AsyncDelegateCommand _addPinCommand;
        public AsyncDelegateCommand AddPinCommand =>
            _addPinCommand ?? (_addPinCommand = new AsyncDelegateCommand(ExecuteAddPinCommand, () => !string.IsNullOrEmpty(_pinOverview?.Id) && _pinOverview?.SiteCount != null && _pinOverview?.SiteCount >= 1));

        private AsyncDelegateCommand _removePinCommand;
        public AsyncDelegateCommand RemovePinCommand =>
            _removePinCommand ?? (_removePinCommand = new AsyncDelegateCommand(ExecuteRemovePinCommand, () => !string.IsNullOrEmpty(_pinOverview?.Id) && _pinOverview?.SiteCount != null && _pinOverview?.SiteCount >= 1 && _pinInfo != null));

        private AsyncDelegateCommand _groupSettingCommand;
        public AsyncDelegateCommand GroupSettingCommand =>
            _groupSettingCommand ?? (_groupSettingCommand = new AsyncDelegateCommand(ExecuteGroupSettingCommand, () => !string.IsNullOrEmpty(_pinOverview?.Id)));

        private AsyncDelegateCommand<PinInfoModel> _pinGroupSettingCommand;
        public AsyncDelegateCommand<PinInfoModel> PinGroupSettingCommand =>
            _pinGroupSettingCommand ?? (_pinGroupSettingCommand = new AsyncDelegateCommand<PinInfoModel>(ExecutePinGroupSettingCommand));

        private AsyncDelegateCommand<PinInfoModel> _editPinCommand;
        public AsyncDelegateCommand<PinInfoModel> EditPinCommand =>
            _editPinCommand ?? (_editPinCommand = new AsyncDelegateCommand<PinInfoModel>(ExecuteEditPinCommand));

        #endregion

        public ChannelSettingViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IPinOverviewBLL pinOverviewBLL,
            IProjectBLL projectBLL,
            ISiteInfoBLL siteInfoBLL,
            IPinInfoBLL pinInfoBLL) : base(containerProvider)
        {
            Title = L["ChannelSetting"];
            L.PropertyChanged += (sender, args) =>
            {
                Title = L["ChannelSetting"];
            };
            _eventAggregator = eventAggregator;
            _pinOverviewBLL = pinOverviewBLL;
            _projectBLL = projectBLL;
            _siteInfoBLL = siteInfoBLL;
            _pinInfoBLL = pinInfoBLL;

            InitList();
            InitEvent();
        }

        private void InitList()
        {
            _siteCountList = new List<int>();
            for (int i = 0; i < 255; i++)
            {
                _siteCountList.Add(i + 1);
            }
        }

        private void InitEvent()
        {
            _eventAggregator.GetEvent<SelectedProjectInfoEvent>().Subscribe(SelectedProjectInfo);
        }

        private async void SelectedProjectInfo()
        {
            var projectInfo = _projectBLL?.GetCurrentProjectInfo();

            if (projectInfo != null)
            {
                ProjectInfo = projectInfo;
                var pinOvewview = await _pinOverviewBLL?.GetPinOverviewFromProjectIdAsync(projectInfo.Id);

                if (pinOvewview != null)
                    PinOverview = pinOvewview;
                else
                {
                    PinOverview = new PinOverviewModel();
                    PinOverview.ProjectInfoId = projectInfo.Id.ToGuid();
                }
                PinOverview.PropertyChanged += PinOverview_PropertyChanged;

                await ReloadPinList();
            }

            RaisePropertyChanged(nameof(CanEdit));
            SureSiteCountCommand.RaiseCanExecuteChanged();
            AddPinCommand.RaiseCanExecuteChanged();
            RemovePinCommand.RaiseCanExecuteChanged();
            GroupSettingCommand.RaiseCanExecuteChanged();
        }

        private async Task ReloadPinList()
        {
            var pinInfos = await _pinInfoBLL?.GetPinInfosFromOvewviewIdAsync(PinOverview.Id);

            _pinList.Clear();
            _pinList.AddRange(pinInfos);
        }

        private void PinOverview_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(PinOverviewModel.SiteCount)))
            {
                AddPinCommand.RaiseCanExecuteChanged();
                RemovePinCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task ExecuteSureSiteCountCommand()
        {
            var result = await _pinOverviewBLL?.SaveAsync(PinOverview);
            if (result)
            {
                var pinOvewview = await _pinOverviewBLL?.GetPinOverviewFromProjectIdAsync(ProjectInfo.Id);
                PinOverview = pinOvewview;
                PinOverview.PropertyChanged += PinOverview_PropertyChanged;

                for (int i = 0; i < _pinOverview.SiteCount; i++)
                {
                    var siteInfo = new SiteInfoModel();
                    siteInfo.PinOverviewId = PinOverview.Id.ToGuid();
                    siteInfo.SortId = i;
                    siteInfo.SiteName = $"Site {i}";

                    await _siteInfoBLL?.CreateAsync(siteInfo);
                }
            }
            AddPinCommand.RaiseCanExecuteChanged();
            RemovePinCommand.RaiseCanExecuteChanged();
        }

        private async Task ExecuteAddPinCommand()
        {
            var pinInfo = new PinInfoModel()
            {
                Id = Guid.NewGuid().ToString(),
                PinOverviewId = _pinOverview?.Id?.ToGuid() ?? Guid.Empty,
                IsNew = true,
            };

            var parameters = new DialogParameters();
            parameters.Add("PinInfoModel", pinInfo);

            await DialogService.ShowDialogAsync(nameof(AddPinDialogView), parameters);

            await ReloadPinList();
        }

        private async Task ExecuteRemovePinCommand()
        {
            var result = await DialogService.ShowMessageDialog(L["ConfirmTheDelete"], MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result?.Result == ButtonResult.Yes && _pinInfo != null)
            {
                _pinInfoBLL?.DeletePinAndDetailsByIdAsync(_pinInfo?.Id);

                await ReloadPinList();
            }
        }

        private async Task ExecuteGroupSettingCommand()
        {
            var parameters = new DialogParameters();
            parameters.Add("PinOverviewId", _pinOverview?.Id);
            await DialogService.ShowDialogAsync(nameof(GroupSettingDialogView), parameters);

            await ReloadPinList();
        }

        private async Task ExecutePinGroupSettingCommand(PinInfoModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("PinInfoModel", model);

            await DialogService.ShowDialogAsync(nameof(EditPinGroupDialogView), parameters);

            await ReloadPinList();
        }

        private async Task ExecuteEditPinCommand(PinInfoModel model)
        {
            var parameters = new DialogParameters();
            parameters.Add("PinInfoModel", model);

            await DialogService.ShowDialogAsync(nameof(AddPinDialogView), parameters);

            await ReloadPinList();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}
