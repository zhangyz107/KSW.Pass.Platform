using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class PIRModel : STDFBaseModel
    {
        public PIRModel() : base(5, 10)
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

        public override ushort Length() => 6;

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


            this.valueConverter.SetUint16(idx - 4, data, offset: offset);
            return idx;
        }
    }
}
