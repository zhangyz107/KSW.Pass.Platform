using KSW.ATE01.Results.STDF.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public class MIRModel : STDFBaseModel
    {
        public MIRModel() : base(1, 10)
        {

        }

        /// <summary>
        /// 作业设置的日期和时间
        /// </summary>
        public DateTime SetupTime { get; set; }

        /// <summary>
        /// 作业设置的时间
        /// </summary>
        public uint SetupT { get => (uint)Math.Abs((SetupTime - baseDateTime).Seconds); }

        /// <summary>
        /// 第一部分测试的日期和时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 第一部分测试的时间
        /// </summary>
        public uint StartT { get => (uint)Math.Abs((StartTime - baseDateTime).Seconds); }

        /// <summary>
        /// 测试站号
        /// </summary>
        public byte StartNum { get; set; }

        /// <summary>
        /// 测试模式代码
        /// </summary>
        public char ModeCode { get; set; }

        /// <summary>
        /// Lot重测代码
        /// </summary>
        public char RetestCode { get; set; }

        /// <summary>
        /// 保护代码
        /// </summary>
        public char ProtectionCode { get; set; }

        /// <summary>
        /// 烧录时间
        /// </summary>
        public ushort BurnTime { get; set; } = 0xFFFF;

        /// <summary>
        /// 命令编码
        /// </summary>
        public char CommandCode { get; set; }

        /// <summary>
        /// Lot编号
        /// </summary>
        public string LotId { get; set; }

        /// <summary>
        /// 部件类型
        /// </summary>
        public string PartType { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 测试器类型
        /// </summary>
        public string TesterType { get; set; }

        /// <summary>
        /// 作业名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 作业修订号
        /// </summary>
        public string JobRevision { get; set; }

        /// <summary>
        /// Sublot ID
        /// </summary>
        public string SubLotId { get; set; }

        /// <summary>
        /// 运营商名称
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 执行软件类型
        /// </summary>
        public string SoftwareType { get; set; }

        /// <summary>
        /// 执行软件版本号
        /// </summary>
        public string SoftwareVersion { get; set; }

        /// <summary>
        /// 测试阶段或步骤代码
        /// </summary>
        public string TestCode { get; set; }

        /// <summary>
        /// 测试温度
        /// </summary>
        public string TestTemperature { get; set; }

        /// <summary>
        /// 通用用户文本
        /// </summary>
        public string UserText { get; set; }

        /// <summary>
        /// 辅助数据文件的名称
        /// </summary>
        public string AuxFile { get; set; }

        /// <summary>
        /// 封装类型
        /// </summary>
        public string PackageType { get; set; }

        /// <summary>
        /// 产品系列 ID
        /// </summary>
        public string FamilyId { get; set; }

        /// <summary>
        /// 日期代码
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 设施编号
        /// </summary>
        public string FacilityId { get; set; }

        /// <summary>
        /// 楼层编号
        /// </summary>
        public string FloorId { get; set; }

        /// <summary>
        /// 制造工艺编号
        /// </summary>
        public string ProcessId { get; set; }

        /// <summary>
        /// 工作频率
        /// </summary>
        public string OperationFrequency { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TestSpecName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TestSpecVersion { get; set; }

        /// <summary>
        /// 测试流程编号
        /// </summary>
        public string TestFlowId { get; set; }

        /// <summary>
        /// 测试流程版本
        /// </summary>
        public string TestSetupId { get; set; }

        /// <summary>
        /// 设备设计版本
        /// </summary>
        public string DeviceDesignRev { get; set; }

        /// <summary>
        /// 测试lot编号
        /// </summary>
        public string EngineeringLotId { get; set; }

        /// <summary>
        /// ROM Code ID
        /// </summary>
        public string RomCodeId { get; set; }

        /// <summary>
        /// 测试器序列号
        /// </summary>
        public string TesterSerialNumber { get; set; }

        /// <summary>
        /// 测试人员 ID
        /// </summary>
        public string SupervisorId { get; set; }

        public override ushort Length()
        {
            int length = 19;

            length += this.LotId == null ? 0 : this.LotId.Length + 1;
            length += this.PartType == null ? 0 : this.PartType.Length + 1;
            length += this.NodeName == null ? 0 : this.NodeName.Length + 1;
            length += this.TesterType == null ? 0 : this.TesterType.Length + 1;
            length += this.JobName == null ? 0 : this.JobName.Length + 1;
            length += this.JobRevision == null ? 0 : this.JobRevision.Length + 1;
            length += this.SubLotId == null ? 0 : this.SubLotId.Length + 1;
            length += this.OperatorName == null ? 0 : this.OperatorName.Length + 1;
            length += this.SoftwareType == null ? 0 : this.SoftwareType.Length + 1;
            length += this.SoftwareVersion == null ? 0 : this.SoftwareVersion.Length + 1;
            length += this.TestCode == null ? 0 : this.TestCode.Length + 1;
            length += this.TestTemperature == null ? 0 : this.TestTemperature.Length + 1;
            length += this.UserText == null ? 0 : this.UserText.Length + 1;
            length += this.AuxFile == null ? 0 : this.AuxFile.Length + 1;
            length += this.PackageType == null ? 0 : this.PackageType.Length + 1;
            length += this.FamilyId == null ? 0 : this.FamilyId.Length + 1;
            length += this.DateCode == null ? 0 : this.DateCode.Length + 1;
            length += this.FacilityId == null ? 0 : this.FacilityId.Length + 1;
            length += this.FloorId == null ? 0 : this.FloorId.Length + 1;
            length += this.ProcessId == null ? 0 : this.ProcessId.Length + 1;
            length += this.OperationFrequency == null ? 0 : this.OperationFrequency.Length + 1;
            length += this.TestSpecName == null ? 0 : this.TestSpecName.Length + 1;
            length += this.TestSpecVersion == null ? 0 : this.TestSpecVersion.Length + 1;
            length += this.TestFlowId == null ? 0 : this.TestFlowId.Length + 1;
            length += this.TestSetupId == null ? 0 : this.TestSetupId.Length + 1;
            length += this.DeviceDesignRev == null ? 0 : this.DeviceDesignRev.Length + 1;
            length += this.EngineeringLotId == null ? 0 : this.EngineeringLotId.Length + 1;
            length += this.RomCodeId == null ? 0 : this.RomCodeId.Length + 1;
            length += this.TesterSerialNumber == null ? 0 : this.TesterSerialNumber.Length + 1;
            length += this.SupervisorId == null ? 0 : this.SupervisorId.Length + 1;

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

            idx += this.valueConverter.SetUint32(this.SetupT, data, offset: idx + offset);
            idx += this.valueConverter.SetUint32(this.StartT, data, offset: idx + offset);
            idx += this.valueConverter.SetByte(this.StartNum, data, offset: idx + offset);
            idx += this.valueConverter.SetAsciiChar(this.ModeCode, data, offset: idx + offset);
            idx += this.valueConverter.SetAsciiChar(this.RetestCode, data, offset: idx + offset);
            idx += this.valueConverter.SetAsciiChar(this.ProtectionCode, data, offset: idx + offset);
            idx += this.valueConverter.SetUint16(this.BurnTime, data, offset: idx + offset);
            idx += this.valueConverter.SetAsciiChar(this.CommandCode, data, offset: idx + offset);

            bool havePreviousNull = false;
            idx += this.valueConverter.WriteAsciiString(this.LotId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.PartType, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.NodeName, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TesterType, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.JobName, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.JobRevision, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.SubLotId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.OperatorName, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.SoftwareType, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.SoftwareVersion, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TestCode, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TestTemperature, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.UserText, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.AuxFile, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.PackageType, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.FamilyId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.DateCode, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.FacilityId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.FloorId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.ProcessId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.OperationFrequency, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TestSpecName, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TestSpecVersion, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TestFlowId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TestSetupId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.DeviceDesignRev, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.EngineeringLotId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.RomCodeId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.TesterSerialNumber, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);
            idx += this.valueConverter.WriteAsciiString(this.SupervisorId, data, havePreviousNull: ref havePreviousNull, offset: idx + offset);

            this.valueConverter.SetUint16(idx - 4, data, offset: offset);
            return idx;
        }
    }
}
