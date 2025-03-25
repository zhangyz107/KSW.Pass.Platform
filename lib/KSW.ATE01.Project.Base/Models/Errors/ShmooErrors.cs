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
    public class ShmooErrors
    {
        private static readonly Lazy<ShmooErrors> _lazy = new Lazy<ShmooErrors>(() => new ShmooErrors());
        private readonly LanguageManager L = LanguageManager.Instance;

        public static ShmooErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ModuleName ModuleType { get; private set; }

        public ShmooErrors()
        {
            ModuleType = ModuleName.Shmoo;
        }

        public void InternalError(Exception inner, string location)
        {
            //ErrorService.Instance.ThrowInternalError(null, uint.MaxValue, inner, location, null);
        }

        public void TestNameIsNullOrEmpty()
        {
            var functionName = nameof(TestNameIsNullOrEmpty);
            var errorInfo = GetErrorInfo(L["ShmooTestNameEmpty"]);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.ErrorName = functionName;
            errorInfo.IsAlarm = true;
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
