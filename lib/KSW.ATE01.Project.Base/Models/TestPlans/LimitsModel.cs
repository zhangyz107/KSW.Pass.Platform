using KSW.ATE01.Project.Base.Enums.Results;
using System;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 电压限制模型
    /// </summary>
    [Serializable]
    public class LimitsModel
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 测试项Id
        /// </summary>
        public Guid TestItemId { get; set; }

        /// <summary>
        /// 测试项名
        /// </summary>       
        public string TestItemName { get; set; }

        /// <summary>
        /// 电压限制名称
        /// </summary>
        public string LimitName { get; set; }

        /// <summary>
        /// 站点
        /// </summary>
        public int Site { get; set; }

        /// <summary>
        /// 测试编号
        /// </summary>
        public uint TestNumber { get; set; }

        /// <summary>
        /// 电压下限
        /// </summary>
        public double LowLimit { get; set; }

        /// <summary>
        /// 电压上限
        /// </summary>
        public double HighLimit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// 软件Bin号
        /// </summary>
        public uint FailSoftwareBin { get; set; }

        /// <summary>
        /// 软件Bin号
        /// </summary>
        public uint PassSoftwareBin { get; set; }

        /// <summary>
        /// 硬件Bin号
        /// </summary>
        public uint FailHardwareBin { get; set; }

        /// <summary>
        /// 硬件Bin号
        /// </summary>
        public uint PassHardwareBin { get; set; }

        /// <summary>
        /// 测试结果
        /// </summary>
        public Test DUTResult { get; set; }
    }
}
