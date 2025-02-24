using KSW.ATE01.Project.Base.Services.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    public class ATE01ShareMemory
    {
        private static MemoryTemplate _memoryTemplate = new MemoryTemplate();
        private static int _memorySize = 1024 * 1024 * 16;

        public static bool CreateShareMemory()
        {
            return _memoryTemplate.CreateMemory("ATE01ShareMemory", _memorySize) && OpenShareMemory();
        }

        public static bool OpenShareMemory()
        {
            return _memoryTemplate.OpenMemory("ATE01ShareMemory");
        }

        public static string LoadedTestPlanFilePath
        {
            get
            {
                string result;
                GetWaveKitDataFromMemory(nameof(LoadedTestPlanFilePath), out result);
                return result;
            }
            set
            {
                AppendOrUpdateWaveKitDataToMemory(nameof(LoadedTestPlanFilePath), value);
            }
        }

        public static string TestPlanFilePath
        {
            get
            {
                string result;
                GetWaveKitDataFromMemory(nameof(TestPlanFilePath), out result);
                return result;
            }
            set
            {
                AppendOrUpdateWaveKitDataToMemory(nameof(TestPlanFilePath), value);
            }
        }

        private static void AppendOrUpdateWaveKitDataToMemory(string memoryBlockName, string strValue)
        {
            byte[] bytes = Encoding.Default.GetBytes(strValue);
            Array.Resize<byte>(ref bytes, 262144);
            _memoryTemplate.AppendOrUpdateDataToMemory(memoryBlockName, bytes);
        }

        private static void GetWaveKitDataFromMemory(string memoryBlockName, out string strValue)
        {
            byte[] bytes;
            _memoryTemplate.GetDataFromMemory(memoryBlockName, out bytes);
            strValue = Encoding.Default.GetString(bytes).Trim(new char[1]);
        }
    }
}
