using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Application.Extensions
{
    public static class StringExtension
    {
        public static string RemoveNode(this string strline)
        {
            int num = strline.IndexOf("//");
            if (num < 0)
            {
                return strline;
            }
            return strline.Remove(num);
        }

        public static void SplitValidAndComment(this string strline, out string valid, out string comment)
        {
            valid = string.Empty;
            comment = string.Empty;
            int num = strline.IndexOf("//");
            if (num == -1)
            {
                valid = strline;
            }
            else if (num >= 0)
            {
                valid = strline.Substring(0, num);
                comment = strline.Substring(num, strline.Length - num);
            }
            valid = valid.Trim();
            comment = comment.Trim();
        }
    }
}
