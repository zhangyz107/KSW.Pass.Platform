using KSW.ATE01.Project.Base.Enums.Loggers;
using KSW.ATE01.Project.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Services.Loggers
{
    public class DataLog
    {
        #region Fields
        private static bool _debugLogPrint;
        private static string _testStartTime;
        #endregion

        #region Properties
        public static bool DebugLogPrint
        {
            get => _debugLogPrint;
            set => _debugLogPrint = value;
        }

        public static string TestStartTime 
        { 
            get => _testStartTime;
            set => _testStartTime = value;
        }

        public static string CsvFilePath { get; set; } = string.Empty;

        public static string TxtFilePath { get; set; } = string.Empty;

        public static string SummaryFilePath { get; set; } = string.Empty;

        public static string StdfFilePath { get; set; } = string.Empty;
        #endregion

        public List<string> Print(DataLogFlag contentFlag)
        {
            var result = new List<string>();

            switch (contentFlag)
            {
                case DataLogFlag.HeaderOrStart:
                    if (DebugLogPrint)
                    {
                        LogHelper.WriteLog("=====================================================================================");
                        LogHelper.WriteLog("======================================  Start  ======================================");
                        LogHelper.WriteLog("=====================================================================================");
                        LogHelper.WriteLog($"Enter Datalog.Print() function case {DataLogFlag.HeaderOrStart}.");
                    }
                    try
                    {
                        TestStartTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        if (DebugLogPrint)
                        {
                            LogHelper.WriteLog(string.Concat(new string[]
                            {
                                "Get csv/txt/stdf/summary files name from meshead successful.",
                                Environment.NewLine,
                                "\t\t\t\t\t\t\t\t\t\tCSVFileName:",
                                CsvFilePath,
                                Environment.NewLine,
                                "\t\t\t\t\t\t\t\t\t\tTXTFileName:",
                                TxtFilePath,
                                Environment.NewLine,
                                "\t\t\t\t\t\t\t\t\t\tSTDFFileName:",
                                StdfFilePath,
                                Environment.NewLine,
                                "\t\t\t\t\t\t\t\t\t\tSummaryFileName:",
                                SummaryFilePath
                            }));
                        }

                        if (DebugLogPrint)
                        {
                            LogHelper.WriteLog("Datalog error flag 'IsDataLogHasErrorOrException' has been set to false.");
                        }
                    }
                    catch (Exception ex)
                    {
                        PrintExceptionToLog(ex);


                        if (DebugLogPrint)
                        {
                            LogHelper.WriteLog($"Execute function Print case {DataLogFlag.HeaderOrStart} in DataLog occured exception, exception message is {ex.Message}");
                        }
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public static void PrintExceptionToLog(Exception ex)
        {
            string @namespace = ex.TargetSite.ReflectedType.Namespace;
            string name = ex.TargetSite.Name;
            LogHelper.WriteError(string.Format("\t{0}.{1} \n\tError Message:\t{2}\n\tMethod:\t{3}", new object[]
            {
                @namespace,
                name,
                ex.Message,
                ex.TargetSite
            }));
        }
    }
}
