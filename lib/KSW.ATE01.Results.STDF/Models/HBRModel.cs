using KSW.ATE01.Results.STDF.Exception;
using KSW.ATE01.Results.STDF.Services.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class HBRModel : STDFBaseModel
    {
        public HBRModel() : base(1, 40)
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
        /// 硬件Bin号
        /// </summary>
        public ushort HBinNum { get; set; }

        /// <summary>
        /// 硬件Bin数
        /// </summary>
        public uint HBinCount { get; set; }

        /// <summary>
        /// 硬件Bin Pass/Fail
        /// </summary>
        public char HBinPF { get; set; }

        /// <summary>
        /// 硬件Bin名称
        /// </summary>
        public string HBinName { get; set; }

        public override ushort Length()
        {
            ushort length = 13;
            if (this.HBinName == null)
            {
                return length;
            }
            else if (HBinName.Length > 255)
            {
                throw new Stdf4ParserException(L["StringTooLong"]);
            }
            else
            {
                length += (ushort)(HBinName.Length + 1);
            }
            return length;
        }

        public override ushort SetBytes(byte[] data, int offset = 0)
        {
            ushort idx = 2; // SKipping the first two length bytes.

            data[offset + idx] = this.RecordType;
            idx += 1;

            data[offset + idx] = this.RecordSubType;
            idx += 1;

            data[offset + idx] = this.HeadNum;
            idx += 1;

            data[offset + idx] = this.SiteNum;
            idx += 1;

            idx += valueConverter.SetUint16(this.HBinNum, data, offset + idx);
            idx += valueConverter.SetUint32(this.HBinCount, data, offset + idx);


            data[offset + idx] = Convert.ToByte(this.HBinPF);

            if (this.HBinName != null)
            {
                idx += valueConverter.WriteAsciiString(this.HBinName, data, offset + idx);
            }

            valueConverter.SetUint16(idx - 4, data, offset: offset);
            return idx;
        }
    }
}
