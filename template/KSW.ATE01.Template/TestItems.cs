using KSW.ATE01.Project.Base.Enums.Results;
using KSW.ATE01.Project.Base.Models;
using System.Windows;

namespace CustomerProgram
{
    public class TestItems
    {
        /// <summary>
        /// 批次测试开始时调用。每批次测试只调用一次
        /// Call "TestStart" before lot test, executing once for each lot.
        /// </summary>
        public Test TestStart()
        {
            try
            {
                var commonData = CommonData.Instance;
                if (commonData != null && commonData.TestPlan != null)
                {
                    var patternFiles = commonData.TestPlan.Global?.Select(x => x.PatternFile).ToArray();
                    TemplateDigital.LoadPatterns(patternFiles);
                }

                return Test.Pass;
            }
            catch (Exception)
            {
                return Test.Fail;
            }
        }

        /// <summary>
        /// 批次测试结束时调用。每批次测试只调用一次。
        /// Call "TestEnd" after lot test, executing once for each lot.
        /// </summary>
        public Test TestEnd()
        {
            try
            {
                return Test.Pass;
            }
            catch (Exception)
            {

                return Test.Fail;
            }
        }

        /// <summary>
        /// 每次Flow测试之前调用。
        /// Call "FlowStart" before flow execution, once for each flow.
        /// </summary>
        public Test FlowStart()
        {
            try
            {

                return Test.Pass;
            }
            catch (Exception)
            {

                return Test.Fail;
            }
        }

        /// <summary>
        /// 每次Flow测试之后调用。
        /// Call "FlowEnd" after flow execution, once for each flow.
        /// </summary>
        public Test FlowEnd()
        {
            try
            {
                return Test.Pass;
            }
            catch (Exception)
            {
                return Test.Fail;
            }
        }

        /*
         *
         * 
         * CPCode
         * 
         * 
         * 
         */


    }
}
