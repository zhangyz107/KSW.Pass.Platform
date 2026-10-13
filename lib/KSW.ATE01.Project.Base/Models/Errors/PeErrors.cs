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
    public class PeErrors
    {
        private static readonly Lazy<PeErrors> _lazy = new Lazy<PeErrors>(() => new PeErrors());
        private readonly LanguageManager L = LanguageManager.Instance;

        public static PeErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ModuleName ModuleType { get; private set; }

        public PeErrors()
        {
            ModuleType = ModuleName.Pe;
        }

        public void InternalError(Exception inner, string location)
        {

        }

        public void WorkVoltageOutOfRange(string location,params object[] param)
        {
            var funcionName = nameof(WorkVoltageOutOfRange);
            var errorInfo = GetErrorInfo(L["PeWorkVoltageOutOfRange"], param);
            errorInfo.Behavior = BehaviorType.ForceFail;
            errorInfo.ErrorName = funcionName;
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
