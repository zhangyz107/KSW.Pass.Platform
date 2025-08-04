using KSW.ATE01.Results.STDF.Exception;
using KSW.ATE01.Results.STDF.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Services.Converter
{
    public class StdfValueConverter
    {
        private LanguageManager L => LanguageManager.Instance;

        public byte[] TwoBytes { get; } = new byte[2];
        public byte[] FourBytes { get; } = new byte[4];
        public byte[] TwoFiftyFiveBytes { get; } = new byte[255];

        private byte _cpuType = BitConverter.IsLittleEndian ? (byte)2 : (byte)1;

        /// <summary>
        /// CPU 类型，1 = 大端，2 = 小端。
        /// 
        /// 设置 CPU 类型时，ReverseBytesOnRead 和
        /// ReverseBytesOnWrite 属性将更新以匹配
        /// 如果在读/写作期间需要字节反转。
        /// 在正常情况下，这是很好的自动确定
        /// 是预期的效果。但是，有可能
        /// 有人会想以一个字节顺序读取并写入
        /// 另一个，两个标志允许这种情况。
        /// </summary>
        public byte CpuType
        {
            get
            {
                return this._cpuType;
            }

            set
            {
                this._cpuType = value;
                if (this._cpuType == 0)
                {
                    throw new Stdf4ParserException(L["CpuTypeError"]);
                }
                else if ((this._cpuType == 2 && BitConverter.IsLittleEndian) || (this._cpuType == 1 && !BitConverter.IsLittleEndian))
                {
                    this.ReverseBytesOnRead = false;
                    this.ReverseBytesOnWrite = false;
                }
                else if (this._cpuType == 1 || this._cpuType == 2)
                {
                    this.ReverseBytesOnRead = true;
                    this.ReverseBytesOnWrite = true;
                }
                else
                {
                    throw new Stdf4ParserException(L["UnsupportedCpuType"]);
                }
            }
        }

        /// <summary>
        /// 在读取时反转字节，即在读取期间更改字节顺序时。
        /// </summary>
        public bool ReverseBytesOnRead { get; set; } = false;

        /// <summary>
        /// 在写入时反转字节节，即在写入期间更改字节节序时。
        /// </summary>
        public bool ReverseBytesOnWrite { get; set; } = false;

        /// <summary>
        /// 从传递的数据中获取两个字节。考虑这一点。ReverseBytesOnRead，如果为 true，则反转字节顺序。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte[] GetTwoBytes(ReadOnlySpan<byte> data, int offset = 0)
        {
            if (offset + 1 >= data.Length)
            {
                throw new Stdf4ParserException(string.Format(L["ReadDataError"], data.Length, offset + 1));
            }

            if (this.ReverseBytesOnRead)
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 1], data[offset + 0]);
            }
            else
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 0], data[offset + 1]);
            }
            return TwoBytes;
        }

        /// <summary>
        /// 从传递的数据中获取两个字节。考虑这一点。ReverseBytesOnRead，如果为 true，则反转字节顺序。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public byte[] GetTwoBytes(byte[] data, int offset = 0)
        {
            if (this.ReverseBytesOnRead)
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 1], data[offset + 0]);
            }
            else
            {
                (TwoBytes[0], TwoBytes[1]) = (data[offset + 0], data[offset + 1]);
            }
            return TwoBytes;
        }

        /// <summary>
        /// 取两个字节并返回一个 int16。
        /// </summary.
        public short GetInt16AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            short value = BitConverter.ToInt16(this.GetTwoBytes(data, offset), 0);
            offset += 2;
            return value;
        }

        public ushort GetUInt16AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            ushort value = BitConverter.ToUInt16(this.GetTwoBytes(data, offset), 0);
            offset += 2;
            return value;
        }

        /// <summary>
        /// 从传递的数据中获取四个字节。考虑这一点。ReverseBytesOnRead，如果为 true，则反转字节顺序。
        /// </summary>
        public byte[] GetFourBytes(ReadOnlySpan<byte> data, int offset = 0)
        {
            if (this.ReverseBytesOnRead)
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 3], data[offset + 2], data[offset + 1], data[offset + 0]);
            }
            else
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 0], data[offset + 1], data[offset + 2], data[offset + 3]);
            }
            return FourBytes;
        }

        /// <summary>
        /// 从传递的数据中获取四个字节。考虑这一点。ReverseBytesOnRead，如果为 true，则反转字节顺序。
        /// </summary>
        public byte[] GetFourBytes(byte[] data, int offset = 0)
        {
            if (this.ReverseBytesOnRead)
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 3], data[offset + 2], data[offset + 1], data[offset + 0]);
            }
            else
            {
                (this.FourBytes[0], this.FourBytes[1], this.FourBytes[2], this.FourBytes[3])
                 = (data[offset + 0], data[offset + 1], data[offset + 2], data[offset + 3]);
            }
            return FourBytes;
        }


        public int GetInt32AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            int value = BitConverter.ToInt32(this.GetFourBytes(data, offset), 0);
            offset += 4;
            return value;
        }

        public uint GetUInt32AndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset + 4 > data.Length)
            {
                return 0;
            }

            uint value = BitConverter.ToUInt32(this.GetFourBytes(data, offset), 0);
            offset += 4;
            return value;
        }

        /// <summary>
        /// 取四个字节并返回一个浮点。
        /// </summary>
        public (float, bool) GetSingleAndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset + 4 > data.Length)
            {
                // Could throw an exception here.
                return (0.0f, false);
            }

            float value = BitConverter.ToSingle(this.GetFourBytes(data, offset), 0);
            offset += 4;
            return (value, true);
        }

        /// <summary>
        /// 取四个字节并返回一个浮点。
        /// </summary>
        public float? GetNullableSingleAndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset + 4 > data.Length)
            {
                return null;
            }

            float value = BitConverter.ToSingle(this.GetFourBytes(data, offset), 0);
            offset += 4;
            return value;
        }

        public ushort SetByte(byte value, byte[] intoData, int offset = 0)
        {
            intoData[offset] = value;
            return 1;
        }

        public ushort SetAsciiChar(char value, byte[] intoData, int offset = 0)
        {
            if (value > 0xff)
            {
                // Technically 7f is the upper limit of non-extended ASCII, but we will not overflow at 255.
                throw new Stdf4ParserException(L["CharacterOutOfRange"]);
            }
            intoData[offset] = (byte)((int)value & 0xff);
            return 1;
        }

        /// <summary>
        /// 在字节数组中设置两个字节，反转字节顺序，如此所示。反向字节写入
        /// </summary>
        public ushort SetUint16(ushort value, byte[] intoData, int offset = 0)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                intoData[offset + 0] = bytes[1];
                intoData[offset + 1] = bytes[0];
            }
            else
            {
                intoData[offset + 0] = bytes[0];
                intoData[offset + 1] = bytes[1];
            }
            return 2;
        }

        /// <summary>
        /// 在字节数组中设置两个字节，反转字节顺序，如此所示。ReverseBytesOnWrite,这是 int 版本，因为 uint16 math 被转换为 int。因此，如果值太大或负值，这可能会引发 ArgumentOutOfRangeException。
        /// </summary>
        public ushort SetUint16(int value, byte[] intoData, int offset = 0)
        {
            if (value > UInt16.MaxValue || value < 0)
            {
                throw new ArgumentOutOfRangeException(L["UInt16OutOfRange"]);
            }

            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                intoData[offset + 0] = bytes[1];
                intoData[offset + 1] = bytes[0];
            }
            else
            {
                intoData[offset + 0] = bytes[0];
                intoData[offset + 1] = bytes[1];
            }
            return 2;
        }


        /// <summary>
        /// 在字节数组中设置四个字节，反转字节顺序，如图所示。ReverseBytesOnWrite
        /// </summary>
        public ushort SetUint32(uint value, byte[] intoData, int offset = 0)
        {
            // Using shift does not work without special checks and
            // manipulation because it does not account for endianness.
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                intoData[offset + 0] = bytes[3];
                intoData[offset + 1] = bytes[2];
                intoData[offset + 2] = bytes[1];
                intoData[offset + 3] = bytes[0];
            }
            else
            {
                intoData[offset + 0] = bytes[0];
                intoData[offset + 1] = bytes[1];
                intoData[offset + 2] = bytes[2];
                intoData[offset + 3] = bytes[3];
            }
            return 4;
        }

        public ushort SetSingle(float value, byte[] intoData, int offset = 0)
        {
            // Using shift does not work without special checks and
            // manipulation because it does not account for endianness.
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                intoData[offset + 0] = bytes[3];
                intoData[offset + 1] = bytes[2];
                intoData[offset + 2] = bytes[1];
                intoData[offset + 3] = bytes[0];
            }
            else
            {
                intoData[offset + 0] = bytes[0];
                intoData[offset + 1] = bytes[1];
                intoData[offset + 2] = bytes[2];
                intoData[offset + 3] = bytes[3];
            }
            return 4;
        }

        /// <summary>
        /// 从 STDF 记录中获取字符串，并更新传递的偏移量。
        /// 如果偏移量大于或等于数据的长度，
        /// 则返回 null。如果长度为零，则返回空字符串。
        /// 返回 null 是因为有些文件包含记录，如果没有进一步的数据要报告，则记录将结束。
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public string? GetStringAndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset >= data.Length)
            {
                return null;
            }

            int len = data[offset];
            if (len == 0)
            {
                offset += 1;
                return string.Empty;
            }

            if (offset + 1 + len > data.Length)
            {
                throw new Stdf4ParserException(string.Format(L["InvalidData"], len, (data.Length - offset - 1)));
            }

            string str = System.Text.Encoding.ASCII.GetString(data.Slice(offset + 1, len).ToArray());
            offset += len + 1;
            return str;
        }


        public char? GetNullableCharAndUpdateOffset(ReadOnlySpan<byte> data, ref int offset)
        {
            if (offset >= data.Length)
            {
                return null;
            }

            string str = System.Text.Encoding.ASCII.GetString(data.Slice(offset, 1).ToArray());
            offset += 1;
            return str[0];
        }

        /// <summary>
        /// 将 ASCII 字符串写入给定的字节数组。不包括
        /// 训练 null，因为 STDF 记录类型不调用它。
        /// 值为 null 将写入零字节并设置 havePreviousNull
        /// 标志设置为 true，则空字符串将写入零的单个长度字节。
        /// havePreviousNull ref 参数用于跟踪 pervious 属性是否具有
        /// 空值。STDF 记录无法处理 null 然后非 null 字段（只有零长度），因此如果有人创建记录而不将中间字段设置为空字符串或无效值（对于可为 null 类型），则应引发异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="intoData"></param>
        /// <param name="havePreviousNull"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ushort WriteAsciiString(string? value, byte[] intoData, ref bool havePreviousNull, int offset = 0)
        {
            if (value is null)
            {
                havePreviousNull = true;
                return 0; //Nothing written.
            }
            else if (havePreviousNull)
            {
                throw new Stdf4ParserException(L["HaveNonNullValue"]);
            }
            else if (value == string.Empty)
            {
                intoData[offset + 0] = 0;
                return 1;
            }
            else if (value.Length > 255)
            {
                throw new Stdf4ParserException(L["StringTooLong"]);
            }
            else
            {
                intoData[offset + 0] = (byte)value.Length;
                var asciiString = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                for (int i = 0; i < asciiString.Length; i++)
                {
                    intoData[offset + i + 1] = asciiString[i];
                }
                return (ushort)(value.Length + 1);
            }
        }

        public ushort WriteNullableChar(char? value, byte[] intoData, ref bool havePreviousNull, int offset = 0)
        {
            if (value == null)
            {
                havePreviousNull = true;
                return 0; //Nothing written.
            }
            else if (havePreviousNull)
            {
                throw new Stdf4ParserException(L["HaveNonNullValue"]);
            }
            else if ((int)value > 0xff)
            {
                throw new Stdf4ParserException(L["OutOfASCIIRange"]);
            }
            else
            {
                intoData[offset] = (byte)((int)value & 0xff);
                return 1;
            }
        }


        public (byte, byte) UshortToBytes(ushort value)
        {
            // Using shift does not work without special checks and
            // manipulation because it does not account for endianness.
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                return (bytes[1], bytes[0]);
            }
            else
            {
                return (bytes[0], bytes[1]);
            }
        }

        /// <summary>
        /// 根据给定值返回一个字节元组。
        /// 如果要更改值的字节顺序，请将 reverseBytes 设置为 true。
        /// </summary>
        public (byte, byte, byte, byte) UintToBytes(uint value, bool reverseBytes)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (this.ReverseBytesOnWrite)
            {
                return (bytes[3], bytes[2], bytes[1], bytes[0]);
            }
            else
            {
                return (bytes[0], bytes[1], bytes[2], bytes[3]);
            }
        }

        public ushort WriteAsciiString(string? value, byte[] intoData, int offset = 0)
        {
            if (value is null)
            {
                intoData[offset + 0] = 0;
                return 1;
            }
            else if (value.Length > 255)
            {
                throw new Stdf4ParserException(L["StringTooLong"]);
            }
            else
            {
                var asciiString = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                for (int i = 0; i < asciiString.Length; i++)
                {
                    intoData[offset + i] = asciiString[i];
                }
                return (ushort)value.Length;
            }
        }
    }
}
