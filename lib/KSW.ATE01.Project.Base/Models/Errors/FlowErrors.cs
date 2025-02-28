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
    public class FlowErrors
    {
        private static readonly Lazy<FlowErrors> _lazy = new Lazy<FlowErrors>(() => new FlowErrors());
        private readonly LanguageManager L = LanguageManager.Instance;

        public static FlowErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ModuleName ModuleType { get; private set; }

        public FlowErrors()
        {
            InitError();
        }

        private void InitError()
        {
            ModuleType = ModuleName.Flow;
        }

        public void InternalError(Exception inner, string location)
        {
            ErrorService.Instance.ThrowInternalError(null, inner, location);
        }

        public void FailToExecuteForceHalt(string location, params object[] args)
        {
            var errorInfo = GetErrorInfo(L["FailToExecuteForceHalt"], args);
            errorInfo.Behavior = BehaviorType.ForceHalt;
            errorInfo.ErrorName = nameof(FailToExecuteForceHalt);
            errorInfo.IsAlarm = false;
            ErrorService.Instance.ThrowError(errorInfo, null, location);
        }

        public void DefaultFunctionExecutionError(string location, params object[] args)
        {
            var errorInfo = GetErrorInfo(L["DefaultFunctionExecutionError"], args);
            errorInfo.Behavior = BehaviorType.ForceBin;
            errorInfo.ErrorName = nameof(DefaultFunctionExecutionError);
            errorInfo.IsAlarm = false;
            ErrorService.Instance.ThrowError(errorInfo, null, location);
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
