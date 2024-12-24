using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace KSW.ATE01.Application.Models.RealTimeTxt
{
    public class BookmarkModel : IComparable<BookmarkModel>
    {
        public Guid BookmarkId { get; set; }

        /// <summary>
        /// Run名字
        /// </summary>
        public string RunName { get; set; }

        /// <summary>
        /// 区域起点
        /// </summary>
        public TextPointer Start { get; set; }
        /// <summary>
        /// 选择区域
        /// </summary>
        public TextPointer End { get; set; }

        public int CompareTo(BookmarkModel? other)
        {
            var result = 0;
            var runIndex = RunName.Replace("run", "");
            var otherIndex = other.RunName.Replace("run", "");
            if (int.TryParse(runIndex, out int currentIndex) && int.TryParse(otherIndex, out int index))
            {
                if (currentIndex > index)
                    result = 1;
                else if (currentIndex == index)
                    result = 0;
                else if (currentIndex< index)
                    result = -1;
            }
            return result;
        }
    }
}
