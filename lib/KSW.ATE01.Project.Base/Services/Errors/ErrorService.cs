using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Services.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Services.Errors
{
    public class ErrorService
    {
        #region Fields
        private static object _lock = new object();
        private static ErrorService _instance;
        private Action<ErrorMessage> _errorNotify;
        private static bool _isPrintToRealTimeTxt;
        #endregion

        #region Properties
        public static ErrorService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ErrorService();
                        }
                    }
                }
                return _instance;
            }
        }

        public event Action<ErrorMessage> ErrorNotify
        {
            add
            {
                Action<ErrorMessage> oldEvent = _errorNotify;
                Action<ErrorMessage> newEvent = null;
                do
                {
                    newEvent = oldEvent;
                    Action<ErrorMessage> value2 = (Action<ErrorMessage>)Delegate.Combine(newEvent, value);
                    oldEvent = Interlocked.CompareExchange<Action<ErrorMessage>>(ref _errorNotify, value2, newEvent);

                } while (oldEvent != newEvent);
            }
            remove
            {
                Action<ErrorMessage> oldEvent = _errorNotify;
                Action<ErrorMessage> newEvent = null;
                do
                {
                    newEvent = oldEvent;
                    Action<ErrorMessage> value2 = (Action<ErrorMessage>)Delegate.Combine(newEvent, value);
                    oldEvent = Interlocked.CompareExchange<Action<ErrorMessage>>(ref _errorNotify, value2, newEvent);

                } while (oldEvent != newEvent);
            }
        }
        #endregion

        public ErrorService()
        {

            _isPrintToRealTimeTxt = false;
        }

        private static void HandleMessage(ErrorMessage error, string message)
        {
            switch (error.Behavior)
            {
                case Enums.Errors.BehaviorType.None:
                    if (Instance._errorNotify == null)
                        return;
                    Instance._errorNotify(error);
                    break;
                case Enums.Errors.BehaviorType.Off:
                    break;
                case Enums.Errors.BehaviorType.Default:
                    PrintResultLog.Message(GetErrorMessage(error, message, OutputType.RealTimeTxt));
                    LogHelper.WriteLog(GetErrorMessage(error, message, OutputType.CommonLog));
                    if (Instance._errorNotify == null)
                        return;
                    Instance._errorNotify(error);
                    break;
                case Enums.Errors.BehaviorType.Continue:
                    if (_isPrintToRealTimeTxt)
                    {
                        PrintResultLog.Message(GetErrorMessage(error, message, OutputType.RealTimeTxt));
                    }
                    LogHelper.WriteLog(GetErrorMessage(error, message, OutputType.CommonLog));
                    if (Instance._errorNotify == null)
                        return;
                    Instance._errorNotify(error);
                    break;
                default:
                    break;
            }
        }

        public static string GetErrorMessage(ErrorMessage error, string message, OutputType outputType)
        {
            var result = string.Empty;
            if (outputType - OutputType.RealTimeTxt > 2)
            {
                if (outputType != OutputType.Exception)
                {
                    result = message;
                }
                else
                {
                    result = string.Concat(new string[]
                    {
                        "\n",
                        message,
                        "\nError Code:0x",
                        //errorMessage.Number.ToString("X"),
                        " Location:",
                        //errorMessage_0.Location
                    });
                }
            }
            else
            {
                result = message;
            }
            return result;
        }
        #region Public

        public static void SetPrintLogToRealTimeTxt(bool isPrint)
        {

        }

        public static string GetErrorInformation()
        {
            var result = string.Empty;

            try
            {

            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }

        public static void ThrowInternalError()
        {

        }

        
        #endregion
    }
}
