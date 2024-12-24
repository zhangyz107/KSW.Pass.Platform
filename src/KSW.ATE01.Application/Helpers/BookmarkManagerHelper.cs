using KSW.ATE01.Application.Models.RealTimeTxt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace KSW.ATE01.Application.Helpers
{
    public class BookmarkManagerHelper
    {
        private static List<BookmarkModel> _bookmarkModels = new List<BookmarkModel>();

        public static void AddBookmark(string runName, TextPointer start, TextPointer end)
        {
            var hasAdded = _bookmarkModels.Where(x => x.RunName.Equals(runName)).Any();
            if (hasAdded)
                return;

            if (start == null || end == null)
                return;

            _bookmarkModels.Add(new BookmarkModel()
            {
                BookmarkId = Guid.NewGuid(),
                RunName = runName,
                Start = start,
                End = end,
            });
            _bookmarkModels.Sort((x, y) => x.CompareTo(y));
        }

        public static BookmarkModel GotoBookmark(string runName, bool isNext = true)
        {
            BookmarkModel result = null;
            var otherIndex = runName.Replace("run", "");
            if (_bookmarkModels.Any())
            {
                foreach (var bookmark in _bookmarkModels)
                {
                    var runIndex = bookmark.RunName.Replace("run", "");
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
                                result = bookmark;
                            break;
                        }
                        else if (compareIndex < 0)
                        {
                            result = bookmark;
                        }
                    }
                }
            }

            return result;
        }

        public static void ClearBookmark()
        {
            _bookmarkModels.Clear();
        }
    }
}
