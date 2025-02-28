using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Domain.Projects.Core.Enums
{
    public enum FlowStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 1,

        /// <summary>
        /// 程序集加载成功
        /// </summary>
        AssemblyLoadSucceed = 2,

        /// <summary>
        /// 程序集加载失败
        /// </summary>
        AssemblyLoadFailed = 3,

        /// <summary>
        /// 测试开始执行成功
        /// </summary>
        TestStartExecuteSucceed = 4,

        /// <summary>
        /// 测试开始执行失败
        /// </summary>
        TestStartExecuteFailed = 5,

        /// <summary>
        /// 测试结束执行成功
        /// </summary>
        TestEndExecuteSucceed = 6,

        /// <summary>
        /// 测试结束执行失败
        /// </summary>
        TestEndExecuteFailed = 7,

        /// <summary>
        /// 流程开始执行成功
        /// </summary>
        FlowStartExecuteSucceed = 8,

        /// <summary>
        /// 流程开始执行失败
        /// </summary>
        FlowStartExecuteFailed = 9,

        /// <summary>
        /// 流程结束执行成功
        /// </summary>
        FlowEndExecuteSucceed = 10,

        /// <summary>
        /// 流程结束执行失败
        /// </summary>
        FlowEndExecuteFailed = 11,

        /// <summary>
        /// 执行测试项失败
        /// </summary>
        ExecuteTestItemFail = 12,
    }
}
