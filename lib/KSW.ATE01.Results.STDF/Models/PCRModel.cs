using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class PCRModel : STDFBaseModel
    {
        public PCRModel() : base(1, 30)
        {

        }

        /// <summary>
        /// 测试头编号
        /// </summary>
        public byte HeadNum { get; set; }

        /// <summary>
        /// 站点编号
        /// </summary>
        public byte SiteNum { get; set; }

        /// <summary>
        /// 测试零件数量
        /// </summary>
        public uint PartCount { get; set; }

        /// <summary>
        /// 重新测试的零件数量
        /// </summary>
        public uint RetestedCount { get; set; }

        /// <summary>
        /// 测试期间的中止次数
        /// </summary>
        public uint AbortCount { get; set; }

        /// <summary>
        /// 测试的合格（合格）零件数量
        /// </summary>
        public uint GoodCount { get; set; }

        /// <summary>
        /// 测试的功能部件数量
        /// </summary>
        public uint FunctionalCount { get; set; }

        public override ushort Length() => 26;

        public override ushort SetBytes(byte[] data, int offset = 0)
        {
            ushort idx = 2;

            data[offset + idx] = this.RecordType;
            idx += 1;

            data[offset + idx] = this.RecordSubType;
            idx += 1;

            data[offset + idx] = this.HeadNum;
            idx += 1;

            data[offset + idx] = this.SiteNum;
            idx += 1;

            idx += this.valueConverter.SetUint32(this.PartCount, data, offset + idx);
            idx += this.valueConverter.SetUint32(this.RetestedCount, data, offset + idx);
            idx += this.valueConverter.SetUint32(this.AbortCount, data, offset + idx);
            idx += this.valueConverter.SetUint32(this.GoodCount, data, offset + idx);
            idx += this.valueConverter.SetUint32(this.FunctionalCount, data, offset + idx);

            this.valueConverter.SetUint16(idx - 4, data, offset: offset);
            return idx;
        }
    }
}
