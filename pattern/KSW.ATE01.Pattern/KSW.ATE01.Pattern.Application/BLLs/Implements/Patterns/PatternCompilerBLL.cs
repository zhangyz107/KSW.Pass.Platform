/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：PatternCompilerBLL.cs
// 功能描述：向量编译业务逻辑服务
//
// 作者：zhangyingzhong
// 日期：2024/12/30 16:03
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.Application;
using KSW.ATE01.Pattern.Application.BLLs.Abstractions.Patterns;
using KSW.ATE01.Pattern.Application.Events;
using KSW.ATE01.Pattern.Application.Events.Patterns;
using KSW.ATE01.Pattern.Application.Extensions;
using KSW.ATE01.Pattern.Application.Models.Projects;
using KSW.ATE01.Pattern.Domain.Projects.Core.Enums;
using KSW.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Regex = System.Text.RegularExpressions.Regex;

namespace KSW.ATE01.Pattern.Application.BLLs.Implements.Patterns
{
    /// <summary>
    /// 向量编译业务逻辑服务
    /// </summary>
    public class PatternCompilerBLL : ServiceBase, IPatternCompilerBLL
    {
        #region Field
        private readonly IEventAggregator _eventAggregator;
        private bool _compileError;
        private bool _recordingSwitchStart;
        private bool _recordingSwitchTrig;
        private bool _ignoreCurrentVectorRowTrig;
        private bool _ignoreCurrentVectorRowStart;
        private int _nestLoopIndex = -1;
        private int _dataBlockIndexStart = -1;
        private int _dataBlockIndexTrig = -1;
        private int _haltInVectorLinesPosition = -1;
        private int _validVectorLinesCountInPatternFile;
        private string _tempNestLoopOutermostLoopName = string.Empty;
        private const int bufferSize = 81920; // 80KB
        private Regex _digitalInstrumentRegex = new Regex("^\\s*digital_ins\\s*=\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);
        private Regex _opCodeModeRegex = new Regex("^\\s*opcode_mode\\s*=\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);
        private Regex _timeSetRegex = new Regex("^\\s*import\\s*tset\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);
        private Regex _instrumentsRegex = new Regex("^\\s*instruments\\s*=\\s*", RegexOptions.IgnoreCase);
        private Regex _atpPinsRegex = new Regex("\\s*([-a-zA-Z0-9_]*)\\s*([-a-zA-Z0-9_]*)\\s*\\(\\s*\\$tset\\s*,(.+)\\)", RegexOptions.IgnoreCase);
        private Regex _vectorRegex = new Regex("\\s*((([^{:>]+):){0,})\\s*([^{:>]*)>\\s*([-a-zA-Z0-9_]+)\\s+(.+?)\\s*;(.*)", RegexOptions.IgnoreCase);
        private Regex _mteRegex = new Regex("\\(\\s*mte\\s*=\\s*([\\s\\w!]+)\\s*\\)", RegexOptions.IgnoreCase);
        private Regex _commandLoopRegex = new Regex("\\s*loop([abc])\\s+([0-9]+)\\s+(>.+;).?", RegexOptions.IgnoreCase);
        private Regex _commandEndLoopRegex = new Regex("\\s*end_loop([abc])\\s+([0-9a-zA-Z_]+\\s*>.+;).?", RegexOptions.IgnoreCase);
        private Regex _commandRepeatRegex = new Regex("\\s*repeat\\s+([0-9]+)\\s+(>.+;).?", RegexOptions.IgnoreCase);
        private Regex _srmuCodeAnalysisRegex = new Regex("(if\\s*\\(\\s*([!\\w]+)\\s*\\)\\s*)*([!\\w]+)*(\\s+|\\()*([!\\w\\s]+)*", RegexOptions.IgnoreCase);

        private List<string> _listTotalMaskCC = new List<string>();
        private List<string> _dataBlockMarkerParameter = new List<string>();
        private Dictionary<string, int> _pseudoInstruDic = new Dictionary<string, int>();
        private Dictionary<string, int> _nestLoopBuffDic = new Dictionary<string, int>();
        private Dictionary<string, byte> _pinValueNameExchangeDic = new Dictionary<string, byte>();

        private List<string> LVM_MASKCC = new List<string> { "stv", "maskb", "maska", "rsrm" };
        private List<string> SRM_MASKCC = new List<string>
        {
            "accfail", "ccnd", "padd", "sadd", "clr_fail", "ign", "icc", "ifc", "stv", "maskb",
            "maska", "rlvm"
        };
        private Dictionary<string, int> VM_PSEUDO_INSTRU = new Dictionary<string, int>
        {
            { "repeat", 1 },
            { "loop", 2 },
            { "match", 3 },
            { "jump", 4 },
            { "halt", 5 },
            { "reburst", 6 },
            { "endloop", 7 },
            { "trig", 8 },
            { "start", 13 },
            { "cpua", 9 },
            { "cpuaend", 10 },
            { "fstart", 11 },
            { "fstop", 12 }
        };
        private Dictionary<string, int> LVM_PSEUDO_INSTRU = new Dictionary<string, int>
        {
            { "nop", 0 },
            { "halt", 0 },
            { "repeat", 1 },
            { "call", 4 },
            { "trig", 8 }
        };
        private Dictionary<string, int> SRM_PSEUDO_INSTRU = new Dictionary<string, int>
        {
            { "nop", 0 },
            { "clr_flag", 2 },
            { "loopa", 4 },
            { "loopb", 4 },
            { "loopc", 4 },
            { "fstart", 7 },
            { "fstop", 7 },
            { "enable", 8 },
            { "set_cpu", 11 },
            { "poploopa", 13 },
            { "poploopb", 13 },
            { "poploopc", 13 },
            { "jump", 17 },
            { "end_loopa", 23 },
            { "end_loopb", 23 },
            { "end_loopc", 23 },
            { "return_lvm", 26 },
            { "trig", 27 },
            { "repeat", 30 },
            { "repeat_cc", 31 }
        };

        private Dictionary<string, int> SRM_Condition_Value = new Dictionary<string, int>
        {
            { "none", 0 },
            { "fail", 1 },
            { "cpua", 2 },
            { "cpub", 4 },
            { "cpuc", 8 },
            { "cpud", 16 },
            { "ext", 32 },
            { "scf", 64 },
            { "xfail", 128 },
            { "xext", 256 },
            { "!fail", 513 },
            { "!cpua", 1026 },
            { "!cpub", 2052 },
            { "!cpuc", 4104 },
            { "!cpud", 8208 },
            { "!ext", 16416 },
            { "!scf", 32832 }
        };
        private Dictionary<string, int> SRM_Specific_ClrFlag_Condition_Value = new Dictionary<string, int>
        {
            { "fail", 1 },
            { "cpua", 2 },
            { "cpub", 4 },
            { "cpuc", 8 },
            { "cpud", 16 },
            { "ext", 32 },
            { "scf", 64 },
            { "xfail", 128 },
            { "xext", 256 }
        };
        private Dictionary<string, int> SRM_Specific_SetCPU_Condition_Value = new Dictionary<string, int>
        {
            { "cpua", 1 },
            { "cpub", 2 },
            { "cpuc", 4 },
            { "cpud", 8 }
        };

        private Dictionary<string, byte> VM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
        {
            { "0", 0 },
            { "1", 1 },
            { "L", 4 },
            { "H", 5 },
            { "M", 6 },
            { "X", 7 },
            { "0L", 8 },
            { "0H", 9 },
            { "1L", 10 },
            { "1H", 11 },
            { "D", 12 },
            { "C", 13 },
            { "V", 14 }
        };
        private Dictionary<string, byte> LVM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
        {
            { "0", 0 },
            { "X", 1 },
            { "-", 2 },
            { "1", 3 },
            { "L", 4 },
            { "M", 5 },
            { "V", 6 },
            { "H", 7 }
        };
        private Dictionary<string, byte> SRM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
        {
            { "0", 0 },
            { "X", 1 },
            { "-", 2 },
            { "1", 3 },
            { "L", 4 },
            { "M", 5 },
            { "V", 6 },
            { "H", 7 }
        };
        #endregion

        public PatternCompilerBLL(
            IContainerProvider containerProvider,
            IEventAggregator eventAggregator) : base(containerProvider)
        {
            _eventAggregator = eventAggregator;
        }

        #region Public
        public async Task<PatternModel> AnalysisPattern(string patternFilePath)
        {
            _compileError = false;
            var result = new PatternModel();

            await AnalysisPatternTimeSetAsync(patternFilePath, result);
            var instrumentInfo = await AnalysisPatternDigitalInstrumentAsync(patternFilePath);
            result.InstrumentName = instrumentInfo;
            var pinList = await AnalysisPatternPinsAsync(patternFilePath, result);
            UpdateParametersByModuleType(result.ModuleType);
            var instruments = new List<InstrumentModel>();
            AnalysisPatternInstrument(patternFilePath, pinList, instruments);
            if (instruments.Any())
            {
                result.InstrumentInfo = instruments.FirstOrDefault();
                GetInstrumentsInfoFromDataBlock(result, patternFilePath, pinList, instruments, out _dataBlockMarkerParameter);
            }
            AnalysisPatternVector(patternFilePath, pinList, result);
            return result;
        }

        public async Task AnalyzeAndCompilePatternAsync(string patternFile)
        {
            _compileError = false;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = new PatternModel();
            await AnalysisPatternTimeSetAsync(patternFile, result);
            _eventAggregator.GetEvent<RecordMessageEvent>().Publish(new Models.Instruments.RecordMessageModel()
            {
                RecordMessage = "解析向量文件的时钟设置",
                RecordTime = DateTime.Now
            });

            var instrumentInfo = await AnalysisPatternDigitalInstrumentAsync(patternFile);
            result.InstrumentName = instrumentInfo;
            _eventAggregator.GetEvent<RecordMessageEvent>().Publish(new Models.Instruments.RecordMessageModel()
            {
                RecordMessage = "解析向量文件的设备名称",
                RecordTime = DateTime.Now
            });

            var pinList = await AnalysisPatternPinsAsync(patternFile, result);
            _eventAggregator.GetEvent<RecordMessageEvent>().Publish(new Models.Instruments.RecordMessageModel()
            {
                RecordMessage = "解析向量文件的引脚名称",
                RecordTime = DateTime.Now
            });
            UpdateParametersByModuleType(result.ModuleType);

            await AnalysisAndCompilePatternVector(patternFile, pinList, result);
            stopwatch.Stop();
            _eventAggregator.GetEvent<RecordMessageEvent>().Publish(new Models.Instruments.RecordMessageModel()
            {
                RecordMessage = $"解析用时{stopwatch.ElapsedMilliseconds}ms",
                RecordTime = DateTime.Now
            });

            stopwatch.Restart();
            await MergeAllFiles(patternFile, pinList, result);
            stopwatch.Stop();
            _eventAggregator.GetEvent<RecordMessageEvent>().Publish(new Models.Instruments.RecordMessageModel()
            {
                RecordMessage = $"合并文件用时{stopwatch.ElapsedMilliseconds}ms",
                RecordTime = DateTime.Now
            });
        }

        private async Task MergeAllFiles(string patternFile, List<string> pinList, PatternModel result)
        {
            var dir = Path.GetDirectoryName(patternFile);
            var uniqueTempFolder = Path.Combine(dir, Path.GetFileNameWithoutExtension(patternFile));
            var targetFile = Path.Combine(dir, $"{Path.GetFileNameWithoutExtension(patternFile)}.bin");
            var inputFiles = new List<string>();

            foreach (var pin in pinList)
            {
                var currentPinFile = Path.Combine(uniqueTempFolder, $"{pin}.bin");
                if (File.Exists(currentPinFile))
                {
                    inputFiles.Add(currentPinFile);
                }
            }

            using (var outputStream = new FileStream(targetFile,
    FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
            {
                outputStream.WriteByte((byte)result.ModuleType);
                await MergeFilesAsync(outputStream, inputFiles);
            }
        }

        public static async Task MergeFilesAsync(FileStream outputStream, List<string> inputFiles, IProgress<int> progress = null)
        {
            var totalFiles = inputFiles.Count;
            var processedFiles = 0;

            foreach (var inputFile in inputFiles)
            {
                using (var inputStream = new FileStream(inputFile,
                    FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, true))
                {
                    await inputStream.CopyToAsync(outputStream);

                    processedFiles++;
                    progress?.Report((processedFiles * 100) / totalFiles);
                }
            }
        }

        public async Task ExportPattern(PatternModel patternModel, string exportFilePath)
        {
            if (patternModel == null)
                return;

            switch (patternModel.ModuleType)
            {
                case ModuleType.VM_Vector:
                case ModuleType.LVM_Vector:
                    await ExportLvmPattern(patternModel, exportFilePath);
                    break;
                case ModuleType.SRM_Vector:
                    ExportSrmPattern(patternModel, exportFilePath);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Private
        private async Task AnalysisPatternTimeSetAsync(string pathPattern, PatternModel patternResult)
        {
            string input;
            bool isMatch = false;
            try
            {
                using (StreamReader streamReader = new StreamReader(pathPattern))
                {
                    while ((input = (await streamReader.ReadLineAsync()).Trim()) != null)
                    {
                        if (_timeSetRegex.IsMatch(input))
                        {
                            isMatch = true;
                            break;
                        }
                    }
                }

                if (isMatch)
                {
                    string[] array = _timeSetRegex.Match(input).Groups[1].Value.Split(new string[3] { ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (patternResult.TimingSets.Contains(array[i].Trim()))
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr003"]}{array[i].Trim()}."));
                        }
                        patternResult.TimingSets.Add(array[i].Trim());
                    }
                }
                else
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(L["PatternCompileErr001"]);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<string> AnalysisPatternDigitalInstrumentAsync(string pathPattern)
        {
            var strDigitalInstrument = string.Empty;
            string input;
            bool isMatch = false;

            try
            {
                using (var streamReader = new StreamReader(pathPattern))
                {
                    while ((input = (await streamReader.ReadLineAsync()).Trim()) != null)
                    {
                        if (_atpPinsRegex.IsMatch(input))
                        {
                            isMatch = true;
                            break;
                        }
                    }
                }

                if (isMatch)
                {
                    strDigitalInstrument = _digitalInstrumentRegex.Match(input).Groups[1].Value;
                }
                return strDigitalInstrument;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void UpdateParametersByModuleType(ModuleType moduleType)
        {
            switch (moduleType)
            {
                case ModuleType.VM_Vector:
                    _pseudoInstruDic = VM_PSEUDO_INSTRU;
                    _pinValueNameExchangeDic = VM_PIN_VALUENAME_EXCHANGE;
                    break;
                case ModuleType.LVM_Vector:
                    _pseudoInstruDic = LVM_PSEUDO_INSTRU;
                    _pinValueNameExchangeDic = LVM_PIN_VALUENAME_EXCHANGE;
                    _listTotalMaskCC = LVM_MASKCC;
                    break;
                case ModuleType.SRM_Vector:
                    _pseudoInstruDic = SRM_PSEUDO_INSTRU;
                    _pinValueNameExchangeDic = SRM_PIN_VALUENAME_EXCHANGE;
                    _listTotalMaskCC = SRM_MASKCC;
                    break;
            }
        }

        private async Task<List<string>> AnalysisPatternPinsAsync(string pathPattern, PatternModel patternResult)
        {
            var patternPins = new List<string>();
            StreamReader streamReader = new StreamReader(pathPattern);
            string empty = string.Empty;
            string text = string.Empty;
            string empty2 = string.Empty;
            do
            {
                if (!streamReader.EndOfStream)
                {
                    empty = text;
                    do
                    {
                        text = streamReader.ReadLine().Trim().RemoveNode();
                    }
                    while (text.Length == 0);
                    empty2 = empty + " " + text;
                    //if (this.eventPinAnalysis != null)
                    //{
                    //    empty2 = this.eventPinAnalysis(empty2, Path.GetFileNameWithoutExtension(path_Pattern));
                    //}
                    continue;
                }
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(L["PatternCompileErr002"]);
                return patternPins;
            }
            while (!_atpPinsRegex.IsMatch(empty2));
            Match match = _atpPinsRegex.Match(empty2);
            var memoryName = match.Groups[1].Value.Trim();
            var vectorName = match.Groups[2].Value.Trim();
            string[] array = match.Groups[3].Value.Split(new string[3] { ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Replace("(", ":").Replace(")", "");
                //patternPinsWithDigitalMode.Add(array[i]);
                string text2 = array[i].Split(':')[0];
                if (patternPins.Contains(text2))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr03"]}{text2}."));
                }
                patternPins.Add(text2);
            }
            switch (memoryName.ToLower())
            {
                default:
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr04"]}{memoryName}."));
                    break;
                case "srm_vector":
                    patternResult.ModuleType = ModuleType.SRM_Vector;
                    break;
                case "lvm_vector":
                    patternResult.ModuleType = ModuleType.LVM_Vector;
                    break;
                case "vm_vector":
                    patternResult.ModuleType = ModuleType.VM_Vector;
                    break;
            }
            return patternPins;
        }

        private void AnalysisPatternInstrument(string pathPattern, List<string> atpPinsPinGroups, List<InstrumentModel> patternInstrument)
        {
            using StreamReader streamReader = new StreamReader(pathPattern);
            string text;
            do
            {
                if (!streamReader.EndOfStream)
                {
                    text = streamReader.ReadLine().RemoveNode().Trim();
                    if (!_instrumentsRegex.IsMatch(text))
                    {
                        continue;
                    }
                    string text2 = text;
                    while (true)
                    {
                        if (!text2.Contains("}"))
                        {
                            if (_atpPinsRegex.IsMatch(text2))
                            {
                                break;
                            }
                            text2 += streamReader.ReadLine().RemoveNode().Trim();
                            continue;
                        }
                        int num = text2.IndexOf("{");
                        int num2 = text2.IndexOf("}");
                        if (num >= 0 && num2 >= 0 && num <= num2)
                        {
                            string[] array = text2.Substring(num + 1, num2 - num - 1).Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < array.Length; i++)
                            {
                                InstrumentModel instrumentInstance = GetInstrumentInstance(array[i].Trim(), atpPinsPinGroups);
                                if (instrumentInstance != null)
                                {
                                    FillRegularExpressionForPinPinGroup(instrumentInstance, atpPinsPinGroups);
                                    patternInstrument.Add(instrumentInstance);
                                }
                            }
                        }
                        else
                        {
                            _compileError = true;
                            var msg = L["PatternCompileErr005"];
                            _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{msg}", text2));
                        }
                        return;
                    }
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(L["PatternCompileErr006"]);
                    break;
                }
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(L["PatternCompileInfo001"]);
                break;
            }
            while (!_atpPinsRegex.IsMatch(text));
        }

        private InstrumentModel GetInstrumentInstance(string strInstrument, List<string> atpPinsPinGroups)
        {
            InstrumentModel result = null;
            string[] array = strInstrument.Split(new string[2] { ":", " " }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 6)
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr007"]}", strInstrument));
                return result;
            }
            result = new InstrumentModel();
            result.StrInstrument = strInstrument;
            string[] array2 = array[0].Replace("(", "").Replace(")", "").Split(new string[3] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array2.Length; i++)
            {
                if (!atpPinsPinGroups.IsEmpty() != null && (atpPinsPinGroups.Any(x => x.Equals(array2[i])) == true))
                {
                    result.DicPinItem.Add(array2[i], new List<string> { array2[i] });
                    continue;
                }
                //if (testPlan.Keys.Contains(array2[i]))
                //{
                //    result.DicPinItem.Add(array2[i], testPlan[array2[i]]);
                //    continue;
                //}
                _compileError = true;
                //if (this.eventPrintCompileInfo != null)
                //{
                //    this.eventPrintCompileInfo($"error PCE1015: Can not find Instrument pin '{array2[i]}' in TestPlan.");
                //}
            }
            result.DigitalMode = array[1];
            if (!int.TryParse(array[2], out var result2))
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr008"]}", array[2]));
            }
            else
            {
                if (result2 < 1 || result2 > 32)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr009"]}", result2));
                }
                if (array[3].ToLower() == "parallel" && result2 != 1)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr010"]}", result2));
                }
                result.InstrumentWidth = result2;
            }
            result.InstrumentMode = array[3];
            result.BitOrder = array[4];
            result.Format = array[5];
            return result;
        }

        private void FillRegularExpressionForPinPinGroup(InstrumentModel instrument, List<string> atpPinsPinGroups)
        {
            string text = string.Empty;
            if (instrument.DigitalMode.ToLower() == "digsrc")
            {
                text = "D";
            }
            else if (instrument.DigitalMode.ToLower() == "digcap")
            {
                text = "V";
            }
            List<string> list = new List<string>();
            for (int i = 0; i < atpPinsPinGroups.Count; i++)
            {
                if (instrument.DicPinItem.Keys.Contains(atpPinsPinGroups[i]))
                {
                    int count = instrument.DicPinItem[atpPinsPinGroups[i]].Count;
                    list.Add(text + "{" + count + "}");
                }
                else
                {
                    list.Add("[01LHMXDCV]+");
                }
            }
            string pattern = string.Join("\\s+", list) + "\\s*";
            instrument.RegularExpression = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        private void GetInstrumentsInfoFromDataBlock(PatternModel patternModel, string pathPattern, List<string> atpPinsPinGroups, List<InstrumentModel> instruments, out List<string> dataBlockMarkerParameter)
        {
            int num = 1;
            bool flag = true;
            List<string> list = new List<string>();
            dataBlockMarkerParameter = new List<string>();
            GroupInstrumentBySrcAndCap(instruments, out var listSrcInstrument, out var listCapInstrument);
            List<int> normalPinIndex = GetNormalPinIndex(atpPinsPinGroups, listSrcInstrument, listCapInstrument);
            using StreamReader streamReader = new StreamReader(pathPattern);
            while (!streamReader.EndOfStream && !_atpPinsRegex.IsMatch(streamReader.ReadLine().Trim()))
            {
                num++;
            }
            string text = string.Empty;
            string text2 = string.Empty;
            while (!streamReader.EndOfStream)
            {
                streamReader.ReadLine().SplitValidAndComment(out var valid, out var comment);
                text = text + " " + valid;
                text2 = text2 + " " + comment;
                num++;
                if (!_vectorRegex.IsMatch(text))
                {
                    continue;
                }
                list.Add(text);
                if (_commandLoopRegex.IsMatch(text))
                {
                    Match match = _commandLoopRegex.Match(text);
                    string text3 = "Loop" + match.Groups[1].Value;
                    int.Parse(match.Groups[2].Value);
                    _tempNestLoopOutermostLoopName = ((_tempNestLoopOutermostLoopName.Length == 0) ? text3 : _tempNestLoopOutermostLoopName);
                    flag = false;
                }
                else if (_commandEndLoopRegex.IsMatch(text))
                {
                    string value = _commandEndLoopRegex.Match(text).Groups[1].Value;
                    if (_tempNestLoopOutermostLoopName == "Loop" + value)
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    ProgramOneRowVector(patternModel, num, list, listSrcInstrument, listCapInstrument, atpPinsPinGroups, dataBlockMarkerParameter, normalPinIndex);
                    list.Clear();
                    _tempNestLoopOutermostLoopName = string.Empty;
                }
                text = string.Empty;
                text2 = string.Empty;
                if (_compileError)
                {
                    break;
                }
            }
        }

        private void GroupInstrumentBySrcAndCap(List<InstrumentModel> list_PatternInstrument, out List<InstrumentModel> listSrcInstrument, out List<InstrumentModel> listCapInstrument)
        {
            listSrcInstrument = new List<InstrumentModel>();
            listCapInstrument = new List<InstrumentModel>();
            listSrcInstrument = list_PatternInstrument.Where((InstrumentModel x) => x.DigitalMode.ToLower() == "digsrc").ToList();
            listCapInstrument = list_PatternInstrument.Where((InstrumentModel x) => x.DigitalMode.ToLower() == "digcap").ToList();
        }

        private List<int> GetNormalPinIndex(List<string> list_ATPPinsPinGroups, List<InstrumentModel> list_SrcInstrument, List<InstrumentModel> list_CapInstrument)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < list_SrcInstrument.Count; i++)
            {
                List<int> collection = list_SrcInstrument[i].DicPinItem.Keys.Select((string x) => list_ATPPinsPinGroups.IndexOf(x)).ToList();
                list.AddRange(collection);
            }
            for (int j = 0; j < list_CapInstrument.Count; j++)
            {
                List<int> collection2 = list_CapInstrument[j].DicPinItem.Keys.Select((string x) => list_ATPPinsPinGroups.IndexOf(x)).ToList();
                list.AddRange(collection2);
            }
            List<int> list2 = new List<int>();
            for (int k = 0; k < list_ATPPinsPinGroups.Count; k++)
            {
                if (!list.Contains(k))
                {
                    list2.Add(k);
                }
            }
            return list2;
        }

        private void ProgramOneRowVector(PatternModel patternModel, int rowNumber, List<string> listRowVector, List<InstrumentModel> listSrcInstrument, List<InstrumentModel> listCapInstrument, List<string> atpPinsPinGroups, List<string> listDataBlockMarkerParameter, List<int> normalPinIndex)
        {
            _nestLoopBuffDic = new Dictionary<string, int>();
            _nestLoopIndex = -1;
            int num = 0;
            while (true)
            {
                if (num >= listRowVector.Count)
                {
                    return;
                }
                int num2 = 1;
                Match match = _vectorRegex.Match(listRowVector[num]);
                string value = match.Groups[1].Value;
                string strPseudo = string.Empty;
                string strPseudoParameter = string.Empty;
                _ = match.Groups[5].Value;
                string value2 = match.Groups[6].Value;
                string value3 = match.Groups[7].Value;
                List<string> list = (from y in match.Groups[4].Value.Trim().Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                     select y.Trim()).ToList();
                List<string> list2 = (from x in list
                                      where _listTotalMaskCC.Contains(x.Trim().ToLower())
                                      select x into y
                                      select y.Trim()).ToList();
                for (int i = 0; i < list2.Count; i++)
                {
                    if (list.Contains(list2[i]))
                    {
                        list.Remove(list2[i]);
                    }
                }
                if (value.Where((char x) => x == ':').Count() <= 1)
                {
                    if (list.Count <= 1)
                    {
                        string text = ((list.Count == 1) ? list[0] : string.Empty);
                        switch (patternModel.ModuleType)
                        {
                            case ModuleType.SRM_Vector:
                                if (!GetPseudoWithParameterSRM(text, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr030"]}", rowNumber, text));
                                }
                                break;
                            case ModuleType.VM_Vector:
                            case ModuleType.LVM_Vector:
                                if (!GetPseudoWithParameter(text, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr030"]}", rowNumber, text));
                                }
                                break;
                        }
                        if (value3.Replace(" ", "").Replace("\t", "").Length != 0)
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr031"]}", rowNumber - (listRowVector.Count - 1 - num)));
                        }
                        string[] array = value2.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length != atpPinsPinGroups.Count)
                        {
                            break;
                        }
                        if (array.Contains("D") || array.Contains("V"))
                        {
                            for (int j = 0; j < normalPinIndex.Count; j++)
                            {
                                if (array[normalPinIndex[j]].Contains("D") || array[normalPinIndex[j]].Contains("V"))
                                {
                                    _compileError = true;
                                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr032"]}", rowNumber - (listRowVector.Count - 1 - num)));
                                }
                            }
                        }
                        if (strPseudo.ToLower() == "start")
                        {
                            _recordingSwitchStart = true;
                            _dataBlockIndexStart++;
                            if (!listDataBlockMarkerParameter.Contains(strPseudoParameter))
                            {
                                listDataBlockMarkerParameter.Add(strPseudoParameter);
                            }
                            _ignoreCurrentVectorRowStart = true;
                            if (value2.ToUpper().Contains("D"))
                            {
                                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr033"]}", rowNumber - (listRowVector.Count - 1 - num)));
                            }
                        }
                        else if (strPseudo.ToLower() == "trig")
                        {
                            _recordingSwitchTrig = true;
                            _dataBlockIndexTrig++;
                            _ignoreCurrentVectorRowTrig = true;
                            if (value2.ToUpper().Contains("V"))
                            {
                                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr034"]}", rowNumber - (listRowVector.Count - 1 - num)));
                            }
                        }
                        else if (strPseudo.ToLower() == "repeat")
                        {
                            if (!int.TryParse(strPseudoParameter, out var result))
                            {
                                _compileError = true;
                                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr035"]}", rowNumber - (listRowVector.Count - 1 - num), strPseudoParameter));
                            }
                            num2 = result;
                        }
                        else if (strPseudo.ToLower() == "loopa" || strPseudo.ToLower() == "loopb" || strPseudo.ToLower() == "loopc")
                        {
                            if (!int.TryParse(strPseudoParameter, out var result2))
                            {
                                _compileError = true;
                                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr036"]}", rowNumber - (listRowVector.Count - 1 - num), strPseudoParameter));
                            }
                            _nestLoopBuffDic.Add(strPseudo.ToLower(), result2);
                            _nestLoopIndex++;
                        }
                        if (_recordingSwitchStart && !_ignoreCurrentVectorRowStart)
                        {
                            foreach (InstrumentModel item in listSrcInstrument)
                            {
                                if (item.RegularExpression.IsMatch(value2))
                                {
                                    int num3 = 1;
                                    for (int num4 = _nestLoopIndex; num4 >= 0; num4--)
                                    {
                                        num3 *= _nestLoopBuffDic.ElementAt(num4).Value;
                                    }
                                    if (item.StartTrigBlockIndexAndCount.Keys.Contains(_dataBlockIndexStart))
                                    {
                                        item.StartTrigBlockIndexAndCount[_dataBlockIndexStart] += num3 * num2;
                                    }
                                    else
                                    {
                                        item.StartTrigBlockIndexAndCount.Add(_dataBlockIndexStart, num3 * num2);
                                    }
                                    continue;
                                }
                                List<int> pinsIndex = GetPinsIndex(item.DicPinItem, atpPinsPinGroups);
                                for (int k = 0; k < pinsIndex.Count; k++)
                                {
                                    if (array[pinsIndex[k]].Contains("D"))
                                    {
                                        _compileError = true;
                                        _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr037"]}", rowNumber - (listRowVector.Count - 1 - num), "D"));
                                    }
                                }
                            }
                        }
                        _ignoreCurrentVectorRowStart = false;
                        if (_recordingSwitchTrig && !_ignoreCurrentVectorRowTrig)
                        {
                            foreach (InstrumentModel item2 in listCapInstrument)
                            {
                                if (item2.RegularExpression.IsMatch(value2))
                                {
                                    int num5 = 1;
                                    for (int num6 = _nestLoopIndex; num6 >= 0; num6--)
                                    {
                                        num5 *= _nestLoopBuffDic.ElementAt(num6).Value;
                                    }
                                    if (item2.StartTrigBlockIndexAndCount.Keys.Contains(_dataBlockIndexTrig))
                                    {
                                        item2.StartTrigBlockIndexAndCount[_dataBlockIndexTrig] += num5 * num2;
                                    }
                                    else
                                    {
                                        item2.StartTrigBlockIndexAndCount.Add(_dataBlockIndexTrig, num5 * num2);
                                    }
                                    continue;
                                }
                                List<int> pinsIndex2 = GetPinsIndex(item2.DicPinItem, atpPinsPinGroups);
                                for (int l = 0; l < pinsIndex2.Count; l++)
                                {
                                    if (array[pinsIndex2[l]].Contains("V"))
                                    {
                                        _compileError = true;
                                        _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr037"]}", rowNumber - (listRowVector.Count - 1 - num), "V"));

                                    }
                                }
                            }
                        }
                        _ignoreCurrentVectorRowTrig = false;
                        if (strPseudo.ToLower() == "end_loopa" || strPseudo.ToLower() == "end_loopb" || strPseudo.ToLower() == "end_loopc")
                        {
                            _nestLoopIndex--;
                        }
                        num++;
                        continue;
                    }
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr027"]}", rowNumber));
                    return;
                }
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr028"]}", rowNumber));
                return;
            }
            _compileError = true;
            _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr014"]}", rowNumber - (listRowVector.Count - 1 - num)));
        }

        private List<int> GetPinsIndex(Dictionary<string, List<string>> dicPinItem, List<string> atpPinsPinGroups)
        {
            return dicPinItem.Keys.Select((string x) => atpPinsPinGroups.IndexOf(x)).ToList();
        }

        private bool GetPseudoWithParameterSRM(string uCode, out string strPseudo, out string strPseudoParameter)
        {
            strPseudo = string.Empty;
            strPseudoParameter = string.Empty;
            if (_srmuCodeAnalysisRegex.IsMatch(uCode))
            {
                Match match = _srmuCodeAnalysisRegex.Match(uCode);
                strPseudo = match.Groups[3].Value.Trim();
                strPseudoParameter = match.Groups[5].Value.Trim();
                return true;
            }
            return false;
        }

        private void AnalysisPatternVector(string pathPattern, List<string> atpPinsPinGroups, PatternModel patternResult)
        {
            using (StreamReader streamReader = new StreamReader(pathPattern))
            {
                _ = streamReader.BaseStream.Length / 100L;
                long num = 1L;
                long num2 = 0L;
                while (!streamReader.EndOfStream && !_atpPinsRegex.IsMatch(streamReader.ReadLine().Trim()))
                {
                    num++;
                }
                string text = string.Empty;
                string text2 = string.Empty;
                List<byte> list = new List<byte>();
                while (!streamReader.EndOfStream)
                {
                    PatternVectorModel vectorModel = new PatternVectorModel();
                    LabelModel labelModel = new LabelModel();
                    CommandModel commandModel = new CommandModel();
                    var pins = new List<PinModel>();

                    streamReader.ReadLine().SplitValidAndComment(out var valid, out var comment);
                    text = text + " " + valid;
                    text2 = text2 + " " + comment;
                    num++;
                    if (/*this.eventPrintCompilePercent != null && */num % 25000L == 0L)
                    {
                        //this.eventPrintCompilePercent((double)streamReader.BaseStream.Position * 100.0 / (double)streamReader.BaseStream.Length);
                        list.Clear();
                    }
                    if (!_vectorRegex.IsMatch(text))
                    {
                        continue;
                    }
                    List<byte> list2 = new List<byte>();
                    Match match = _vectorRegex.Match(text);
                    string value = match.Groups[1].Value;
                    string strPseudo = string.Empty;
                    string strPseudoParameter = string.Empty;
                    string value2 = match.Groups[5].Value;
                    string value3 = match.Groups[6].Value;
                    string comment2 = text2 + match.Groups[7].Value;
                    string strMTE = string.Empty;
                    string text3 = match.Groups[4].Value.Trim();
                    if (_mteRegex.IsMatch(text3))
                    {
                        Match match2 = _mteRegex.Match(text3);
                        strMTE = match2.Groups[1].Value;
                        text3 = text3.Replace(match2.Value, string.Empty);
                    }
                    List<string> list3 = (from y in text3.Trim().Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                          select y.Trim()).ToList();
                    List<string> list4 = (from x in list3
                                          where _listTotalMaskCC.Contains(x.Trim().ToLower())
                                          select x into y
                                          select y.Trim()).ToList();
                    for (int i = 0; i < list4.Count; i++)
                    {
                        if (list3.Contains(list4[i]))
                        {
                            list3.Remove(list4[i]);
                        }
                    }
                    for (int j = 0; j < list4.Count; j++)
                    {
                        list4[j] = list4[j].ToLower();
                    }
                    if (value.Where((char x) => x == ':').Count() <= 1)
                    {
                        if (list3.Count <= 1)
                        {
                            string uCode = ((list3.Count == 1) ? list3[0] : string.Empty);
                            switch (patternResult.ModuleType)
                            {
                                case ModuleType.VM_Vector:
                                    if (!GetPseudoWithParameter(uCode, out strPseudo, out strPseudoParameter))
                                    {
                                        _compileError = true;
                                        _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr011"]}", num));
                                    }
                                    labelModel = VectorAnalysisLabel(value, num, num2);
                                    pins = VectorAnalysisPins(value3, num, atpPinsPinGroups);
                                    commandModel = VectorAnalysisPseudoInstru(strPseudo, strPseudoParameter, num);
                                    vectorModel.TimingSet = value2;
                                    //if (_saveComment)
                                    //{
                                    //    VectorAnalysis_Comment(comment2, num, num2);
                                    //}
                                    break;
                                case ModuleType.LVM_Vector:
                                    if (!GetPseudoWithParameter(uCode, out strPseudo, out strPseudoParameter))
                                    {
                                        _compileError = true;
                                        _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr011"]}", num));
                                    }
                                    labelModel = VectorAnalysisLabel(value, num, num2);
                                    //VectorAnalysisLvmMaskCCWithOpcode(list4, strPseudo, strPseudoParameter, num, num2);
                                    vectorModel.TimingSet = value2;
                                    commandModel = VectorAnalysisLvmPseudoParameters(strPseudo, strPseudoParameter, num);
                                    pins = VectorAnalysisPins(value3, num, atpPinsPinGroups);
                                    //if (_saveComment)
                                    //{
                                    //    VectorAnalysis_Comment(comment2, num, num2);
                                    //}
                                    break;
                                case ModuleType.SRM_Vector:
                                    //VectorAnalysisMte(strMTE, num, num2, list5, out var containsMTE);
                                    VectorAnalysisSrmPseudoWithParametersModifiler(uCode, num, num2, out strPseudo, out strPseudoParameter);
                                    labelModel = VectorAnalysisLabel(value, num, num2);
                                    //VectorAnalysisSrmMaskCCWithOpcode(list4, listBitValueModifier, listBitValueOpcode, num, list5);
                                    vectorModel.TimingSet = value2;
                                    pins = VectorAnalysisPins(value3, num, atpPinsPinGroups);
                                    //if (_saveComment)
                                    //{
                                    //    VectorAnalysis_Comment(comment2, num, num2);
                                    //}
                                    break;
                            }
                            vectorModel.Label = labelModel;
                            vectorModel.Command = commandModel;
                            vectorModel.Pins = pins;
                            patternResult.PatternVectors.Add(vectorModel);
                            text = string.Empty;
                            text2 = string.Empty;
                            _validVectorLinesCountInPatternFile++;
                            list.AddRange(list2);
                            if (strPseudo.ToLower() == "halt" && _haltInVectorLinesPosition == -1)
                            {
                                _haltInVectorLinesPosition = _validVectorLinesCountInPatternFile;
                            }
                            num2++;
                            if (list.Count > 0)
                            {
                                list.Clear();
                            }
                            if (_compileError)
                            {
                                return;
                            }
                            continue;
                        }
                        _compileError = true;
                        _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr027"]}", num));
                        return;
                    }
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr028"]}", num));
                    return;
                }
            }
            if (_validVectorLinesCountInPatternFile < 32)
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr029"]}", _validVectorLinesCountInPatternFile, 32));
            }
        }

        private bool GetPseudoWithParameter(string uCode, out string strPseudo, out string strPseudoParameter)
        {
            List<string> list = uCode.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            bool flag = false;
            switch (list.Count)
            {
                default:
                    strPseudo = string.Empty;
                    strPseudoParameter = string.Empty;
                    return false;
                case 0:
                    strPseudo = string.Empty;
                    strPseudoParameter = string.Empty;
                    return true;
                case 1:
                    strPseudo = list[0];
                    strPseudoParameter = string.Empty;
                    return true;
                case 2:
                    strPseudo = list[0];
                    strPseudoParameter = list[1];
                    return true;
            }
        }

        private LabelModel VectorAnalysisLabel(string strLabel, long currentLine, long currentVectorLine)
        {
            var result = new LabelModel();
            string[] array = strLabel.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length > 2)
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr012"]}", currentLine, strLabel));
            }
            else if (array.Length == 1)
            {
                result.IndexInVectors = currentVectorLine;
                result.LabelFullContent = strLabel;
                result.LabelType = LabelCommandType.Define;
            }
            else
            {
                if (array.Length != 2)
                {
                    return result;
                }
                LabelCommandType labelCommandType = LabelCommandType.Define;
                switch (array[0].ToLower())
                {
                    case "start_label":
                        labelCommandType = LabelCommandType.Start;
                        break;
                    case "global":
                        labelCommandType = LabelCommandType.Global;
                        break;
                    case "global_subr":
                        labelCommandType = LabelCommandType.GlobalSubr;
                        break;
                    case "local":
                        labelCommandType = LabelCommandType.Define;
                        break;
                    case "subr":
                        labelCommandType = LabelCommandType.Subr;
                        break;
                    case "keepalive_subr":
                        labelCommandType = LabelCommandType.KeepaliveSubr;
                        break;
                    default:
                        _compileError = true;
                        _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr012"]}", currentLine, array[0]));
                        return result;
                    case "stop_subr":
                        labelCommandType = LabelCommandType.StopSubr;
                        break;
                }

                result.IndexInVectors = currentVectorLine;
                result.LabelFullContent = strLabel;
                result.LabelType = LabelCommandType.Define;
            }
            return result;
        }

        private List<PinModel> VectorAnalysisPins(string timesetAndPinsValue, long currentLine, List<string> atpPinsPinGroups)
        {
            var pinList = new List<PinModel>();
            string[] array = timesetAndPinsValue.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != atpPinsPinGroups.Count)
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr014"]}", currentLine));
                return pinList;
            }

            for (int i = 0; i < array.Length; i++)
            {
                AddPinValueToList(array[i], currentLine, pinList, atpPinsPinGroups[i]);
            }

            return pinList;
        }

        private void AddPinValueToList(string strPinValue, long currentLine, List<PinModel> pinsValue, string pinName)
        {
            if (_pinValueNameExchangeDic.Keys.Contains(strPinValue))
            {
                pinsValue.Add(new PinModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    PinName = pinName,
                    VectorValue = KSW.Helpers.Enum.GetEnumValueFromDescription<VectorValueType>(strPinValue),
                });
                return;
            }

            char[] array = strPinValue.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (_pinValueNameExchangeDic.Keys.Contains(array[i].ToString()))
                {
                    pinsValue.Add(new PinModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        PinName = pinName,
                        VectorValue = KSW.Helpers.Enum.GetEnumValueFromDescription<VectorValueType>(strPinValue),
                    });
                    continue;
                }
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr015"]}", array[i].ToString()));
            }
        }

        private CommandModel VectorAnalysisPseudoInstru(string pseudoInstru, string pseudoInstruParameter, long currentLine)
        {
            var result = new CommandModel();
            result.CommandFullContent = pseudoInstru;

            if (pseudoInstru.Length == 0)
                return result;

            if (!_pseudoInstruDic.Keys.Contains(pseudoInstru.ToLower()))
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr016"]}", currentLine, pseudoInstru));
                return result;
            }
            switch (pseudoInstru.ToLower())
            {
                case "fstop":
                case "fstart":
                case "endloop":
                case "cpua":
                case "reburst":
                case "trig":
                case "cpuaend":
                case "halt":
                    if (pseudoInstruParameter.Length != 0)
                    {
                        _compileError = true;
                        _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr017"]}", currentLine, pseudoInstru));
                    }
                    else
                    {
                        result.Type = KSW.Helpers.Enum.GetEnumValueFromDescription<CommandType>(pseudoInstru.ToLower());
                    }
                    break;
                default:
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr018"]}", currentLine, pseudoInstru));
                    break;
                case "start":
                case "repeat":
                case "loop":
                    {
                        if (pseudoInstruParameter.Length == 0)
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr019"]}", currentLine, pseudoInstru));
                            break;
                        }
                        int parameter = 0;
                        result.Type = KSW.Helpers.Enum.GetEnumValueFromDescription<CommandType>(pseudoInstru.ToLower());
                        if (int.TryParse(pseudoInstruParameter, out parameter))
                        {
                            if (parameter >= 2 && parameter <= 1048575)
                            {
                                result.CommandParameter = parameter;
                                break;
                            }
                            _compileError = true;
                            _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr020"]}", currentLine, pseudoInstru, 2, 1048575));
                        }
                        else
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr021"]}", currentLine, pseudoInstru));
                        }
                        break;
                    }
            }

            return result;
        }

        private CommandModel VectorAnalysisLvmPseudoParameters(string strPseudo, string strPseudoParameter, long currentLine)
        {
            var result = new CommandModel();
            result.CommandFullContent = strPseudo;
            if (!strPseudo.IsEmpty())
            {
                result.Type = KSW.Helpers.Enum.GetEnumValueFromDescription<CommandType>(strPseudo.ToLower());
            }
            if (strPseudo.ToLower() == "cflag" || strPseudo.ToLower() == "return")
            {
                if (strPseudoParameter.Length == 0)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr022"]}", currentLine, strPseudo));
                    return result;
                }
                if (int.TryParse(strPseudoParameter, out var parameter) && (parameter < 0 || parameter > 4095))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr023"]}", currentLine, strPseudoParameter, strPseudo, 0, 4095));
                    return result;
                }
            }
            if (strPseudo.ToLower() == "repeat")
            {
                if (strPseudoParameter.Length == 0)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr022"]}", currentLine, strPseudo));
                    return result;
                }
                if (int.TryParse(strPseudoParameter, out var parameter) && (parameter < 2 || parameter > int.MaxValue))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr023"]}", currentLine, strPseudoParameter, strPseudo, 2, int.MaxValue));
                    return result;
                }
            }

            if (strPseudoParameter.Length == 0)
            {
                return result;
            }

            int result3 = 0;
            int num = -1;
            if (int.TryParse(strPseudoParameter, out result3))
            {
                num = ((!(strPseudo.ToLower() == "call")) ? 2 : 0);
                if (result3 >= num && result3 <= int.MaxValue)
                {
                    result.CommandParameter = result3;
                    return result;
                }
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr024"]}", currentLine, strPseudoParameter, num, int.MaxValue));
            }
            else
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr021"]}", currentLine, strPseudoParameter, num, int.MaxValue, string.Join(",", _dataBlockMarkerParameter)));
            }

            return result;
        }

        private CommandModel VectorAnalysisSrmPseudoWithParametersModifiler(string uCode, long currentLine, long currentVectorLine, out string strPseudo, out string strPseudoParameter)
        {
            var result = new CommandModel();
            strPseudo = string.Empty;
            strPseudoParameter = string.Empty;

            string empty = string.Empty;
            if (_srmuCodeAnalysisRegex.IsMatch(uCode))
            {
                Match match = _srmuCodeAnalysisRegex.Match(uCode);
                empty = match.Groups[2].Value.Trim();
                strPseudo = match.Groups[3].Value.Trim();
                strPseudoParameter = match.Groups[5].Value.Trim();
                result.CommandFullContent = strPseudo;
                result.CommandParameter = strPseudoParameter;
                FactoryGetModifierOpcodeOperand(empty, strPseudo, strPseudoParameter, currentLine, currentVectorLine);
            }
            else
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr017"]}", currentLine, uCode));
            }

            return result;
        }

        private bool FactoryGetModifierOpcodeOperand(string strConditional, string strPseudo, string strPseudoParameter, long currentLine, long currentVectorLine)
        {
            int value = 0;
            //if (_srmModifilerDic.ContainsKey(strPseudo.ToLower()))
            //{
            //    value = ((!(strPseudo.ToLower() == "enable") || strPseudoParameter.Contains("and")) ? _srmModifilerDic[strPseudo.ToLower()] : 0);
            //}
            //else if (_srmModifilerDic.ContainsKey(strConditional))
            //{
            //    if (!SRM_Modifiler_ConditionFlag.ContainsKey(strConditional.ToLower()))
            //    {
            //        _compileError = true;
            //        //this.eventPrintCompileInfo?.Invoke($"line {currentLine} - error PCE1033: Undefined condition flag '{strConditional}' in atp file.");
            //        return false;
            //    }
            //    value = _srmModifilerDic[strConditional];
            //}

            //if (!_pseudoInstruDic.ContainsKey(strPseudo.ToLower()))
            //{
            //    _compileError = true;
            //    //this.eventPrintCompileInfo?.Invoke($"line {currentLine} - error PCE1006: Undefined PseudoInstruction '{strPseudo}' in atp file.");
            //    return false;
            //}

            if (strPseudo.ToLower() == "repeat" || strPseudo.ToLower() == "loopa" || strPseudo.ToLower() == "loopb" || strPseudo.ToLower() == "loopc")
            {
                if (strPseudoParameter.Length == 0)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr019"]}", currentLine, strPseudo));
                    return false;
                }
                if (int.TryParse(strPseudoParameter, out var result) && (result < 2 || result > 65535))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr023"]}", currentLine, strPseudoParameter, strPseudo, 2, 65535));
                    return false;
                }
            }
            int num = 0;
            if (int.TryParse(strPseudoParameter, out var result2))
            {
                num = result2;
            }
            //else if (_dicPretreatmentLabelNameVectorLineNumber.ContainsKey(strPseudoParameter))
            //{
            //    _dicInUseVectorLineNumberLabelName.Add(currentVectorLine, new LabelModel
            //    {
            //        IndexInVectors = currentVectorLine,
            //        LabelFullContent = strPseudoParameter + ":",
            //        LabelName = strPseudoParameter + ":"
            //    });
            //    num = _dicPretreatmentLabelNameVectorLineNumber[strPseudoParameter];
            //    if (num < 0 || num > 4095)
            //    {
            //        _compileError = true;
            //        //this.eventPrintCompileInfo?.Invoke($"line {currentLine} - error PCE1034: The definition line number '{num}' of the lable '{strPseudoParameter}' must be in the range of {0} to {4095}.");
            //        return false;
            //    }
            //}
            else
            {
                if (!GetConditionFlagValue(strPseudo, strPseudoParameter.Split(new string[4] { " ", "\t", "and", "or" }, StringSplitOptions.RemoveEmptyEntries).ToList(), out var value2, out var _))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr026"]}", currentLine, strPseudoParameter));
                    return false;
                }
                num = value2;
            }

            return true;
        }

        private bool GetConditionFlagValue(string strPseudo, List<string> listConditionFlags, out int value, out List<string> listUnsupportedConditionFlag)
        {
            value = 0;
            listUnsupportedConditionFlag = new List<string>();
            if (listConditionFlags.Count == 0)
            {
                string text = strPseudo.ToLower();
                if (!(text == "fstart"))
                {
                    if (text == "fstop")
                    {
                        value = 2;
                    }
                }
                else
                {
                    value = 1;
                }
            }
            else
            {
                Dictionary<string, int> dic_TempConditioCollection = new Dictionary<string, int>();
                if (strPseudo.ToLower() == "set_cpu")
                {
                    dic_TempConditioCollection = SRM_Specific_SetCPU_Condition_Value;
                }
                else if (strPseudo.ToLower() == "clr_flag")
                {
                    dic_TempConditioCollection = SRM_Specific_ClrFlag_Condition_Value;
                }
                else
                {
                    dic_TempConditioCollection = SRM_Condition_Value;
                }
                listUnsupportedConditionFlag = listConditionFlags.Where((string x) => !dic_TempConditioCollection.ContainsKey(x.ToLower())).ToList();
                if (listUnsupportedConditionFlag.Count != 0)
                {
                    return false;
                }
                List<int> list = listConditionFlags.Select((string x) => dic_TempConditioCollection[x.ToLower()]).ToList();
                if (list.Count > 1 && list.Contains(0))
                {
                    return false;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    value |= list[i];
                }
            }
            return true;
        }

        private async Task ExportLvmPattern(PatternModel patternModel, string exportFilePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(exportFilePath);
            var pattern = patternModel.PatternVectors.FirstOrDefault();
            using (var fs = new FileStream(exportFilePath, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    //time set行
                    var timeSet = $"import tset {string.Join(",", patternModel.TimingSets)};";
                    sw.WriteLine(timeSet);

                    if (!patternModel.InstrumentName.IsEmpty())
                    {
                        //digital
                        var digital = $"digital_ins={patternModel.InstrumentName};";
                        sw.WriteLine(digital);
                    }

                    if (!patternModel.InstrumentInfo.StrInstrument.IsEmpty())
                    {
                        //instrument
                        sw.WriteLine("instruments = {");
                        sw.WriteLine($"{patternModel.InstrumentInfo.StrInstrument};");
                        sw.WriteLine("}");
                    }

                    //moduleType
                    sw.WriteLine(patternModel.ModuleType.Description());

                    //pattern header
                    var patternHeader = GetPatternHeader(pattern);
                    sw.WriteLine($"{fileName}\t($tset,\t {patternHeader})");

                    //pattern vector
                    sw.WriteLine("{");
                    var colHeaderLength = $"{fileName}\t\t\t".Length;
                    foreach (var vector in patternModel.PatternVectors)
                    {
                        var timingSetAndVector = GetTimingSetAndVector(vector, colHeaderLength);
                        sw.WriteLine(timingSetAndVector);
                    }
                    sw.WriteLine("}");

                    sw.Flush();
                }
            }

        }

        private string GetPatternHeader(PatternVectorModel pattern)
        {
            var patternHeader = string.Empty;
            if (pattern == null || pattern.Pins.IsEmpty())
                return patternHeader;

            int index = 0;
            foreach (var pin in pattern.Pins)
            {
                patternHeader += pin.PinName;

                if (pattern.Pins.IndexOf(pin) != pattern.Pins.Count - 1)
                    patternHeader += ",";

                if (index == 7)
                {
                    patternHeader += "\t";
                    index = 0;
                }
                else
                    index++;
            }

            return patternHeader;
        }

        private string GetTimingSetAndVector(PatternVectorModel vector, int colHeaderLength)
        {
            var result = string.Empty;
            if (!vector.Label.LabelFullContent.IsEmpty())
                result += vector.Label.LabelFullContent;

            if (!vector.Command.CommandFullContent.IsEmpty())
            {
                if (result.IsEmpty())
                    result += vector.Command.CommandFullContent;
                else
                    result += $",{vector.Command.CommandFullContent}";
            }

            // 固定字符串宽度
            result = result.PadRight(colHeaderLength);

            // TimingSet填写
            result += $"> {vector.TimingSet}\t";

            int index = 0;
            foreach (var pin in vector.Pins)
            {
                result += $"{pin.VectorValue.Description()} ";
                if (vector.Pins.IndexOf(pin) == vector.Pins.Count - 1)
                    result += ";";

                if (index == 7)
                {
                    result += "\t";
                    index = 0;
                }
                else
                    index++;
            }

            return result;
        }

        private void ExportSrmPattern(PatternModel patternModel, string exportFilePath)
        {
            throw new NotImplementedException();
        }

        public struct ReadLineStruct
        {
            public long LineNumber;
            public long ValidVectorNumber;
            public string LineStr;
        }

        private async Task AnalysisAndCompilePatternVector(string patternFile, List<string> pinList, PatternModel patternResult)
        {
            const int BUFFER_SIZE = 4 * 1024 * 1024;

            var lines = new BlockingCollection<ReadLineStruct>(boundedCapacity: 10000); // 缓冲1万行
            var vectorDic = new Dictionary<string, Channel<PinPatternModel>>();
            var moduleType = patternResult.ModuleType;
            var isEnd = false;

            var dir = Path.GetDirectoryName(patternFile);
            var uniqueTempFolder = Path.Combine(dir, Path.GetFileNameWithoutExtension(patternFile));
            if (!Directory.Exists(uniqueTempFolder))
                Directory.CreateDirectory(uniqueTempFolder);

            Task producer = Task.Run(() =>
            {
                using var fileStream = new FileStream(patternFile, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, FileOptions.SequentialScan);
                using var reader = new StreamReader(fileStream, Encoding.UTF8, bufferSize: BUFFER_SIZE);

                string? line;
                var lineNumber = 1L;
                var validVectorCount = 0L;

                while ((line = reader.ReadLine()) != null && !_atpPinsRegex.IsMatch(line.Trim()))
                {
                    lineNumber++;
                }

                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(new ReadLineStruct()
                    {
                        LineNumber = lineNumber,
                        ValidVectorNumber = validVectorCount,
                        LineStr = line
                    });
                    validVectorCount++;
                    lineNumber++;

                    if (validVectorCount % 10000 == 0)
                    {
                        _eventAggregator.GetEvent<RecordMessageEvent>().Publish(new Models.Instruments.RecordMessageModel()
                        {
                            RecordMessage = $"已读取{validVectorCount}行向量",
                            RecordTime = DateTime.Now
                        });
                        Log.LogDebug($"已读取{validVectorCount}行向量");
                    }
                }
                lines.CompleteAdding();
            });

            int maxConcurrency = Math.Min(pinList.Count, Environment.ProcessorCount);
            var process = ProcessAllVectorsAsync(vectorDic, pinList, uniqueTempFolder, maxConcurrency);
            //var process = 
            //    Task.Run(async () =>
            //{
            //    var storeTasks = new List<Task>();
            //    while (vectorDic.Count != pinList.Count)
            //        await Task.Delay(10);

            //    foreach (var pinVector in vectorDic)
            //    {
            //        var pinName = pinVector.Key;
            //        var channel = pinVector.Value;

            //        storeTasks.Add(
            //        Task.Run(async () =>
            //        {
            //            var isFirst = true;
            //            var lastInstruction = CommandType.nop;
            //            object lastCommandParameter = null;
            //            var startIndex = 0;
            //            var endIndex = 0;
            //            var totalLength = 0L;
            //            var lengthStartPos = 0;
            //            var isPack = false;
            //            var isFinish = true;
            //            var andTempVector = true;
            //            var maxVectorCount = 124;
            //            var loopVectors = new List<PinPatternModel>();
            //            var tempVectors = new List<PinPatternModel>();

            //            using (var fs = new FileStream(Path.Combine(uniqueTempFolder, pinName + ".bin"), FileMode.Create))
            //            {
            //                using (var writer = new BinaryWriter(fs))
            //                {
            //                    while (true)
            //                    {
            //                        if (isEnd && channel.Reader.Count == 0)
            //                            break;

            //                        if (channel.TryDequeue(out var pinPattern))
            //                        {
            //                            if (isFirst)
            //                            {
            //                                isFirst = false;
            //                                lastInstruction = pinPattern.Instruction;
            //                                lastCommandParameter = pinPattern.CommandParameter;
            //                                var nameBytes = Encoding.UTF8.GetBytes(pinName);
            //                                writer.Write(nameBytes.Length);
            //                                lengthStartPos += 4;
            //                                writer.Write(nameBytes);
            //                                lengthStartPos += nameBytes.Length;
            //                                if (!string.IsNullOrEmpty(pinPattern.TimingSet))
            //                                {
            //                                    var timingSetBytes = Encoding.UTF8.GetBytes(pinPattern.TimingSet);
            //                                    if (timingSetBytes.IsEmpty())
            //                                    {
            //                                        writer.Write(0);
            //                                        lengthStartPos += 1;
            //                                    }
            //                                    else
            //                                    {
            //                                        writer.Write(timingSetBytes.Length);
            //                                        lengthStartPos += 4;
            //                                        writer.Write(timingSetBytes);
            //                                        lengthStartPos += timingSetBytes.Length;
            //                                    }
            //                                }
            //                                writer.Write(new byte[8]); //向量长度占位
            //                            }

            //                            if (pinPattern.Instruction != lastInstruction || endIndex - startIndex >= maxVectorCount)
            //                            {
            //                                var count = endIndex - startIndex;

            //                                if (pinPattern.Instruction == CommandType.halt)
            //                                {
            //                                    isPack = true;
            //                                    if (count < maxVectorCount)
            //                                    {
            //                                        count += 1;
            //                                        andTempVector = false;
            //                                        tempVectors.Add(pinPattern);
            //                                    }
            //                                    else
            //                                    {
            //                                        andTempVector = true;
            //                                        isFinish = false;
            //                                    }
            //                                }
            //                                else if (pinPattern.Instruction == CommandType.loop)
            //                                {
            //                                    loopVectors.Clear();
            //                                    lastInstruction = pinPattern.Instruction;
            //                                    lastCommandParameter = pinPattern.CommandParameter;
            //                                    loopVectors.Add(pinPattern);
            //                                    andTempVector = false;
            //                                    isPack = true;
            //                                }
            //                                else if (pinPattern.Instruction == CommandType.endloop)
            //                                {
            //                                    loopVectors.Add(pinPattern);
            //                                    if (loopVectors.Any())
            //                                    {
            //                                        var groups = PackLoopPattern(lastCommandParameter, loopVectors);
            //                                        if (groups.Any())
            //                                        {
            //                                            totalLength += groups.Count * 64;
            //                                            foreach (var group in groups)
            //                                            {
            //                                                writer.Write(group.VectorNumber);
            //                                                writer.Write(group.Instruction);
            //                                                writer.Write(group.Data);
            //                                            }
            //                                        }
            //                                    }
            //                                    lastInstruction = CommandType.nop;
            //                                    lastCommandParameter = null;
            //                                    startIndex = ++endIndex;
            //                                    isPack = false;
            //                                    isFinish = true;
            //                                }
            //                                else
            //                                {
            //                                    switch (lastInstruction)
            //                                    {
            //                                        case CommandType.loop:
            //                                            loopVectors.Add(pinPattern);
            //                                            isPack = false;
            //                                            break;
            //                                        default:
            //                                            andTempVector = true;
            //                                            isPack = true;
            //                                            break;
            //                                    }
            //                                }

            //                                if (isPack)
            //                                {
            //                                    var patterns = tempVectors;
            //                                    if (patterns.Any())
            //                                    {
            //                                        var group = PackPattern(lastInstruction, lastCommandParameter, patterns);
            //                                        startIndex = endIndex;
            //                                        endIndex++;
            //                                        writer.Write(group.VectorNumber);
            //                                        writer.Write(group.Instruction);
            //                                        writer.Write(group.Data);
            //                                        totalLength += 64;
            //                                    }
            //                                    isFinish = true;
            //                                    tempVectors.Clear();
            //                                    if (andTempVector)
            //                                        tempVectors.Add(pinPattern);
            //                                }
            //                            }
            //                            else
            //                            {
            //                                endIndex++;
            //                                if (lastInstruction == CommandType.loop)
            //                                    loopVectors.Add(pinPattern);
            //                                else
            //                                    tempVectors.Add(pinPattern);
            //                                isFinish = false;
            //                            }
            //                        }
            //                        else
            //                            await Task.Delay(1);

            //                        if (startIndex % 10000 == 0)
            //                            writer.Flush();

            //                    }

            //                    if (!isFinish)
            //                    {
            //                        var patterns = tempVectors;
            //                        var group = PackPattern(lastInstruction, lastCommandParameter, patterns);
            //                        writer.Write(group.VectorNumber);
            //                        writer.Write(group.Instruction);
            //                        writer.Write(group.Data);
            //                        totalLength += 64;
            //                    }

            //                    //补充数据长度
            //                    var totalLengthBytes = BitConverter.GetBytes(totalLength);
            //                    writer.BaseStream.Position = lengthStartPos;
            //                    writer.BaseStream.Write(totalLengthBytes, 0, totalLengthBytes.Length);
            //                    writer.Flush();
            //                }
            //            }

            //        })
            //            );
            //    }

            //    await Task.WhenAll(storeTasks);

            //});

            foreach (var line in lines.GetConsumingEnumerable())
            {
                await ProcessLine(moduleType, pinList, vectorDic, line);
            }

            foreach (var vector in vectorDic)
            {
                var channel = vector.Value;
                channel?.Writer.TryComplete();
            }
            await process;
            //Parallel.ForEach(lines.GetConsumingEnumerable(),
            //    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            //    line =>
            //    {
            //        var vector = ProcessLine(moduleType, pinList, line);
            //        if (vector != null)
            //            vectorList.Add((PatternVectorModel)vector); // 你的处理逻辑
            //    });
        }

        private async Task ProcessAllVectorsAsync(IDictionary<string, Channel<PinPatternModel>> vectorChannels, ICollection<string> pinList, string uniqueTempFolder, int maxConcurrency, CancellationToken cancellationToken = default)
        {
            // 1. 等待所有引脚的数据通道都已准备好（即 vectorChannels 包含所有 pinList 中的引脚）
            while (vectorChannels.Count != pinList.Count)
            {
                // 使用非忙等待：若缺失引脚，可以短暂等待或通过信号通知。这里简化用 Delay。
                await Task.Delay(10, cancellationToken);
            }

            // 2. 限制并发写入数量
            using var semaphore = new SemaphoreSlim(maxConcurrency);
            var tasks = vectorChannels.Select(async kv =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    await ProcessSinglePinAsync(kv.Key, kv.Value.Reader, uniqueTempFolder, cancellationToken);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 处理单个引脚的向量通道，将数据打包后写入 .bin 文件。
        /// </summary>
        private async Task ProcessSinglePinAsync(string pinName, ChannelReader<PinPatternModel> reader, string uniqueTempFolder, CancellationToken cancellationToken)
        {
            // 复用列表，避免重复分配
            var tempVectors = new List<PinPatternModel>();

            string filePath = Path.Combine(uniqueTempFolder, $"{pinName}.bin");
            await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                                                4096, useAsync: true);
            using var writer = new BinaryWriter(fs, Encoding.UTF8, leaveOpen: false);

            // ---- 写入固定头部信息（引脚名称、TimingSet等） ----
            // 名称长度及名称
            byte[] nameBytes = Encoding.UTF8.GetBytes(pinName);
            writer.Write(nameBytes.Length);
            writer.Write(nameBytes);
            long headerEndPos = 4 + nameBytes.Length;

            // 注意：原始代码中只有非首个元素且 TimingSet 非空时才写入；实际首元素逻辑为 isFirst 时写入 TimingSet。
            // 但为了简化且合理，这里假设 TimingSet 来自第一个数据包（原始代码中在 isFirst 内判断 pinPattern.TimingSet）。
            // 然而在流式处理中第一个数据包还未读取，因此这里先预留 TimingSet 写入位置，稍后回填。
            // 更好的方式：先读取第一个数据包再写头部。这里为保持逻辑相近，暂不改变原始行为，
            // 原始代码在 isFirst 块内读取了 pinPattern.TimingSet 并写入，因此我们必须先消费一个数据包。
            // 但由于 ChannelReader 是异步流，我们需要先读取第一个 item。修改如下：

            // 读取第一个数据包（必须存在，否则整个文件无意义）
            if (!await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false) || !reader.TryRead(out var firstItem))
            {
                // 没有数据，不创建文件
                return;
            }

            // 写入 TimingSet（若存在）
            if (!string.IsNullOrEmpty(firstItem.TimingSet))
            {
                byte[] timingBytes = Encoding.UTF8.GetBytes(firstItem.TimingSet);
                writer.Write(timingBytes.Length);
                writer.Write(timingBytes);
                headerEndPos += 4 + timingBytes.Length;
            }
            else
            {
                writer.Write(0); // 长度0
                headerEndPos += 4;
            }

            // 预留 8 字节给总长度
            long totalLengthPos = headerEndPos;
            writer.Write(0L); // 占位符
            headerEndPos += 8;

            // ---- 状态机变量 ----
            long totalLength = 0;
            CommandType lastInstruction = firstItem.Instruction;
            object lastCommandParameter = firstItem.CommandParameter;
            int startIndex = 0, endIndex = 1;      // 第一个 item 已计入

            // 将第一个 item 放入临时列表
            tempVectors.Add(firstItem);

            // 如果第一个就是 halt 或 loop，原始逻辑复杂，我们按通用流处理
            // 开始处理剩余的数据包
            await foreach (var pinPattern in reader.ReadAllAsync(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // 每次循环重新计算最大向量个数（根据当前指令类型）
                int maxVectorCount = (lastInstruction == CommandType.nop) ? 124 : 112;
                int count = endIndex - startIndex;

                // 特殊指令处理：halt / loop / endloop
                if (pinPattern.Instruction == CommandType.halt)
                {
                    // halt 需要和前面的向量一起打包（如果未满则包含当前 halt）
                    if (count < maxVectorCount)
                    {
                        count++;
                        tempVectors.Add(pinPattern);    // 把 halt 加入当前批次
                        WritePack(writer, lastInstruction, lastCommandParameter, tempVectors,
                                        ref totalLength, ref startIndex, ref endIndex);
                        tempVectors.Clear();
                        // 重置状态，halt 不延续
                        lastInstruction = CommandType.nop;
                        lastCommandParameter = null;
                        continue;
                    }
                    else
                    {
                        // 当前批次已满，先打包现有数据，halt 作为下一批第一个
                        WritePack(writer, lastInstruction, lastCommandParameter, tempVectors,
                                        ref totalLength, ref startIndex, ref endIndex);
                        tempVectors.Clear();
                        tempVectors.Add(pinPattern);
                        lastInstruction = pinPattern.Instruction;
                        lastCommandParameter = pinPattern.CommandParameter;
                        continue;
                    }
                }
                else if (pinPattern.Instruction == CommandType.loop)
                {
                    // 先打包之前积累的数据
                    if (tempVectors.Count > 0)
                    {
                        WritePack(writer, lastInstruction, lastCommandParameter, tempVectors,
                                        ref totalLength, ref startIndex, ref endIndex);
                        tempVectors.Clear();
                    }
                    // 开始新的 loop 块
                    tempVectors.Add(pinPattern);
                    lastInstruction = pinPattern.Instruction;
                    lastCommandParameter = pinPattern.CommandParameter;
                    continue;
                }
                else if (pinPattern.Instruction == CommandType.endloop)
                {
                    tempVectors.Add(pinPattern);
                    if (tempVectors.Count > 1)  // 至少包含 loop 和 endloop
                    {
                        var groups = PackLoopPattern(lastCommandParameter, tempVectors);
                        foreach (var group in groups)
                        {
                            writer.Write(group.VectorNumber);
                            writer.Write(group.Instruction);
                            writer.Write(group.Data);
                            totalLength += 64;
                        }
                    }
                    // 重置状态
                    lastInstruction = CommandType.nop;
                    lastCommandParameter = null;
                    startIndex = endIndex;
                    endIndex = startIndex + 1;
                    tempVectors.Clear();
                    continue;
                }

                // 无需打包，继续累积
                if (tempVectors.Count >= maxVectorCount)
                {
                    WritePack(writer, lastInstruction, lastCommandParameter, tempVectors,
                                  ref totalLength, ref startIndex, ref endIndex);
                    tempVectors.Clear();
                }
                endIndex++;
                tempVectors.Add(pinPattern);
            }

            // 循环结束后，若还有未打包的剩余数据
            if (tempVectors.Count > 0)
            {
                WritePack(writer, lastInstruction, lastCommandParameter, tempVectors,
                                ref totalLength, ref startIndex, ref endIndex);
            }

            // 最后回填文件总长度
            writer.BaseStream.Position = totalLengthPos;
            writer.Write(totalLength);
            writer.Flush();
        }

        /// <summary>
        /// 执行一次打包并写入文件。
        /// </summary>
        private void WritePack(BinaryWriter writer,
                                            CommandType instruction,
                                            object commandParameter,
                                            List<PinPatternModel> patterns,
                                            ref long totalLength,
                                            ref int startIndex,
                                            ref int endIndex)
        {
            if (patterns.Count == 0) return;
            var group = PackPattern(instruction, commandParameter, patterns);
            writer.Write(group.VectorNumber);
            writer.Write(group.Instruction);
            writer.Write(group.Data);
            totalLength += 64;
            // 更新索引（使 startIndex 指向当前批次末尾）
            startIndex = endIndex;
            endIndex = startIndex + 1;
            // 注意：endIndex 在调用方会维护，此处不修改
        }

        private async Task ProcessLine(ModuleType moduleType, List<string> pinList, Dictionary<string, Channel<PinPatternModel>> dicQueue, ReadLineStruct line)
        {
            try
            {
                // 使用有意义的变量名
                var lineNumber = line.LineNumber;
                var validVectorCount = line.ValidVectorNumber;
                string currentLine = line.LineStr;

                // 优化2: 使用StringBuilder替代字符串拼接，避免创建大量临时字符串
                var textBuilder = new StringBuilder(256);  // 预分配合理容量

                // 优化3: 使用HashSet实现O(1)查找，替代原来的List.Contains() O(n)查找
                var maskHashSet = new HashSet<string>(_listTotalMaskCC.Select(x => x.Trim().ToLower()));

                // 优化5: 复用集合对象，减少GC压力
                var dataList = new List<string>(32);      // 预分配典型容量
                var maskList = new List<string>(16);

                string label, timingSet, pinsPart;
                bool flowControl = VaildLine(lineNumber, currentLine, maskHashSet, dataList, maskList, out label, out timingSet, out pinsPart);
                if (!flowControl)
                    return;

                string uCode = dataList.Count == 1 ? dataList[0] : string.Empty;
                string strPseudo = string.Empty;
                string strPseudoParameter = string.Empty;
                CommandModel commandModel = new CommandModel();
                // 优化7: 简化switch逻辑
                bool hasPseudo = GetPseudoWithParameter(uCode, out strPseudo, out strPseudoParameter);

                if (moduleType == ModuleType.VM_Vector || moduleType == ModuleType.LVM_Vector)
                {
                    if (!hasPseudo)
                    {
                        _compileError = true;
                        _eventAggregator.GetEvent<MessageUpdateEvent>()
                            .Publish(string.Format(L["PatternCompileErr011"], lineNumber));
                        return;
                    }

                    var labelModel = VectorAnalysisLabel(label, lineNumber, validVectorCount);

                    if (moduleType == ModuleType.VM_Vector)
                    {
                        commandModel = VectorAnalysisPseudoInstru(strPseudo, strPseudoParameter, lineNumber);
                    }
                    else
                    {
                        commandModel = VectorAnalysisLvmPseudoParameters(strPseudo, strPseudoParameter, lineNumber);
                    }
                }
                else if (moduleType == ModuleType.SRM_Vector)
                {
                    VectorAnalysisSrmPseudoWithParametersModifiler(uCode, lineNumber, validVectorCount,
                        out strPseudo, out strPseudoParameter);
                    var labelModel = VectorAnalysisLabel(label, lineNumber, validVectorCount);
                }

                await VectorAnalysisPins(timingSet, commandModel, pinsPart, lineNumber, pinList, dicQueue);

                _validVectorLinesCountInPatternFile++;

                // 优化8: 使用OrdinalIgnoreCase避免ToLower()
                if (strPseudo.Equals("halt", StringComparison.OrdinalIgnoreCase) &&
                    _haltInVectorLinesPosition == -1)
                {
                    _haltInVectorLinesPosition = _validVectorLinesCountInPatternFile;
                }

                if (_compileError) return;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool VaildLine(long lineNumber, string currentLine, HashSet<string> maskHashSet, List<string> dataList, List<string> maskList, out string label, out string timingSet, out string pinsPart)
        {
            label = string.Empty;
            timingSet = string.Empty;
            pinsPart = string.Empty;
            // 优化2: 直接使用ReadOnlySpan避免字符串分配
            ReadOnlySpan<char> lineSpan = currentLine.AsSpan();
            int commentIndex = lineSpan.IndexOf('#');

            ReadOnlySpan<char> validPart = commentIndex >= 0
                ? lineSpan.Slice(0, commentIndex).Trim()
                : lineSpan.Trim();

            if (validPart.IsEmpty) return false;

            // 快速检查是否向量行
            if (!_vectorRegex.IsMatch(validPart.ToString()))
                return false;

            // 优化3: 使用ReadOnlySpan解析，避免多次ToString()
            var match = _vectorRegex.Match(validPart.ToString());
            if (!match.Success) return false;

            label = match.Groups[1].Value;
            timingSet = match.Groups[5].Value;
            pinsPart = match.Groups[6].Value;
            string vectorData = match.Groups[4].Value;

            // 解析MTE - 使用Span优化
            string strMTE = string.Empty;
            ReadOnlySpan<char> vectorSpan = vectorData.AsSpan();
            var mteMatch = _mteRegex.Match(vectorData);
            if (mteMatch.Success)
            {
                strMTE = mteMatch.Groups[1].Value;
                vectorSpan = vectorData.AsSpan().Slice(mteMatch.Length).ToString().AsSpan();
            }

            // 优化4: 使用Span.Split替代string.Split
            dataList.Clear();
            maskList.Clear();

            if (!vectorSpan.IsEmpty)
            {
                // 手动解析CSV，避免Split分配
                int start = 0;
                for (int i = 0; i <= vectorSpan.Length; i++)
                {
                    if (i == vectorSpan.Length || vectorSpan[i] == ',')
                    {
                        if (i > start)
                        {
                            var part = vectorSpan.Slice(start, i - start).Trim();
                            if (!part.IsEmpty)
                            {
                                dataList.Add(part.ToString());
                            }
                        }
                        start = i + 1;
                    }
                }
            }

            // 优化5: 从后往前遍历，避免多次Remove
            for (int i = dataList.Count - 1; i >= 0; i--)
            {
                string item = dataList[i];
                if (maskHashSet.Contains(item))
                {
                    maskList.Add(item);
                    dataList.RemoveAt(i);
                }
            }

            // 优化6: 手动计数冒号，避免LINQ
            int colonCount = 0;
            foreach (char c in label)
            {
                if (c == ':')
                {
                    colonCount++;
                    if (colonCount > 1) break;
                }
            }

            if (colonCount > 1)
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>()
                    .Publish(string.Format(L["PatternCompileErr024"], lineNumber));
                return false;
            }

            if (dataList.Count > 1)
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>()
                    .Publish(string.Format(L["PatternCompileErr020"], lineNumber));
                return false;
            }

            return true;
        }

        private async Task VectorAnalysisPins(string timingSet, CommandModel commandModel, string timesetAndPinsValue, long currentLine, List<string> pinList, Dictionary<string, Channel<PinPatternModel>> dicQueue)
        {
            string[] array = timesetAndPinsValue.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != pinList.Count)
            {
                _compileError = true;
                _eventAggregator.GetEvent<MessageUpdateEvent>().Publish(string.Format($"{L["PatternCompileErr014"]}", currentLine));
                return;
            }

            for (int i = 0; i < array.Length; i++)
            {
                var pinName = pinList[i];
                var strPinValue = array[i];
                if (!dicQueue.ContainsKey(pinName))
                {
                    dicQueue[pinName] = Channel.CreateUnbounded<PinPatternModel>(); //Channel.CreateBounded<PinPatternModel>(new BoundedChannelOptions(10000)
                    //{
                    //    FullMode = BoundedChannelFullMode.Wait
                    //});
                }

                var channel = dicQueue[pinName];
                await channel.Writer.WriteAsync(new PinPatternModel
                {
                    PinName = pinName,
                    TimingSet = timingSet,
                    Instruction = commandModel.Type,
                    CommandParameter = commandModel.CommandParameter,
                    VectorValue = KSW.Helpers.Enum.GetEnumValueFromDescription<VectorValueType>(strPinValue),
                });

                //while (channel.Count > 10000)
                //{
                //    Thread.Sleep(10);
                //}

                //channel.Enqueue(new PinPatternModel
                //{
                //    PinName = pinName,
                //    TimingSet = timingSet,
                //    Instruction = commandModel.Type,
                //    CommandParameter = commandModel.CommandParameter,
                //    VectorValue = KSW.Helpers.Enum.GetEnumValueFromDescription<VectorValueType>(strPinValue),
                //});
            }

            return;
        }

        private static List<PatternVectorGroupModel> PackLoopPattern(object patternParameter, List<PinPatternModel> patterns)
        {
            var result = new List<PatternVectorGroupModel>();
            int loopCount = 0;
            if (patternParameter is int param)
                loopCount = param;
            else
                return result;

            int startIndex = 0;
            var x = (long)patterns.Count * (long)loopCount;
            var patternsVector = patterns.Select(x => x.VectorValue).ToList();
            try
            {

                if (patterns.Count >= 224)
                {
                    var n = patterns.Count / 112;   // 512bit(=64byte)，其中9bit时向量数，7bit时指令Id,剩下62Byte(当指令Id不为0时，还有6byte作为操作数，剩余56byte,已知一个1byte表达两个向量符号 = 56 * 2)
                    var reset = patterns.Count % 112;

                    if ((n >= 2 && reset > 0) || n > 3)
                    {
                        var loopPack = GeneratorLoopPack(patternsVector, loopCount, 112);
                        result.Add(loopPack);
                        startIndex += 112;

                        var normalPackCount = n - 2;
                        if (normalPackCount > 0)
                        {
                            for (int i = 0; i < normalPackCount; i++)
                            {
                                var normalPack = GeneratorNormalPack(patternsVector.SkipFast(startIndex), 112);
                                result.Add(normalPack);
                                startIndex += 112;
                            }
                        }
                        else if (normalPackCount == 0)
                        {
                            var normalPack = GeneratorNormalPack(patternsVector.SkipFast(startIndex), 112);
                            result.Add(normalPack);
                            startIndex += 112;
                        }

                        var endPack = GeneratorEndPack(patternsVector, startIndex, reset);
                        result.Add(endPack);
                    }
                    else
                    {
                        var loopPack = GeneratorLoopPack(patternsVector, loopCount, 112);
                        result.Add(loopPack);
                        startIndex += 112;
                        var endPack = GeneratorEndPack(patternsVector, startIndex, reset);
                        result.Add(endPack);
                    }
                }
                else if (x >= 372)
                {
                    var k = 224 / patterns.Count;
                    var n = loopCount / k;
                    var loopVectors = new List<VectorValueType>();
                    var totoalVectorCount = (long)patternsVector.Count * (long)loopCount;
                    for (int i = 0; i < k; i++)
                        loopVectors.AddRange(patternsVector);

                    var half = loopVectors.Count / 2;
                    var loopPack = GeneratorLoopPack(loopVectors, n, half);
                    result.Add(loopPack);
                    startIndex += half;
                    var endPack = GeneratorEndPack(loopVectors, startIndex, loopVectors.Count - half);
                    result.Add(endPack);

                    var restLoop = loopCount - n * k;
                    var normalNumber = totoalVectorCount - ((long)loopVectors.Count * (long)n);
                    var count = normalNumber / 124;
                    var rest = (int)(normalNumber % 124);
                    var restVectors = Enumerable.Repeat(patternsVector, restLoop).SelectMany(x => x);   //不实际存储的可枚举序列
                    startIndex = 0;
                    for (var i = 0; i < count; i++)
                    {
                        var normalPack = GeneratorNormalPack(restVectors, 124);
                        result.Add(normalPack);
                        startIndex += 124;
                    }

                    if (rest > 0)
                    {
                        var normalPack = GeneratorNormalPack(restVectors.Skip(startIndex), rest);
                        result.Add(normalPack);
                    }
                }
                else
                {
                    startIndex = 0;
                    var isFinish = true;
                    var totalVectors = Enumerable.Repeat(patternsVector, loopCount).SelectMany(x => x);
                    var totoalVectorCount = (long)patternsVector.Count * (long)loopCount;

                    var n = totoalVectorCount / 124;
                    var rest = (int)(totoalVectorCount % 124);

                    for (int i = 0; i <= n; i++)
                    {
                        if (i != n)
                        {
                            var normalGroup = GeneratorNormalPack(totalVectors.SkipFast(startIndex), 124);
                            result.Add(normalGroup);
                            startIndex += 124;
                        }
                        else
                        {
                            var vectors = totalVectors.SkipFast(i * 124).Take(rest);
                            if (vectors.Any())
                            {
                                var end = new PatternVectorGroupModel()
                                {
                                    VectorNumber = (byte)vectors.Count(),
                                };
                                startIndex = 0;
                                byte symbol = 0;
                                var index = 0;
                                foreach (var vector in vectors)
                                {
                                    if (index % 2 == 0)
                                    {
                                        symbol = (byte)vector;
                                        isFinish = false;
                                    }
                                    else
                                    {
                                        symbol |= (byte)((byte)vector << 4);
                                        end.Data[startIndex++] = symbol;
                                        isFinish = true;
                                    }
                                    index++;
                                }

                                if (!isFinish)
                                    end.Data[startIndex] = symbol;

                                result.Add(end);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }

            return result;
        }

        private static PatternVectorGroupModel GeneratorLoopPack(List<VectorValueType> patterns, int loopCount, int vectorNumber)
        {
            var intBytes = BitConverter.GetBytes(loopCount);
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)vectorNumber,
                Instruction = (byte)CommandType.loop,
            };

            Array.Copy(intBytes, 0, result.Data, 0, intBytes.Length);

            byte vector = 0;
            var index = 0;
            bool isFinish = true;
            for (int i = 0; i < vectorNumber; i++)
            {
                if (i % 2 == 0)
                {
                    vector = (byte)patterns[i];
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)patterns[i] << 4);
                    result.Data[6 + index++] = vector;
                    isFinish = true;
                }
            }

            if (!isFinish)
                result.Data[6 + index] = vector;

            return result;
        }

        private static PatternVectorGroupModel GeneratorNormalPack(IEnumerable<VectorValueType> patterns, int vectorNumber)
        {
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)vectorNumber,
            };

            byte vector = 0;
            var index = 0;
            var i = 0;
            bool isFinish = true;

            foreach (var item in patterns)
            {
                if (i % 2 == 0)
                {
                    vector = (byte)item;
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)item << 4);
                    result.Data[index++] = vector;
                    isFinish = true;
                }
                i++;
                if (i == vectorNumber)
                    break;
            }

            if (!isFinish)
                result.Data[index] = vector;

            return result;
        }

        private static PatternVectorGroupModel GeneratorEndPack(List<VectorValueType> patterns, int startIndex, int reset)
        {
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)reset,
                Instruction = (byte)CommandType.endloop,
            };

            byte vector = 0;
            var index = 0;
            bool isFinish = true;
            for (int i = 0; i < reset; i++)
            {
                if (i % 2 == 0)
                {
                    vector = (byte)patterns[startIndex + i];
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)patterns[startIndex + i] << 4);
                    result.Data[6 + index++] = vector;
                    isFinish = true;
                }
            }

            if (!isFinish)
                result.Data[6 + index] = vector;

            return result;
        }

        private static PatternVectorGroupModel PackPattern(CommandType instruction, object patternParamter, List<PinPatternModel> patterns)
        {
            var result = new PatternVectorGroupModel()
            {
                VectorNumber = (byte)patterns.Count,
                Instruction = (byte)instruction,
            };
            int startIndex = 0;
            if (instruction != CommandType.nop)
            {
                startIndex = 6;
                if (patternParamter is int param)
                {
                    var parameterArray = new byte[6];
                    var intParam = instruction == CommandType.loop ? param - 1 : param; //循环情况下
                    var parameterBytes = BitConverter.GetBytes(intParam).ToArray();    // 不反序
                    Array.Copy(parameterBytes, 0, result.Data, 0, parameterBytes.Length);
                }
            }

            byte vector = 0;
            var vectorList = new List<byte>();
            bool isFinish = true;
            var index = 0;
            foreach (var pinPatternModel in patterns)
            {
                if (index % 2 == 0)
                {
                    vector = (byte)pinPatternModel.VectorValue;
                    isFinish = false;
                }
                else
                {
                    vector |= (byte)((byte)pinPatternModel.VectorValue << 4);
                    vectorList.Add(vector);
                    isFinish = true;
                }
                index++;
            }

            if (!isFinish && patterns.Any())
                vectorList.Add(vector);

            if (vectorList.Count <= result.Data.Count())
                Array.Copy(vectorList.ToArray(), 0, result.Data, startIndex, vectorList.Count);

            return result;
        }
        #endregion
    }
}
