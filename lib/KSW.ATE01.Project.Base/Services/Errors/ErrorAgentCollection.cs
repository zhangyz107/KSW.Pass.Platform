using KSW.ATE01.Project.Base.Models.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Services.Errors
{
    /// <summary>
    /// 错误代理集合
    /// </summary>
    public class ErrorAgentCollection
    {
        private readonly List<ErrorAgent> _agents;
        private static readonly Lazy<ErrorAgentCollection> _lazy = new Lazy<ErrorAgentCollection>(() => new ErrorAgentCollection());

        public ErrorAgentCollection()
        {
            _agents = new List<ErrorAgent>();
        }

        public static ErrorAgentCollection Instance
        {
            get { return _lazy.Value; }
        }

        public virtual void TryAddAgent(ErrorAgent agent)
        {
            if (_agents.Any(x => x.Equals(agent)))
                return;

            _agents.Add(agent);
        }

        public virtual void TryRemoveAgent(ErrorAgent agent)
        {
            if (_agents.All(x => x.Equals(agent)))
                return;

            _agents.Remove(agent);
        }

        public ErrorAgent TryGetAgent(string source)
        {
            return _agents.FirstOrDefault(x => x.Source.Equals(source));
        }

        public ErrorMessage GetErrorMessageByInfo(ErrorInfo errorInfo, Exception exception, string location, bool isGetInfoOnly, params object[] parameters)
        {
            return new ErrorMessage()
            {
                Number = errorInfo.Number,
                Behavior = errorInfo.Behavior,
                ErrorName = errorInfo.Code,
                Exception = exception,
                Location = location,
            };
        }

        public ErrorMessage GetErrorTemplateByCode(uint number)
        {
            var errorInfo = GetErrorInfoByCode(number);
            return this.GetErrorMessageByInfo(errorInfo, null, "", false, new object[0]);
        }

        private ErrorInfo GetErrorInfoByCode(uint number)
        {
            var defaultErrorInfo = _agents.FirstOrDefault();
            return GetErrorInfoByCode(defaultErrorInfo, number);
        }

        private ErrorInfo GetErrorInfoByCode(ErrorAgent defaultErrorInfo, uint number)
        {
            return defaultErrorInfo.TryGetErrorInfo(number);
        }
    }
}
