using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.Data
{
    /// <summary>
    /// 进度条参数
    /// </summary>
    public class ProcessBarParameters
    {
        #region Properties
        /// <summary>
        /// 是否为等待框
        /// </summary>
        public bool IsIndeterminate { get; set; }

        /// <summary>
        /// 进度内容
        /// </summary>
        public string ProcessContent { get; set; }

        /// <summary>
        /// 进度
        /// </summary>
        public double ProcessRate { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        public Func<Task> DoWork { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<double, string> UpdateProgress { get; set; }
        #endregion
    }
}
