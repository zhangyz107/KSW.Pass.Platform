using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Shmoo;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Digitals;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Ppmus;
using KSW.ATE01.Instrument.IO.Enums.Shmoos;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Language;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.ATE01.Project.Base.Services.Loggers;
using KSW.ATE01.Project.Base.Services.Memory;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Shmoo
{
    public class Shmoo : InstrumentCommandBase<Shmoo>, IShmoo
    {
        private string _testName;
        private const double _epsilon = 1.401298464324817E-45;
        private readonly Dictionary<string, LevelModel> _levelDic = new Dictionary<string, LevelModel>();
        private readonly Dictionary<string, TimingModel> _timeDic = new Dictionary<string, TimingModel>();

        private LanguageManager L => LanguageManager.Instance;

        public IPrint Print => new Print(_testName);

        public static IShmoo ShmooTest(string testName)
        {
            if (string.IsNullOrEmpty(testName))
                ErrorMessages.Shmoo.TestNameIsNullOrEmpty();

            Instance._testName = testName;
            return Instance;
        }

        public void SetX(string type, string mode, double begin, double end, double stepSize, string pinList, string timingName = "", double delayS = 0)
        {
            ShmooTestManagerHelper.Instance.TrySetX(_testName, type, mode, begin, end, stepSize, pinList, timingName, delayS);
        }

        public void SetY(string type, string mode, double begin, double end, double stepSize, string pinList, string timingName = "", double delayS = 0)
        {
            ShmooTestManagerHelper.Instance.TrySetY(_testName, type, mode, begin, end, stepSize, pinList, timingName, delayS);
        }

        public void Mode(AxisDirection axisType)
        {
            ShmooTestManagerHelper.Instance.TrySetMode(_testName, axisType);
        }

        public void Run(ActiveMode activeMode = ActiveMode.Normal, string patternName = "")
        {
            var commonData = CommonData.Instance;
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
            var shmooTestDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();
            if (!shmooTestDic.ContainsKey(_testName))
                return;
            var shmooTest = shmooTestDic[_testName].TestInfo;
            if (shmooTest.XAxis == null && shmooTest.YAxis == null)
                return;

            // 获取即将播放的Pattern文件名
            var args = commonData.TestItemArgs;
            if (args.Any() && string.IsNullOrEmpty(patternName))
                patternName = args.FirstOrDefault()?.ParamValue;

            if (!ShmooTestManagerHelper.Instance.Tests.ContainsKey(_testName))
                ShmooTestManagerHelper.Instance.Results.Add(_testName, new List<ShmooResult>());
            else
                ShmooTestManagerHelper.Instance.Results[_testName].Clear();

            var timingName = string.Empty;
            ShmooAxis xaxis = shmooTest.XAxis;
            ShmooAxis yaxis = shmooTest.YAxis;

            if (IsTimingAxis(xaxis) || IsTimingAxis(yaxis))
            {
                ShmooAxis activeXAxis = IsTimingAxis(xaxis) ? xaxis : yaxis;
                timingName = activeXAxis.TimingName;
                GetRawTimingAndLevelInfo(timingName);
            }

            switch (shmooTest.Direction)
            {
                case AxisDirection.None:
                    var shmooAxis = xaxis ?? yaxis;
                    this.InternalRunShmoo(activeMode, shmooAxis, this.GetAxisLoopCount(shmooAxis), null, 0, patternName);
                    break;

                case AxisDirection.XY:
                    this.InternalRunShmoo(activeMode, shmooTest.XAxis, this.GetAxisLoopCount(xaxis), yaxis, GetAxisLoopCount(yaxis), patternName);
                    break;

                case AxisDirection.YX:
                    this.InternalRunShmoo(activeMode, shmooTest.YAxis, this.GetAxisLoopCount(yaxis), xaxis, this.GetAxisLoopCount(xaxis), patternName);
                    break;
            }
        }

        // Helper Methods
        private bool IsTimingAxis(ShmooAxis axis)
        {
            return axis != null && axis.AxisType == AxisType.Timing;
        }

        private int GetAxisLoopCount(ShmooAxis axis)
        {
            if (axis == null)
            {
                return 0;
            }

            double num = (axis.End - axis.Begin) / axis.Step;
            double calculatedEnd = axis.Begin + (int)num * axis.Step;

            if (Math.Abs(calculatedEnd - axis.End) >= _epsilon)
            {
                return (int)num + 1;
            }

            return (int)num;
        }


        private async Task InternalRunShmoo(ActiveMode activeMode, ShmooAxis axis1, int axis1LoopCount, ShmooAxis axis2, int axis2LoopCount, string moduleName)
        {
            var results = ShmooTestManagerHelper.Instance.Results[_testName];
            int i = 0;
            while (i <= axis1LoopCount)
            {
                double axisValue = CalculateAxisValue(axis1, i);
                bool isValidAxis1 = SetShmooType(axis1, axisValue);

                if (axis2 != null)
                {
                    ProcessAxis(axis2, axis2LoopCount, axisValue, isValidAxis1, results, activeMode, moduleName);
                }
                else if (isValidAxis1)
                {
                    results.Add(ShmooPattern(activeMode, moduleName, axisValue, 0.0));
                }
                else
                {
                    results.Add(ShmooResult.GetInvalidResult(axisValue, 0.0));
                }

                if (axis1.Delay != 0.0)
                {
                    int delay = (int)axis1.Delay * 1000;
                    await Task.Delay(delay);
                }
                i++;
            }

            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
            Dictionary<string, ShmooModel> shmooDictionary = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();

            if (shmooDictionary.ContainsKey(_testName))
            {
                shmooDictionary[_testName].Results = results;
                ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.WriteObject(shmooDictionary);
            }
        }

        private double CalculateAxisValue(ShmooAxis axis, int loopIndex)
        {
            double calculatedValue = axis.Begin + loopIndex * axis.Step;
            if (axis.Step < 0.0)
            {
                return (calculatedValue < axis.End) ? axis.End : calculatedValue;
            }
            return (calculatedValue > axis.End) ? axis.End : calculatedValue;
        }

        private bool SetShmooType(ShmooAxis axis, double value)
        {
            var result = false;
            switch (axis.AxisType)
            {
                case AxisType.Level:
                    result = this.TrySetLevelMode(axis.Mode, axis.PinList, value);
                    break;
                case AxisType.Power:
                    result = TrySetPower(axis.Mode, axis.PinList, value);
                    break;
                default:
                    result = TrySetTimingMode(axis.Mode, axis.PinList, value);
                    break;
            }

            return result;
        }

        #region TrySetLevelMode
        private bool TrySetLevelMode(string levelMode, string pinList, double currentValue)
        {
            var pins = GetPinList(pinList);
            switch (levelMode.ToLower())
            {
                case "vil":
                    return ProcessVilCondition(pins, currentValue);
                case "vih":
                    return ProcessVihCondition(pins, currentValue);
                case "vol":
                    return ProcessVolCondition(pins, currentValue);
                case "voh":
                    return ProcessVohCondition(pins, currentValue);
                case "iol":
                    return ProcessIolCondition(pins, currentValue);
                case "ioh":
                    return ProcessIohCondition(pins, currentValue);
                case "vt":
                    return ProcessVtCondition(pins, currentValue);
                default:
                    return false;
            }
        }

        private bool ProcessVilCondition(List<ChannelModel> pins, double currentValue)
        {
            bool result = true;
            foreach (var pin in pins)
            {
                var pinName = pin.PinName;
                if (System.Convert.ToDouble(_levelDic[pinName].Vih) < currentValue)
                {
                    var msg = L["LevelValueLowerErrorTips"];
                    PrintResultLog.Message(string.Format($"{msg}", pin, "Vih", "Vil", currentValue));
                    result = false;
                }
                var level = _levelDic[pinName];
                if (level == null)
                    continue;

                var vih = System.Convert.ToDouble(level.Vih);
                var vol = System.Convert.ToDouble(level.Vol);
                var voh = System.Convert.ToDouble(level.Voh);
                var vt = System.Convert.ToDouble(level.Vt);
                var iol = System.Convert.ToDouble(level.Iol);
                var ioh = System.Convert.ToDouble(level.Ioh);

                //todo:设置Vil
                Ppmu.Pins(pinName).SetDriverAndComparator(currentValue, vih, vol, voh, vt, iol, ioh, true, Enums.Ppmus.HizType.vt, 0);
            }
            return result;
        }

        private bool ProcessVihCondition(List<ChannelModel> pins, double currentValue)
        {
            bool result = true;
            foreach (var pin in pins)
            {
                var pinName = pin.PinName;
                if (System.Convert.ToDouble(_levelDic[pinName].Vil) > currentValue)
                {
                    var msg = L["LevelValueHigherErrorTips"];
                    PrintResultLog.Message(string.Format($"{msg}", pin, "Vil", "Vih", currentValue));
                    result = false;
                }

                var level = _levelDic[pinName];
                if (level == null)
                    continue;

                var vil = System.Convert.ToDouble(level.Vil);
                var vol = System.Convert.ToDouble(level.Vol);
                var voh = System.Convert.ToDouble(level.Voh);
                var vt = System.Convert.ToDouble(level.Vt);
                var iol = System.Convert.ToDouble(level.Iol);
                var ioh = System.Convert.ToDouble(level.Ioh);

                //todo:设置Vih
                Ppmu.Pins(pinName).SetDriverAndComparator(vil, currentValue, vol, voh, vt, iol, ioh, true, Enums.Ppmus.HizType.vt, 0);
            }
            return result;
        }

        private bool ProcessVolCondition(List<ChannelModel> pins, double currentValue)
        {
            bool result = true;
            foreach (var pin in pins)
            {
                var pinName = pin.PinName;
                if (System.Convert.ToDouble(_levelDic[pinName].Voh) < currentValue)
                {
                    var msg = L["LevelValueLowerErrorTips"];
                    PrintResultLog.Message(string.Format($"{msg}", pin, "Voh", "Vol", currentValue));
                    result = false;
                }

                var level = _levelDic[pinName];
                if (level == null)
                    continue;

                var vil = System.Convert.ToDouble(level.Vil);
                var vih = System.Convert.ToDouble(level.Vih);
                var voh = System.Convert.ToDouble(level.Voh);
                var vt = System.Convert.ToDouble(level.Vt);
                var iol = System.Convert.ToDouble(level.Iol);
                var ioh = System.Convert.ToDouble(level.Ioh);

                //todo:设置Vol
                Ppmu.Pins(pinName).SetDriverAndComparator(vil, vih, currentValue, voh, vt, iol, ioh, true, Enums.Ppmus.HizType.vt, 0);
            }
            return result;
        }

        private bool ProcessVohCondition(List<ChannelModel> pins, double currentValue)
        {
            bool result = true;
            foreach (var pin in pins)
            {
                var pinName = pin.PinName;
                if (System.Convert.ToDouble(_levelDic[pinName].Vol) > currentValue)
                {
                    var msg = L["LevelValueHigherErrorTips"];
                    PrintResultLog.Message(string.Format($"{msg}", pin, "Vol", "Voh", currentValue));
                    result = false;
                }

                var level = _levelDic[pinName];
                if (level == null)
                    continue;

                var vil = System.Convert.ToDouble(level.Vil);
                var vih = System.Convert.ToDouble(level.Vih);
                var vol = System.Convert.ToDouble(level.Vol);
                var vt = System.Convert.ToDouble(level.Vt);
                var iol = System.Convert.ToDouble(level.Iol);
                var ioh = System.Convert.ToDouble(level.Ioh);

                //todo:设置Voh
                Ppmu.Pins(pinName).SetDriverAndComparator(vil, vih, vol, currentValue, vt, iol, ioh, true, Enums.Ppmus.HizType.vt, 0);
            }
            return result;
        }

        private bool ProcessIolCondition(List<ChannelModel> pins, double currentValue)
        {
            bool result = true;
            foreach (var pin in pins)
            {
                var pinName = pin.PinName;
                if (System.Convert.ToDouble(_levelDic[pinName].Ioh) > currentValue)
                {
                    var msg = L["LevelValueLowerErrorTips"];
                    PrintResultLog.Message(string.Format($"{msg}", pin, "Ioh", "Iol", currentValue));
                    result = false;
                }

                var level = _levelDic[pinName];
                if (level == null)
                    continue;

                var vil = System.Convert.ToDouble(level.Vil);
                var vih = System.Convert.ToDouble(level.Vih);
                var vol = System.Convert.ToDouble(level.Vol);
                var voh = System.Convert.ToDouble(level.Voh);
                var vt = System.Convert.ToDouble(level.Vt);
                var ioh = System.Convert.ToDouble(level.Ioh);

                //todo:设置Iol
                Ppmu.Pins(pinName).SetDriverAndComparator(vil, vih, vol, voh, vt, currentValue, ioh, true, Enums.Ppmus.HizType.vt, 0);
            }
            return result;
        }

        private bool ProcessIohCondition(List<ChannelModel> pins, double currentValue)
        {
            bool result = true;
            foreach (var pin in pins)
            {
                var pinName = pin.PinName;
                if (System.Convert.ToDouble(_levelDic[pinName].Iol) < currentValue)
                {
                    var msg = L["LevelValueHigherErrorTips"];
                    PrintResultLog.Message(string.Format($"{msg}", pin, "Iol", "Ioh", currentValue));
                    result = false;
                }

                var level = _levelDic[pinName];
                if (level == null)
                    continue;

                var vil = System.Convert.ToDouble(level.Vil);
                var vih = System.Convert.ToDouble(level.Vih);
                var vol = System.Convert.ToDouble(level.Vol);
                var voh = System.Convert.ToDouble(level.Voh);
                var iol = System.Convert.ToDouble(level.Iol);
                var vt = System.Convert.ToDouble(level.Vt);

                //todo:设置Ioh
                Ppmu.Pins(pinName).SetDriverAndComparator(vil, vih, vol, voh, vt, iol, currentValue, true, Enums.Ppmus.HizType.vt, 0);
            }
            return result;
        }

        private bool ProcessVtCondition(List<ChannelModel> pins, double currentValue)
        {
            bool result = true;
            foreach (var pin in pins)
            {
                var pinName = pin.PinName;
                var level = _levelDic[pinName];

                if (level == null)
                    continue;

                var vil = System.Convert.ToDouble(level.Vil);
                var vih = System.Convert.ToDouble(level.Vih);
                var vol = System.Convert.ToDouble(level.Vol);
                var voh = System.Convert.ToDouble(level.Voh);
                var iol = System.Convert.ToDouble(level.Iol);
                var ioh = System.Convert.ToDouble(level.Ioh);

                //todo:设置Vt
                Ppmu.Pins(pinName).SetDriverAndComparator(vil, vih, vol, voh, currentValue, iol, ioh, true, Enums.Ppmus.HizType.vt, 0);
            }
            return result;
        }
        #endregion

        #region TrySetPower
        private bool TrySetPower(string mode, string pinList, double currentValue)
        {
            if (string.IsNullOrEmpty(mode))
            {
                PrintResultLog.Message(L["ForceModeEmptyError"]);
                return false;
            }

            var (isValid, typeName, min, max) = mode switch
            {
                "Force_I" => (true, "Current", -1.2, 1.2),
                "Force_V" => (true, "Voltage", -2.0, 7.0),
                _ => (false, string.Empty, 0.0, 0.0)
            };

            if (!isValid)
            {
                PrintResultLog.Message(string.Format(L["ForceModeEmptyError"], mode));
                return false;
            }

            if (currentValue < min || currentValue > max)
            {
                PrintResultLog.Message(string.Format(L["IsOutOfAllowedRange"], typeName, currentValue, min, max));
                return false;
            }

            if (mode == "Force_V")
            {
                //设置Force_V
                //Dps.Pins(pinList).SetVoltage(currentValue);
            }
            return true;
        }

        #endregion

        #region TrySetTimingMode
        private bool TrySetTimingMode(string mode, string pinList, double value)
        {
            var pins = GetPinList(pinList);
            var result = false;

            switch (mode)
            {
                case "Period":
                    result = ProcessPeriodMode(pins, value);
                    break;
                case "StrobeB":
                    result = SetDigitalPinValue(pins, value, WaveformEdgeType.StrobeB);
                    break;
                case "DriveD":
                    result = VaildateAndSetDrive(pins, value, p => System.Convert.ToDouble(_timeDic[p].Period), "Period", "DriveD", WaveformEdgeType.DriveD);
                    break;
                case "DriveC":
                    result = VaildateAndSetDrive(pins, value, p => double.Parse(_timeDic[p].DriveD), "DriveD", "DriveC", WaveformEdgeType.DriveC);
                    break;
                case "DriveB":
                    result = VaildateAndSetDrive(pins, value, p => double.Parse(_timeDic[p].DriveC), "DriveC", "DriveB", WaveformEdgeType.DriveB);
                    break;
                case "DriveA":
                    result = VaildateAndSetDrive(pins, value, p => double.Parse(_timeDic[p].DriveB), "DriveB", "DriveA", WaveformEdgeType.DriveA);
                    break;
                default:
                    break;
            }

            return result;
        }

        private bool ProcessPeriodMode(List<ChannelModel> pins, double value)
        {
            var result = true;
            try
            {
                var timingModelDic = new Dictionary<TimingModel, string>();
                foreach (var pin in pins)
                {
                    if (!_timeDic.ContainsKey(pin.PinName))
                        continue;

                    var pinList = pin.PinName;
                    var timing = _timeDic[pin.PinName];
                    if (timingModelDic.Any())
                    {
                        if (timingModelDic.ContainsKey(timing))
                        {
                            pinList = timingModelDic[timing];
                            timingModelDic[timing] = pinList + $",{pin.PinName}";
                        }
                        else
                            timingModelDic.Add(timing, pinList);
                    }
                    else
                        timingModelDic.Add(timing, pinList);
                }

                foreach (var timingModel in timingModelDic)
                {
                    var timing = timingModel.Key;
                    var pinList = timingModel.Value;

                    var period = timing.Period;
                    var radio = value / (period * 1.0); //同比例调整

                    double.TryParse(timing.DriveA, out double driveA);
                    double.TryParse(timing.DriveB, out double driveB);
                    double.TryParse(timing.DriveC, out double driveC);
                    double.TryParse(timing.DriveD, out double driveD);
                    var strobeA = System.Convert.ToDouble(timing.StrobeA);
                    var strobeB = System.Convert.ToDouble(timing.StrobeB);

                    driveA *= radio;
                    driveB *= radio;
                    driveC *= radio;
                    driveD *= radio;
                    strobeA *= radio;
                    strobeB *= radio;

                    Digital.Pins(pinList).SetTimingDetail(value, driveA, driveB, driveC, driveD, timing.Fmt, timing.StrobeMode, strobeA, strobeB);
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        private bool SetDigitalPinValue(List<ChannelModel> pins, double value, WaveformEdgeType edgeType)
        {
            var result = true;
            try
            {
                var timingModelDic = new Dictionary<TimingModel, string>();

                //对TimingModel相同的引脚进行分组 
                foreach (var pin in pins)
                {
                    if (!_timeDic.ContainsKey(pin.PinName))
                        continue;
                    var pinList = pin.PinName;
                    var timing = _timeDic[pin.PinName];
                    if (timingModelDic.Any())
                    {
                        if (timingModelDic.ContainsKey(timing))
                        {
                            pinList = timingModelDic[timing];
                            timingModelDic[timing] = pinList + $",{pin.PinName}";
                        }
                        else
                            timingModelDic.Add(timing, pinList);
                    }
                    else
                    {
                        timingModelDic.Add(timing, pinList);
                    }
                }

                foreach (var timingModel in timingModelDic)
                {
                    var timing = timingModel.Key;
                    var pinList = timingModel.Value;

                    var currentValue = System.Convert.ToInt32(value);
                    double.TryParse(timing.DriveA, out double driveA);
                    double.TryParse(timing.DriveB, out double driveB);
                    double.TryParse(timing.DriveC, out double driveC);
                    double.TryParse(timing.DriveD, out double driveD);

                    switch (edgeType)
                    {
                        case WaveformEdgeType.DriveA:
                            Digital.Pins(pinList).SetTimingDetail(timing.Period, currentValue, driveB, driveC, driveD, timing.Fmt, timing.StrobeMode, timing.StrobeA, timing.StrobeB);
                            break;
                        case WaveformEdgeType.DriveB:
                            Digital.Pins(pinList).SetTimingDetail(timing.Period, driveA, currentValue, driveC, driveD, timing.Fmt, timing.StrobeMode, timing.StrobeA, timing.StrobeB);
                            break;
                        case WaveformEdgeType.DriveC:
                            Digital.Pins(pinList).SetTimingDetail(timing.Period, driveA, driveB, currentValue, driveD, timing.Fmt, timing.StrobeMode, timing.StrobeA, timing.StrobeB);
                            break;
                        case WaveformEdgeType.DriveD:
                            Digital.Pins(pinList).SetTimingDetail(timing.Period, driveA, driveB, driveC, currentValue, timing.Fmt, timing.StrobeMode, timing.StrobeA, timing.StrobeB);
                            break;
                        case WaveformEdgeType.StrobeB:
                            Digital.Pins(pinList).SetTimingDetail(timing.Period, driveA, driveB, driveC, driveD, timing.Fmt, timing.StrobeMode, timing.StrobeB, currentValue);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        private bool VaildateAndSetDrive(List<ChannelModel> pins, double value, Func<string, double> getCompareValue, string compareField, string targetField, WaveformEdgeType edgeType)
        {
            bool result = true;
            var invalidPin = pins.FirstOrDefault(x => getCompareValue(x.PinName) < value);
            if (invalidPin != null)
            {
                PrintResultLog.Message(string.Format(L["ForceModeEmptyError"], invalidPin.PinName, compareField, targetField, value));
                result = false;
            }

            SetDigitalPinValue(pins, value, edgeType);
            return result;
        }
        #endregion

        private async Task ProcessAxis(ShmooAxis axis, int axisLoopCount, double num, bool isValidAxis1, List<ShmooResult> list, ActiveMode activeMode, string moduleName)
        {
            for (int j = 0; j <= axisLoopCount; j++)
            {
                double num2 = CalculateAxisValue(axis, j);

                if (!isValidAxis1)
                {
                    list.Add(ShmooResult.GetInvalidResult(num, num2));
                    continue;
                }

                bool isValidAxis2 = SetShmooType(axis, num2);
                if (!isValidAxis2)
                {
                    list.Add(ShmooResult.GetInvalidResult(num, num2));
                }
                else
                {
                    list.Add(ShmooPattern(activeMode, moduleName, num, num2));
                    if (axis.Delay != 0.0)
                    {
                        int delay = (int)axis.Delay * 1000;
                        await Task.Delay(delay);
                    }
                }
            }
        }

        private ShmooResult ShmooPattern(ActiveMode activeMode, string moduleName, double x, double y)
        {
            //todo:运行Pattern



            return null;
        }

        private void GetRawTimingAndLevelInfo(string timingName)
        {
            ShareMemoryInTestPlan<TestPlanModel> instance = ShareMemoryInTestPlan<TestPlanModel>.Instance;
            instance.Open();
            var testPlan = instance.ReadObject();

            var commonData = CommonData.Instance;
            var testItem = testPlan.TestItem.FirstOrDefault(x => x.TestItemName.Equals(commonData?.TestItemName));
            if (testItem?.Levels != null && testItem?.Levels.Any() == true)
            {
                foreach (var item in testItem.Levels)
                {
                    var pinList = GetPinList(item.PinGroupName);
                    if (pinList != null && pinList.Any())
                    {
                        foreach (var pin in pinList)
                        {
                            if (_levelDic.ContainsKey(pin.PinName))
                                _levelDic[pin.PinName] = item;
                            else
                                _levelDic.Add(pin.PinName, item);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(timingName))
                return;

            if (testItem?.Timings != null && testItem?.Timings.Any() == true)
            {
                var timings = testItem.Timings.Where(x => x.TimingName.ToLower().Equals(timingName.ToLower()));

                if (timings != null && timings.Any())
                    foreach (var item in testItem.Timings)
                    {
                        var pinList = GetPinList(item.PinName);
                        if (pinList != null && pinList.Any())
                        {
                            foreach (var pin in pinList)
                            {
                                if (_timeDic.ContainsKey(pin.PinName))
                                    _timeDic[pin.PinName] = item;
                                else
                                    _timeDic.Add(pin.PinName, item);
                            }
                        }
                    }
            }
        }

        public void SetPath(string path)
        {
            ShmooTestManagerHelper.Instance.TrySetPrint(_testName, path);
        }
    }
}
