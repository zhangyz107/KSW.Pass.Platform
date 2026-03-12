using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Events;
using KSW.ATE01.Project.Base.Extensions;
using KSW.ATE01.Project.Base.Language;
using KSW.ATE01.Project.Base.Models.Patterns;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KSW.ATE01.Project.Base.Helpers
{
    public class PatternHelper
    {
        private static readonly Lazy<PatternHelper> _instance = new Lazy<PatternHelper>(() => new PatternHelper());
        private static IEventAggregator _eventAggregator;
        private static LanguageManager L => LanguageManager.Instance;

        public PatternHelper()
        {
            var container = ContainerLocator.Container;
            _eventAggregator = container?.Resolve<IEventAggregator>() ?? null;
        }
        #region Fields
        private PatternModel _pattern;

        private static bool _compileError;
        private static bool _recordingSwitchStart;
        private static bool _recordingSwitchTrig;
        private static bool _ignoreCurrentVectorRowTrig;
        private static bool _ignoreCurrentVectorRowStart;
        private static int _nestLoopIndex = -1;
        private static int _dataBlockIndexStart = -1;
        private static int _dataBlockIndexTrig = -1;
        private static int _haltInVectorLinesPosition = -1;
        private static int _validVectorLinesCountInPatternFile;
        private static ushort _vectorGroupSize = 512 / 8;
        private static int _nopMaxVectorCount = 124; //(512-16) / 4
        private static int _paramMaxVectorCount = 112; //(512 - 16 - 48) / 4    带参数的向量组
        private static ushort _patternUnitLength = 4 * 1024;    // Pattern单位长度
        private static int _maxVectorGroupByUnitSize = _patternUnitLength / _vectorGroupSize;  // 一个单位长的包可以容纳多少向量组（64）
        private static int _maxUnitCount = 15; // 65535 / 4096 取整
        private static long _mbByte = 128 * 1024 * 1024L;
        private static string _tempNestLoopOutermostLoopName = string.Empty;

        private static Regex _digitalInstrumentRegex = new Regex("^\\s*digital_ins\\s*=\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);
        private static Regex _opCodeModeRegex = new Regex("^\\s*opcode_mode\\s*=\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);
        private static Regex _timeSetRegex = new Regex("^\\s*import\\s*tset\\s*(.+?)\\s*;", RegexOptions.IgnoreCase);
        private static Regex _instrumentsRegex = new Regex("^\\s*instruments\\s*=\\s*", RegexOptions.IgnoreCase);
        private static Regex _atpPinsRegex = new Regex("\\s*([-a-zA-Z0-9_]*)\\s*([-a-zA-Z0-9_]*)\\s*\\(\\s*\\$tset\\s*,(.+)\\)", RegexOptions.IgnoreCase);
        private static Regex _vectorRegex = new Regex("\\s*((([^{:>]+):){0,})\\s*([^{:>]*)>\\s*([-a-zA-Z0-9_]+)\\s+(.+?)\\s*;(.*)", RegexOptions.IgnoreCase);
        private static Regex _mteRegex = new Regex("\\(\\s*mte\\s*=\\s*([\\s\\w!]+)\\s*\\)", RegexOptions.IgnoreCase);
        private static Regex _commandLoopRegex = new Regex("\\s*loop([abc])\\s+([0-9]+)\\s+(>.+;).?", RegexOptions.IgnoreCase);
        private static Regex _commandEndLoopRegex = new Regex("\\s*end_loop([abc])\\s+([0-9a-zA-Z_]+\\s*>.+;).?", RegexOptions.IgnoreCase);
        private static Regex _commandRepeatRegex = new Regex("\\s*repeat\\s+([0-9]+)\\s+(>.+;).?", RegexOptions.IgnoreCase);
        private static Regex _srmuCodeAnalysisRegex = new Regex("(if\\s*\\(\\s*([!\\w]+)\\s*\\)\\s*)*([!\\w]+)*(\\s+|\\()*([!\\w\\s]+)*", RegexOptions.IgnoreCase);

        private static List<string> _listTotalMaskCC = new List<string>();
        private static List<string> _dataBlockMarkerParameter = new List<string>();
        private static Dictionary<string, int> _pseudoInstruDic = new Dictionary<string, int>();
        private static Dictionary<string, int> _nestLoopBuffDic = new Dictionary<string, int>();
        private static Dictionary<string, byte> _pinValueNameExchangeDic = new Dictionary<string, byte>();

        private static List<string> LVM_MASKCC = new List<string> { "stv", "maskb", "maska", "rsrm" };
        private static List<string> SRM_MASKCC = new List<string>
        {
            "accfail", "ccnd", "padd", "sadd", "clr_fail", "ign", "icc", "ifc", "stv", "maskb",
            "maska", "rlvm"
        };
        private static Dictionary<string, int> VM_PSEUDO_INSTRU = new Dictionary<string, int>
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
        private static Dictionary<string, int> LVM_PSEUDO_INSTRU = new Dictionary<string, int>
        {
            { "nop", 0 },
            { "halt", 0 },
            { "repeat", 1 },
            { "call", 4 },
            { "trig", 8 }
        };
        private static Dictionary<string, int> SRM_PSEUDO_INSTRU = new Dictionary<string, int>
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

        private static Dictionary<string, int> SRM_Condition_Value = new Dictionary<string, int>
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
        private static Dictionary<string, int> SRM_Specific_ClrFlag_Condition_Value = new Dictionary<string, int>
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
        private static Dictionary<string, int> SRM_Specific_SetCPU_Condition_Value = new Dictionary<string, int>
        {
            { "cpua", 1 },
            { "cpub", 2 },
            { "cpuc", 4 },
            { "cpud", 8 }
        };

        private static Dictionary<string, byte> VM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
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
        private static Dictionary<string, byte> LVM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
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
        private static Dictionary<string, byte> SRM_PIN_VALUENAME_EXCHANGE = new Dictionary<string, byte>
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

        #region Public
        public static PatternModel AnalysisPattern(string patternFilePath)
        {
            _compileError = false;
            var result = new PatternModel();
            var patternFileName = Path.GetFileNameWithoutExtension(patternFilePath);
            result.FileName = patternFileName;
            try
            {
                AnalysisPatternTimeSet(patternFilePath, result);
                var instrumentInfo = AnalysisPatternDigitalInstrument(patternFilePath);
                result.InstrumentName = instrumentInfo;
                var pinList = AnalysisPatternPins(patternFilePath, result);
                UpdateParametersByModuleType(patternFilePath, result.ModuleType);
                var instruments = new List<InstrumentModel>();
                AnalysisPatternInstrument(patternFilePath, pinList, instruments);
                if (instruments.Any())
                {
                    result.InstrumentInfo = instruments.FirstOrDefault();
                    GetInstrumentsInfoFromDataBlock(result, patternFilePath, pinList, instruments, out _dataBlockMarkerParameter);
                }
                AnalysisPatternVector(patternFilePath, pinList, result);

                _instance.Value._pattern = result;
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public static List<PatternPackageModel> ConversionPatternModel(Dictionary<string, List<PinPatternModel>> pinPatternList, ref long dataStartAddress, out int patternDataLength)
        {
            var result = new List<PatternPackageModel>();
            patternDataLength = 0;
            if (pinPatternList == null || !pinPatternList.Any())
                return result;

            try
            {
                bool isFirst = true;
                foreach (var pinPattern in pinPatternList)
                {
                    var patterGroups = GetPatternGroups(pinPattern.Value);  //获取所有向量组

                    if (patterGroups.Any())
                    {
                        int currentGroup = 0;
                        var totalGroups = patterGroups.Count;
                        int currentPackageGroups = 0;   //当前包含向量组数
                        var isNewPackage = true;
                        PatternPackageModel currentPackage = null;
                        foreach (var group in patterGroups)
                        {
                            if (isNewPackage)
                            {
                                currentPackage = new PatternPackageModel();
                                currentPackage.PinName = pinPattern.Key;
                                currentPackage.Address = BitConverter.GetBytes(dataStartAddress).Reverse().Skip(3).ToArray();
                                currentPackage.PatternGroups = new List<PatternGroupModel>();
                                isNewPackage = false;
                                result.Add(currentPackage);
                            }

                            if (currentPackage != null)
                            {
                                currentPackage.Length += _vectorGroupSize;
                                currentPackage.PatternGroups.Add(group);
                                var currentSize = group.Vectors.Count + 2;
                                if (group.Parameter != null)
                                    currentSize += group.Parameter.Count;

                                if (currentSize < _vectorGroupSize) //(512 / 8)一个完整的向量组长度
                                {
                                    var less = _vectorGroupSize - currentSize;
                                    var emptyVectorArray = new byte[less];
                                    group.Vectors.AddRange(emptyVectorArray);
                                }
                                currentPackageGroups++;
                            }

                            if (currentPackageGroups == _maxUnitCount * _maxVectorGroupByUnitSize)
                            {
                                var lengthBytes = BitConverter.GetBytes(currentPackage.Length).Reverse().ToArray();
                                currentPackage.LengthBytes = lengthBytes;
                                dataStartAddress += currentPackage.Length;
                                if (isFirst)
                                    patternDataLength += currentPackage.Length;
                                isNewPackage = true;
                            }
                        }

                        if (!isNewPackage)
                        {
                            var currentLength = currentPackage.Length;
                            if (currentLength % _patternUnitLength != 0)
                            {
                                var m = (ushort)(currentLength / _patternUnitLength);
                                currentPackage.Length = (ushort)((m + 1) * _patternUnitLength);
                            }
                            var lengthBytes = BitConverter.GetBytes(currentPackage.Length).Reverse().ToArray();
                            dataStartAddress += currentPackage.Length;
                            if (isFirst)
                                patternDataLength += currentPackage.Length;
                            currentPackage.LengthBytes = lengthBytes;
                        }

                    }

                    isFirst = false;
                }


                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static List<PatternGroupModel> GetPatternGroups(List<PinPatternModel> pinPatterns)
        {
            var result = new List<PatternGroupModel>();
            if (pinPatterns == null || !pinPatterns.Any())
                return result;

            var currentGroup = new PatternGroupModel();
            currentGroup.VectorNumber = 0;
            bool isNewGroup = true;
            bool isLow = true;
            byte vector = 0;
            var lastInstruction = CommandType.nop;
            object commandParameter = null;
            foreach (var pinPattern in pinPatterns)
            {
                if (isNewGroup)
                {
                    currentGroup.Instruction = pinPattern.Instruction;
                    if (pinPattern.CommandParameter != null)
                    {
                        switch (currentGroup.Instruction)
                        {
                            case CommandType.loop:
                                if (pinPattern.CommandParameter is int loopCount)
                                {
                                    var loopParameterArray = new byte[6];
                                    var parameterBytes = BitConverter.GetBytes(loopCount).ToArray();    // 不反序
                                    Array.Copy(parameterBytes, 0, loopParameterArray, 0, parameterBytes.Length);
                                    currentGroup.Parameter.AddRange(loopParameterArray);
                                }
                                break;
                            case CommandType.endloop:
                                var endParameterArray = new byte[6];
                                currentGroup.Parameter.AddRange(endParameterArray);
                                break;
                        }
                    }
                    isNewGroup = false;
                }

                switch (pinPattern.Instruction)
                {
                    case CommandType.nop:
                        if (isLow)
                        {
                            vector = (byte)pinPattern.VectorValue;
                            currentGroup.VectorNumber++;
                            isLow = false;
                        }
                        else
                        {
                            vector |= (byte)((int)pinPattern.VectorValue << 4);
                            currentGroup.VectorNumber++;
                            currentGroup.Vectors.Add(vector);
                            isLow = true;
                        }

                        // 向量数已满
                        if (currentGroup.VectorNumber == _nopMaxVectorCount)
                        {
                            result.Add(currentGroup);
                            currentGroup = new PatternGroupModel();
                            currentGroup.VectorNumber = 0;
                            isNewGroup = true;
                        }
                        break;
                    case CommandType.loop:
                        if (lastInstruction != pinPattern.Instruction)
                        {
                            if (!isNewGroup)
                            {
                                if (!isLow)
                                {
                                    currentGroup.Vectors.Add(vector);
                                    isLow = true;
                                }
                                result.Add(currentGroup);
                            }

                            currentGroup = new PatternGroupModel();
                            currentGroup.VectorNumber = 0;
                            currentGroup.Instruction = pinPattern.Instruction;
                            if (pinPattern.CommandParameter != null && pinPattern.CommandParameter is int loopCount)
                            {
                                var parameterArray = new byte[6];
                                var parameterBytes = BitConverter.GetBytes(loopCount).ToArray();    // 不反序
                                Array.Copy(parameterBytes, 0, parameterArray, 0, parameterBytes.Length);
                                currentGroup.Parameter.AddRange(parameterArray);
                            }
                            vector = (byte)pinPattern.VectorValue;
                            currentGroup.VectorNumber++;
                            isLow = false;
                        }
                        else
                        {
                            if (isLow)
                            {
                                vector = (byte)pinPattern.VectorValue;
                                currentGroup.VectorNumber++;
                                isLow = false;
                            }
                            else
                            {
                                vector |= (byte)((int)pinPattern.VectorValue << 4);
                                currentGroup.VectorNumber++;
                                currentGroup.Vectors.Add(vector);
                                isLow = true;
                            }

                            // 向量数已满
                            if (currentGroup.VectorNumber == _paramMaxVectorCount)
                            {
                                result.Add(currentGroup);
                                currentGroup = new PatternGroupModel();
                                currentGroup.VectorNumber = 0;
                                isNewGroup = true;
                            }

                        }
                        break;
                    case CommandType.endloop:
                        if (lastInstruction != pinPattern.Instruction)
                        {
                            if (!isNewGroup)
                            {
                                if (!isLow)
                                {
                                    currentGroup.Vectors.Add(vector);
                                    isLow = true;
                                }
                                result.Add(currentGroup);
                            }

                            var tempEndLoopGroup = new PatternGroupModel();
                            tempEndLoopGroup.VectorNumber = 0;
                            tempEndLoopGroup.Instruction = pinPattern.Instruction;
                            var endParameterArray = new byte[6];
                            tempEndLoopGroup.Parameter.AddRange(endParameterArray);
                            var tempVector = (byte)pinPattern.VectorValue;
                            tempEndLoopGroup.VectorNumber++;
                            tempEndLoopGroup.Vectors.Add(tempVector);
                            result.Add(tempEndLoopGroup);

                            currentGroup = new PatternGroupModel();
                            currentGroup.VectorNumber = 0;
                            isNewGroup = true;
                        }
                        break;
                    default:
                        break;
                }

                lastInstruction = pinPattern.Instruction;
            }

            if (!isNewGroup)
            {
                if (!isLow)
                    currentGroup.Vectors.Add(vector);

                result.Add(currentGroup);
            }

            return result;
        }

        /// <summary>
        /// 获取Patter包数据
        /// </summary>
        /// <param name="patternModel"></param>
        /// <param name="dataStartAddress"></param>
        /// <param name="patternDataLength"></param>
        /// <returns></returns>
        public static List<PatternPackageModel> ConversionPatternModel(PatternModel patternModel, ref long dataStartAddress, out int patternDataLength)
        {
            var result = new List<PatternPackageModel>();
            patternDataLength = 0;
            if (patternModel == null)
                return result;

            if (!patternModel.PatternVectors.Any())
                return result;

            var lengthBytes = BitConverter.GetBytes(_patternUnitLength).Reverse().ToArray();
            try
            {
                //先将向量根据命令分组
                var groupPatternVectors = new Dictionary<int, List<PatternVectorModel>>();
                var groupIndex = 0;
                foreach (var vector in patternModel.PatternVectors)
                {
                    List<PatternVectorModel> currentGroup = null;
                    if (!groupPatternVectors.ContainsKey(groupIndex) || groupPatternVectors[groupIndex] == null)
                    {
                        groupPatternVectors[groupIndex] = new List<PatternVectorModel>();
                        currentGroup = groupPatternVectors[groupIndex];
                    }
                    else
                        currentGroup = groupPatternVectors[groupIndex];

                    //根据命令实际情况分组
                    if (vector.Command != null && (vector.Command.Type == CommandType.loop || vector.Command.Type == CommandType.repeat))
                    {
                        var commandGroup = new List<PatternVectorModel>();
                        groupPatternVectors[++groupIndex] = commandGroup;
                        commandGroup.Add(vector);
                        currentGroup = new List<PatternVectorModel>();
                        groupPatternVectors[++groupIndex] = currentGroup;
                    }
                    else if (currentGroup != null)
                    {
                        currentGroup.Add(vector);
                    }
                }
                patternDataLength = (groupIndex + 1) * (int)_patternUnitLength;
                var packageModelDic = new Dictionary<int, PatternPackageModel>();
                var groupCount = 0;
                int vectorUnitByte = 62;
                foreach (var groupVectors in groupPatternVectors)
                {
                    var isEven = groupVectors.Value.Count % 2 == 0;
                    packageModelDic.Clear();

                    if (isEven)
                    {
                        for (int i = 0; i < groupVectors.Value.Count; i += 2)
                        {
                            var isInit = true;
                            if (i != 0)
                                isInit = false;

                            var row1 = groupVectors.Value[i];
                            var row2 = groupVectors.Value[i + 1];

                            var length = row1.Pins.Count > row2.Pins.Count ? row2.Pins.Count : row1.Pins.Count;
                            var pinList = row1.Pins.Count > row2.Pins.Count ? row1.Pins : row2.Pins;

                            for (int j = 0; j < length; j++)
                            {
                                PatternPackageModel lastPackageModel = null;
                                var pinName = pinList[j].PinName;
                                if (isInit)
                                {
                                    lastPackageModel = new PatternPackageModel();
                                    lastPackageModel.PinName = pinName;
                                    var addr = dataStartAddress;
                                    lastPackageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
                                    lastPackageModel.Length = _patternUnitLength;
                                    lastPackageModel.LengthBytes = lengthBytes;
                                    lastPackageModel.PatternGroups = new List<PatternGroupModel>();
                                    lastPackageModel.PatternGroups.Add(new PatternGroupModel());
                                    packageModelDic.Add(j, lastPackageModel);
                                    result.Add(lastPackageModel);
                                    dataStartAddress += _patternUnitLength;
                                }
                                else
                                {
                                    lastPackageModel = packageModelDic[j];
                                }

                                var pinByte = (byte)((int)row2.Pins[j].VectorValue << 4 | (int)row1.Pins[j].VectorValue);
                                var vectorIncrease = 2;

                                var lastPatternModel = lastPackageModel?.PatternGroups?.LastOrDefault();
                                if (lastPatternModel != null)
                                {
                                    var byteLength = lastPatternModel.VectorNumber + lastPatternModel.Parameter.Count;
                                    if (byteLength >= vectorUnitByte * 2)
                                    {
                                        lastPatternModel = new PatternGroupModel();
                                        if (lastPackageModel.PatternGroups.Count < 8)
                                        {
                                            lastPackageModel.PatternGroups.Add(lastPatternModel);
                                            lastPatternModel.Vectors.Add(pinByte);
                                            lastPatternModel.VectorNumber += vectorIncrease;
                                        }
                                        else
                                        {
                                            lastPackageModel = new PatternPackageModel();
                                            lastPackageModel.PinName = pinName;
                                            var addr = dataStartAddress;
                                            lastPackageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
                                            lastPackageModel.Length = _patternUnitLength;
                                            lastPackageModel.LengthBytes = lengthBytes;
                                            lastPackageModel.PatternGroups = new List<PatternGroupModel>();
                                            lastPackageModel.PatternGroups.Add(new PatternGroupModel());
                                            if (packageModelDic.ContainsKey(j))
                                                packageModelDic[j] = lastPackageModel;
                                            else
                                                packageModelDic.Add(j, lastPackageModel);
                                            result.Add(lastPackageModel);

                                            lastPatternModel = lastPackageModel?.PatternGroups?.LastOrDefault();
                                            lastPatternModel.Vectors.Add(pinByte);
                                            lastPatternModel.VectorNumber += vectorIncrease;
                                            dataStartAddress += _patternUnitLength;
                                        }

                                    }
                                    else
                                    {
                                        lastPatternModel.Vectors.Add(pinByte);
                                        lastPatternModel.VectorNumber += vectorIncrease;
                                    }

                                }
                            }

                            if (isInit)
                                groupCount++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < groupVectors.Value.Count; i += 2)
                        {
                            var isInit = true;
                            if (i != 0)
                                isInit = false;

                            var row1 = groupVectors.Value[i];
                            PatternVectorModel row2 = null;
                            if (i != groupVectors.Value.Count - 1)
                                row2 = groupVectors.Value[i + 1];

                            var length = row2 != null ? row1.Pins.Count > row2.Pins.Count ? row1.Pins.Count : row2.Pins.Count : row1.Pins.Count;
                            var pinList = row1.Pins;
                            if (row2 != null)
                                pinList = row1.Pins.Count > row2?.Pins.Count ? row1.Pins : row2?.Pins;

                            for (int j = 0; j < length; j++)
                            {
                                PatternPackageModel lastPackageModel = null;
                                var pinName = pinList[j].PinName;
                                if (isInit)
                                {
                                    lastPackageModel = new PatternPackageModel();
                                    lastPackageModel.PinName = pinName;
                                    var addr = dataStartAddress;
                                    lastPackageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
                                    lastPackageModel.Length = _patternUnitLength;
                                    lastPackageModel.LengthBytes = lengthBytes;
                                    lastPackageModel.PatternGroups = new List<PatternGroupModel>();
                                    lastPackageModel.PatternGroups.Add(new PatternGroupModel());
                                    packageModelDic.Add(j, lastPackageModel);
                                    result.Add(lastPackageModel);
                                    dataStartAddress += _patternUnitLength;
                                }
                                else
                                {
                                    lastPackageModel = packageModelDic[j];
                                }

                                GetCommandAndParameter(row1, out CommandType instruction, out List<byte> commandParameters);
                                var pinByte = row2 != null ? (byte)((int)row2.Pins[j].VectorValue << 4 | (int)row1.Pins[j].VectorValue) : (byte)row1.Pins[j].VectorValue;
                                var vectorIncrease = row2 != null ? 2 : 1;

                                var lastPatternModel = lastPackageModel?.PatternGroups?.LastOrDefault();
                                if (lastPatternModel != null)
                                {
                                    var byteLength = lastPatternModel.VectorNumber + lastPatternModel.Parameter.Count;
                                    if (byteLength >= vectorUnitByte * 2)
                                    {
                                        lastPatternModel = new PatternGroupModel();
                                        if (lastPackageModel.PatternGroups.Count < 8)
                                        {
                                            lastPackageModel.PatternGroups.Add(lastPatternModel);
                                            lastPatternModel.Instruction = instruction;
                                            lastPatternModel.Parameter = commandParameters;
                                            lastPatternModel.Vectors.Add(pinByte);
                                            lastPatternModel.VectorNumber += vectorIncrease;
                                        }
                                        else
                                        {
                                            lastPackageModel = new PatternPackageModel();
                                            lastPackageModel.PinName = pinName;
                                            var addr = dataStartAddress;
                                            lastPackageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
                                            lastPackageModel.Length = _patternUnitLength;
                                            lastPackageModel.LengthBytes = lengthBytes;
                                            lastPackageModel.PatternGroups = new List<PatternGroupModel>();
                                            lastPackageModel.PatternGroups.Add(new PatternGroupModel());
                                            if (packageModelDic.ContainsKey(j))
                                                packageModelDic[j] = lastPackageModel;
                                            else
                                                packageModelDic.Add(j, lastPackageModel);
                                            result.Add(lastPackageModel);

                                            lastPatternModel = lastPackageModel?.PatternGroups?.LastOrDefault();
                                            lastPatternModel.Instruction = instruction;
                                            lastPatternModel.Parameter = commandParameters;
                                            lastPatternModel.Vectors.Add(pinByte);
                                            lastPatternModel.VectorNumber += vectorIncrease;
                                            dataStartAddress += _patternUnitLength;
                                        }

                                    }
                                    else
                                    {
                                        lastPatternModel.Instruction = instruction;
                                        lastPatternModel.Parameter = commandParameters;
                                        lastPatternModel.Vectors.Add(pinByte);
                                        lastPatternModel.VectorNumber += vectorIncrease;
                                    }

                                }
                            }

                            if (isInit)
                                groupCount++;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private static void GetCommandAndParameter(PatternVectorModel row1, out CommandType instruction, out List<byte> commandParameters)
        {
            instruction = row1.Command.Type;
            commandParameters = new List<byte>();
            switch (row1.Command.Type)
            {
                case CommandType.loop:
                case CommandType.repeat:
                    if (int.TryParse(row1?.Command?.CommandParameter?.ToString(), out int loopCount))
                    {
                        var resultArray = new byte[6];
                        var paramArray = BitConverter.GetBytes(loopCount).Reverse().ToArray();
                        Array.Copy(paramArray, 0, resultArray, 2, paramArray.Length);
                        commandParameters.AddRange(resultArray);
                    }
                    else
                        throw new ArgumentException($"指令参数错误：{row1?.Command?.CommandParameter}");
                    break;
                case CommandType.stop:
                    break;
                case CommandType.endloop:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 获取Patter包数据
        /// </summary>
        /// <param name="patternModel"></param>
        /// <param name="dataStartAddress"></param>
        /// <returns></returns>
        public static List<PatternPackageModel> ConversionPatternModel(BinPatternModel patternModel, ref long dataStartAddress)
        {
            var result = new List<PatternPackageModel>();
            if (patternModel == null)
                return result;

            if (!patternModel.PinPacks.Any())
                return result;

            var lengthBytes = BitConverter.GetBytes(_patternUnitLength).Reverse().ToArray();
            var pinDataLength = 0;
            foreach (var pack in patternModel.PinPacks)
            {
                PatternPackageModel lastPackageModel = null;
                pinDataLength = 0;
                foreach (var data in pack.Data)
                {
                    var index = pack.Data.IndexOf(data);

                    if (index % 64 == 0)    //一个打包包含64个向量组 4096 / 64 = 64
                    {
                        lastPackageModel = new PatternPackageModel();
                        lastPackageModel.PinName = pack.PinName;
                        var addr = dataStartAddress;
                        lastPackageModel.Address = BitConverter.GetBytes(addr).Reverse().Skip(3).ToArray();
                        lastPackageModel.Length = _patternUnitLength;
                        lastPackageModel.LengthBytes = lengthBytes;
                        lastPackageModel.PatternGroups = new List<PatternGroupModel>();
                        result.Add(lastPackageModel);
                        dataStartAddress += _patternUnitLength;
                        pinDataLength += _patternUnitLength;
                    }

                    if (lastPackageModel != null)
                    {
                        var group = new PatternGroupModel();
                        group.VectorNumber = data.VectorNumber;
                        group.Instruction = (CommandType)data.Instruction;
                        if (group.Instruction != CommandType.nop)
                        {
                            var parameter = data.Data.AsSpan(0, 6);
                            group.Parameter = new List<byte>(parameter.ToArray());
                            var vectors = data.Data.AsSpan(6);
                            group.Vectors = new List<byte>(vectors.ToArray());
                        }
                        else
                        {
                            group.Parameter = new List<byte>();
                            group.Vectors = new List<byte>(data.Data);
                        }
                        lastPackageModel.PatternGroups.Add(group);
                    }

                }
                patternModel.PinDataLength = pinDataLength;
            }

            return result;
        }
        #endregion

        #region Private
        private static void AnalysisPatternTimeSet(string pathPattern, PatternModel patternResult)
        {
            using StreamReader streamReader = new StreamReader(pathPattern);
            string input;
            try
            {
                do
                {
                    if (!streamReader.EndOfStream)
                    {
                        input = streamReader.ReadLine().Trim();
                        continue;
                    }
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(L["PatternCompileErr001"]);
                    return;
                } while (!_timeSetRegex.IsMatch(input));
                string[] array = _timeSetRegex.Match(input).Groups[1].Value.Split(new string[3] { ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array.Length; i++)
                {
                    if (patternResult.TimingSets.Contains(array[i].Trim()))
                    {
                        _compileError = true;
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr003"]}{array[i].Trim()}."));
                    }
                    patternResult.TimingSets.Add(array[i].Trim());
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static string AnalysisPatternDigitalInstrument(string pathPattern)
        {
            var strDigitalInstrument = string.Empty;
            using StreamReader streamReader = new StreamReader(pathPattern);
            string input;
            try
            {
                do
                {
                    if (!streamReader.EndOfStream)
                    {
                        input = streamReader.ReadLine().Trim();
                        if (_digitalInstrumentRegex.IsMatch(input))
                        {
                            strDigitalInstrument = _digitalInstrumentRegex.Match(input).Groups[1].Value;
                            break;
                        }
                        continue;
                    }
                    break;
                }
                while (!_atpPinsRegex.IsMatch(input));
            }
            catch (Exception)
            {

                throw;
            }

            return strDigitalInstrument;
        }

        private static void UpdateParametersByModuleType(string pathPattern, ModuleType moduleType)
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

        private static List<string> AnalysisPatternPins(string pathPattern, PatternModel patternResult)
        {
            var patternPins = new List<string>();
            using StreamReader streamReader = new StreamReader(pathPattern);
            string empty = string.Empty;
            string text = string.Empty;
            string empty2 = string.Empty;
            try
            {
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
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(L["PatternCompileErr002"]);
                    return patternPins;
                }
                while (!_atpPinsRegex.IsMatch(empty2));
                Match match = _atpPinsRegex.Match(empty2);
                var memoryName = match.Groups[1].Value.Trim();
                var vectorName = match.Groups[2].Value.Trim();
                string[] array = match.Groups[3].Value.Split(new string[3] { ",", "\t", " " }, StringSplitOptions.RemoveEmptyEntries);
                patternResult.VectorName = vectorName;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = array[i].Replace("(", ":").Replace(")", "");
                    //patternPinsWithDigitalMode.Add(array[i]);
                    string text2 = array[i].Split(':')[0];
                    if (patternPins.Contains(text2))
                    {
                        _compileError = true;
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr03"]}{text2}."));
                    }
                    patternPins.Add(text2);
                }
                switch (memoryName.ToLower())
                {
                    default:
                        _compileError = true;
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr04"]}{memoryName}."));
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
            }
            catch (Exception)
            {

                throw;
            }

            return patternPins;
        }

        private static void AnalysisPatternInstrument(string pathPattern, List<string> atpPinsPinGroups, List<InstrumentModel> patternInstrument)
        {
            using StreamReader streamReader = new StreamReader(pathPattern);
            string text;
            try
            {
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
                                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{msg}", text2));
                            }
                            return;
                        }
                        _compileError = true;
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(L["PatternCompileErr006"]);
                        break;
                    }
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(L["PatternCompileInfo001"]);
                    break;
                }
                while (!_atpPinsRegex.IsMatch(text));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static InstrumentModel GetInstrumentInstance(string strInstrument, List<string> atpPinsPinGroups)
        {
            InstrumentModel result = null;
            string[] array = strInstrument.Split(new string[2] { ":", " " }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (array.Length != 6)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr007"]}", strInstrument));
                    return result;
                }
                result = new InstrumentModel();
                result.StrInstrument = strInstrument;
                string[] array2 = array[0].Replace("(", "").Replace(")", "").Split(new string[3] { ",", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < array2.Length; i++)
                {
                    if (atpPinsPinGroups.Any() && atpPinsPinGroups.Any(x => x.Equals(array2[i])) == true)
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
                }
                result.DigitalMode = array[1];
                if (!int.TryParse(array[2], out var result2))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr008"]}", array[2]));
                }
                else
                {
                    if (result2 < 1 || result2 > 32)
                    {
                        _compileError = true;
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr009"]}", result2));
                    }
                    if (array[3].ToLower() == "parallel" && result2 != 1)
                    {
                        _compileError = true;
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr010"]}", result2));
                    }
                    result.InstrumentWidth = result2;
                }
                result.InstrumentMode = array[3];
                result.BitOrder = array[4];
                result.Format = array[5];
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static void FillRegularExpressionForPinPinGroup(InstrumentModel instrument, List<string> atpPinsPinGroups)
        {
            string text = string.Empty;
            List<string> list = new List<string>();

            // 屏蔽instrument设置
            //if (instrument.DigitalMode.ToLower() == "digsrc")
            //{
            //    text = "D";
            //}
            //else if (instrument.DigitalMode.ToLower() == "digcap")
            //{
            //    text = "V";
            //}

            //for (int i = 0; i < atpPinsPinGroups.Count; i++)
            //{
            //    if (instrument.DicPinItem.Keys.Contains(atpPinsPinGroups[i]))
            //    {
            //        int count = instrument.DicPinItem[atpPinsPinGroups[i]].Count;
            //        list.Add(text + "{" + count + "}");
            //    }
            //    else
            //    {
            //        list.Add("[01LHMXDCV]+");
            //    }
            //}

            for (int i = 0; i < atpPinsPinGroups.Count; i++)
            {
                list.Add("[01LHMXDCV]+");
            }

            string pattern = string.Join("\\s+", list) + "\\s*";
            instrument.RegularExpression = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        private static void GetInstrumentsInfoFromDataBlock(PatternModel patternModel, string pathPattern, List<string> atpPinsPinGroups, List<InstrumentModel> instruments, out List<string> dataBlockMarkerParameter)
        {
            int num = 1;
            bool flag = true;
            List<string> list = new List<string>();
            dataBlockMarkerParameter = new List<string>();
            GroupInstrumentBySrcAndCap(instruments, out var listSrcInstrument, out var listCapInstrument);
            List<int> normalPinIndex = GetNormalPinIndex(atpPinsPinGroups, listSrcInstrument, listCapInstrument);
            using StreamReader streamReader = new StreamReader(pathPattern);
            try
            {
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
                        _tempNestLoopOutermostLoopName = _tempNestLoopOutermostLoopName.Length == 0 ? text3 : _tempNestLoopOutermostLoopName;
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
            catch (Exception)
            {

                throw;
            }
        }

        private static void GroupInstrumentBySrcAndCap(List<InstrumentModel> list_PatternInstrument, out List<InstrumentModel> listSrcInstrument, out List<InstrumentModel> listCapInstrument)
        {
            listSrcInstrument = new List<InstrumentModel>();
            listCapInstrument = new List<InstrumentModel>();
            listSrcInstrument = list_PatternInstrument.Where((x) => x.DigitalMode.ToLower() == "digsrc").ToList();
            listCapInstrument = list_PatternInstrument.Where((x) => x.DigitalMode.ToLower() == "digcap").ToList();
        }

        private static List<int> GetNormalPinIndex(List<string> list_ATPPinsPinGroups, List<InstrumentModel> list_SrcInstrument, List<InstrumentModel> list_CapInstrument)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < list_SrcInstrument.Count; i++)
            {
                List<int> collection = list_SrcInstrument[i].DicPinItem.Keys.Select((x) => list_ATPPinsPinGroups.IndexOf(x)).ToList();
                list.AddRange(collection);
            }
            for (int j = 0; j < list_CapInstrument.Count; j++)
            {
                List<int> collection2 = list_CapInstrument[j].DicPinItem.Keys.Select((x) => list_ATPPinsPinGroups.IndexOf(x)).ToList();
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

        private static void ProgramOneRowVector(PatternModel patternModel, int rowNumber, List<string> listRowVector, List<InstrumentModel> listSrcInstrument, List<InstrumentModel> listCapInstrument, List<string> atpPinsPinGroups, List<string> listDataBlockMarkerParameter, List<int> normalPinIndex)
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
                if (value.Where((x) => x == ':').Count() <= 1)
                {
                    if (list.Count <= 1)
                    {
                        string text = list.Count == 1 ? list[0] : string.Empty;
                        switch (patternModel.ModuleType)
                        {
                            case ModuleType.SRM_Vector:
                                if (!GetPseudoWithParameterSRM(text, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr026"]}", rowNumber, text));
                                }
                                break;
                            case ModuleType.VM_Vector:
                            case ModuleType.LVM_Vector:
                                if (!GetPseudoWithParameter(text, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr026"]}", rowNumber, text));
                                }
                                break;
                        }
                        if (value3.Replace(" ", "").Replace("\t", "").Length != 0)
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr027"]}", rowNumber - (listRowVector.Count - 1 - num)));
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
                                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr028"]}", rowNumber - (listRowVector.Count - 1 - num)));
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
                                _eventAggregator.GetEvent<PatternWarnMessageEvent>().Publish(string.Format($"{L["PatternCompileWarn001"]}", rowNumber - (listRowVector.Count - 1 - num)));
                            }
                        }
                        else if (strPseudo.ToLower() == "trig")
                        {
                            _recordingSwitchTrig = true;
                            _dataBlockIndexTrig++;
                            _ignoreCurrentVectorRowTrig = true;
                            if (value2.ToUpper().Contains("V"))
                            {
                                _eventAggregator.GetEvent<PatternWarnMessageEvent>().Publish(string.Format($"{L["PatternCompileWarn002"]}", rowNumber - (listRowVector.Count - 1 - num)));
                            }
                        }
                        else if (strPseudo.ToLower() == "repeat")
                        {
                            if (!int.TryParse(strPseudoParameter, out var result))
                            {
                                _compileError = true;
                                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr029"]}", rowNumber - (listRowVector.Count - 1 - num), strPseudoParameter));
                            }
                            num2 = result;
                        }
                        else if (strPseudo.ToLower() == "loopa" || strPseudo.ToLower() == "loopb" || strPseudo.ToLower() == "loopc")
                        {
                            if (!int.TryParse(strPseudoParameter, out var result2))
                            {
                                _compileError = true;
                                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr029"]}", rowNumber - (listRowVector.Count - 1 - num), strPseudoParameter));
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
                                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr030"]}", rowNumber - (listRowVector.Count - 1 - num), "D"));
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
                                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr030"]}", rowNumber - (listRowVector.Count - 1 - num), "V"));

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
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr020"]}", rowNumber));
                    return;
                }
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr024"]}", rowNumber));
                return;
            }
            _compileError = true;
            _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr013"]}", rowNumber - (listRowVector.Count - 1 - num)));
        }

        private static List<int> GetPinsIndex(Dictionary<string, List<string>> dicPinItem, List<string> atpPinsPinGroups)
        {
            return dicPinItem.Keys.Select((x) => atpPinsPinGroups.IndexOf(x)).ToList();
        }

        private static bool GetPseudoWithParameterSRM(string uCode, out string strPseudo, out string strPseudoParameter)
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

        //private static void AnalysisPatternVector(string pathPattern, List<string> atpPinsPinGroups, PatternModel patternResult)
        //{
        //    using (StreamReader streamReader = new StreamReader(pathPattern))
        //    {
        //        try
        //        {
        //            _ = streamReader.BaseStream.Length / 100L;
        //            long num = 1L;
        //            long num2 = 0L;
        //            while (!streamReader.EndOfStream && !_atpPinsRegex.IsMatch(streamReader.ReadLine().Trim()))
        //            {
        //                num++;
        //            }
        //            string text = string.Empty;
        //            string text2 = string.Empty;
        //            List<byte> list = new List<byte>();
        //            while (!streamReader.EndOfStream)
        //            {
        //                PatternVectorModel vectorModel = new PatternVectorModel();
        //                LabelModel labelModel = null;
        //                CommandModel commandModel = null;
        //                var pins = new List<PinModel>();

        //                streamReader.ReadLine().SplitValidAndComment(out var valid, out var comment);
        //                text = text + " " + valid;
        //                text2 = text2 + " " + comment;
        //                num++;
        //                if (num % 25000L == 0L)
        //                {
        //                    list.Clear();
        //                }
        //                if (!_vectorRegex.IsMatch(text))
        //                {
        //                    continue;
        //                }
        //                List<byte> list2 = new List<byte>();
        //                Match match = _vectorRegex.Match(text);
        //                string value = match.Groups[1].Value;
        //                string strPseudo = string.Empty;
        //                string strPseudoParameter = string.Empty;
        //                string value2 = match.Groups[5].Value;
        //                string value3 = match.Groups[6].Value;
        //                string comment2 = text2 + match.Groups[7].Value;
        //                string strMTE = string.Empty;
        //                string text3 = match.Groups[4].Value.Trim();
        //                if (_mteRegex.IsMatch(text3))
        //                {
        //                    Match match2 = _mteRegex.Match(text3);
        //                    strMTE = match2.Groups[1].Value;
        //                    text3 = text3.Replace(match2.Value, string.Empty);
        //                }
        //                List<string> list3 = (from y in text3.Trim().Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries)
        //                                      select y.Trim()).ToList();
        //                List<string> list4 = (from x in list3
        //                                      where _listTotalMaskCC.Contains(x.Trim().ToLower())
        //                                      select x into y
        //                                      select y.Trim()).ToList();
        //                for (int i = 0; i < list4.Count; i++)
        //                {
        //                    if (list3.Contains(list4[i]))
        //                    {
        //                        list3.Remove(list4[i]);
        //                    }
        //                }
        //                for (int j = 0; j < list4.Count; j++)
        //                {
        //                    list4[j] = list4[j].ToLower();
        //                }
        //                if (value.Where((x) => x == ':').Count() <= 1)
        //                {
        //                    if (list3.Count <= 1)
        //                    {
        //                        string uCode = list3.Count == 1 ? list3[0] : string.Empty;
        //                        switch (patternResult.ModuleType)
        //                        {
        //                            case ModuleType.VM_Vector:
        //                                if (!GetPseudoWithParameter(uCode, out strPseudo, out strPseudoParameter))
        //                                {
        //                                    _compileError = true;
        //                                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr011"]}", num));
        //                                }
        //                                labelModel = VectorAnalysisLabel(value, num, num2);
        //                                pins = VectorAnalysisPins(value3, num, atpPinsPinGroups);
        //                                commandModel = VectorAnalysisPseudoInstru(strPseudo, strPseudoParameter, num);
        //                                vectorModel.TimingSet = value2;
        //                                break;
        //                            case ModuleType.LVM_Vector:
        //                                if (!GetPseudoWithParameter(uCode, out strPseudo, out strPseudoParameter))
        //                                {
        //                                    _compileError = true;
        //                                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr011"]}", num));
        //                                }
        //                                labelModel = VectorAnalysisLabel(value, num, num2);
        //                                vectorModel.TimingSet = value2;
        //                                commandModel = VectorAnalysisLvmPseudoParameters(strPseudo, strPseudoParameter, num);
        //                                pins = VectorAnalysisPins(value3, num, atpPinsPinGroups);
        //                                break;
        //                            case ModuleType.SRM_Vector:
        //                                VectorAnalysisSrmPseudoWithParametersModifiler(uCode, num, num2, out strPseudo, out strPseudoParameter);
        //                                labelModel = VectorAnalysisLabel(value, num, num2);
        //                                vectorModel.TimingSet = value2;
        //                                pins = VectorAnalysisPins(value3, num, atpPinsPinGroups);
        //                                break;
        //                        }
        //                        vectorModel.Label = labelModel;
        //                        vectorModel.Command = commandModel;
        //                        vectorModel.Pins = pins;
        //                        patternResult.PatternVectors.Add(vectorModel);
        //                        text = string.Empty;
        //                        text2 = string.Empty;
        //                        _validVectorLinesCountInPatternFile++;
        //                        list.AddRange(list2);
        //                        if (strPseudo.ToLower() == "halt" && _haltInVectorLinesPosition == -1)
        //                        {
        //                            _haltInVectorLinesPosition = _validVectorLinesCountInPatternFile;
        //                        }
        //                        num2++;
        //                        if (list.Count > 0)
        //                        {
        //                            list.Clear();
        //                        }
        //                        if (_compileError)
        //                        {
        //                            return;
        //                        }
        //                        continue;
        //                    }
        //                    _compileError = true;
        //                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr020"]}", num));
        //                    return;
        //                }
        //                _compileError = true;
        //                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr024"]}", num));
        //                return;
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }
        //    }
        //    if (_validVectorLinesCountInPatternFile < 32)
        //    {
        //        _compileError = true;
        //        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr025"]}", _validVectorLinesCountInPatternFile, 32));
        //    }
        //}

        private static void AnalysisPatternVector(string pathPattern, List<string> atpPinsPinGroups, PatternModel patternResult)
        {
            using (StreamReader streamReader = new StreamReader(pathPattern))
            {
                try
                {
                    // 使用有意义的变量名
                    long lineNumber = 1L;
                    long validVectorCount = 0L;

                    // 优化1: 使用ReadLine()直接赋值检查，避免重复调用
                    string currentLine;
                    while ((currentLine = streamReader.ReadLine()) != null && !_atpPinsRegex.IsMatch(currentLine.Trim()))
                    {
                        lineNumber++;
                    }

                    // 优化2: 使用StringBuilder替代字符串拼接，避免创建大量临时字符串
                    var textBuilder = new StringBuilder(256);  // 预分配合理容量
                    var commentBuilder = new StringBuilder(256);

                    // 优化3: 使用HashSet实现O(1)查找，替代原来的List.Contains() O(n)查找
                    var maskHashSet = new HashSet<string>(_listTotalMaskCC.Select(x => x.Trim().ToLower()));

                    // 优化4: 缓存频繁访问的属性到局部变量
                    var moduleType = patternResult.ModuleType;
                    var patternVectors = patternResult.PatternVectors;

                    // 优化5: 复用集合对象，减少GC压力
                    var dataList = new List<string>(32);      // 预分配典型容量
                    var maskList = new List<string>(16);

                    while ((currentLine = streamReader.ReadLine()) != null)
                    {
                        currentLine.SplitValidAndComment(out var valid, out var comment);

                        textBuilder.Append(' ').Append(valid);
                        commentBuilder.Append(' ').Append(comment);
                        lineNumber++;

                        // 快速失败：如果不是向量行则跳过
                        if (!_vectorRegex.IsMatch(textBuilder.ToString()))
                        {
                            // 定期清理避免内存膨胀
                            if (lineNumber % 100 == 0)
                            {
                                textBuilder.Clear();
                                commentBuilder.Clear();
                            }
                            continue;
                        }

                        var match = _vectorRegex.Match(textBuilder.ToString());
                        string label = match.Groups[1].Value;
                        string timingSet = match.Groups[5].Value;
                        string pinsPart = match.Groups[6].Value;
                        string vectorData = match.Groups[4].Value.Trim();

                        // 解析MTE
                        string strMTE = string.Empty;
                        var mteMatch = _mteRegex.Match(vectorData);
                        if (mteMatch.Success)
                        {
                            strMTE = mteMatch.Groups[1].Value;
                            vectorData = vectorData.Replace(mteMatch.Value, string.Empty);
                        }

                        // 优化6: 复用集合并避免LINQ开销
                        dataList.Clear();
                        maskList.Clear();

                        if (!string.IsNullOrEmpty(vectorData))
                        {
                            var parts = vectorData.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < parts.Length; i++)
                            {
                                string trimmed = parts[i].Trim();
                                if (!string.IsNullOrEmpty(trimmed))
                                {
                                    dataList.Add(trimmed);
                                }
                            }
                        }

                        // 优化7: 从后往前遍历，避免多次Remove操作
                        for (int i = dataList.Count - 1; i >= 0; i--)
                        {
                            string item = dataList[i];
                            if (maskHashSet.Contains(item.ToLower()))
                            {
                                maskList.Add(item.ToLower());
                                dataList.RemoveAt(i);
                            }
                        }

                        // 优化8: 手动计数并提前退出，避免LINQ的Count()遍历整个字符串
                        int colonCount = 0;
                        foreach (char c in label)
                        {
                            if (c == ':')
                            {
                                colonCount++;
                                if (colonCount > 1) break;  // 提前退出
                            }
                        }

                        if (colonCount > 1)
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<PatternErrorMessageEvent>()
                                .Publish(string.Format($"{L["PatternCompileErr024"]}", lineNumber));
                            return;
                        }

                        if (dataList.Count > 1)
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<PatternErrorMessageEvent>()
                                .Publish(string.Format($"{L["PatternCompileErr020"]}", lineNumber));
                            return;
                        }

                        string uCode = dataList.Count == 1 ? dataList[0] : string.Empty;
                        string strPseudo = string.Empty;
                        string strPseudoParameter = string.Empty;

                        // 优化9: 使用对象初始化器
                        var vectorModel = new PatternVectorModel
                        {
                            TimingSet = timingSet
                        };

                        // 优化10: 将公共代码移出switch
                        switch (moduleType)
                        {
                            case ModuleType.VM_Vector:
                                if (!GetPseudoWithParameter(uCode, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    _eventAggregator.GetEvent<PatternErrorMessageEvent>()
                                        .Publish(string.Format($"{L["PatternCompileErr011"]}", lineNumber));
                                    return;
                                }
                                vectorModel.Label = VectorAnalysisLabel(label, lineNumber, validVectorCount);
                                vectorModel.Command = VectorAnalysisPseudoInstru(strPseudo, strPseudoParameter, lineNumber);
                                break;

                            case ModuleType.LVM_Vector:
                                if (!GetPseudoWithParameter(uCode, out strPseudo, out strPseudoParameter))
                                {
                                    _compileError = true;
                                    _eventAggregator.GetEvent<PatternErrorMessageEvent>()
                                        .Publish(string.Format($"{L["PatternCompileErr011"]}", lineNumber));
                                    return;
                                }
                                vectorModel.Label = VectorAnalysisLabel(label, lineNumber, validVectorCount);
                                vectorModel.Command = VectorAnalysisLvmPseudoParameters(strPseudo, strPseudoParameter, lineNumber);
                                break;

                            case ModuleType.SRM_Vector:
                                VectorAnalysisSrmPseudoWithParametersModifiler(uCode, lineNumber, validVectorCount,
                                    out strPseudo, out strPseudoParameter);
                                vectorModel.Label = VectorAnalysisLabel(label, lineNumber, validVectorCount);
                                break;
                        }

                        // 优化11: 复用引脚列表
                        vectorModel.Pins = VectorAnalysisPins(pinsPart, lineNumber, atpPinsPinGroups);

                        patternVectors.Add(vectorModel);

                        // 优化12: 清理StringBuilder为下一轮准备
                        textBuilder.Clear();
                        commentBuilder.Clear();

                        _validVectorLinesCountInPatternFile++;

                        // 优化13: 使用StringComparison.OrdinalIgnoreCase避免创建小写字符串
                        if (strPseudo.Equals("halt", StringComparison.OrdinalIgnoreCase) &&
                            _haltInVectorLinesPosition == -1)
                        {
                            _haltInVectorLinesPosition = _validVectorLinesCountInPatternFile;
                        }

                        validVectorCount++;

                        if (_compileError) return;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // 最终验证
            if (_validVectorLinesCountInPatternFile < 32)
            {
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>()
                    .Publish(string.Format($"{L["PatternCompileErr025"]}",
                        _validVectorLinesCountInPatternFile, 32));
            }
        }

        private static bool GetPseudoWithParameter(string uCode, out string strPseudo, out string strPseudoParameter)
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

        private static LabelModel VectorAnalysisLabel(string strLabel, long currentLine, long currentVectorLine)
        {
            var result = new LabelModel();
            string[] array = strLabel.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length > 2)
            {
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr012"]}", currentLine, strLabel));
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
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr012"]}", currentLine, array[0]));
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

        private static List<PinModel> VectorAnalysisPins(string timesetAndPinsValue, long currentLine, List<string> atpPinsPinGroups)
        {
            var pinList = new List<PinModel>();
            string[] array = timesetAndPinsValue.Split(new string[2] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != atpPinsPinGroups.Count)
            {
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr013"]}", currentLine));
                return pinList;
            }

            for (int i = 0; i < array.Length; i++)
            {
                AddPinValueToList(array[i], currentLine, pinList, atpPinsPinGroups[i]);
            }

            return pinList;
        }

        private static void AddPinValueToList(string strPinValue, long currentLine, List<PinModel> pinsValue, string pinName)
        {
            if (_pinValueNameExchangeDic.Keys.Contains(strPinValue))
            {
                pinsValue.Add(new PinModel()
                {
                    PinName = pinName,
                    VectorValue = EnumHelper.GetEnumValueFromDescription<VectorValueType>(strPinValue),
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
                        PinName = pinName,
                        VectorValue = EnumHelper.GetEnumValueFromDescription<VectorValueType>(strPinValue),
                    });
                    continue;
                }
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr014"]}", array[i].ToString()));
            }
        }

        private static CommandModel VectorAnalysisPseudoInstru(string pseudoInstru, string pseudoInstruParameter, long currentLine)
        {
            var result = new CommandModel();
            result.CommandFullContent = pseudoInstru;

            if (pseudoInstru.Length == 0)
                return result;

            if (!_pseudoInstruDic.Keys.Contains(pseudoInstru.ToLower()))
            {
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr015"]}", currentLine, pseudoInstru));
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
                        _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr016"]}", currentLine, pseudoInstru));
                    }
                    else
                    {
                        result.Type = EnumHelper.GetEnumValueFromDescription<CommandType>(pseudoInstru.ToLower());
                    }
                    break;
                default:
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr017"]}", currentLine, pseudoInstru));
                    break;
                case "start":
                case "repeat":
                case "loop":
                    {
                        if (pseudoInstruParameter.Length == 0)
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr018"]}", currentLine, pseudoInstru));
                            break;
                        }
                        int parameter = 0;
                        result.Type = EnumHelper.GetEnumValueFromDescription<CommandType>(pseudoInstru.ToLower());
                        if (int.TryParse(pseudoInstruParameter, out parameter))
                        {
                            if (parameter >= 2 && parameter <= 1048575)
                            {
                                result.CommandParameter = parameter;
                                break;
                            }
                            _compileError = true;
                            _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr019"]}", currentLine, pseudoInstru, 2, 1048575));
                        }
                        else
                        {
                            _compileError = true;
                            _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr022"]}", currentLine, pseudoInstruParameter, 2, 1048575, string.Join(",", _dataBlockMarkerParameter)));
                        }
                        break;
                    }
            }

            return result;
        }

        private static CommandModel VectorAnalysisLvmPseudoParameters(string strPseudo, string strPseudoParameter, long currentLine)
        {
            var result = new CommandModel();
            result.CommandFullContent = strPseudo;
            if (!string.IsNullOrEmpty(strPseudo))
            {
                result.Type = EnumHelper.GetEnumValueFromDescription<CommandType>(strPseudo.ToLower());
            }
            if (strPseudo.ToLower() == "cflag" || strPseudo.ToLower() == "return")
            {
                if (strPseudoParameter.Length == 0)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr019"]}", currentLine, strPseudo));
                    return result;
                }
                if (int.TryParse(strPseudoParameter, out var parameter) && (parameter < 0 || parameter > 4095))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr021"]}", currentLine, strPseudoParameter, strPseudo, 0, 4095));
                    return result;
                }
            }
            if (strPseudo.ToLower() == "repeat")
            {
                if (strPseudoParameter.Length == 0)
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr017"]}", currentLine, strPseudo));
                    return result;
                }
                if (int.TryParse(strPseudoParameter, out var parameter) && (parameter < 2 || parameter > int.MaxValue))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr021"]}", currentLine, strPseudoParameter, strPseudo, 2, int.MaxValue));
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
                num = !(strPseudo.ToLower() == "call") ? 2 : 0;
                if (result3 >= num && result3 <= int.MaxValue)
                {
                    result.CommandParameter = result3;
                    return result;
                }
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr019"]}", currentLine, strPseudoParameter, num, int.MaxValue));
            }
            else
            {
                _compileError = true;
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr022"]}", currentLine, strPseudoParameter, num, int.MaxValue, string.Join(",", _dataBlockMarkerParameter)));
            }

            return result;
        }

        private static CommandModel VectorAnalysisSrmPseudoWithParametersModifiler(string uCode, long currentLine, long currentVectorLine, out string strPseudo, out string strPseudoParameter)
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
                _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr015"]}", currentLine, uCode));
            }

            return result;
        }

        private static bool FactoryGetModifierOpcodeOperand(string strConditional, string strPseudo, string strPseudoParameter, long currentLine, long currentVectorLine)
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
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr017"]}", currentLine, strPseudo));
                    return false;
                }
                if (int.TryParse(strPseudoParameter, out var result) && (result < 2 || result > int.MaxValue))
                {
                    _compileError = true;
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr021"]}", currentLine, strPseudoParameter, strPseudo, 2, int.MaxValue));
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
                    _eventAggregator.GetEvent<PatternErrorMessageEvent>().Publish(string.Format($"{L["PatternCompileErr023"]}", currentLine, strPseudoParameter));
                    return false;
                }
                num = value2;
            }

            return true;
        }

        private static bool GetConditionFlagValue(string strPseudo, List<string> listConditionFlags, out int value, out List<string> listUnsupportedConditionFlag)
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
                listUnsupportedConditionFlag = listConditionFlags.Where((x) => !dic_TempConditioCollection.ContainsKey(x.ToLower())).ToList();
                if (listUnsupportedConditionFlag.Count != 0)
                {
                    return false;
                }
                List<int> list = listConditionFlags.Select((x) => dic_TempConditioCollection[x.ToLower()]).ToList();
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

        private static async Task ExportLvmPattern(PatternModel patternModel, string exportFilePath)
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

                    if (!string.IsNullOrEmpty(patternModel.InstrumentName))
                    {
                        //digital
                        var digital = $"digital_ins={patternModel.InstrumentName};";
                        sw.WriteLine(digital);
                    }

                    if (!string.IsNullOrEmpty(patternModel.InstrumentInfo.StrInstrument))
                    {
                        //instrument
                        sw.WriteLine("instruments = {");
                        sw.WriteLine($"{patternModel.InstrumentInfo.StrInstrument};");
                        sw.WriteLine("}");
                    }

                    //moduleType
                    sw.WriteLine(patternModel.ModuleType.GetDescription());

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

        private static string GetPatternHeader(PatternVectorModel pattern)
        {
            var patternHeader = string.Empty;
            if (pattern == null || !pattern.Pins.Any())
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

        private static string GetTimingSetAndVector(PatternVectorModel vector, int colHeaderLength)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(vector.Label.LabelFullContent))
                result += vector.Label.LabelFullContent;

            if (!string.IsNullOrEmpty(vector.Command.CommandFullContent))
            {
                if (string.IsNullOrEmpty(result))
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
                result += $"{pin.VectorValue.GetDescription()} ";
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

        public static Dictionary<string, List<PinPatternModel>> GetSinglePinPatternList(PatternModel pattern)
        {
            var result = new Dictionary<string, List<PinPatternModel>>();
            if (pattern.PatternVectors == null || !pattern.PatternVectors.Any())
                return result;

            var lastInstruction = CommandType.nop;
            object lastCommandParameter = null;

            foreach (var vectorModel in pattern.PatternVectors)
            {
                if (vectorModel.Command != null)
                {
                    switch (vectorModel.Command.Type)
                    {
                        case CommandType.loop:
                            lastInstruction = vectorModel.Command.Type;
                            lastCommandParameter = vectorModel.Command.CommandParameter;
                            break;
                        case CommandType.endloop:
                            lastInstruction = CommandType.nop;
                            lastCommandParameter = null;
                            break;
                        default:
                            break;
                    }
                }

                foreach (var pinModel in vectorModel.Pins)
                {
                    List<PinPatternModel> pinPatternModelList;
                    if (!result.ContainsKey(pinModel.PinName))
                    {
                        pinPatternModelList = new List<PinPatternModel>();
                        result.Add(pinModel.PinName, pinPatternModelList);
                    }
                    else
                        pinPatternModelList = result[pinModel.PinName];

                    pinPatternModelList.Add(new PinPatternModel
                    {
                        PinName = pinModel.PinName,
                        Instruction = vectorModel.Command.Type == CommandType.nop ? lastInstruction : vectorModel.Command.Type,
                        CommandParameter = vectorModel.Command.Type == CommandType.nop ? lastCommandParameter : vectorModel.Command.CommandParameter,
                        TimingSet = vectorModel.TimingSet,
                        VectorValue = pinModel.VectorValue,
                    });
                }
            }

            return result;
        }
        #endregion
    }
}
