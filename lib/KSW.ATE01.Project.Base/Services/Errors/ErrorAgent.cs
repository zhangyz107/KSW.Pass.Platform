using KSW.ATE01.Project.Base.Models.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Services.Errors
{
    /// <summary>
    /// 错误代理
    /// </summary>
    public class ErrorAgent
    {
        #region Fields
        private readonly Dictionary<uint, ErrorInfo> _agentDic;
        #endregion

        #region Properties
        public string Source { get; private set; }
        #endregion

        public ErrorAgent(string source)
        {
            _agentDic = new Dictionary<uint, ErrorInfo>();
            Source = source;
        }

        public void AddErrorInfo(ErrorInfo errorInfo)
        {
            if (_agentDic.ContainsKey(errorInfo.Number))
                return;

            _agentDic.Add(errorInfo.Number, errorInfo);
        }

        public ErrorInfo TryGetErrorInfo(uint number)
        {
            _agentDic.TryGetValue(number, out ErrorInfo errorInfo);
            return errorInfo;
        }
    }
}
