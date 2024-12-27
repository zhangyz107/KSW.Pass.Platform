using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace KSW.ATE01.Application.Models.RealTimeTxt
{
    public class HighlightModel : IComparable<HighlightModel>
    {
        private readonly string _prefix;

        public HighlightModel(string prefix)
        {
            _prefix = prefix;
        }

        public Guid BookmarkId { get; set; }

        /// <summary>
        /// Run名字
        /// </summary>
        public string RunName { get; set; }

        /// <summary>
        /// Run前缀
        /// </summary>
        public string Prefix { get; set; }

        public int CompareTo(HighlightModel? other)
        {
            var result = 0;
            var runIndex = RunName.Replace(_prefix, "");
            var otherIndex = other.RunName.Replace(_prefix, "");
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
