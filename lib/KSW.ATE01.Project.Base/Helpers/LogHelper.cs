using KSW.ATE01.Project.Base.Enums.Loggers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    public class LogHelper
    {
        #region Fields
        private static ConcurrentQueue<Tuple<LogType, string, string>> _logQueue = new ConcurrentQueue<Tuple<LogType, string, string>>();
        private static Thread _dequeueThread = ThreadInitial();
        private static string _errorMessageLogPath = "C:\\ATE01\\Log\\SiteErrorMessage.txt";
        private static string _logMessagePath = "C:\\ATE01\\Log\\Log.txt";
        #endregion

        #region Properties
        public static string ErrorMessageLogPath
        {
            get
            {
                if (!Directory.Exists(Path.GetDirectoryName(_errorMessageLogPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_errorMessageLogPath));
                }
                return _errorMessageLogPath;
            }
            set
            {
                _errorMessageLogPath = value;
                if (!Directory.Exists(Path.GetDirectoryName(_errorMessageLogPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_errorMessageLogPath));
                }
            }
        }

        public static string InfoMessageLogPath
        {
            get
            {
                if (!Directory.Exists(Path.GetDirectoryName(_logMessagePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_logMessagePath));
                }
                return _logMessagePath;
            }
            set
            {
                _logMessagePath = value;
                if (!Directory.Exists(Path.GetDirectoryName(_logMessagePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_logMessagePath));
                }
            }
        }
        #endregion

        private static Thread ThreadInitial()
        {
            Thread thread = new Thread(new ThreadStart(DequeueToDoLoop));
            thread.IsBackground = true;
            thread.Start();
            return thread;
        }

        private static void DequeueToDoLoop()
        {
            while (true)
            {
                if (_logQueue.Count > 0)
                {
                    if (_logQueue.TryDequeue(out Tuple<LogType, string, string> tuple))
                    {
                        switch (tuple.Item1)
                        {
                            case LogType.Log:
                                WriteLogPrivate(tuple.Item2);
                                break;
                            case LogType.LogWithPath:
                                WriteLogPrivate(tuple.Item2, tuple.Item3);
                                break;
                            case LogType.Error:
                                WriteErrorPrivate(tuple.Item2);
                                break;
                            case LogType.ErrorWithPath:
                                WriteErrorPrivate(tuple.Item2, tuple.Item3);
                                break;
                            case LogType.Clear:
                                ClearErrorLogPrivate();
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public static void WriteError(string errorLog)
        {
            _logQueue.Enqueue(new Tuple<LogType, string, string>(LogType.Error, errorLog, string.Empty));
        }

        public static void WriteError(string errorLog, string errorLogPath)
        {
            _logQueue.Enqueue(new Tuple<LogType, string, string>(LogType.ErrorWithPath, errorLog, errorLogPath));
        }

        public static void WriteLog(string log)
        {
            _logQueue.Enqueue(new Tuple<LogType, string, string>(LogType.Log, log, string.Empty));
        }

        public static void WriteLog(string log, string logPath)
        {
            _logQueue.Enqueue(new Tuple<LogType, string, string>(LogType.LogWithPath, log, logPath));
        }

        public static void ClearErrorLog()
        {
            _logQueue.Enqueue(new Tuple<LogType, string, string>(LogType.Clear, string.Empty, string.Empty));
        }

        private static void WriteErrorPrivate(string errorLog)
        {
            using (StreamWriter streamWriter = new StreamWriter(ErrorMessageLogPath, true))
            {
                streamWriter.Write(string.Format("{0} : {1}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), errorLog));
            }
        }

        private static void WriteErrorPrivate(string errorLog, string errorLogPath)
        {
            using (StreamWriter streamWriter = new StreamWriter(GetErrorLogPath(errorLogPath), true))
            {
                streamWriter.Write(string.Format("{0} : {1}\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), errorLog));
            }
        }

        private static void WriteLogPrivate(string log)
        {
            using (StreamWriter streamWriter = new StreamWriter(InfoMessageLogPath, true))
            {
                streamWriter.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "," + log + "\n");
            }
        }

        private static void WriteLogPrivate(string log, string logPath)
        {
            using (StreamWriter streamWriter = new StreamWriter(GetLogPath(logPath), true))
            {
                streamWriter.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "," + log + "\n");
            }
        }

        private static void ClearErrorLogPrivate()
        {
            using (StreamWriter streamWriter = new StreamWriter(LogHelper.ErrorMessageLogPath))
            {
                streamWriter.Write(string.Format("{0} : Clear alle error log\n", DateTime.Now.ToString("YYYY-MM-DD HH:mm:ss:fff")));
            }
        }

        private static string GetErrorLogPath(string inputErrorLogPath)
        {
            if (string.IsNullOrWhiteSpace(inputErrorLogPath))
            {
                return ErrorMessageLogPath;
            }
            if (!Directory.Exists(Path.GetDirectoryName(inputErrorLogPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(inputErrorLogPath));
            }
            return inputErrorLogPath;
        }

        private static string GetLogPath(string inputLogPath)
        {
            if (string.IsNullOrWhiteSpace(inputLogPath))
            {
                return InfoMessageLogPath;
            }
            if (!Directory.Exists(Path.GetDirectoryName(inputLogPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(inputLogPath));
            }
            return inputLogPath;
        }
    }
}
