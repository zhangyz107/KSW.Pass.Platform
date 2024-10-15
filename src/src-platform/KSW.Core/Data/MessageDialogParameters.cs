using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSW.Data
{
    /// <summary>
    /// 消息窗参数
    /// </summary>
    public class MessageDialogParameters
    {
        /// <summary>
        /// 指定要显示的文本
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// 消息窗按钮
        /// </summary>
        public MessageBoxButton MessageButton { get; set; }

        /// <summary>
        /// 消息窗图标
        /// </summary>
        public MessageBoxImage MessageIcon { get; set; }
    }
}
