using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Language;
using KSW.ATE01.Project.Base.Services.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    public class IOErrors
    {
        private static readonly Lazy<IOErrors> _lazy = new Lazy<IOErrors>(() => new IOErrors());
        private readonly LanguageManager L = LanguageManager.Instance;


        public static IOErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ModuleName ModuleType { get; private set; }

        public IOErrors()
        {
            InitError();
        }

        private void InitError()
        {
            Instance.ModuleType = ModuleName.IO;
        }

        public void IOResultAbnormal()
        {
            var functionName = nameof(IOResultAbnormal);
            var errorInfo = GetErrorInfo(L["IOResultAbnormal"]);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.Code = functionName;
            errorInfo.IsAlarm = false;
            ErrorService.Instance.ThrowError(errorInfo, null, functionName);
        }

        public void IOConfigurationFailed()
        {
            var functionName = nameof(IOConfigurationFailed);
            var errorInfo = GetErrorInfo(L["IOConfigurationFailed"]);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.Code = functionName;
            errorInfo.IsAlarm = false;
            ErrorService.Instance.ThrowError(errorInfo, null, functionName);
        }

        public void IOInvalidQuery()
        {
            var functionName = nameof(IOInvalidQuery);
            var errorInfo = GetErrorInfo(L["IOInvalidQuery"]);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.Code = functionName;
            errorInfo.IsAlarm = false;
            ErrorService.Instance.ThrowError(errorInfo, null, functionName);
        }

        private ErrorInfo GetErrorInfo(string errorMessage, params object[] param)
        {
            return new ErrorInfo()
            {
                ModuleName = ModuleType.ToString(),
                Message = param != null && param.Any() ? string.Format(errorMessage, param) : errorMessage,
            };
        }
    }
}
