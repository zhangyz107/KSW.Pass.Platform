using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Helpers;
using System.Windows;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    public class Message
    {
        private static string _moduleName;
        private static string _functionName;
        private static string _lastErrorMessage;
        private static ErrorStatus _initializeStatus;
        private static ErrorStatus _errorStatus;
        private static TesterMode _testerMode;
        private static List<string> _errorMessageList = new List<string>();

        /// <summary>
        /// 测试模式
        /// </summary>
        public static TesterMode TesterMode { get => _testerMode; set => _testerMode = value; }

        /// <summary>
        /// 模块名
        /// </summary>
        public static string ModuleName { get => _moduleName; set => _moduleName = value; }

        /// <summary>
        /// 方法名
        /// </summary>
        public static string FunctionName { get => _functionName; set => _functionName = value; }

        /// <summary>
        /// 初始化状态
        /// </summary>
        public static ErrorStatus InitializeStatus { get => _initializeStatus; set => _initializeStatus = value; }

        /// <summary>
        /// 错误状态
        /// </summary>
        public static ErrorStatus ErrorStatus { get => _errorStatus; set => _errorStatus = value; }

        /// <summary>
        /// 最后的错误信息
        /// </summary>
        public static string LastErrorMessage { get => _lastErrorMessage; set => _lastErrorMessage = value; }

        /// <summary>
        /// 错误信息集合
        /// </summary>
        public static List<string> ErrorMessageList { get => _errorMessageList; set => _errorMessageList = value; }

        public static void ErrorMessage(string errorMessage, string errorMessageSummary = "Exception", bool initializedErrorType = false)
        {
            string text = string.Concat(new string[]
            {
                "Module Name   : ",
                _moduleName,
                "\nFunction Name : ",
                _functionName,
                "\nError Message : ",
                errorMessageSummary,
                "\nDetailed Description : ",
                errorMessage
            });

            if (initializedErrorType)
                _initializeStatus = ErrorStatus.Error;

            _lastErrorMessage = text;
            _errorStatus = ErrorStatus.Error;
            _errorMessageList.Add(errorMessage ?? "");
            LogHelper.WriteError(string.Concat(new string[]
            {
                errorMessage,
                ", in ",
                _moduleName,
                ", ",
                _functionName
            }));

            MessageBox.Show(errorMessage ?? "", "Error Message");
        }

        public static string GetErrorMessageAndClear()
        {
            var result = string.Join("\n", _errorMessageList);
            _errorMessageList.Clear();
            return result;
        }

        public static void MessageTip(string infoMessage)
        {
            string text = string.Concat(new string[]
            {
                "Module Name   : ",
                _moduleName,
                "\nFunction Name : ",
                _functionName,
                "\nError Message : ",
                infoMessage
            });
            MessageBox.Show(null, text, "Info Message", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        public static MessageBoxResult MessageWarning(string infoMessage)
        {
            string text = string.Concat(new string[]
            {
                "Module Name   : ",
                _moduleName,
                "\nFunction Name : ",
                _functionName,
                "\n",
                infoMessage
            });
            return MessageBox.Show(null, text, "Info Message", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
        }

        public static void StatusClear()
        {
            _errorStatus = ErrorStatus.Normal;
        }

        public static void InitializeStatusClear()
        {
            _initializeStatus = ErrorStatus.Normal;
        }
    }
}
