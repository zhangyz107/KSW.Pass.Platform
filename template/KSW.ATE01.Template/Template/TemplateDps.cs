using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerProgram.Template
{
    public static class TemplateDps
    {
        /// <summary>
        ///  OS Test Template
        /// </summary>
        /// <param name="pinList"></param>
        /// <param name="forceI"></param>
        /// <param name="outResult"></param>
        /// <param name="wait_ms"></param>
        /// <returns></returns>
        public static bool OS(string pinList, double forceI, int wait_ms = 15, double clampILow = -200e-6, double clampIHi = 200e-6, double forceV = -3)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"OS test with pins {pinList} and forceI {forceI}A occurred exception, exception message is {ex.Message}");
            }
        }

        /// <summary>
        ///  OS Test Template
        /// </summary>
        /// <param name="pinList"></param>
        /// <param name="forceI"></param>
        /// <param name="outResult"></param>
        /// <param name="wait_ms"></param>
        /// <returns></returns>
        public static bool PowerShort(string pinList, double forceV, double forceI, double measureI, int wait_ms = 15)
        {
            try
            {
                // mode             
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"OS test with pins {pinList} and forceI {forceI}A occurred exception, exception message is {ex.Message}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinList"></param>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public static bool ApplyVoltage(string pinList, double voltage, double current, long wait_ms = 3)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                throw new Exception($"");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinList"></param>
        /// <param name="forceV"></param>
        /// <param name="forceCurrentRange"></param>
        /// <param name="wait_ms"></param>
        /// <param name="relayMode"></param>
        /// <returns></returns>
        public static bool Reset(string pinList, double dischargeCurrent = 25e-3, int wait_ms = 3)
        {
            try
            {
                // mode               
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Reset pins {pinList} occurred exception, exception message is {ex.Message}");
            }
        }
    }
}
