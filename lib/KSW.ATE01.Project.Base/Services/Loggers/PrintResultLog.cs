using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Loggers;
using KSW.ATE01.Project.Base.Services.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Services.Loggers
{
    public class PrintResultLog
    {
        #region Fields
        private static object _lock = new object();
        private static PrintResultLog _instance;
        private static bool _printRealTimeTxt = false;
        private Action<LogMessage> _logNotigy = null;
        #endregion

        #region Properties
        public static bool PrintRealTimeTxt
        {
            get => _printRealTimeTxt;
            set => _printRealTimeTxt = value;
        }

        public static PrintResultLog Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PrintResultLog();
                        }
                    }
                }
                return _instance;
            }
        }

        public event Action<LogMessage> LogNotigy
        {
            add
            {
                Action<LogMessage> oldEvent = _logNotigy;
                Action<LogMessage> newEvent = null;
                do
                {
                    newEvent = oldEvent;
                    Action<LogMessage> value2 = (Action<LogMessage>)Delegate.Combine(newEvent, value);
                    oldEvent = Interlocked.CompareExchange<Action<LogMessage>>(ref _logNotigy, value2, newEvent);

                } while (oldEvent != newEvent);
            }
            remove
            {
                Action<LogMessage> oldEvent = _logNotigy;
                Action<LogMessage> newEvent = null;
                do
                {
                    newEvent = oldEvent;
                    Action<LogMessage> value2 = (Action<LogMessage>)Delegate.Combine(newEvent, value);
                    oldEvent = Interlocked.CompareExchange<Action<LogMessage>>(ref _logNotigy, value2, newEvent);

                } while (oldEvent != newEvent);
            }
        }
        #endregion

        public static void Message(string message)
        {
            if (Instance._logNotigy != null)
            {
                Instance._logNotigy(new LogMessage()
                {
                    Message = message
                });
            }
        }

        public static void Pattern(string pattern)
        {

        }

        public static void PrintErrorMessage(string errorMessage)
        {
            Message(errorMessage);
        }

        public static void PrintRealTimeWithTestItem()
        {
            if (PrintRealTimeTxt && Instance._logNotigy != null)
            {

            }
        }

        public static void PrintRealTimeTextWithSummary()
        {
            if (PrintRealTimeTxt && Instance._logNotigy != null)
            {

            }
        }
    }
}
