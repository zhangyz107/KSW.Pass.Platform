using KSW.ATE01.Results.STDF.Enums;
using KSW.ATE01.Results.STDF.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class PTRModel : STDFBaseModel
    {
        public PTRModel() : base(15, 10)
        {

        }

        /// <summary>
        /// 测试编码
        /// </summary>
        public uint TestNum { get; set; }

        /// <summary>
        /// 测试头编号
        /// </summary>
        public byte HeadNum { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public byte SiteNum { get; set; }

        /// <summary>
        /// 测试标志
        /// </summary>
        public byte TestFlag { get; set; }

        /// <summary>
        /// 参数标志
        /// </summary>
        public byte ParamFlag { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        public float Result { get; set; }

        /// <summary>
        /// 测试描述文字或标签
        /// </summary>
        public string TestText { get; set; }

        /// <summary>
        /// 报警名称
        /// </summary>
        public string AlarmId { get; set; }

        /// <summary>
        /// 可选标志
        /// </summary>
        public byte OptionalFalg { get; set; }

        /// <summary>
        /// 测试结果缩放指数
        /// </summary>
        public sbyte ResultScale { get; set; }

        /// <summary>
        /// 下限缩放指数
        /// </summary>
        public sbyte LLMScale { get; set; }

        /// <summary>
        /// 上限缩放指数
        /// </summary>
        public sbyte HLMScale { get; set; }

        /// <summary>
        /// 测试下限值
        /// </summary>
        public float LowLimit { get; set; }

        /// <summary>
        /// 测试上限值
        /// </summary>
        public float HighLimit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// ANSI C 结果格式字符串
        /// </summary>
        public string ResultFormatString { get; set; }

        /// <summary>
        /// ANSI C 下限格式字符串
        /// </summary>
        public string LLMFormatString { get; set; }

        /// <summary>
        /// ANSI C 上限格式字符串
        /// </summary>
        public string HLMFormatString { get; set; }

        /// <summary>
        /// 规格下限
        /// </summary>
        public float LowSpecification { get; set; }

        /// <summary>
        /// 规格上限
        /// </summary>
        public float HighSpecification { get; set; }

        public override ushort Length()
        {
            int length = 32;

            length += this.TestText == null ? 0 : this.TestText.Length + 1;
            length += this.AlarmId == null ? 0 : this.AlarmId.Length + 1;
            length += this.Units == null ? 0 : this.Units.Length + 1;
            length += this.ResultFormatString == null ? 0 : this.ResultFormatString.Length + 1;
            length += this.LLMFormatString == null ? 0 : this.LLMFormatString.Length + 1;
            length += this.HLMFormatString == null ? 0 : this.HLMFormatString.Length + 1;

            if (length > ushort.MaxValue)
            {
                throw new Stdf4ParserException(L["RecordLengthTooLong"]);
            }

            return (ushort)length;
        }

        public override ushort SetBytes(byte[] data, int offset = 0)
        {
            ushort idx = 2;
            idx += this.valueConverter.SetByte(this.RecordType, data, offset: idx + offset);
            idx += this.valueConverter.SetByte(this.RecordSubType, data, offset: idx + offset);

            idx += this.valueConverter.SetUint32(this.TestNum, data, offset: idx + offset);
            idx += this.valueConverter.SetByte(this.HeadNum, data, offset: idx + offset);
            idx += this.valueConverter.SetByte(this.SiteNum, data, offset: idx + offset);
            idx += this.valueConverter.SetByte(this.TestFlag, data, offset: idx + offset);
            idx += this.valueConverter.SetByte(this.ParamFlag, data, offset: idx + offset);
            idx += this.valueConverter.SetSingle(this.Result, data, offset: idx + offset);

            bool havePreviousNull = false;
            idx += this.valueConverter.WriteAsciiString(this.TestText, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.AlarmId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);

            if(havePreviousNull)
                throw new Stdf4ParserException(L["HaveNonNullValue"]);

            idx += this.valueConverter.SetByte(this.OptionalFalg, data, offset: idx + offset);
            idx += this.valueConverter.SetByte((byte)this.ResultScale, data, offset: idx + offset);
            idx += this.valueConverter.SetByte((byte)this.LLMScale, data, offset: idx + offset);
            idx += this.valueConverter.SetByte((byte)this.HLMScale, data, offset: idx + offset);
            idx += this.valueConverter.SetSingle(this.LowLimit, data, offset: idx + offset);
            idx += this.valueConverter.SetSingle(this.HighLimit, data, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.Units, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.ResultFormatString, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.LLMFormatString, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.HLMFormatString, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);

            if (havePreviousNull)
                throw new Stdf4ParserException(L["HaveNonNullValue"]);

            idx += this.valueConverter.SetSingle(this.LowSpecification, data, offset: idx + offset);
            idx += this.valueConverter.SetSingle(this.HighSpecification, data, offset: idx + offset);

            this.valueConverter.SetUint16(idx - 4, data, offset: offset);
            return idx;
        }
    }
}
