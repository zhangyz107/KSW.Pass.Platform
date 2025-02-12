using KSW.ATE01.Instrument.IO.BLLs.Implements.Ppmus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerProgram.Template
{
    public static class TemplatePpmu
    {
        /// <summary>
        ///  OS Test Template
        /// </summary>
        /// <param name="pinList"></param>
        /// <param name="forceI"></param>
        /// <param name="outResult"></param>
        /// <param name="wait_ms"></param>
        /// <returns></returns>
        public static bool OS(string pinList, double forceI, int wait_ms = 0, double clampVLow = -1.5, double clampVHi = 2)
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
        public static bool PowerShort(string pinList, double forceV, double forceCurrentRange, double measureCurrentRange, int wait_ms = 3)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Power short test with pins {pinList} and force ({forceCurrentRange},{forceCurrentRange}) with measure current range {measureCurrentRange}A occurred exception, exception message is {ex.Message}");
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
        public static bool ApplyVoltage(string pinList, double forceV, double forceCurrentRange, int wait_ms = 3)
        {
            try
            {
                // mode
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Apply voltage with pins {pinList} and current range {forceCurrentRange}A occurred exception, exception message is {ex.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinList"></param>
        /// <param name="wait_ms"></param>
        /// <returns></returns>
        public static bool Reset(string pinList, int wait_ms = 3)
        {
            try
            {
                // mode
                //Ppmu.Pins(pinList).SetFIMV.FVMV();  // Gate off channel 的状态
                //Ppmu.Pins(pinList).SetVoltage(0);
                //Hardware.Wait(wait_ms);// wait_ms); //Charge power
                //Ppmu.Pins(pinList).Gate.Off();
                //Ppmu.Pins(pinList).Disconnect();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Reset pins {pinList} occurred exception, exception message is {ex.Message}");
            }
        }
    }
}
