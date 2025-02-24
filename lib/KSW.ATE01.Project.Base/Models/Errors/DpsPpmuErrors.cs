using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Enums.Patterns;
using KSW.ATE01.Project.Base.Language;
using KSW.ATE01.Project.Base.Services.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    public class DpsPpmuErrors
    {
        private static readonly Lazy<DpsPpmuErrors> _lazy = new Lazy<DpsPpmuErrors>(() => new DpsPpmuErrors());
        private readonly LanguageManager L = LanguageManager.Instance;

        public static DpsPpmuErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ModuleName ModuleType { get; private set; }

        public DpsPpmuErrors()
        {
            InitError();
        }

        private void InitError()
        {
            Instance.ModuleType = ModuleName.DpsPpmu;
        }

        public void InternalError(Exception inner, string location)
        {
            //ErrorService.Instance.ThrowInternalError(null, uint.MaxValue, inner, location, null);
        }

        public void DriverAndComparatorOutOfRange(string location, params object[] param) 
        {
            var functionName = nameof(DriverAndComparatorOutOfRange);
            var errorInfo = GetErrorInfo(L["DriverAndComparatorOutOfRange"], param);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.Code = functionName;
            errorInfo.IsAlarm = true;
            ErrorService.Instance.ThrowError(errorInfo, null, location);
        }

        public void PinListIsNullOrEmpty()
        {
            var functionName = nameof(PinListIsNullOrEmpty);
            var errorInfo = GetErrorInfo(L["PinListNullOrEmpty"]);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.Code = functionName;
            errorInfo.IsAlarm = true;
            ErrorService.Instance.ThrowError(errorInfo, null, functionName);
        }

        public void FVMIOutOfRange(string location, params object[] param)
        {
            var functionName = nameof(FVMIOutOfRange);
            var errorInfo = GetErrorInfo(L["FVMIOutOfRange"], param);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.Code = functionName;
            errorInfo.IsAlarm = true;
            ErrorService.Instance.ThrowError(errorInfo, null, location);
        }

        public void FIMVOutOfRange(string location, params object[] param)
        {
            var functionName = nameof(FIMVOutOfRange);
            var errorInfo = GetErrorInfo(L["FIMVOutOfRange"], param);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.Code = functionName;
            errorInfo.IsAlarm = true;
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
