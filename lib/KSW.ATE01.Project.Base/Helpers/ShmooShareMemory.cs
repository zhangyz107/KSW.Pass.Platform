using KSW.ATE01.Project.Base.Services.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    public class ShmooShareMemory<T> : ShareMemoryObjectBase<T>
    {
        private static object _locker = new object();
        private static ShmooShareMemory<T> _instance;

        public static ShmooShareMemory<T> Instance
        {
            get
            {
                if (_instance == null)
                    lock (_locker)
                        if (_instance == null)
                            _instance = new ShmooShareMemory<T>();

                return _instance;
            }
        }

        public override string ShareMemoryName => "ShmooShareMemory";

        public override int ShareMemorySize => 16 * 1024 * 1024;
    }
}
