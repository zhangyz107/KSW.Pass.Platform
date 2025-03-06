using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Services.Memory
{
    public class ShareMemoryInTestPlan<T> : ShareMemoryObjectBase<T>
    {
        private static object _lock = new object();
        private static ShareMemoryInTestPlan<T> _instance;

        public static ShareMemoryInTestPlan<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ShareMemoryInTestPlan<T>();
                        }
                    }
                }
                return _instance;
            }
        }

        public override string ShareMemoryName
        {
            get
            {
                return "TestPlanShareMemory";
            }
        }

        public override int ShareMemorySize
        {
            get
            {
                return 64 * 1024 * 1024;
            }
        }

    }
}
