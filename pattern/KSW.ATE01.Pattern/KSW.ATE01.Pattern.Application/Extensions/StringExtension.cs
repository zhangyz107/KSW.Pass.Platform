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

        /// <summary>
        /// 将 16 进制字符串转换为字节数组
        /// </summary>
        /// <param name="hexString">包含 16 进制数字的字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] ToByteArray(this string hexString)
        {
            // 检查字符串是否为空或无效
            if (string.IsNullOrEmpty(hexString))
                throw new ArgumentNullException("十六进制字符串不能为空", nameof(hexString));

            // 移除前缀 "0x" 或 "0X"
            if (hexString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                hexString = hexString.Substring(2);
            }

            // 检查字符串长度是否为偶数
            if (hexString.Length % 2 != 0)
                throw new ArgumentException("十六进制字符串的长度必须为偶数", nameof(hexString));

            // 转换为字节数组
            int numberOfChars = hexString.Length;
            byte[] bytes = new byte[numberOfChars / 2];
            for (int i = 0; i < numberOfChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}
