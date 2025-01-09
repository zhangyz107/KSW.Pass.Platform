using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns;
using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.Ui;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace KSW.ATE01.Pattern.Start.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPatternCompilerBLL _patternCompilerBLL;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region Properties
        public ObservableCollection<PatternInfoModel> PatternInfos { get; private set; } = new ObservableCollection<PatternInfoModel>();
        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _openCommand;
        public DelegateCommand OpenCommand =>
            _openCommand ?? (_openCommand = new DelegateCommand(ExecuteOpenCommand));
        #endregion

        public ShellViewModel(
            IContainerProvider containerProvider,
            IPatternCompilerBLL patternCompilerBLL,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _patternCompilerBLL = patternCompilerBLL;
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

        private void ExecuteOpenCommand()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "atp files(*.atp)|*.atp";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                var file = openFileDialog.FileName;
                var fileName = Path.GetFileNameWithoutExtension(file);
                var patternsDir = Path.GetDirectoryName(file);
                var binFileName = Path.Combine(patternsDir, $"{fileName}.bin");

                var releaseDir = Path.GetDirectoryName(patternsDir);
                var xlsms = Directory.GetFiles(releaseDir, "*.xlsm");
                var testPlanFilePath = string.Empty;
                if (!xlsms.IsEmpty())
                {
                    testPlanFilePath = xlsms.FirstOrDefault();
                }
                var testPlanSheetName = "Channel";

                _patternCompilerBLL.SetCompilerPath(file, testPlanFilePath, testPlanSheetName, binFileName);

                _patternCompilerBLL.CompilePattern();
            }
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
