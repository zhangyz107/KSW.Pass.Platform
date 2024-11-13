using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.ATE01.Pattern.Start.Views;
using KSW.Ui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Start.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region Properties
        public ObservableCollection<PatternInfoModel> PatternInfos { get; private set; } = new ObservableCollection<PatternInfoModel>();
        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));
        #endregion

        public ShellViewModel(IContainerProvider containerProvider, IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
        }


        private void ExecuteLoadingCommand()
        {
            var length = 10;
            for (int i = 0; i < length; i++)
            {
                var tempPattern = new PatternInfoModel();
                tempPattern.Vector = i + 1;
                tempPattern.Label = $"Label{i}";
                tempPattern.Command = new CommandInfoModel()
                {
                    Type = CommandType.repeat,
                };
                tempPattern.Instrument = $"Instrument{i}";
                tempPattern.TimingName = "time_fun";
                tempPattern.PinInfos = GetPinInfos();

                PatternInfos.Add(tempPattern);
            }

            var pattern = PatternInfos.FirstOrDefault();

            _eventAggregator?.GetEvent<PatternColInfoUpdateEvent>().Publish(pattern);
        }

        private List<PinInfoModel> GetPinInfos()
        {
            var result = new List<PinInfoModel>();
            var length = 16;
            for (int i = 0; i < length; i++)
            {
                var pinInfo = new PinInfoModel()
                {
                    PinName = $"A{i + 1}",
                    VectorValue = VectorValueType.X
                };
                result.Add(pinInfo);
            }

            return result;
        }
    }
}
