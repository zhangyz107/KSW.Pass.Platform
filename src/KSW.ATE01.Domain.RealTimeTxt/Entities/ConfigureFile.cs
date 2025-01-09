using System.Collections.ObjectModel;
using System.Text;

namespace KSW.ATE01.Domain.RealTimeTxt.Entities
{
    [Serializable]
    public class ConfigureFile
    {
        /// <summary>
        /// 文本编码
        /// </summary>
        public string FileEncoding { get; set; } = "UTF8";

        /// <summary>
        /// 文件更改间隔
        /// </summary>
        public int FileChangeInterval { get; set; } = 100;

        /// <summary>
        /// 文件重新打开间隔
        /// </summary>
        public int FileReopenInterval { get; set; } = 10;

        /// <summary>
        /// 关键字集合
        /// </summary>
        public List<Keywords> Keywords { get; set; } = new List<Keywords>();
    }
}
