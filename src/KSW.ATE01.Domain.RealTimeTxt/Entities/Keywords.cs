using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KSW.ATE01.Domain.RealTimeTxt.Entities
{
    [Serializable]
    public class Keywords
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Color? Foreground { get; set; }

        /// <summary>
        /// 是否高亮
        /// </summary>
        public bool? IsHighlight { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
