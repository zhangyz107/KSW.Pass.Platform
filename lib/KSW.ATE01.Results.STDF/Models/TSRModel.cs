using KSW.ATE01.Results.STDF.Enums;
using KSW.ATE01.Results.STDF.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class TSRModel : STDFBaseModel
    {
        public TSRModel() : base(10, 30)
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
        /// 测试类型
        /// </summary>
        public char TestType { get; set; }

        /// <summary>
        /// 测试编码
        /// </summary>
        public uint TestNum { get; set; }

        /// <summary>
        /// 测试执行次数
        /// </summary>
        public uint ExecutionCount { get; set; }

        /// <summary>
        /// 测试失败次数
        /// </summary>
        public uint FailCount { get; set; }

        /// <summary>
        /// 测试报警次数
        /// </summary>
        public uint AlarmCount { get; set; }

        /// <summary>
        /// 测试项名称
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// 序列器（程序段/流程）名称
        /// </summary>
        public string SequencerName { get; set; }

        /// <summary>
        /// 测试标签或文本
        /// </summary>
        public string TestLabel { get; set; }

        /// <summary>
        /// 可选标志
        /// </summary>
        public byte OptioanlFlag { get; set; }

        /// <summary>
        /// 平均测试执行时间（秒）
        /// </summary>
        public float TestTime { get; set; }

        /// <summary>
        /// 测试结果最小值
        /// </summary>
        public float TestMin { get; set; }

        /// <summary>
        /// 测试结果最大值
        /// </summary>
        public float TestMax { get; set; }

        /// <summary>
        /// 测试结果总和
        /// </summary>
        public float TestSums { get; set; }

        /// <summary>
        /// 测试结果方差
        /// </summary>
        public float TestSqrs { get; set; }

        public override ushort Length()
        {
            ushort length = 40;
            if (this.TestName is null)
            {
                /* no length change */
            }
            else if (this.TestName.Length > 255)
            {
                throw new Stdf4ParserException(L["StringTooLong"]);
            }
            else
            {
                // The extra byte is for the length of the string.
                length += (ushort)(this.TestName.Length + 1);
            }

            if (this.SequencerName is null)
            {
                /* no length change */
            }
            else if (this.SequencerName.Length > 255)
            {
                throw new Stdf4ParserException(L["StringTooLong"]);
            }
            else
            {
                length += (ushort)(this.SequencerName.Length + 1);
            }

            if (this.TestLabel is null)
            {
                /* no length change */
            }
            else if (this.TestLabel.Length > 255)
            {
                throw new Stdf4ParserException(L["StringTooLong"]);
            }
            else
            {
                length += (ushort)(this.TestLabel.Length + 1);
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

            data[offset + idx] = Convert.ToByte(this.TestType);
            idx += 1;

            idx += this.valueConverter.SetUint32(ExecutionCount, data, offset + idx);
            idx += this.valueConverter.SetUint32(FailCount, data, offset + idx);
            idx += this.valueConverter.SetUint32(AlarmCount, data, offset + idx);

            idx += this.valueConverter.WriteAsciiString(this.TestName, data, offset + idx);
            idx += this.valueConverter.WriteAsciiString(this.SequencerName, data, offset + idx);
            idx += this.valueConverter.WriteAsciiString(this.TestLabel, data, offset + idx);

            data[offset + idx] = this.OptioanlFlag;
            idx += 1;

            idx += this.valueConverter.SetUint16(idx - 4, data, offset);
            return idx;
        }
    }
}
