using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KSW.ATE01.Start.Styles
{
    /// <summary>
    /// 提示信息风格
    /// </summary>
    public static class SnackbarMessageStyle
    {
        /// <summary>
        /// 成功背景色
        /// </summary>
        public static Color SuccessColor { get; private set; } = Color.FromRgb(246, 255, 237);

        /// <summary>
        /// 错误背景色
        /// </summary>
        public static Color ErrorColor { get; private set; } = Color.FromRgb(255, 242, 240);
    }
}
