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
    public class ALPGErrors
    {
        private static readonly Lazy<ALPGErrors> _lazy = new Lazy<ALPGErrors>(() => new ALPGErrors());
        private readonly LanguageManager L = LanguageManager.Instance;

        public static ALPGErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ModuleName ModuleType { get; private set; }

        public ALPGErrors()
        {
            ModuleType = ModuleName.ALPG;
        }

        public void InternalError(Exception inner,string location)
        {

        }

        public void ALPGNumOutOfRange()
        {
            var functionName = nameof(ALPGNumOutOfRange);
            var errorInfo = GetErrorInfo("超出ALPG Num范围，仅支持0-3");
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
