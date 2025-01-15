using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Domain.Projects.Core.Enums
{
    /// <summary>
    /// 向量值类型
    /// </summary>
    public enum VectorValueType
    {
        /// <summary>
        /// 0
        /// </summary>
        [Description("0")]
        Zero = 1,

        /// <summary>
        /// 1
        /// </summary>
        [Description("1")]
        One = 2,

        /// <summary>
        /// L
        /// </summary>
        [Description("L")]
        L = 3,

        /// <summary>
        /// H
        /// </summary>
        [Description("H")]
        H = 4,

        /// <summary>
        /// M
        /// </summary>
        [Description("M")]
        M = 5,

        /// <summary>
        /// X
        /// </summary>
        [Description("X")]
        X = 6,

        /// <summary>
        /// V
        /// </summary>
        [Description("V")]
        V = 7,

        /// <summary>
        /// -
        /// </summary>
        [Description("-")]
        None =8,

        /// <summary>
        /// 0L
        /// </summary>
        [Description("0L")]
        ZeroL,

        /// <summary>
        /// 0H
        /// </summary>
        [Description("0H")]
        ZeroH,

        /// <summary>
        /// 1L
        /// </summary>
        [Description("1L")]
        OneL,

        /// <summary>
        /// 1H
        /// </summary>
        [Description("1H")]
        OneH,

        /// <summary>
        /// D
        /// </summary>
        [Description("D")]
        D,

        /// <summary>
        /// C
        /// </summary>
        [Description("C")]
        C,
    }
}
