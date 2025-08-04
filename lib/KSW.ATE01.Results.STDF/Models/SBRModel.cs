using KSW.ATE01.Results.STDF.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class SBRModel : STDFBaseModel
    {
        public SBRModel() : base(1, 50)
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
        /// 软件Bin号
        /// </summary>
        public ushort SBinNum { get; set; }

        /// <summary>
        /// 软件Bin数
        /// </summary>
        public uint SBinCount { get; set; }

        /// <summary>
        /// 软件Bin Pass/Fail
        /// </summary>
        public char SBinPF { get; set; }

        /// <summary>
        /// 软件Bin名称
        /// </summary>
        public string SBinName { get; set; }

        public override ushort Length()
        {
            ushort length = 13;
            if (SBinName == null)
            {
                return length;
            }
            else if (SBinName.Length > 255)
            {
                throw new Stdf4ParserException(L["StringTooLong"]);
            }
            else
            {
                length += (ushort)(SBinName.Length + 1);
            }

            return length;
        }

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

            idx += valueConverter.SetUint16(this.SBinNum, data, offset + idx);
            idx += this.valueConverter.SetUint32(this.SBinCount, data, offset + idx);

            data[offset + idx] = Convert.ToByte(this.SBinPF);

            if (this.SBinName != null)
            {
                idx += this.valueConverter.WriteAsciiString(this.SBinName, data, offset + idx);
            }

            idx += this.valueConverter.SetUint16(idx - 4, data, offset); 
            return idx;

        }
    }
}
