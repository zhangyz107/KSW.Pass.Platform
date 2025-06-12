using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Helpers
{
    /// <summary>
    /// Byte数组转换帮助类
    /// </summary>
    public static class ByteConverterHelper
    {
        /// <summary>
        /// 获取32位无符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="isBigEndian">默认小端字节序</param>
        /// <returns></returns>
        public static uint GetUInt32(byte[] data, int offset = 0, bool isBigEndian = false)
        {
            if (data == null || data.Length <= 0)
                return 0;

            var length = sizeof(uint);
            var tempArray = new byte[length];

            Array.Copy(data, offset, tempArray, 0, length);
            if (isBigEndian)
            {
                Array.Reverse(tempArray);
            }
            return BitConverter.ToUInt32(tempArray);
        }

        /// <summary>
        /// 获取32位有符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="isBigEndian">默认小端字节序</param>
        /// <returns></returns>
        public static int GetInt32(byte[] data, int offset = 0, bool isBigEndian = false)
        {
            if (data == null || data.Length <= 0)
                return 0;

            var length = sizeof(int);
            var tempArray = new byte[length];

            Array.Copy(data, offset, tempArray, 0, length);
            if (isBigEndian)
            {
                Array.Reverse(tempArray);
            }
            return BitConverter.ToInt32(tempArray);
        }

        /// <summary>
        /// 获取16位有符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="isBigEndian">默认小端字节序</param>
        /// <returns></returns>
        public static short GetInt16(byte[] data, int offset = 0, bool isBigEndian = false)
        {
            if (data == null || data.Length <= 0)
                return 0;

            var length = sizeof(short);
            var tempArray = new byte[length];

            Array.Copy(data, offset, tempArray, 0, length);
            if (isBigEndian)
            {
                Array.Reverse(tempArray);
            }
            return BitConverter.ToInt16(tempArray);
        }

        /// <summary>
        /// 获取16位无符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="isBigEndian">默认小端字节序</param>
        /// <returns></returns>
        public static ushort GetUInt16(byte[] data, int offset = 0, bool isBigEndian = false)
        {
            if (data == null || data.Length <= 0)
                return 0;

            var length = sizeof(ushort);
            var tempArray = new byte[length];

            Array.Copy(data, offset, tempArray, 0, length);
            if (isBigEndian)
            {
                Array.Reverse(tempArray);
            }
            return BitConverter.ToUInt16(tempArray);
        }

        /// <summary>
        /// 获取64位有符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="isBigEndian">默认小端字节序</param>
        /// <returns></returns>
        public static long GetInt64(byte[] data, int offset = 0, bool isBigEndian = false)
        {
            if (data == null || data.Length <= 0)
                return 0;

            var length = sizeof(long);
            var tempArray = new byte[length];

            Array.Copy(data, offset, tempArray, 0, length);
            if (isBigEndian)
            {
                Array.Reverse(tempArray);
            }
            return BitConverter.ToInt64(tempArray);
        }

        /// <summary>
        /// 获取64位无符号整数
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="isBigEndian">默认小端字节序</param>
        /// <returns></returns>
        public static ulong GetUInt64(byte[] data, int offset = 0, bool isBigEndian = false)
        {
            if (data == null || data.Length <= 0)
                return 0;

            var length = sizeof(ulong);
            var tempArray = new byte[length];

            Array.Copy(data, offset, tempArray, 0, length);
            if (isBigEndian)
            {
                Array.Reverse(tempArray);
            }
            return BitConverter.ToUInt64(tempArray);
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isBigEndian">默认小端字节序</param>
        /// <returns></returns>
        public static byte[] GetBytes(object value, bool isBigEndian = false)
        {
            byte[] result = null;
            if (value == null)
                return result;

            var type = value.GetType();
            if (type == typeof(byte))
                result = new byte[] { (byte)value };
            else if (type == typeof(short))
                result = BitConverter.GetBytes((short)value);
            else if (type == typeof(int))
                result = BitConverter.GetBytes((int)value);
            else if (type == typeof(long))
                result = BitConverter.GetBytes((long)value);
            else if (type == typeof(ulong))
                result = BitConverter.GetBytes((ulong)value);
            else if (type == typeof(ushort))
                result = BitConverter.GetBytes((ushort)value);
            else if (type == typeof(uint))
                result = BitConverter.GetBytes((uint)value);

            if (result != null && result.Length > 0 && isBigEndian)
            {
                Array.Reverse(result);
            }
            return result;
        }

    }
}
