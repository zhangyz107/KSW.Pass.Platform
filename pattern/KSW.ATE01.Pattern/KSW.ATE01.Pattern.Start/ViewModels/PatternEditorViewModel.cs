/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PatternEditorViewModel.cs
// 功能描述：向量编辑视图模型
//
// 作者：zhangyingzhong
// 日期：2025/01/10 15:33
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Instruments;
using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Events.Instruments;
using KSW.ATE01.Pattern.Application.Events.Patterns;
using KSW.ATE01.Pattern.Application.Models.Instruments;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Instruments.Entities;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.ATE01.Pattern.Start.Views;
using KSW.Ui;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Configuration;

namespace KSW.ATE01.Pattern.Start.ViewModels
{
    /// <summary>
    /// 向量编辑视图模型
    /// </summary>
    public class PatternEditorViewModel : ViewModelBase
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IPatternCompilerBLL _patternCompilerBLL;
        private PatternModel _patternModel;
        private PatternVectorModel _vectorRow;
        private bool _showPinOverview = true;
        private bool _showDebug;
        private InstrumentManageView _instrumentManageView;
        private const double _timeResolution = 6.25e-10;
        private const double _timeSet = 1e-6;
        private const WaveformType _waveformType = WaveformType.NR;
        private const StrobeType _strobeType = StrobeType.Edge;
        private const long _mbByte = 128 * 1024 * 1024L;
        private const int _channelGroups = 32;
        private const long _vectorUnit = 4 * 1024;
        #endregion

        #region Properties
        public ObservableCollection<PatternVectorModel> VectorInfos { get; private set; } = new ObservableCollection<PatternVectorModel>();

        public ObservableCollection<PinModel> PinCols { get; set; } = new ObservableCollection<PinModel>();

        public PatternVectorModel VectorRow
        {
            get => _vectorRow;
            set
            {
                if (SetProperty(ref _vectorRow, value))
                {
                    DeleteVectorCommand.RaiseCanExecuteChanged();
                }

            }
        }

        public bool ShowPinOverview
        {
            get => _showPinOverview;
            set => SetProperty(ref _showPinOverview, value);
        }

        public bool ShowDebug
        {
            get => _showDebug;
            set => SetProperty(ref _showDebug, value);
        }

        public InstrumentManageView InstrumentManageView
        {
            get => _instrumentManageView;
            set => SetProperty(ref _instrumentManageView, value);
        }

        #endregion

        #region Command
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ?? (_loadingCommand = new DelegateCommand(ExecuteLoadingCommand));

        private DelegateCommand _addVectorCommand;
        public DelegateCommand AddVectorCommand =>
            _addVectorCommand ?? (_addVectorCommand = new DelegateCommand(ExecuteAddVectorCommand, () => { return _patternModel != null; }));

        private DelegateCommand _openCommand;
        public DelegateCommand OpenCommand =>
             _openCommand ?? (_openCommand = new DelegateCommand(ExecuteOpenCommand));

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
             _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand, () => { return _patternModel != null; }));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand =>
             _saveAsCommand ?? (_saveAsCommand = new DelegateCommand(ExecuteSaveAsCommand, () => { return _patternModel != null; }));

        private AsyncDelegateCommand _exportAtpCommand;
        public AsyncDelegateCommand ExportAtpCommand =>
             _exportAtpCommand ?? (_exportAtpCommand = new AsyncDelegateCommand(ExecuteExportAtpCommand, () => { return _patternModel != null; }));

        private DelegateCommand _refreshCommand;
        public DelegateCommand RefreshCommand =>
            _refreshCommand ?? (_refreshCommand = new DelegateCommand(ExecuteRefreshCommand));

        private DelegateCommand _pinOverviewVisibleCommand;
        public DelegateCommand PinOverviewVisibleCommand =>
            _pinOverviewVisibleCommand ?? (_pinOverviewVisibleCommand = new DelegateCommand(ExecutePinOverviewVisibleCommand));

        private DelegateCommand<object> _insertVectorCommand;
        public DelegateCommand<object> InsertVectorCommand =>
            _insertVectorCommand ?? (_insertVectorCommand = new DelegateCommand<object>(ExecuteInsertVectorCommand, (x) => { return _patternModel != null; }));

        private DelegateCommand<object> _deleteVectorCommand;
        public DelegateCommand<object> DeleteVectorCommand =>
            _deleteVectorCommand ?? (_deleteVectorCommand = new DelegateCommand<object>(ExecuteDeleteVectorCommand, (x) => { return _vectorRow != null; }));
        #endregion

        public PatternEditorViewModel(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator,
            IPatternCompilerBLL patternCompilerBLL) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
            _patternCompilerBLL = patternCompilerBLL;

            var equipmentDebugStr = ConfigurationManager.AppSettings["EquipmentDebug"];
            bool.TryParse(equipmentDebugStr, out _showDebug);

            _instrumentManageView = containerProvider.Resolve<InstrumentManageView>();

            eventAggregator.GetEvent<PatternModelUpdateEvent>().Subscribe(PatternModelUpdate, ThreadOption.UIThread);
            eventAggregator.GetEvent<InstrumentSendEvent>().Subscribe(InstrumentSend);
            VectorInfos.CollectionChanged += PatternInfos_CollectionChanged;
        }

        private void ExecuteLoadingCommand()
        {
            AddVectorCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            SaveAsCommand.RaiseCanExecuteChanged();
            ExportAtpCommand.RaiseCanExecuteChanged();
            InsertVectorCommand.RaiseCanExecuteChanged();
            DeleteVectorCommand.RaiseCanExecuteChanged();
        }

        private void PatternModelUpdate(PatternModel model)
        {
            if (model == null)
                return;

            if (model.PatternVectors.IsEmpty())
                return;

            _patternModel = model;
            VectorInfos.Clear();
            _eventAggregator?.GetEvent<PatternColInfoUpdateEvent>().Publish(model);

            ExecuteRefreshCommand();

            foreach (var item in model.PatternVectors)
                VectorInfos.Add(item);
        }

        private void InstrumentSend(InstrumentInfoModel instrumentInfo)
        {
            if (_patternModel == null)
                return;

            if (_patternModel.PatternVectors.IsEmpty())
                return;

            var pins = _patternModel.PatternVectors.FirstOrDefault()?.Pins;
            if (pins.IsEmpty())
                return;

            var factory = ContainerProvider.Resolve<IInstrumentControlFactory>();
            var control = factory?.GetInstrumentControlService(instrumentInfo.ConnectType);
            if (control == null && control?.IsConnected(instrumentInfo) == false)
                return;

            var period = Convert.ToInt32(_timeSet / _timeResolution);
            var periodBytes = BitConverter.GetBytes(period);
            var formatByte = Convert.ToByte(_waveformType);
            var strobeByte = Convert.ToByte(_strobeType);
            var dBytes = new byte[4];
            var twoBytes = new byte[2];
            var sizeBytes = BitConverter.GetBytes(_mbByte);


            foreach (var item in pins)
            {
                var index = pins.IndexOf(item);
                var chNum = Convert.ToByte(index);
                var patternStartAddress = index * _mbByte;
                var patternEndAddress = (index + 1) * _mbByte;
                var channelGroup = index / _channelGroups + 1;
                var receiveStartAddress = (channelGroup * _channelGroups + index) * _mbByte;
                var receiveEndAddress = (channelGroup * _channelGroups + index + 1) * _mbByte;
                var patternStartBytes = BitConverter.GetBytes(patternStartAddress);
                var patternStopBytes = BitConverter.GetBytes(patternEndAddress);
                var receiveStartBytes = BitConverter.GetBytes(receiveStartAddress);
                var receiveStopBytes = BitConverter.GetBytes(receiveEndAddress);

                var patternStartAddressBytes = new byte[5];
                var patternStopAddressBytes = new byte[5];
                Array.Copy(patternStartBytes, patternStartAddressBytes, patternStartAddressBytes.Length);
                Array.Copy(patternStopBytes, patternStopAddressBytes, patternStopAddressBytes.Length);

                var receiveStartAddressBytes = new byte[5];
                var receiveStopAddressBytes = new byte[5];
                Array.Copy(receiveStartBytes, receiveStartAddressBytes, receiveStartAddressBytes.Length);
                Array.Copy(receiveStopBytes, receiveStopAddressBytes, receiveStopAddressBytes.Length);

                // 发送PinType
                control.Send(instrumentInfo,
                [
                    chNum,2
                ]);

                // 发送Timing
                var timingByteList = new List<byte>();
                timingByteList.Add(chNum);
                timingByteList.AddRange(periodBytes);
                timingByteList.Add(formatByte);
                timingByteList.Add(strobeByte);
                timingByteList.AddRange(dBytes); //D0
                timingByteList.AddRange(dBytes); //D1
                timingByteList.AddRange(dBytes); //D2
                timingByteList.AddRange(dBytes); //D3
                timingByteList.AddRange(dBytes); //R0
                timingByteList.AddRange(dBytes); //R1
                timingByteList.Add((byte)0);  //  PWA_EN
                timingByteList.Add((byte)0);  //  CD_EN
                timingByteList.AddRange(twoBytes);  //  FD_EN(2字节)
                timingByteList.Add((byte)0);  //  PWA_D
                timingByteList.Add((byte)0);  //  CD_D
                timingByteList.AddRange(twoBytes);   // FD_D(2字节)
                timingByteList.Add((byte)0);  //  PWA_CA
                timingByteList.Add((byte)0);  //  CD_CA
                timingByteList.AddRange(twoBytes);  //  FD_CA(2字节)
                timingByteList.Add((byte)0);  //  PWA_CB
                timingByteList.Add((byte)0);  //  CD_CB
                timingByteList.AddRange(twoBytes);  //  FD_CB(2字节)
                control.Send(instrumentInfo, timingByteList.ToArray());


                // 发送Pattern参数
                var patternParamByteList = new List<byte>();
                patternParamByteList.Add(chNum);
                patternParamByteList.Add(1);
                patternParamByteList.AddRange(patternStartAddressBytes);
                patternParamByteList.AddRange(patternStopAddressBytes);
                patternParamByteList.AddRange(receiveStartAddressBytes);
                patternParamByteList.AddRange(receiveStopAddressBytes);
                patternParamByteList.Add(0);    //  接收数据存入DDR
                patternParamByteList.Add(4);    //  比特数
                control.Send(instrumentInfo, patternParamByteList.ToArray());
            }

            #region 发送Pattern文件
            bool isEven = (_patternModel.PatternVectors.Count % 2 == 0);
            var dicVectors = new Dictionary<int, List<byte>>();
            int vectorLength = 0;
            if (isEven)
            {
                for (int i = 0; i < _patternModel.PatternVectors.Count; i += 2)
                {
                    var row1 = _patternModel.PatternVectors[i];
                    var row2 = _patternModel.PatternVectors[i + 1];
                    for (int j = 0; j < pins.Count; j++)
                    {
                        var pinByte = (byte)(row2.Pins[j].VectorValue.Value() << 4 | row1.Pins[j].VectorValue.Value());
                        if (dicVectors.ContainsKey(j))
                        {
                            dicVectors[j].Add(pinByte);
                        }
                        else
                        {
                            dicVectors[j] = new List<byte>();
                            dicVectors[j].Add(pinByte);
                        }
                    }
                    vectorLength++;
                }
            }
            else
            {
                for (int i = 0; i < _patternModel.PatternVectors.Count - 1; i += 2)
                {
                    var row1 = _patternModel.PatternVectors[i];
                    var row2 = _patternModel.PatternVectors[i + 1];
                    for (int j = 0; j < pins.Count; j++)
                    {
                        var pinByte = (byte)(row2.Pins[j].VectorValue.Value() << 4 | row1.Pins[j].VectorValue.Value());
                        if (dicVectors.ContainsKey(j))
                        {
                            dicVectors[j].Add(pinByte);
                        }
                        else
                        {
                            dicVectors[j] = new List<byte>();
                            dicVectors[j].Add(pinByte);
                        }
                    }
                    vectorLength++;
                }

                var lastRow = _patternModel.PatternVectors[_patternModel.PatternVectors.Count - 1];
                for (int j = 0; j < pins.Count; j++)
                {
                    var pinByte = Convert.ToByte(lastRow.Pins[j].VectorValue);
                    if (dicVectors.ContainsKey(j))
                    {
                        dicVectors[j].Add(pinByte);
                    }
                    else
                    {
                        dicVectors[j] = new List<byte>();
                        dicVectors[j].Add(pinByte);
                    }
                }
                vectorLength++;
            }

            int vectorUnitByte = 62;
            foreach (var vector in dicVectors)
            {
                var groups = vectorLength / (vectorUnitByte * 8) + 1;
                for (int i = 0; i < groups; i++)
                {
                    var tempPatternByte = new byte[_vectorUnit];
                    tempPatternByte[i] = (byte)vector.Key;
                    var addr = vector.Key * _mbByte + i * _vectorUnit;
                    var addrBytes = BitConverter.GetBytes(addr);
                    Array.Copy(addrBytes, 0, tempPatternByte, 1, 5);
                    var unitBytes = BitConverter.GetBytes(_vectorUnit);
                    Array.Copy(unitBytes, 0, tempPatternByte, 6, 2);

                    GetVectorPattern(tempPatternByte, 8, vector.Value.ToArray(), vectorUnitByte);
                    control.Send(instrumentInfo, tempPatternByte);
                }
            }
            #endregion

        }

        private void GetVectorPattern(byte[] tempPatternByte, int startIndex, byte[] vectors, int unitLength)
        {
            var unitCount = vectors.Count() / unitLength + 1;
            for (int i = 0; i < unitCount; i++)
            {
                tempPatternByte[startIndex++] = (byte)unitLength;
                tempPatternByte[startIndex++] = 0;
                if (i != unitCount - 1)
                {
                    Array.Copy(vectors, i * unitLength, tempPatternByte, startIndex, unitLength);
                    startIndex += unitLength;
                }
                else
                {
                    var lastCount = vectors.Count() - (i * unitLength);
                    Array.Copy(vectors, i * unitLength, tempPatternByte, startIndex, lastCount);
                }
            }
        }

        private void PatternInfos_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var index = 0;
            foreach (var item in VectorInfos)
                item.Label.IndexInVectors = ++index;
        }
        //private List<PinModel> GetPinInfos()
        //{
        //    var result = new List<PinModel>();
        //    var length = 16;
        //    for (int i = 0; i < length; i++)
        //    {
        //        var pinInfo = new PinModel()
        //        {
        //            PinName = $"A{i + 1}",
        //            VectorValue = VectorValueType.X
        //        };
        //        result.Add(pinInfo);
        //    }

        //    return result;
        //}

        private void ExecuteAddVectorCommand()
        {
            VectorInfos.Add(new PatternVectorModel()
            {
                Label = new LabelModel(),
                Command = new CommandModel()
            });
        }

        private void ExecuteOpenCommand()
        {

        }

        private void ExecuteSaveCommand()
        {

        }

        private void ExecuteSaveAsCommand()
        {

        }

        private async Task ExecuteExportAtpCommand()
        {
            var fileSaveDialog = new SaveFileDialog();
            fileSaveDialog.Filter = ".atp文件|*.atp";
            if (fileSaveDialog.ShowDialog() == true)
            {
                var filePath = fileSaveDialog.FileName;
                await _patternCompilerBLL.ExportPattern(_patternModel, filePath);
            }
        }

        private void ExecuteRefreshCommand()
        {
            if (_patternModel == null)
                return;

            if (_patternModel.PatternVectors.IsEmpty())
                return;

            PinCols.Clear();
            var tempRow = _patternModel?.PatternVectors?.FirstOrDefault();

            foreach (var item in tempRow.Pins)
                PinCols.Add(item);

        }

        private void ExecutePinOverviewVisibleCommand()
        {
            ShowPinOverview = !_showPinOverview;
        }

        private void ExecuteInsertVectorCommand(object obj)
        {
            if (obj is PatternVectorModel item)
            {
                var index = VectorInfos.IndexOf(item);
                VectorInfos.Insert(index + 1, new PatternVectorModel());
            }
        }

        private void ExecuteDeleteVectorCommand(object obj)
        {
            if (obj is PatternVectorModel item)
                VectorInfos.Remove(item);

            if (VectorRow != null)
                VectorInfos.Remove(VectorRow);
        }
    }
}
