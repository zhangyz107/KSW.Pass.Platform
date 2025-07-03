using KSW.ATE01.Instrument.IO.Enums.Results;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Enums.Results;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Language;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Results;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.ATE01.Project.Base.Services.Loggers;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Results
{
    public class Result
    {
        private static LanguageManager L => LanguageManager.Instance;

        public static bool LimitError { get; set; }
        public static List<string> LimitErrorMessage { get; private set; } = new List<string>();

        public static void TestLimit<T>(IEnumerable<IChannelResultModel<T>> resultValue, bool forceCustomerProgramLimit, uint testNumber = 0, double limitLow = double.NaN, double limitHigh = double.NaN, string limitName = "", string unit = "", uint failHardBin = 0, uint failSoftBin = 0, uint passHardBin = 0, uint passSoftBin = 0, string dutResult = "")
        {
            var commonData = CommonData.Instance;
            var result = new List<TestItemResultModel>();
            var testItemLimit = commonData.TestItemLimit;
            LimitErrorMessage.Clear();

            if (testItemLimit != null)
            {
                if (!forceCustomerProgramLimit || LimitDataValid(testItemLimit, testNumber, limitLow, limitHigh, unit, limitName, failHardBin, failSoftBin, passHardBin, passSoftBin, dutResult))
                {
                    var type = typeof(T);
                    if (type == typeof(double) || type == typeof(decimal))
                    {
                        result = GetTestItemResult(resultValue, testItemLimit);
                    }
                }
                else if (LimitErrorMessage.Count > 0)
                {
                    var errorMessage = string.Join(Environment.NewLine, LimitErrorMessage);
                    LogHelper.WriteError(errorMessage);
                }
            }
        }

        private static bool LimitDataValid(LimitsModel testItemLimit, uint testNumber, double limitLow, double limitHigh, string unit, string limitName, uint failHardBin, uint failSoftBin, uint passHardBin, uint passSoftBin, string dutResult)
        {
            bool result = true;

            if (testItemLimit == null)
                result = false;

            if (!double.IsNaN(limitLow) && !double.IsNaN(limitHigh))
            {
                if (limitLow > limitHigh)
                {
                    if (!LimitErrorMessage.Contains(L["LimitValueError"]))
                        LimitErrorMessage.Add(L["LimitValueError"]);
                }
                else
                {
                    testItemLimit.LowLimit = limitLow;
                    testItemLimit.HighLimit = limitHigh;
                }
            }
            else
            {
                if (!LimitErrorMessage.Contains(L["LimitValueSetError"]))
                    LimitErrorMessage.Add(L["LimitValueSetError"]);
            }

            if (!string.IsNullOrEmpty(limitName))
                testItemLimit.LimitName = limitName;

            if (testNumber != 0)
                testItemLimit.TestNumber = testNumber;

            if (!string.IsNullOrEmpty(unit))
                testItemLimit.Units = unit;

            if (failHardBin != 0)
                testItemLimit.FailHardwareBin = failHardBin;

            if (failSoftBin != 0)
                testItemLimit.FailSoftwareBin = failSoftBin;

            if (passHardBin != 0)
                testItemLimit.PassHardwareBin = passHardBin;

            if (passSoftBin != 0)
                testItemLimit.PassSoftwareBin = passSoftBin;

            if (!string.IsNullOrEmpty(dutResult) && Enum.TryParse(dutResult, out Test dutResultValue))
                testItemLimit.DUTResult = dutResultValue;

            return result;
        }

        private static List<TestItemResultModel> GetTestItemResult<T>(IEnumerable<IChannelResultModel<T>> resultValue, LimitsModel testItemLimit)
        {
            var result = new List<TestItemResultModel>();
            if (resultValue == null || resultValue.Count() <= 0)
                return result;

            var groupResultValues = resultValue.GroupBy(x => x.PinName);
            uint index = 0;
            foreach (var groupResultValue in groupResultValues)
            {
                foreach (var value in groupResultValue)
                {
                    var tempItemResult = new TestItemResultModel();
                    tempItemResult.TestItemName = testItemLimit.TestItemName;
                    tempItemResult.LimitName = testItemLimit.LimitName;
                    tempItemResult.TestNumber = testItemLimit.TestNumber * 10 + index;
                    tempItemResult.LowLimit = testItemLimit.LowLimit;
                    tempItemResult.HighLimit = testItemLimit.HighLimit;
                    tempItemResult.Units = testItemLimit.Units;
                    tempItemResult.TestValue = value.SiteResult is double ? (double)(value.SiteResult as double?) : double.NaN;
                    tempItemResult.FailHardwareBin = testItemLimit.FailHardwareBin;
                    tempItemResult.FailSoftwareBin = testItemLimit.FailSoftwareBin;
                    tempItemResult.PassHardwareBin = testItemLimit.PassHardwareBin;
                    tempItemResult.PassSoftwareBin = testItemLimit.PassSoftwareBin;
                    tempItemResult.PinName = value.PinName;
                    tempItemResult.ChannelName = value.Site;
                    tempItemResult.DUTResult = CompareTestItemValue(tempItemResult);
                    if (LimitErrorMessage.Count > 0)
                    {
                        var errorMessage = string.Join(Environment.NewLine, LimitErrorMessage);
                        tempItemResult.Log  = errorMessage;
                    }
                    result.Add(tempItemResult);
                }
                index++;
            }

            PrintResultLog.PrintRealTimeWithTestItem(result);

            return result;
        }

        private static Test CompareTestItemValue(TestItemResultModel tempItemResult)
        {
            var lowlimit = tempItemResult.LowLimit;
            var highlimit = tempItemResult.HighLimit;
            var testValue = tempItemResult.TestValue;
            if ((testValue > lowlimit || Math.Abs(testValue - lowlimit) < 1e-12) && (testValue < highlimit || Math.Abs(testValue - highlimit) < 1e-12))
                return Test.Pass;

            return Test.Fail;
        }

        public static void TestLimit<T>(IEnumerable<IChannelResultModel<T>> resultValue, double lowLimit, double highLimit, string limitName)
        {
            TestLimit(resultValue, true, 0, lowLimit, highLimit, limitName, "", 0, 0, 0, 0, "");
        }

        public static void TestLimit<T>(IEnumerable<IChannelResultModel<T>> resultValue, int failHardBin = 0, int failSoftBin = 0, int passHardBin = 0, string passSoftBin = "", string strResult = "", double lowVal = 0.0, double hiVal = 0.0, CompareSign lowCompareSign = CompareSign.SignGreaterEqual, CompareSign highCompareSign = CompareSign.SignGreaterEqual, ScaleType scaletype = ScaleType.ScaleNone, UnitType unit = UnitType.UnitNone, string formatStr = "%6.4f", string TName = "", LimitCompareType compareMode = LimitCompareType.CompareAverage, string PinName = "", double forceVal = 0.0, UnitType forceunit = UnitType.UnitNone, string customUnit = "", string customForceunit = "", LimitForceResults ForceResults = LimitForceResults.ForceNone, long TNum = 0L)
        {
            Result.TestLimit(resultValue, false, 0U, double.NaN, double.NaN, "", "", 0U, 0U, 0U, 0U, "");
        }
    }
}
