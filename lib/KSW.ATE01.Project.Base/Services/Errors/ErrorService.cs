using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Events;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Exceptions;
using KSW.ATE01.Project.Base.Models.Loggers;
using KSW.ATE01.Project.Base.Services.Loggers;

namespace KSW.ATE01.Project.Base.Services.Errors
{
    public class ErrorService
    {
        #region Fields
        private static readonly Lazy<ErrorService> _lazy = new Lazy<ErrorService>(() => new ErrorService());
        private static bool _isPrintToRealTimeTxt;
        private static bool _errorFlag;
        private static IEventAggregator _eventAggregator;
        #endregion

        #region Properties
        public static ErrorService Instance
        {
            get { return _lazy.Value; }
        }
        #endregion

        public ErrorService()
        {
            var container = ContainerLocator.Container;
            _eventAggregator = container?.Resolve<IEventAggregator>() ?? null;
            _isPrintToRealTimeTxt = false;
        }
        #region Public

        public static void SetPrintLogToRealTimeTxt(bool isPrint)
        {
            _isPrintToRealTimeTxt = isPrint;
        }

        public void ThrowError(ErrorInfo errorInfo, Exception inner, string location)
        {
            ErrorHandle(errorInfo, inner, location);
        }

        public static void AutoResetErrorFlag()
        {
            _errorFlag = false;
            Message.ErrorStatus = ErrorStatus.Normal;
        }

        public static void SetErrorFlag()
        {
            _errorFlag = true;
        }

        public void ThrowInternalError(ErrorInfo errorInfo, Exception inner, string location)
        {
            ErrorHandle(errorInfo, inner, location);
        }

        private void ErrorHandle(ErrorInfo errorInfo, Exception exception, string location)
        {
            if (exception != null)
            {
                if (exception is HardwareErrorException hwErrorException)
                {
                    HandleHardwareException(hwErrorException);
                }
                throw new ATEException(exception?.Message);
            }
            else
            {
                if (errorInfo.Behavior == BehaviorType.Off)
                    return;

                var errorMessageByInfo = GetErrorMessageByInfo(errorInfo, null, location);
                errorMessageByInfo.ModuleName = errorInfo.ModuleName;
                try
                {
                    HandleMessage(errorMessageByInfo, errorInfo.Message);
                }
                catch (Exception ex)
                {
                    throw new ATEException(ex.Message);
                }
            }
        }

        private ErrorMessage GetErrorMessageByInfo(ErrorInfo errorInfo, Exception exception, string location)
        {
            return new ErrorMessage()
            {
                ModuleName = errorInfo.ModuleName,
                AlarmFlag = errorInfo.IsAlarm,
                Behavior = errorInfo.Behavior,
                ErrorName = errorInfo.ErrorName,
                Exception = exception,
                Message = errorInfo.Message,
                Location = location,
            };
        }

        private static void HandleMessage(ErrorMessage error, string message)
        {
            switch (error.Behavior)
            {
                case BehaviorType.None:
                    _eventAggregator?.GetEvent<ErrorNotifyEvent>().Publish(error);
                    break;
                case BehaviorType.ForceFail:
                case BehaviorType.ForceHalt:
                    _eventAggregator?.GetEvent<ErrorNotifyEvent>().Publish(error);
                    if (_errorFlag)
                    {
                        Message.ErrorStatus = ErrorStatus.Error;
                    }
                    throw new ATEException(GetErrorMessage(error, message, OutputType.Exception));
                case BehaviorType.ForceBin:
                    _eventAggregator?.GetEvent<PrintToRealTimeTxtEvent>().Publish(new RealTimeMessage()
                    {
                        Message = message,
                    });
                    LogHelper.WriteLog(message);
                    if (_errorFlag)
                    {
                        Message.ErrorStatus = ErrorStatus.Error;
                        Message.ErrorMessageList.Add(message);
                    }
                    _eventAggregator?.GetEvent<ErrorNotifyEvent>().Publish(error);
                    break;
                case BehaviorType.Off:
                    break;
                case BehaviorType.Default:
                    PrintResultLog.Message(GetErrorMessage(error, message, OutputType.RealTimeTxt));
                    LogHelper.WriteLog(GetErrorMessage(error, message, OutputType.CommonLog));
                    _eventAggregator?.GetEvent<ErrorNotifyEvent>().Publish(error);
                    break;
                case BehaviorType.Continue:
                    if (_isPrintToRealTimeTxt)
                        PrintResultLog.Message(GetErrorMessage(error, message, OutputType.RealTimeTxt));
                    LogHelper.WriteLog(GetErrorMessage(error, message, OutputType.CommonLog));
                    _eventAggregator?.GetEvent<ErrorNotifyEvent>().Publish(error);
                    break;
                default:
                    break;
            }
        }

        private static string GetErrorMessage(ErrorMessage error, string message, OutputType outputType)
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
                        "\nError Name:",
                        error.ErrorName,
                        " Location:",
                        error.Location
                    });
                }
            }
            else
            {
                result = message;
            }
            return result;
        }

        private static void HandleHardwareException(HardwareErrorException exception)
        {
            var text = string.Concat(new object[]
            {
                "Chassis:",
                exception.Chassis,
                ",Slot:",
                exception.Slot,
                ",Channel:",
                exception.Channel.Channel
            });

            _eventAggregator?.GetEvent<PrintToRealTimeTxtEvent>().Publish(new RealTimeMessage()
            {
                Message = text
            });
            LogHelper.WriteError(text);
        }


        #endregion
    }
}
