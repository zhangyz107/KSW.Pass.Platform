using KSW.ATE01.Project.Base.Enums.Errors;
using KSW.ATE01.Project.Base.Language;
using KSW.ATE01.Project.Base.Services.Errors;

namespace KSW.ATE01.Project.Base.Models.Errors
{
    /// <summary>
    /// 一般情况错误
    /// </summary>
    public class InsGeneralErrors
    {
        private static readonly Lazy<InsGeneralErrors> _lazy = new Lazy<InsGeneralErrors>(() => new InsGeneralErrors());
        private readonly LanguageManager L = LanguageManager.Instance;

        public static InsGeneralErrors Instance
        {
            get { return _lazy.Value; }
        }

        public ModuleName ModuleType { get; private set; }

        public InsGeneralErrors()
        {
            InitError();
        }

        private void InitError()
        {
            ModuleType = ModuleName.InsGeneral;
        }

        public void MarkerLog(string location, params object[] args)
        {
            var errorInfo = GetErrorInfo("{0}", args);
            errorInfo.Behavior = BehaviorType.Continue;
            errorInfo.ErrorName = nameof(MarkerLog);
            errorInfo.IsAlarm = false;
            ErrorService.Instance.ThrowError(errorInfo, null, location);
        }

        public void MarkerError(string location, params object[] args)
        {
            var errorInfo = GetErrorInfo("{0}", args);
            errorInfo.Behavior = BehaviorType.ForceBin;
            errorInfo.ErrorName = nameof(MarkerError);
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
