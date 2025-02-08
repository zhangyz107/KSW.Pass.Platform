using System;

namespace KSW.ATE01.Project.Base.Models.TestPlans
{
    /// <summary>
    /// 电压模型
    /// </summary>
    public class LevelModel
    {
        /// <summary>
        /// 标识符
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string PinGroupName { get; set; }

        /// <summary>
        /// 输入低电压
        /// </summary>
        public decimal Vil { get; set; }

        /// <summary>
        /// 输入高电压
        /// </summary>
        public decimal Vih { get; set; }

        /// <summary>
        /// 输出低电压
        /// </summary>
        public decimal Vol { get; set; }

        /// <summary>
        /// 输出高电压
        /// </summary>
        public decimal Voh { get; set; }
        /// <summary>
        /// 输出低电流
        /// </summary>
        public decimal Iol { get; set; }

        /// <summary>
        /// 输出高电流
        /// </summary>
        public decimal Ioh { get; set; }

        /// <summary>
        /// 电压基准
        /// </summary>
        public decimal Vt { get; set; }

        /// <summary>
        /// 钳位低电压
        /// </summary>
        public decimal Vcl { get; set; }

        /// <summary>
        /// 钳位高电压
        /// </summary>
        public decimal Vch { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal PS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal I { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Tdelay { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Comment { get; set; }
    }
}
