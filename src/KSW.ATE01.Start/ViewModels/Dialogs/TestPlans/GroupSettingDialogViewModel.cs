using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Start.Styles;
using KSW.Ui;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    public class GroupSettingDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IGroupInfoBLL _groupInfoBLL;
        private readonly IPinInfoBLL _pinInfoBLL;
        private readonly IPinGroupRelationshipBLL _pinGroupRelationshipBLL;
        private GroupInfoModel _groupInfo;
        private string _pinOverviewId;
        private SnackbarMessageQueue _messageQueue;
        private SolidColorBrush _messageBackground;
        private ObservableCollection<GroupInfoModel> _groupInfoList = new ObservableCollection<GroupInfoModel>();
        private ObservableCollection<PinGroupRelationshipModel> _relationshipList = new ObservableCollection<PinGroupRelationshipModel>();
        private Dictionary<Guid, string> _pinInfoSelectList;
        private PinGroupRelationshipModel _selectRelationship;

        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => L["GroupSetting"];

        public ObservableCollection<GroupInfoModel> GroupInfoList
        {
            get => _groupInfoList;
            set => SetProperty(ref _groupInfoList, value);
        }

        public GroupInfoModel GroupInfo
        {
            get => _groupInfo;
            set
            {
                if (SetProperty(ref _groupInfo, value))
                {
                    AddPinNameCommand.RaiseCanExecuteChanged();
                    ChangeRelationShipList(value?.Id);
                }
            }
        }

        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set => SetProperty(ref _messageQueue, value);
        }

        /// <summary>
        /// 提示消息的背景色
        /// </summary>
        public SolidColorBrush MessageBackground
        {
            get => _messageBackground;
            set => SetProperty(ref _messageBackground, value);
        }

        /// <summary>
        /// 关系列表
        /// </summary>
        public ObservableCollection<PinGroupRelationshipModel> RelationshipList
        {
            get => _relationshipList;
            set => SetProperty(ref _relationshipList, value);
        }

        /// <summary>
        /// 引脚选择列表
        /// </summary>
        public Dictionary<Guid, string> PinInfoSelectList
        {
            get => _pinInfoSelectList;
            set => SetProperty(ref _pinInfoSelectList, value);
        }

        /// <summary>
        /// 选中的组关系
        /// </summary>
        public PinGroupRelationshipModel SelectRelationship
        {
            get => _selectRelationship;
            set => SetProperty(ref _selectRelationship, value);
        }
        #endregion

        #region Commands
        private DelegateCommand _addGroupNameCommand;
        public DelegateCommand AddGroupNameCommand =>
            _addGroupNameCommand ?? (_addGroupNameCommand = new DelegateCommand(ExecuteAddGroupNameCommand, CanAddGroupName));

        private AsyncDelegateCommand<GroupInfoModel> _sureGroupNameCommand;
        public AsyncDelegateCommand<GroupInfoModel> SureGroupNameCommand =>
            _sureGroupNameCommand ?? (_sureGroupNameCommand = new AsyncDelegateCommand<GroupInfoModel>(ExecuteSureGroupNameCommand));

        private AsyncDelegateCommand<GroupInfoModel> _deleteGroupNameCommand;
        public AsyncDelegateCommand<GroupInfoModel> DeleteGroupNameCommand =>
            _deleteGroupNameCommand ?? (_deleteGroupNameCommand = new AsyncDelegateCommand<GroupInfoModel>(ExecuteDeleteGroupNameCommand));

        private AsyncDelegateCommand _addPinNameCommand;
        public AsyncDelegateCommand AddPinNameCommand =>
            _addPinNameCommand ?? (_addPinNameCommand = new AsyncDelegateCommand(ExecuteAddPinNameCommand, CanAddPinName));

        private AsyncDelegateCommand<PinGroupRelationshipModel> _surePinNameCommand;
        public AsyncDelegateCommand<PinGroupRelationshipModel> SurePinNameCommand =>
            _surePinNameCommand ?? (_surePinNameCommand = new AsyncDelegateCommand<PinGroupRelationshipModel>(ExecuteSurePinNameCommand));

        private AsyncDelegateCommand<PinGroupRelationshipModel> _deletePinNameCommand;
        public AsyncDelegateCommand<PinGroupRelationshipModel> DeletePinNameCommand =>
            _deletePinNameCommand ?? (_deletePinNameCommand = new AsyncDelegateCommand<PinGroupRelationshipModel>(ExecuteDeletePinNameCommand));

        private AsyncDelegateCommand _okCommand;
        public AsyncDelegateCommand OKCommand =>
            _okCommand ?? (_okCommand = new AsyncDelegateCommand(ExecuteOKCommand, CanOK));

        private DelegateCommand _cancelCommand;
        public DelegateCommand CnacelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(ExecuteCnacelCommand));
        #endregion

        public GroupSettingDialogViewModel(
            IContainerProvider containerProvider,
            IGroupInfoBLL groupInfoBLL,
            IPinInfoBLL pinInfoBLL,
            IPinGroupRelationshipBLL pinGroupRelationshipBLL) : base(containerProvider)
        {
            _groupInfoBLL = groupInfoBLL;
            _pinInfoBLL = pinInfoBLL;
            _pinGroupRelationshipBLL = pinGroupRelationshipBLL;

            _messageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
        }

        private bool CanAddGroupName()
        {
            var result = true;
            if (!_groupInfoList.IsEmpty())
            {
                foreach (var item in _groupInfoList)
                {
                    if (item.GroupName.IsEmpty())
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private bool CanAddPinName()
        {
            var result = true;
            if (_groupInfo == null || _groupInfo?.GroupName.IsEmpty() == true)
            {
                result = false;
            }

            foreach (var item in _relationshipList)
            {
                if (item.PinInfoId.IsEmpty())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private void ExecuteAddGroupNameCommand()
        {
            var groupInfo = new GroupInfoModel()
            {
                PinOverviewId = _pinOverviewId?.ToGuid() ?? Guid.Empty,
                IsNew = true,
                SortId = _groupInfoList.Count + 1,
            };

            _groupInfoList.Add(groupInfo);
            AddGroupNameCommand.RaiseCanExecuteChanged();
        }

        private async Task ExecuteSureGroupNameCommand(GroupInfoModel model)
        {
            try
            {
                if (model.IsNew)
                {
                    var id = await _groupInfoBLL?.CreateAsync(model);
                    var newModel = await _groupInfoBLL?.GetByIdAsync(id);
                    UpdateModel(model, newModel);
                }
                else
                {
                    var newModel = await _groupInfoBLL?.UpdateAsync(model);
                    UpdateModel(model, newModel);
                }
                AddGroupNameCommand.RaiseCanExecuteChanged();
                AddPinNameCommand.RaiseCanExecuteChanged();
                var message = $"{L["OperationSuccessful"]}!";
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.SuccessColor);
                MessageQueue.Enqueue(message);
            }
            catch (Exception e)
            {
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.ErrorColor);
                MessageQueue.Enqueue(e.Message);
            }
        }

        private void UpdateModel(GroupInfoModel oldModel, GroupInfoModel newModel)
        {
            oldModel.Id = newModel.Id;
            oldModel.GroupName = newModel.GroupName;
            oldModel.CreationTime = newModel.CreationTime;
            oldModel.LastModificationTime = newModel.LastModificationTime;
            oldModel.IsNew = newModel.IsNew;
            oldModel.Version = newModel.Version;
        }

        private async Task ExecuteDeleteGroupNameCommand(GroupInfoModel model)
        {
            try
            {
                if (model.IsNew)
                    _groupInfoList.Remove(model);
                else
                {
                    await _groupInfoBLL?.DeleteWithDetailAsync(model.Id);
                    _groupInfoList.Remove(model);
                }

                AddGroupNameCommand.RaiseCanExecuteChanged();
                var message = $"{L["OperationSuccessful"]}!";
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.SuccessColor);
                MessageQueue.Enqueue(message);
            }
            catch (Exception e)
            {
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.ErrorColor);
                MessageQueue.Enqueue(e.Message);
            }
        }

        private async Task ChangeRelationShipList(string groupId)
        {
            var index = 0;
            var relationList = await _pinGroupRelationshipBLL?.GetListByGroupIdAsync(groupId);
            _relationshipList.Clear();
            if (!relationList.IsEmpty())
            {
                foreach (var relation in relationList)
                    relation.SortId = ++index;
                _relationshipList.AddRange(relationList);
            }
        }

        private async Task ExecuteAddPinNameCommand()
        {
            var ids = _relationshipList.Select(x => x.PinInfoId.SafeString()).ToList();
            var relationship = new PinGroupRelationshipModel()
            {
                GroupInfoId = GroupInfo?.Id?.ToGuid(),
                IsNew = true,
                SortId = _relationshipList.Count + 1,
            };
            _relationshipList.Add(relationship);
        }

        private async Task ExecuteSurePinNameCommand(PinGroupRelationshipModel model)
        {
            bool result = true;
            var message = string.Empty;

            if (model.PinInfoId.IsEmpty())
            {
                message = $"{L["PinName"]}{L["CanNotBeEmpty"]}";
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.ErrorColor);
                MessageQueue.Enqueue(message);
                return;
            }

            foreach (var item in _relationshipList)
            {
                var pinInfoId = item.PinInfoId ?? Guid.Empty;
                if (item.Id != model.Id && item.PinInfoId == model.PinInfoId && PinInfoSelectList.ContainsKey(pinInfoId))
                {
                    message = $"{string.Format(L["FieldAlreadyExists"], PinInfoSelectList[pinInfoId])},{L["PleaseEnterAgain"]}";
                    MessageBackground = new SolidColorBrush(SnackbarMessageStyle.ErrorColor);
                    MessageQueue.Enqueue(message);
                    model.PinInfoId = null;
                    result = false;
                    break;
                }
            }

            if (!result)
                return;

            var id = await _pinGroupRelationshipBLL?.SaveAsync(model);
            var newModel = await _pinGroupRelationshipBLL?.GetByIdAsync(id);
            UpdateModel(model, newModel);

            message = $"{L["OperationSuccessful"]}!";
            MessageBackground = new SolidColorBrush(SnackbarMessageStyle.SuccessColor);
            MessageQueue.Enqueue(message);
            AddPinNameCommand.RaiseCanExecuteChanged();
        }

        private void UpdateModel(PinGroupRelationshipModel oldModel, PinGroupRelationshipModel newModel)
        {
            oldModel.Id = newModel.Id;
            oldModel.GroupName = newModel.GroupName;
            oldModel.CreationTime = newModel.CreationTime;
            oldModel.LastModificationTime = newModel.LastModificationTime;
            oldModel.IsNew = newModel.IsNew;
            oldModel.Version = newModel.Version;
        }

        private async Task ExecuteDeletePinNameCommand(PinGroupRelationshipModel model)
        {
            if (model.IsNew)
                _relationshipList.Remove(model);
            else
            {
                await _pinGroupRelationshipBLL?.DeleteAsync(model.Id);
                _relationshipList.Remove(model);
            }

            AddPinNameCommand.RaiseCanExecuteChanged();
            var message = $"{L["OperationSuccessful"]}!";
            MessageBackground = new SolidColorBrush(SnackbarMessageStyle.SuccessColor);
            MessageQueue.Enqueue(message);
        }


        private async Task ExecuteOKCommand()
        {
            var result = true;
            var message = string.Empty;
            foreach (var item in _groupInfoList)
            {
                if (item.IsNew)
                {
                    message = $"{L["GroupName"]}:{item.SortId}{item.GroupName}{L["Notsaved"]}";
                    result = false;
                    break;
                }
            }

            if (result)
            {
                foreach (var item in _relationshipList)
                {
                    if (item.IsNew)
                    {
                        message = $"{L["PinName"]}:{item.SortId}{L["Notsaved"]}";
                        result = false;
                        break;
                    }
                }
            }

            if (!result)
            {
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.SuccessColor);
                MessageQueue.Enqueue(message);
            }
            else
                RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        private bool CanOK()
        {
            var result = true;
            var message = string.Empty;
            foreach (var groupInfo in _groupInfoList)
            {
                if (groupInfo.GroupName.IsEmpty())
                {
                    message = $"{L["GroupName"]}{L["CanNotBeEmpty"]}";
                    result = false;
                    break;
                }
            }

            if (result)
            {
                foreach (var relationship in _relationshipList)
                {
                    if (relationship.PinInfoId.IsEmpty())
                    {
                        message = $"{L["PinName"]}{L["CanNotBeEmpty"]}";
                        result = false;
                        break;
                    }
                }
            }

            if (!result)
            {
                MessageBackground = new SolidColorBrush(SnackbarMessageStyle.ErrorColor);
                MessageQueue.Enqueue(message);
            }
            return result;
        }

        private void ExecuteCnacelCommand()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            var pinOverviewId = parameters.GetValue<string>("PinOverviewId");
            _pinOverviewId = pinOverviewId;

            var index = 0;
            var groupInfos = await _groupInfoBLL?.GetListByOverviewIdAsync(pinOverviewId);
            foreach (var item in groupInfos)
                item.SortId = ++index;
            _groupInfoList.Clear();
            _groupInfoList.AddRange(groupInfos);

            var pinList = await _pinInfoBLL?.GetPinInfosFromOvewviewIdAsync(_pinOverviewId);
            PinInfoSelectList = pinList.ToDictionary(x => x.Id.ToGuid(), y => y.PinName);
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }
    }
}
