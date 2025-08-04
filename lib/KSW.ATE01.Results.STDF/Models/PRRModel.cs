using KSW.ATE01.Results.STDF.Enums;
using KSW.ATE01.Results.STDF.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class PRRModel : STDFBaseModel
    {
        public PRRModel() : base(5, 20)
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
        /// 零件信息标志
        /// </summary>
        public byte PartFlag { get; set; }

        /// <summary>
        /// 数据优先于之前的部分
        /// </summary>
        public bool DataSupersedesPreviousPart
        {
            get => (this.PartFlag & 0b0000_0001) == 0 ? false : true;
            set
            {
                if (value)
                {
                    this.PartFlag |= 0b0000_0001;
                }
                else
                {
                    this.PartFlag &= 0b1111_1110;
                }
            }
        }

        /// <summary>
        /// 测试已完成正常
        /// </summary>
        public bool TestingCompletedNormally
        {
            get => (this.PartFlag & 0b0000_0001) == 0 ? true : false;
            set
            {
                if (value)
                {
                    this.PartFlag &= 0b1111_1110;
                }
                else
                {
                    this.PartFlag |= 0b0000_0001;
                }
            }
        }

        /// <summary>
        /// 部件通过
        /// </summary>
        public bool PartPassed
        {
            get => (this.PartFlag & 0b0000_0001) == 0 ? true : false;
            set
            {
                if (value)
                {
                    this.PartFlag &= 0b1111_1110;
                }
                else
                {
                    this.PartFlag |= 0b0000_0001;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool PartFailFlagValid
        {
            get => (this.PartFlag & 0b0000_0001) == 0 ? false : true;
            set
            {
                if (value)
                {
                    this.PartFlag |= 0b0000_0001;
                }
                else
                {
                    this.PartFlag &= 0b1111_1110;
                }
            }
        }

        /// <summary>
        /// 执行的测试数量
        /// </summary>
        public ushort NumTest { get; set; }

        /// <summary>
        /// 硬件bin
        /// </summary>
        public ushort HardBin { get; set; }

        /// <summary>
        /// 软件bin
        /// </summary>
        public ushort SoftBin { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public short XCoord { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public short YCoord { get; set; }

        /// <summary>
        /// 已用测试时间（以毫秒为单位）
        /// </summary>
        public uint TestTime { get; set; }

        /// <summary>
        /// 零件ID
        /// </summary>
        public string PartId { get; set; }

        /// <summary>
        /// 零件描述文字
        /// </summary>
        public string PartText { get; set; }

        /// <summary>
        /// 固件
        /// </summary>
        public byte[] PartFix { get; set; }

        public override ushort Length()
        {
            int length = 19;

            length += this.PartId == null ? 0 : this.PartId.Length + 1;
            length += this.PartText == null ? 0 : this.PartText.Length + 1;
            length += this.PartFix == null ? 0 : this.PartFix.Length + 1;

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
            idx += this.valueConverter.SetByte(this.PartFlag, data, offset: idx + offset);
            idx += this.valueConverter.SetUint16(this.NumTest, data, offset: idx + offset);
            idx += this.valueConverter.SetUint16(this.HardBin, data, offset: idx + offset);
            idx += this.valueConverter.SetUint16(this.SoftBin, data, offset: idx + offset);
            idx += this.valueConverter.SetUint16(this.XCoord, data, offset: idx + offset);
            idx += this.valueConverter.SetUint16(this.YCoord, data, offset: idx + offset);
            idx += this.valueConverter.SetUint32(this.TestTime, data, offset: idx + offset);

            bool havePreviousNull = false;
            idx += this.valueConverter.WriteAsciiString(this.PartId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.PartText, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);

            if (PartFix is null)
                havePreviousNull = true;
            else if (havePreviousNull)
            {
                throw new Stdf4ParserException(L["HaveNonNullValue"]);
            }
            else if (PartFix.Length == 0)
            {
                data[idx + offset + 0] = 0;
                idx += 1;
            }
            else if (PartFix.Length > 255)
            {
                throw new Stdf4ParserException(L["ByteArrayTooLong"]);
            }
            else
            {
                data[idx + offset + 0] = (byte)PartFix.Length;
                Array.Copy(PartFix, 0, data, idx + offset + 1, PartFix.Length);
                idx += (ushort)(1 + PartFix.Length);
            }

            this.valueConverter.SetUint16(idx - 4, data, offset: offset);
            return idx;
        }
    }
}
