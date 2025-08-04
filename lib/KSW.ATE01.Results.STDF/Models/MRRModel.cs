using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class MRRModel : STDFBaseModel
    {
        public MRRModel() : base(1, 20)
        {

        }

        /// <summary>
        /// 最后测试部件的日期和时间
        /// </summary>
        public DateTime FinishTime { get; set; }

        /// <summary>
        /// 最后测试部件的秒数
        /// </summary>
        public uint FinishT { get => (uint)Math.Abs((FinishTime - baseDateTime).Seconds); }

        /// <summary>
        /// 批次处置代码
        /// </summary>
        public char? DispositionCode { get; set; }

        /// <summary>
        /// 用户提供的批次描述
        /// </summary>
        public string UserDescription { get; set; }

        /// <summary>
        /// 执行者提供的批次描述
        /// </summary>
        public string ExecuteDescription { get; set; }

        public override ushort Length()
        {
            ushort length = 8;

            length += (ushort)(DispositionCode == null ? 0 : 1);

            length += (ushort)(UserDescription == null ? 0 : UserDescription.Length + 1);
            length += (ushort)(ExecuteDescription == null ? 0 : ExecuteDescription.Length + 1);

            return length;
        }

        public override ushort SetBytes(byte[] data, int offset = 0)
        {
            ushort idx = 2;

            idx += this.valueConverter.SetByte(this.RecordType, data, offset: idx + offset);
            idx += this.valueConverter.SetByte(this.RecordSubType, data, offset: idx + offset);
            idx += this.valueConverter.SetUint32(this.FinishT, data, offset: idx + offset);

            bool havePreviousNull = false;
            idx += this.valueConverter.WriteNullableChar(this.DispositionCode, data, offset: idx + offset, havePreviousNull: ref havePreviousNull);
            idx += this.valueConverter.WriteAsciiString(this.UserDescription, data, offset: idx + offset, havePreviousNull: ref havePreviousNull);
            idx += this.valueConverter.WriteAsciiString(this.ExecuteDescription, data, offset: idx + offset, havePreviousNull: ref havePreviousNull);

            this.valueConverter.SetUint16(idx - 4, data, offset: offset);
            return idx;
        }
    }
}
