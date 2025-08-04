using KSW.ATE01.Instrument.IO.BLLs.Abstractions.DataLogs;
using KSW.ATE01.Instrument.IO.Enums.DataLogs;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Language;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Results;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.DataLogs
{
    /// <summary>
    /// 数据日志
    /// </summary>
    public class DataLog : MarshalByRefObject, IDataLog
    {
        #region Private
        private static LanguageManager L => LanguageManager.Instance;
        private static bool isDataLogHasErrorOrException;
        internal static long csvCollectTestItemDeviceCount = 100L;
        private List<string> _testItemTitalList = new List<string>()
        {
            "TestName",
            "TestNumber",
            "Site",
            "LimitName",
            "Pin",
            "Channel",
            "Low",
            "Measured",
            "High",
            "Unit",
            "Result"
        };

        #endregion

        #region Public
        internal static long CSVCollectTestItemDeviceCount
        {
            get => csvCollectTestItemDeviceCount;
            set => csvCollectTestItemDeviceCount = value;
        }

        protected IEnumerable<TestItemResultModel> _testItemResult { get; private set; }

        public static bool IsDataLogHasErrorOrException
        {
            get
            {
                return isDataLogHasErrorOrException;
            }
            set
            {
                isDataLogHasErrorOrException = value;
            }
        }
        #endregion


        public DataLog(IEnumerable<TestItemResultModel> result)
        {
            _testItemResult = result;
        }

        public static IDataLog Result(List<TestItemResultModel> result)
        {
            return new DataLog(result);
        }

        public void PrintTestItemResult(string directoryPath = null)
        {
            var globalSetting = GlobalSetting.Instance;
            var projectInfo = globalSetting.ProjectInfo;
            var directory = projectInfo?.DatalogPath;
            DataLog.IsDataLogHasErrorOrException = false;
            if (!string.IsNullOrEmpty(directoryPath) && Directory.Exists(directoryPath))
                directory = directoryPath;

            if (string.IsNullOrEmpty(directory))
                LogHelper.WriteError(L["SaveDirectoryPathError"]);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            if (_testItemResult != null && _testItemResult.Any())
            {
                if (projectInfo?.SaveRealTimeText == true && PrintRealTimeText(directory))      //保存TxT
                    LogHelper.WriteLog(L["SaveRealTimeTxtSuccessful"]);

                if (projectInfo?.SaveCsv == true)               //保存Csv
                    PrintCsv(directory);

                if (projectInfo?.SaveSTDF == true)               //保存STDF
                    PrintSTDF(directory);

                if (projectInfo?.SaveSummary == true)           //保存Summary
                    PrintSummary(directory);
            }
        }

        private bool PrintRealTimeText(string? directory)
        {
            var flag = false;
            string txtSavePath = GetSavePath(directory, DataLogType.Txt);
            StringBuilder message = new StringBuilder();
            if (_testItemResult != null && _testItemResult.Any())
            {
                string format = "{0,-17} {1,-11} {2,-5} {3,-34} {4,-11} {5,-11} {6,-15:G10} {7,-17:G12} {8,-15:G10} {9,-5} {10,-7}";
                message.Append(Environment.NewLine);
                message.Append(string.Format(format, _testItemTitalList.ToArray()));

                foreach (var item in _testItemResult)
                {
                    message.Append(Environment.NewLine);
                    message.AppendLine(
                        string.Format(format,
                        item.TestItemName,
                        item.TestNumber,
                        item.Site,
                        item.LimitName,
                        item.PinName,
                        item.ChannelName,
                        Math.Round(item.LowLimit, 8),
                        Math.Round(item.TestValue, 8),
                        Math.Round(item.HighLimit, 8),
                        item.Units,
                        item.DUTResult));
                    if (!string.IsNullOrEmpty(item.Log))
                    {
                        var logMessages = item.Log.Split('\n');
                        foreach (var logMessage in logMessages)
                        {
                            message.AppendLine(logMessage);
                        }
                    }
                }

                LogHelper.WriteLog(message.ToString(), txtSavePath);
                flag = true;
            }

            return flag;
        }

        private void PrintCsv(string? directory)
        {
            var flag = false;
            string savePath = GetSavePath(directory, DataLogType.Csv);
            var serialNumber = 0;
            var list = new List<string>();

            try
            {
                //创建标题
                CreateCSVHeader(ref list);

                if (_testItemResult != null && _testItemResult.Any())
                {
                    var groupResultValues = _testItemResult.GroupBy(x => x.Site).OrderBy(y => y.Key);
                    foreach (var groupRowData in groupResultValues)
                    {
                        string[] itemRowData = new string[8];
                        itemRowData[0] = GlobalSetting.Instance.StartTestTime.ToString("yyyy-MM-dd HH:mm:ss:ffff");
                        itemRowData[1] = Results.Result.TestTime.ToString();
                        itemRowData[2] = groupRowData.Key.ToString();
                        itemRowData[3] = (++serialNumber).ToString();
                        //itemRowData[4] = ;
                        //itemRowData[5] = item.PassSoftwareBin.ToString();
                        //itemRowData[6] = item.XCoordinate;
                        //itemRowData[7] = item.YCoordinate;
                        var testValueList = new List<string>();
                        var orderRowData = groupRowData.OrderBy(x => x.PinName);
                        foreach (var testItemResult in orderRowData)
                        {
                            testValueList.Add(Math.Round(testItemResult.TestValue, 8).ToString());
#if DEBUG
                            LogHelper.WriteLog($"Current Row Site:{groupRowData.Key},Current PinName:{testItemResult.PinName},Current TestValue:{testItemResult.TestValue}");
#endif
                        }

                        list.Add(string.Join(",", itemRowData) + "," + string.Join(",", testValueList));
                    }

                    PrintToFile(savePath, list);
                }
            }
            catch (Exception ex)
            {
                Type reflectedType = MethodBase.GetCurrentMethod().ReflectedType;
                string name = MethodBase.GetCurrentMethod().Name;
                LogHelper.WriteError($"\t{reflectedType}.{name} \n\tError Message:\t{ex.Message}\n\tMethod:\t{ex.TargetSite}");
                IsDataLogHasErrorOrException = true;
            }
        }

        private void CreateCSVHeader(ref List<string> list)
        {
            string text = "LimitName,,,,,,,,";
            string text2 = "TestNumber,,,,,,,,";
            string text3 = "LowerLimit,,,,,,,,";
            string text4 = "UpperLimit,,,,,,,,";
            string text5 = "Unit,,,,,,,,";
            string itemHeader = "OP test date,test time(ms),Site #,Serial #,HardBin,SoftBin,XCoord,YCoord";

            var groupResultValues = _testItemResult.GroupBy(x => x.PinName).OrderBy(x => x.Key);
            foreach (var group in groupResultValues)
            {
                var defaultItem = group.FirstOrDefault();
                text += $"{defaultItem.PinName}_{defaultItem.LimitName},";
                text2 += $"{defaultItem.TestNumber},";
                text3 += $"{defaultItem.LowLimit},";
                text4 += $"{defaultItem.HighLimit},";
                text5 += $"{defaultItem.Units},";
            }
            list.Add(text);
            list.Add(text2);
            list.Add(text3);
            list.Add(text4);
            list.Add(text5);
            list.Add(itemHeader);
        }

        private void PrintToFile(string savePath, List<string> content)
        {
            bool flag = true;
            string str = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(savePath, true, Encoding.Default))
                    {
                        for (int i = 0; i < content.Count; i++)
                        {
                            streamWriter.WriteLine(content[i]);
                        }
                    }
                    flag = true;
                    break;
                }
                catch (Exception ex)
                {
                    str = ex.Message;
                    flag = false;
                    Thread.Sleep(1);
                }
            }
            while (flag || stopwatch.ElapsedMilliseconds <= 3000L || MessageBox.Show("CSV 打印异常，异常消息 " + str + " \n 按 是 继续等待打印完成或者按 否 退出打印报告异常", "CSV 打印异常", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
            if (!flag)
            {
                IsDataLogHasErrorOrException = true;
            }
        }

        private void PrintSTDF(string? directory)
        {
            throw new NotImplementedException();
        }

        private void PrintSummary(string? directory)
        {
            throw new NotImplementedException();
        }

        private string GetSavePath(string? directory, DataLogType dataLogType)
        {
            var globalSetting = GlobalSetting.Instance;
            var projectInfo = globalSetting?.ProjectInfo;
            var startTestTime = globalSetting?.StartTestTime ?? DateTime.Now;
            var testItemName = _testItemResult.FirstOrDefault()?.TestItemName;
            var extension = ".txt";
            switch (dataLogType)
            {
                case DataLogType.Txt:
                    extension = ".txt";
                    break;
                case DataLogType.Csv:
                    extension = ".csv";
                    break;
                case DataLogType.STDF:
                    extension = ".stdf";
                    break;
                case DataLogType.Summary:
                    extension = ".sum";
                    break;
                default:
                    break;
            }
            var txtSavePath = Path.Combine(directory, $"{projectInfo?.ProjectName}_{startTestTime.ToString("yyyyMMdd_HHmmss")}{extension}");
            if (!string.IsNullOrEmpty(testItemName))
                txtSavePath = Path.Combine(directory, $"{projectInfo?.ProjectName}_{testItemName}_{startTestTime.ToString("yyyyMMdd_HHmmss")}{extension}");
            return txtSavePath;
        }
    }
}
