using KSW.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Application.Models.RealTimeTxt
{
    public class SearchInfoModel : DtoBase
    {
        /// <summary>
        /// 是否全字段匹配
        /// </summary>
        public bool IsWholeWordMatch { get; set; }

        /// <summary>
        /// 是否循环查找
        /// </summary>
        public bool IsLoopSearch { get; set; }
        
        /// <summary>
        /// 搜索内容
        /// </summary>
        public string SearchContent { get; set; }

        /// <summary>
        /// 是否向上搜索
        /// </summary>
        public bool IsUpSearch { get; set; }
    }
}
