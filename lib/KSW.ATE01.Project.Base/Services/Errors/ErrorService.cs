using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Events;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.Exceptions;
using KSW.ATE01.Project.Base.Models.Loggers;

namespace KSW.ATE01.Project.Base.Services.Errors
{
    public class ErrorService
    {
        #region Fields
        private static readonly Lazy<ErrorService> _lazy = new Lazy<ErrorService>();
        private static bool _isPrintToRealTimeTxt;
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


        private static RealTimeMessage GetRealTimeTxtMessage(ErrorMessage error, string message, OutputType realTimeTxt)
        {
            var result = new RealTimeMessage();
            {
                message = GetErrorMessage(error, message, realTimeTxt);
            }
            return result;
        }
        #region Public

        public static void SetPrintLogToRealTimeTxt(bool isPrint)
        {
            _isPrintToRealTimeTxt = isPrint;
        }

        public void ThrowError(ErrorAgent agent, uint number, Exception inner, string location, params object[] parameters)
        {
            ErrorHandle(agent, number, inner, location, parameters);
        }

        public string GetErrorInformation(ErrorAgent agent, uint number, out bool isExist, string location, params object[] parameters)
        {
            isExist = false;
            var result = string.Empty;
            ErrorAgentCollection.Instance.TryAddAgent(agent);
            var errorInfo = agent.TryGetErrorInfo(number);
            isExist = errorInfo != null;
            if (errorInfo == null)
            {
                return "Information with number 0x" + number.ToString("X") + " was not found.";
            }
            var errorMessageByInfo = ErrorAgentCollection.Instance.GetErrorMessageByInfo(errorInfo, null, location, true, parameters);

            try
            {

            }
            catch (Exception ex)
            {
                result = "Failed to obtain the message. Number:0x" + number.ToString("X") + ". Error information:" + ex.Message;
            }

            return result;
        }

        public void ThrowInternalError(ErrorAgent client, uint number, Exception inner, string location, params object[] parameters)
        {
            ErrorHandle(client, number, inner, location, parameters);
        }

        private void ErrorHandle(ErrorAgent agent, uint number, Exception exception, string location, object[] parameters)
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
                ErrorAgentCollection.Instance.TryAddAgent(agent);
                var errorInfo = agent.TryGetErrorInfo(number);
                if (errorInfo.Behavior == BehaviorType.Off)
                    return;

                var errorMessageByInfo = ErrorAgentCollection.Instance.GetErrorMessageByInfo(errorInfo, null, location, false, parameters);
                errorMessageByInfo.ModuleName = agent.Source;
                try
                {
                    HandleMessage(errorMessageByInfo, "");
                }
                catch (Exception ex)
                {
                    throw new ATEException(ex.Message);
                }
            }
        }

        private static void HandleMessage(ErrorMessage error, string message)
        {
            switch (error.Behavior)
            {
                case BehaviorType.None:
                    _eventAggregator?.GetEvent<ErrorNotifyEvent>().Publish(error);
                    break;
                case BehaviorType.Off:
                    break;
                case BehaviorType.Default:
                    _eventAggregator?.GetEvent<PrintToRealTimeTxtEvent>().Publish(GetRealTimeTxtMessage(error, message, OutputType.RealTimeTxt));
                    LogHelper.WriteLog(GetErrorMessage(error, message, OutputType.CommonLog));
                    _eventAggregator?.GetEvent<ErrorNotifyEvent>().Publish(error);
                    break;
                case BehaviorType.Continue:
                    if (_isPrintToRealTimeTxt)
                        _eventAggregator?.GetEvent<PrintToRealTimeTxtEvent>().Publish(GetRealTimeTxtMessage(error, message, OutputType.RealTimeTxt));
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
                        "\nError Code:0x",
                        error.Number.ToString("X"),
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
