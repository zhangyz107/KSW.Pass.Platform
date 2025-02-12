using KSW.ATE01.Project.Base.Models.Errors;
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
        private bool _isPrintToRealTimeTxt;
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

        private static void HandleMessage(ErrorMessage message)
        {
            switch (message.Behavior)
            {
                case Enums.Errors.BehaviorType.None:
                    break;
                case Enums.Errors.BehaviorType.Off:
                    break;
                case Enums.Errors.BehaviorType.Default:
                    break;
                case Enums.Errors.BehaviorType.Continue:
                    break;
                default:
                    break;
            }
        }

        #region Public

        public static void SetPrintLogToRealTimeTxt(bool isPrint)
        {

        }

        #endregion
        public string GetErrorInformation()
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

        public void ThrowInternalError()
        {

        }
    }
}
