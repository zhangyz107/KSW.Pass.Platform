using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Models.Instruments
{
    /// <summary>
    /// 记录消息信息
    /// </summary>
    public class RecordMessageModel : DtoBase
    {
        #region Fields
        private DateTime _recordTime;
        private string _recordMessage;
        #endregion

        #region Properties
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime
        {
            get => _recordTime;
            set => SetProperty(ref _recordTime, value);
        }

        /// <summary>
        /// 记录内容
        /// </summary>
        public string RecordMessage
        {
            get => _recordMessage;
            set => SetProperty(ref _recordMessage, value);
        }
        #endregion
    }
}
