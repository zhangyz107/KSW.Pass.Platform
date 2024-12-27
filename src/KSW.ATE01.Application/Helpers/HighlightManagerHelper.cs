/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：HighlightManagerHelper.cs
// 功能描述：高亮管理帮助
//
// 作者：zhangyingzhong
// 日期：2024/12/27 10:02
// 修改记录(Revision History)
//
//------------------------------------------------------------*/

using KSW.ATE01.Application.Models.RealTimeTxt;

namespace KSW.ATE01.Application.Helpers
{
    /// <summary>
    /// 高亮管理帮助
    /// </summary>
    public class HighlightManagerHelper
    {
        private static List<HighlightModel> _highlights = new List<HighlightModel>();

        public static List<HighlightModel> Highlights => _highlights;

        public static void AddHighlight(string prefix, string runName)
        {
            if (runName.IsEmpty())
                return;

            var hasAdded = _highlights.Where(x => x.RunName.Equals(runName)).Any();
            if (hasAdded)
                return;

            _highlights.Add(new HighlightModel(prefix)
            {
                BookmarkId = Guid.NewGuid(),
                RunName = runName,
            });
            _highlights.Sort((x, y) => x.CompareTo(y));
        }

        public static HighlightModel GotoHighlight(string prefix, string runName, bool isNext = true)
        {
            HighlightModel result = null;
            var otherIndex = runName.Replace(prefix, "");
            if (_highlights.Any())
            {
                foreach (var highlight in _highlights)
                {
                    var runIndex = highlight.RunName.Replace(prefix, "");
                    var compareIndex = 0;
                    if (int.TryParse(runIndex, out int currentIndex) && int.TryParse(otherIndex, out int index))
                    {
                        if (currentIndex > index)
                            compareIndex = 1;
                        else if (currentIndex == index)
                            compareIndex = 0;
                        else if (currentIndex < index)
                            compareIndex = -1;

                        if (compareIndex > 0)
                        {
                            if (isNext)
                                result = highlight;
                            break;
                        }
                        else if (compareIndex < 0)
                        {
                            result = highlight;
                        }
                    }
                }
            }

            return result;
        }

        public static void ClearHighlight()
        {
            _highlights.Clear();
        }
    }
}
