using KSW.ATE01.Project.Base.Events;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Loggers;
using KSW.ATE01.Project.Base.Models.Results;
using System;
using System.Text;

namespace KSW.ATE01.Project.Base.Services.Loggers
{
    public class PrintResultLog : MarshalByRefObject
    {
        #region Fields
        private static readonly Lazy<PrintResultLog> _lazy = new Lazy<PrintResultLog>(() => new PrintResultLog());
        private static bool _printRealTimeTxt = false;
        private static IEventAggregator _eventAggregator;
        #endregion

        #region Properties
        public static bool PrintRealTimeTxt
        {
            get => _printRealTimeTxt;
            set => _printRealTimeTxt = value;
        }

        public static PrintResultLog Instance
        {
            get { return _lazy.Value; }
        }

        private static List<string> TestItemTitalList = new List<string>()
        {
            "TestName",
            "TestNumber",
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

        public PrintResultLog()
        {
            var container = ContainerLocator.Container;
            _eventAggregator = container?.Resolve<IEventAggregator>() ?? null;
        }

        public static void Message(string message)
        {
            LogHelper.WriteLog(message);
            //_eventAggregator.GetEvent<PrintToRealTimeTxtEvent>().Publish(new RealTimeMessage()
            //{
            //    Message = message
            //});
        }

        public static void Pattern(string pattern)
        {

        }

        public static void PrintErrorMessage(string errorMessage)
        {
            Message(errorMessage);
        }

        public static void PrintRealTimeWithTestItem(List<TestItemResultModel> results)
        {
            StringBuilder message = new StringBuilder();
            if (PrintRealTimeTxt && results != null && results.Count > 0)
            {
                string format = "{0,-17} {1,-11} {2,-34} {3,-11} {4,-11} {5,-15:G10} {6,-17:G12} {7,-15:G10} {8,-5} {9,-7}";
                message.Append(Environment.NewLine);
                message.Append(string.Format(format, TestItemTitalList.ToArray()));

                foreach (var item in results)
                {
                    message.Append(Environment.NewLine);
                    message.AppendLine(
                        string.Format(format,
                        item.TestItemName,
                        item.TestNumber,
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

                LogHelper.WriteLog(message.ToString());
            }
        }

        public static void PrintRealTimeTextWithSummary()
        {

        }
    }
}
