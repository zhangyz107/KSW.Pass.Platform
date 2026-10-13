using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    public static class BitConverterHelper
    {
        public static bool IsLittleEndian = BitConverter.IsLittleEndian;

        public static byte[] GetBytes(short value)
        {
            byte[] bytes = new byte[2];
            if (IsLittleEndian)
            {
                bytes[0] = (byte)value;
                bytes[1] = (byte)(value >> 8);
            }
            else
            {
                bytes[0] = (byte)(value >> 8);
                bytes[1] = (byte)value;
            }

            return bytes;
        }

        /// <summary>
        /// 将16位无符号整型转换为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(ushort value) => GetBytes((short)value);

        /// <summary>
        /// 将32位有符号整数转换为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(int value)
        {
            byte[] bytes = new byte[4];
            if (IsLittleEndian)
            {
                bytes[0] = (byte)value;
                bytes[1] = (byte)(value >> 8);
                bytes[2] = (byte)(value >> 16);
                bytes[3] = (byte)(value >> 24);
            }
            else
            {
                bytes[0] = (byte)(value >> 24);
                bytes[1] = (byte)(value >> 16);
                bytes[2] = (byte)(value >> 8);
                bytes[3] = (byte)value;
            }

            return bytes;
        }

        public static byte[] GetBytes(uint value) => GetBytes((int)value);

        /// <summary>
        /// 将单精度浮点数转换为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (!IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// 将双精度浮点数转换为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(double value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// 将布尔值转换为字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetByte(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// 将字节数组还原为16位有符号整数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static short ToInt16(byte[] bytes, int startIndex = 0)
        {
            CheckArgs(bytes, startIndex, 2);
            if (IsLittleEndian)
                return (short)(bytes[startIndex] | bytes[startIndex + 1] << 8);
            else
                return (short)(bytes[startIndex] << 8 | bytes[startIndex + 1]);

        }

        /// <summary>
        /// 将字节数组还原为16位无符号整数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static ushort ToUInt16(byte[] bytes, int startIndex = 0) => (ushort)ToInt16(bytes, startIndex);

        /// <summary>
        /// 将字节数组还原为32位有符号整数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int ToInt32(byte[] bytes, int startIndex = 0)
        {
            CheckArgs(bytes, startIndex, 4);
            if (IsLittleEndian)
                return (bytes[startIndex] | bytes[startIndex + 1] << 8 | bytes[startIndex + 2] << 16 | bytes[startIndex + 3] << 24);
            else
                return (bytes[startIndex] << 24 | bytes[startIndex + 1] << 16 | bytes[startIndex + 2] << 8 | bytes[startIndex + 3]);
        }

        /// <summary>
        /// 将字节数组还原为32位无符号整数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static uint ToUInt32(byte[] bytes, int startIndex = 0) => (uint)ToInt32(bytes, startIndex);

        /// <summary>
        /// 将字节数组还原为单精度浮点数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static float ToSingle(byte[] bytes, int startIndex = 0)
        {
            CheckArgs(bytes, startIndex, 4);
            var tmp = new byte[4];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, 4);
            if (!IsLittleEndian)
                Array.Reverse(tmp);

            return BitConverter.ToSingle(tmp, 0);
        }

        /// <summary>
        /// 将字节数组还原为双精度浮点数
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static double ToDouble(byte[] bytes,int startIndex = 0)
        {
            CheckArgs(bytes, startIndex, 8);
            var tmp = new byte[8];
            Buffer.BlockCopy(bytes, startIndex, tmp, 0, 8);
            if (!IsLittleEndian)
                Array.Reverse(tmp);

            return BitConverter.ToDouble(tmp, 0);
        }


        private static void CheckArgs(byte[] bytes, int startIndex, int requiredSize)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (startIndex < 0 || startIndex + requiredSize > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "字节数组索引超出范围或长度不足");
        }
    }
}
