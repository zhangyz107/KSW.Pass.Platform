using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Events;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.Ui;
using KSW.UI.WPF.Controls;
using System.Collections.ObjectModel;

namespace KSW.ATE01.Start.ViewModels.Dialogs.TestPlans
{
    public class EditPinGroupDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IGroupInfoBLL _groupInfoBLL;
        private readonly IPinGroupRelationshipBLL _pinGroupRelationshipBLL;
        private PinInfoModel _pinInfo;
        private ObservableCollection<GroupInfoModel> _groupInfoList = new ObservableCollection<GroupInfoModel>();
        private ObservableCollection<PinGroupRelationshipModel> _relationshipList = new ObservableCollection<PinGroupRelationshipModel>();
        #endregion

        #region Properties
        public DialogCloseListener RequestClose { get; }

        public string Title => L["EditPinGroup"];

        public ObservableCollection<GroupInfoModel> GroupInfoList
        {
            get => _groupInfoList;
            set => SetProperty(ref _groupInfoList, value);
        }

        public ObservableCollection<PinGroupRelationshipModel> RelationshipList
        {
            get => _relationshipList;
            set => SetProperty(ref _relationshipList, value);
        }
        #endregion

        #region Commands
        private DelegateCommand _addGroupNameCommand;
        public DelegateCommand AddGroupNameCommand =>
            _addGroupNameCommand ?? (_addGroupNameCommand = new DelegateCommand(ExecuteAddGroupNameCommand, CanAddGroupName));

        private AsyncDelegateCommand<GroupInfoModel> _sureGroupNameCommand;
        public AsyncDelegateCommand<GroupInfoModel> SureGroupNameCommand =>
            _sureGroupNameCommand ?? (_sureGroupNameCommand = new AsyncDelegateCommand<GroupInfoModel>(ExecuteSureGroupNameCommand));

        private AsyncDelegateCommand<GroupInfoModel> _moveRightCommand;
        public AsyncDelegateCommand<GroupInfoModel> MoveRightCommand =>
            _moveRightCommand ?? (_moveRightCommand = new AsyncDelegateCommand<GroupInfoModel>(ExecuteMoveRightCommand, CanMoveRight));

        private AsyncDelegateCommand<GroupInfoModel> _deleteGroupNameCommand;
        public AsyncDelegateCommand<GroupInfoModel> DeleteGroupNameCommand =>
            _deleteGroupNameCommand ?? (_deleteGroupNameCommand = new AsyncDelegateCommand<GroupInfoModel>(ExecuteDeleteGroupNameCommand));

        private AsyncDelegateCommand<PinGroupRelationshipModel> _moveLeftCommand;
        public AsyncDelegateCommand<PinGroupRelationshipModel> MoveLeftCommand =>
            _moveLeftCommand ?? (_moveLeftCommand = new AsyncDelegateCommand<PinGroupRelationshipModel>(ExecuteMoveLeftCommand));
        #endregion

        public EditPinGroupDialogViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IGroupInfoBLL groupInfoBLL,
            IPinGroupRelationshipBLL pinGroupRelationshipBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _groupInfoBLL = groupInfoBLL;
            _pinGroupRelationshipBLL = pinGroupRelationshipBLL;
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

        private void ExecuteAddGroupNameCommand()
        {
            var groupInfo = new GroupInfoModel()
            {
                PinOverviewId = _pinInfo?.PinOverviewId ?? Guid.Empty,
                IsNew = true,
                SortId = _groupInfoList.Count + 1,
            };

            _groupInfoList.Add(groupInfo);
            AddGroupNameCommand.RaiseCanExecuteChanged();
            MoveRightCommand.RaiseCanExecuteChanged();
        }


        private async Task ExecuteSureGroupNameCommand(GroupInfoModel model)
        {
            var message = string.Empty;

            if (model.GroupName.IsEmpty())
            {
                message = $"{L["GroupName"]}{L["CanNotBeEmpty"]}";
                AddGroupNameCommand.RaiseCanExecuteChanged();
                _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                {
                    Content = message,
                    Type = UI.WPF.Enums.NotificationType.Error
                });
                return;
            }

            var isRepeate = false;
            foreach (var item in _groupInfoList)
            {
                if (item.Id != model.Id && item.GroupName.Equals(model.GroupName))
                {
                    isRepeate = true;
                    break;
                }
            }
            if (isRepeate)
            {
                message = $"{string.Format(L["FieldAlreadyExists"], model.GroupName)},{L["PleaseEnterAgain"]}";
                model.GroupName = string.Empty;
                AddGroupNameCommand.RaiseCanExecuteChanged();
                _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
                {
                    Content = message,
                    Type = UI.WPF.Enums.NotificationType.Error
                });
                return;
            }

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
            MoveRightCommand.RaiseCanExecuteChanged();
            message = $"{L["OperationSuccessful"]}!";
            _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
            {
                Content = message,
                Type = UI.WPF.Enums.NotificationType.Success
            });
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

        private bool CanMoveRight(GroupInfoModel model)
        {
            return model?.IsNew == false;
        }

        private async Task ExecuteMoveRightCommand(GroupInfoModel model)
        {
            var relationship = new PinGroupRelationshipModel()
            {
                GroupInfoId = model.Id.ToGuid(),
                PinInfoId = _pinInfo.Id.ToGuid(),
                IsNew = true
            };
            var id = await _pinGroupRelationshipBLL?.SaveAsync(relationship);
            var result = await _pinGroupRelationshipBLL?.GetByIdAsync(id);
            _relationshipList.Add(result);
            _groupInfoList.Remove(model);
        }

        private async Task ExecuteDeleteGroupNameCommand(GroupInfoModel model)
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
            _eventAggregator.GetEvent<ShowShellToastEvent>().Publish(new Toast()
            {
                Content = message,
                Type = UI.WPF.Enums.NotificationType.Success
            });
        }

        private async Task ExecuteMoveLeftCommand(PinGroupRelationshipModel model)
        {
            await _pinGroupRelationshipBLL?.DeleteAsync(model.Id);
            _relationshipList.Remove(model);

            await ReloadGroupInfoList();
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
            var pinInfo = parameters.GetValue<PinInfoModel>("PinInfoModel");
            if (pinInfo != null)
            {
                _pinInfo = pinInfo;

                var relationships = await _pinGroupRelationshipBLL?.GetListByPinIdAsync(pinInfo.Id);
                RelationshipList.AddRange(relationships);

                await ReloadGroupInfoList();
            }
        }

        private async Task ReloadGroupInfoList()
        {
            _groupInfoList.Clear();
            var relationshipGroupIds = _relationshipList.Select(x => x.GroupInfoId);
            var allGroupInfos = await _groupInfoBLL?.GetListByOverviewIdAsync(_pinInfo?.PinOverviewId.SafeString());
            var unselectedGroupInfos = allGroupInfos.Where(x => !relationshipGroupIds.Contains(x.Id.ToGuid()));
            _groupInfoList.AddRange(unselectedGroupInfos);
        }
    }
}
