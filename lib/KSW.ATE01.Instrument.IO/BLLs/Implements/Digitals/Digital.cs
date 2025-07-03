using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Digitals;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Patterns;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Enums.TestPlans;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Models.Errors;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.ATE01.Project.Base.Services.Loggers;
using System.IO;
using System.Windows.Ink;
using Convert = System.Convert;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Digitals
{
    public class Digital : PE131CommandBase<Digital>, IDigital
    {
        private const double _periodResolution = 6.25e-10;
        private const double _ns = 1e-9;
        private const string _timingCalibrationFile = "Timing.ini";

        public static IDigital Pins(string pinList)
        {
            if (string.IsNullOrEmpty(pinList))
                ErrorMessages.DpsPpmu.PinListIsNullOrEmpty();

            Instance.GetPinList(pinList);

            return Instance;
        }

        public void SetTimingDetail(double period, double driveA, double driveB, double driveC, double driveD, Timingformat fmt, StrobeModeType strobeMode, double strobeA, double strobeB)
        {
            if (PinList == null || !PinList.Any())
                return;

            var periodDouble = period * _ns;

            if (periodDouble < 0 || periodDouble > 2.684354559375)
                throw new ArgumentOutOfRangeException("Period", "取值范围:0~2.684354559375s");

            var periodUInt = Convert.ToInt32(periodDouble / _periodResolution);
            var periodBytes = BitConverter.GetBytes(periodUInt).Reverse();

            double driveADouble = driveA * _ns;
            var driveAUInt = Convert.ToInt32(0 / _periodResolution);    //默认给0
            var driveABytes = BitConverter.GetBytes(driveAUInt).Reverse();

            double driveBDouble = (driveB - driveA) * _ns;
            var driveBUInt = Convert.ToInt32(driveBDouble / _periodResolution);
            var driveBBytes = BitConverter.GetBytes(driveBUInt).Reverse();

            double driveCDouble = (driveC - driveA) * _ns;
            var driveCUInt = Convert.ToInt32(driveCDouble / _periodResolution);
            var driveCBytes = BitConverter.GetBytes(driveCUInt).Reverse();

            double driveDDouble = (driveD - driveA) * _ns;
            var driveDUInt = Convert.ToInt32(driveDDouble / _periodResolution);
            var driveDBytes = BitConverter.GetBytes(driveDUInt).Reverse();

            var formatByte = Convert.ToByte(fmt);
            var strobeByte = Convert.ToByte(strobeMode);

            double r0Period = strobeA * _ns;
            var r0PeriodUInt = Convert.ToInt32(0 / _periodResolution);   //默认给0
            var r0PeriodBytes = BitConverter.GetBytes(r0PeriodUInt).Reverse();

            double r1Period = (strobeB - strobeA) * _ns;
            var r1PeriodUInt = Convert.ToInt32(r1Period / _periodResolution);
            var r1PeriodBytes = BitConverter.GetBytes(r1PeriodUInt).Reverse();

            var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

            try
            {
                var controlService = Instance.ControlService;

                foreach (var pin in PinList)
                {
                    foreach (var site in pin.Sites)
                    {
                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                            continue;

                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                        List<CommandInfoModel> commandList = new List<CommandInfoModel>();
                        if (!commandListDic.ContainsKey(slot))
                            commandListDic[slot] = commandList;
                        else
                            commandList = commandListDic[slot];

                        if (channelNum >= 0)
                        {
                            #region 应用校准数据

                            //校准数据归0
                            sbyte pwa_en = 0;
                            byte cd_en = 0;
                            ushort fd_en = 0;
                            sbyte pwa_d = 0;
                            byte cd_d = 0;
                            ushort fd_d = 0;
                            sbyte pwa_ca = 0;
                            byte cd_ca = 0;
                            ushort fd_ca = 0;
                            sbyte pwa_cb = 0;
                            byte cd_cb = 0;
                            ushort fd_cb = 0;
                            byte d_d_d = 0;
                            byte en_d_d = 0;
                            int den_d_c = 0;
                            byte ca_d_d = 0;
                            byte cb_d_d = 0;
                            int cab_d_c = 0;

                            ApplyCalibrationData(period, driveA, strobeA, ref cd_en, ref fd_en, ref cd_d, ref fd_d, ref cd_ca, ref fd_ca, ref cd_cb, ref fd_cb, ref d_d_d, ref en_d_d, ref den_d_c, ref ca_d_d, ref cb_d_d, ref cab_d_c, channelNum, slot);
                            #endregion

                            var fd_enBytes = BitConverter.GetBytes(fd_en).Reverse();
                            var fd_dBytes = BitConverter.GetBytes(fd_d).Reverse();
                            var fd_caBytes = BitConverter.GetBytes(fd_ca).Reverse();
                            var fd_cbBytes = BitConverter.GetBytes(fd_cb).Reverse();
                            var den_dcBytes = BitConverter.GetBytes(den_d_c).Reverse();
                            var cab_dcBytes = BitConverter.GetBytes(cab_d_c).Reverse();

                            var byteList = new List<byte>();
                            byteList.Add((byte)channelNum);        //暂时按顺序下发通道（后续需要映射站点信息）
                            byteList.AddRange(periodBytes);
                            byteList.Add(formatByte);
                            byteList.Add(strobeByte);
                            byteList.AddRange(driveABytes);   //D0
                            byteList.AddRange(driveBBytes);   //D1
                            byteList.AddRange(driveCBytes);   //D2
                            byteList.AddRange(driveDBytes);   //D3
                            byteList.AddRange(r0PeriodBytes); //R0
                            byteList.AddRange(r1PeriodBytes); //R1
                            byteList.Add((byte)pwa_en);       //pwa_en
                            byteList.Add(cd_en);              //cd_en
                            byteList.AddRange(fd_enBytes);    //fd_en
                            byteList.Add((byte)pwa_d);        //pwa_d
                            byteList.Add(cd_d);               //cd_d
                            byteList.AddRange(fd_dBytes);     //fd_d
                            byteList.Add((byte)pwa_ca);       //pwa_ca
                            byteList.Add(cd_ca);              //cd_ca
                            byteList.AddRange(fd_caBytes);    //fd_ca
                            byteList.Add((byte)pwa_cb);       //pwa_cb
                            byteList.Add(cd_cb);              //cd_cb
                            byteList.AddRange(fd_cbBytes);    //fd_cb
                            byteList.Add(d_d_d);              //d_d_d
                            byteList.Add(en_d_d);              //en_d_d
                            byteList.AddRange(den_dcBytes);    //den_d_c
                            byteList.Add(ca_d_d);              //ca_d_d
                            byteList.Add(cb_d_d);              //cb_d_d
                            byteList.AddRange(cab_dcBytes);    //cab_d_c
                            var command = new CommandInfoModel()
                            {
                                CommandCode = "0x010A",
                                CommandContent = byteList.ToArray(),
                            };
                            commandList.Add(command);

#if DEBUG
                            var stringFormat = "CH_NUM:{0},Period:{1},Format:{2},Strobe:{3},D0:{4},D1:{5},D2:{6},D3:{7},R0:{8},R1:{9},PWA_EN:{10},CD_EN:{11},FD_EN:{12},PWA_D:{13},CD_D:{14},FD_D:{15},PWA_CA:{16},CD_CA:{17},FD_CA:{18},PWA_CB:{19},CD_CB:{20},FD_CB:{21},D_D_D:{22},EN_D_D:{23},DEN_D_C:{24},CA_D_D:{25},CB_D_D:{26},CAB_D_C:{27}";
                            PrintResultLog.Message(string.Format(stringFormat, channelNum, periodUInt, formatByte, strobeByte, driveAUInt, driveBUInt, driveCUInt, driveDUInt, r0PeriodUInt, r1PeriodUInt, pwa_en, cd_en, fd_en, pwa_d, cd_d, fd_d, pwa_ca, cd_ca, fd_ca, pwa_cb, cd_cb, fd_cb, d_d_d, en_d_d, den_d_c, ca_d_d, cb_d_d, cab_d_c));
#endif
                        }
                    }
                }

                if (commandListDic.Any())
                {
                    foreach (var commandList in commandListDic)
                    {
                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Configuration, commandList.Value);

                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, slotNum);

                        if (controlService != null && instrumentInfo != null)
                            controlService.Send(instrumentInfo, message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void ApplyCalibrationData(double period, double driveA, double strobeA, ref byte cd_en, ref ushort fd_en, ref byte cd_d, ref ushort fd_d, ref byte cd_ca, ref ushort fd_ca, ref byte cd_cb, ref ushort fd_cb, ref byte d_d_d, ref byte en_d_d, ref int den_d_c, ref byte ca_d_d, ref byte cb_d_d, ref int cab_d_c, int channelNum, int slot)
        {
            var timingList = CalibrationHelper.GetTimingCalibrations(Path.Combine(CalibrationHelper.CalDirectory, _timingCalibrationFile));

            var max_tx_dly = 0.0;
            var cal_cd_d_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Cd_d && x.SlotNum == slot)?.Values;
            var cal_cd_en_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Cd_en && x.SlotNum == slot)?.Values;
            var cal_cd_ca_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Cd_ca && x.SlotNum == slot)?.Values;
            var cal_cd_cb_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Cd_cb && x.SlotNum == slot)?.Values;
            var cal_fd_d_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Fd_d && x.SlotNum == slot)?.Values;
            var cal_fd_en_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Fd_en && x.SlotNum == slot)?.Values;
            var cal_fd_ca_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Fd_ca && x.SlotNum == slot)?.Values;
            var cal_fd_cb_list = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Fd_cb && x.SlotNum == slot)?.Values;
            var datList = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.DAT && x.SlotNum == slot)?.Values;
            var enList = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.EN && x.SlotNum == slot)?.Values;
            var caList = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.CA && x.SlotNum == slot)?.Values;
            var cbList = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.CB && x.SlotNum == slot)?.Values;
            var cableList = timingList.FirstOrDefault(x => x.CalibrationType == Project.Base.Enums.Calibrations.TimingCalibrationType.Cable && x.SlotNum == slot)?.Values;

            if (datList != null && cableList != null)
            {
                var dataLength = datList.Count > cableList.Count ? cableList.Count : datList.Count;

                for (int i = 0; i < dataLength; i++)
                {
                    var tempDatData = datList[i] + cableList[i];
                    if (max_tx_dly < tempDatData)
                        max_tx_dly = tempDatData;
                }
            }

            if (enList != null && cableList != null)
            {
                var dataLength = enList.Count > cableList.Count ? cableList.Count : enList.Count;

                for (int i = 0; i < dataLength; i++)
                {
                    var tempDatData = enList[i] + cableList[i];
                    if (max_tx_dly < tempDatData)
                        max_tx_dly = tempDatData;
                }
            }

            var cfg_tx_dly = max_tx_dly + period;
            if (datList != null && datList.Count > channelNum && enList != null && enList.Count > channelNum && cableList != null && cableList.Count > channelNum)
            {
                var dat_dly_ps = 1000 * (cfg_tx_dly - datList[channelNum] - cableList[channelNum] + driveA);
                var en_dly_ps = 1000 * (cfg_tx_dly - enList[channelNum] - cableList[channelNum] + driveA);

                CaculateDriveDelay(channelNum, 1.0 / _periodResolution, dat_dly_ps, en_dly_ps, cal_cd_d_list, cal_cd_en_list, cal_fd_d_list, cal_fd_en_list, ref cd_d, ref fd_d, ref cd_en, ref fd_en, ref d_d_d, ref en_d_d, ref den_d_c);

            }

            if (caList != null && caList.Count > channelNum && cbList != null && cbList.Count > channelNum && cableList != null && cableList.Count > channelNum)
            {
                var ca_dly_ps = 1000 * (cfg_tx_dly + caList[channelNum] + cableList[channelNum] + strobeA);
                var cb_dly_ps = 1000 * (cfg_tx_dly + cbList[channelNum] + cableList[channelNum] + strobeA);

                CaculateComparatorDelay(channelNum, 1.0 / _periodResolution, ca_dly_ps, cb_dly_ps, cal_cd_ca_list, cal_cd_cb_list, cal_fd_ca_list, cal_fd_cb_list, ref cd_ca, ref fd_ca, ref cd_cb, ref fd_cb, ref ca_d_d, ref cb_d_d, ref cab_d_c);
            }
        }

        private static void CaculateDriveDelay(int channelNum, double iorate_hz, double dat_dly_ps, double en_dly_ps, List<double> cal_cd_d_list, List<double> cal_cd_en_list, List<double> cal_fd_d_list, List<double> cal_fd_en_list, ref byte cd_d, ref ushort fd_d, ref byte cd_en, ref ushort fd_en, ref byte d_d_d, ref byte en_d_d, ref int den_d_c)
        {
            double dly_ps_1dat = 1 / iorate_hz * 1000000000000.0;
            double dly_ps_1clk = dly_ps_1dat * 8;
            double min_dat_en_dly_ps, dat_dly_ps_need, en_dly_ps_need;

            if (dat_dly_ps > en_dly_ps)
                min_dat_en_dly_ps = en_dly_ps;
            else
                min_dat_en_dly_ps = dat_dly_ps;

            while (((min_dat_en_dly_ps - (den_d_c + 1) * dly_ps_1clk) >= 0) && (den_d_c < 1023))
            {
                den_d_c += 1;
            }

            dat_dly_ps_need = dat_dly_ps - den_d_c * dly_ps_1clk;
            en_dly_ps_need = en_dly_ps - den_d_c * dly_ps_1clk;

            while (((dat_dly_ps_need - (d_d_d + 1) * dly_ps_1dat) >= 0) && (d_d_d < 31))
            {
                d_d_d += 1;
            }
            while (((en_dly_ps_need - (en_d_d + 1) * dly_ps_1dat) >= 0) && (en_d_d < 31))
            {
                en_d_d += 1;
            }
            dat_dly_ps_need = dat_dly_ps_need - d_d_d * dly_ps_1dat;
            en_dly_ps_need = en_dly_ps_need - d_d_d * dly_ps_1dat;

            if (cal_cd_d_list != null && cal_cd_d_list.Count > 0)
            {
                while ((cd_d < 63) && ((dat_dly_ps_need - (cal_cd_d_list[(cd_d + 1) * 128 + channelNum] - cal_cd_d_list[channelNum]) * 1000) >= 0))
                {
                    if (cal_cd_d_list.Count < (cd_d + 2) * 128 + channelNum)
                        break;
                    cd_d += 1;
                }
                dat_dly_ps_need = dat_dly_ps_need - (cal_cd_d_list[cd_d * 128 + channelNum] - cal_cd_d_list[channelNum]) * 1000;
            }

            if (cal_cd_en_list != null && cal_cd_en_list.Count > 0)
            {
                while ((cd_en < 63) && ((en_dly_ps_need - (cal_cd_en_list[(cd_en + 1) * 128 + channelNum] - cal_cd_en_list[channelNum]) * 1000) >= 0))
                {
                    if (cal_cd_en_list.Count < (cd_en + 2) * 128 + channelNum)
                        break;
                    cd_en += 1;
                }
                en_dly_ps_need = en_dly_ps_need - (cal_cd_en_list[cd_en * 128 + channelNum] - cal_cd_en_list[channelNum]) * 1000;
            }

            if (cal_fd_d_list != null && cal_fd_d_list.Count > 0)
            {
                while ((fd_d < 63) && ((dat_dly_ps_need - (cal_fd_d_list[(fd_d + 1) * 128 + channelNum] - cal_fd_d_list[channelNum]) * 1000)) >= 0)
                {
                    if (cal_fd_d_list.Count < (fd_d + 2) * 128 + channelNum)
                        break;
                    fd_d += 1;
                }
                dat_dly_ps_need = dat_dly_ps_need - (cal_fd_d_list[fd_d * 128 + channelNum] - cal_fd_d_list[channelNum]) * 1000;
            }

            if (cal_fd_en_list != null && cal_fd_en_list.Count > 0)
            {
                while ((fd_en < 63) && ((en_dly_ps_need - (cal_fd_en_list[(fd_en + 1) * 128 + channelNum] - cal_fd_en_list[channelNum]) * 1000)) >= 0)
                {
                    if (cal_fd_en_list.Count < (fd_en + 2) * 128 + channelNum)
                        break;

                    fd_en += 1;
                }
                en_dly_ps_need = en_dly_ps_need - (cal_fd_en_list[fd_en * 128 + channelNum] - cal_fd_en_list[channelNum]) * 1000;
            }
        }

        private static void CaculateComparatorDelay(int channelNum, double iorate_hz, double ca_dly_ps, double cb_dly_ps, List<double> cal_cd_ca_list, List<double> cal_cd_cb_list, List<double> cal_fd_ca_list, List<double> cal_fd_cb_list, ref byte cd_ca, ref ushort fd_ca, ref byte cd_cb, ref ushort fd_cb, ref byte ca_d_d, ref byte cb_d_d, ref int cab_d_c)
        {
            double dly_ps_1dat = 1 / iorate_hz * 1000000000000.0;
            double dly_ps_1clk = dly_ps_1dat * 8;
            double min_cab_dly_ps, ca_dly_ps_need, cb_dly_ps_need;

            if (ca_dly_ps > cb_dly_ps)
                min_cab_dly_ps = cb_dly_ps;
            else
                min_cab_dly_ps = ca_dly_ps;

            while (((min_cab_dly_ps - (cab_d_c + 1) * dly_ps_1clk) >= 0) && (cab_d_c < 1023))
            {
                cab_d_c += 1;
            }
            ca_dly_ps_need = ca_dly_ps - cab_d_c * dly_ps_1clk;
            cb_dly_ps_need = cb_dly_ps - cab_d_c * dly_ps_1clk;

            while (((ca_dly_ps_need > ca_d_d * dly_ps_1dat)) && (ca_d_d < 30))
            {
                ca_d_d += 1;
            }
            while (((cb_dly_ps_need > cb_d_d * dly_ps_1dat)) && (cb_d_d < 30))
            {
                cb_d_d += 1;
            }
            ca_dly_ps_need = ca_d_d * dly_ps_1dat - ca_dly_ps_need;
            cb_dly_ps_need = cb_d_d * dly_ps_1dat - cb_dly_ps_need;

            if (cal_cd_ca_list != null && cal_cd_ca_list.Count > 0)
            {
                while ((cd_ca < 63) && ((ca_dly_ps_need - (cal_cd_ca_list[(cd_ca + 1) * 128 + channelNum] - cal_cd_ca_list[channelNum]) * 1000) >= 0))
                {
                    if (cal_cd_ca_list.Count < (cd_ca + 2) * 128 + channelNum)
                        break;
                    cd_ca += 1;
                }
                ca_dly_ps_need = ca_dly_ps_need - (cal_cd_ca_list[cd_ca * 128 + channelNum] - cal_cd_ca_list[0 + channelNum]) * 1000;
            }

            if (cal_cd_cb_list != null && cal_cd_cb_list.Count > 0)
            {
                while ((cd_cb < 63) && ((cb_dly_ps_need - (cal_cd_cb_list[(cd_cb + 1) * 128 + channelNum] - cal_cd_cb_list[channelNum]) * 1000) >= 0))
                {
                    if (cal_cd_cb_list.Count < (cd_cb + 2) * 128 + channelNum)
                        break;
                    cd_cb += 1;
                }
                cb_dly_ps_need = cb_dly_ps_need - (cal_cd_cb_list[cd_cb * 128 + channelNum] - cal_cd_cb_list[0 + channelNum]) * 1000;
            }

            if (cal_fd_ca_list != null && cal_fd_ca_list.Count > 0)
            {
                while ((fd_ca < 63) && ((ca_dly_ps_need - (cal_fd_ca_list[(fd_ca + 1) * 128 + channelNum] - cal_fd_ca_list[channelNum]) * 1000)) >= 0)
                {
                    if (cal_fd_ca_list.Count < (fd_ca + 2) * 128 + channelNum)
                        break;
                    fd_ca += 1;
                }
                ca_dly_ps_need = ca_dly_ps_need - (cal_fd_ca_list[fd_ca * 128 + channelNum] - cal_fd_ca_list[channelNum]) * 1000;
            }

            if (cal_fd_cb_list != null && cal_fd_cb_list.Count > 0)
            {
                while ((fd_cb < 63) && ((cb_dly_ps_need - (cal_fd_cb_list[(fd_cb + 1) * 128 + channelNum] - cal_fd_cb_list[channelNum]) * 1000)) >= 0)
                {
                    if (cal_fd_cb_list.Count < (fd_cb + 2) * 128 + channelNum)
                        break;
                    fd_cb += 1;
                }
                cb_dly_ps_need = cb_dly_ps_need - (cal_fd_cb_list[fd_cb * 128 + channelNum] - cal_fd_cb_list[channelNum]) * 1000;
            }
        }

        public void SetTimingByPins()
        {
            if (PinList == null || !PinList.Any())
                return;

            var commonData = CommonData.Instance;
            if (commonData == null || commonData.TestPlan == null || commonData.TestPlan?.Channel == null || commonData.TestPlan?.TestItem == null)
                return;

            if (!string.IsNullOrEmpty(commonData.Timing))
            {
                SetTimingDetail(PinList);
            }
        }

        private static void SetTimingDetail(List<ChannelModel> pinList = null)
        {
            var commonData = CommonData.Instance;
            var testItem = commonData.TestPlan?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(commonData.TestItemName));
            var args = commonData.TestItemArgs;

            if (pinList == null || !pinList.Any())
                pinList = commonData.TestPlan.Channel;

            if (args.Any())
            {
                var patternFileName = args.FirstOrDefault()?.ParamValue;
                var patterns = Pattern.Instance.Patterns;
                var patternFile = patterns.FirstOrDefault(x => x.PatternFileName.Equals(patternFileName));
                var timings = testItem?.Timings;
                if (patternFile != null)
                {
                    var patternTimings = timings?.Where(x => patternFile.TimingSets.Contains(x.TimingName));

                    if (patternTimings.Any())
                    {
                        try
                        {
                            var controlService = Instance.ControlService;

                            foreach (var patternTiming in patternTimings)
                            {
                                var currentPinList = pinList.Where(x => x.Groups.Any(y => y.Name.ToLower().Equals(patternTiming.PinName.ToLower()))).ToList();
                                if (currentPinList == null || !currentPinList.Any())
                                    currentPinList = pinList.Where(x => x.PinName.ToLower().Equals(patternTiming.PinName.ToLower())).ToList();

                                var period = patternTiming.Period * _ns;

                                if (period < 0 || period > 2.684354559375)
                                    throw new ArgumentOutOfRangeException("Period", "取值范围:取值范围:0~2.684354559375s");

                                var periodUInt = Convert.ToInt32(period / _periodResolution);
                                var periodBytes = BitConverter.GetBytes(periodUInt).Reverse();

                                double driveA = 0;
                                double.TryParse(patternTiming.DriveA, out driveA);
                                var driveAUInt = Convert.ToInt32(0 / _periodResolution);     //默认给0        
                                var driveABytes = BitConverter.GetBytes(driveAUInt).Reverse();

                                double driveB = 0;
                                double.TryParse(patternTiming.DriveB, out driveB);
                                driveB = driveB - driveA;
                                var driveBUInt = Convert.ToInt32(driveB * _ns / _periodResolution);
                                var driveBBytes = BitConverter.GetBytes(driveBUInt).Reverse();

                                double driveC = 0;
                                double.TryParse(patternTiming.DriveC, out driveC);
                                driveC = driveC - driveA;
                                var driveCUInt = Convert.ToInt32(driveC * _ns / _periodResolution);
                                var driveCBytes = BitConverter.GetBytes(driveCUInt).Reverse();

                                double driveD = 0;
                                double.TryParse(patternTiming.DriveD, out driveD);
                                driveD = driveD - driveA;
                                var driveDUInt = Convert.ToInt32(driveD * _ns / _periodResolution);
                                var driveDBytes = BitConverter.GetBytes(driveDUInt).Reverse();

                                var formatByte = Convert.ToByte(patternTiming.Fmt);
                                var strobeByte = Convert.ToByte(patternTiming.StrobeMode);

                                var r0PeriodUInt = Convert.ToInt32(0 / _periodResolution);   //默认给0
                                var r0PeriodBytes = BitConverter.GetBytes(r0PeriodUInt).Reverse();

                                double r1Period = (patternTiming.StrobeB - patternTiming.StrobeA) * _ns;
                                var r1PeriodUInt = Convert.ToInt32(r1Period / _periodResolution);
                                var r1PeriodBytes = BitConverter.GetBytes(r1PeriodUInt).Reverse();

                                var commandListDic = new Dictionary<int, List<CommandInfoModel>>();

                                foreach (var pin in currentPinList)
                                {
                                    foreach (var site in pin.Sites)
                                    {
                                        if (!ChannelManagerHelper.IsSiteValid(site.SiteName))
                                            continue;

                                        var channelNum = ChannelManagerHelper.GetChannelNumSiteInfo(site.SiteValue, out int slot);

                                        List<CommandInfoModel> commandList = new List<CommandInfoModel>();
                                        if (!commandListDic.ContainsKey(slot))
                                            commandListDic[slot] = commandList;
                                        else
                                            commandList = commandListDic[slot];

                                        if (channelNum >= 0)
                                        {
                                            #region 应用校准数据
                                            //校准数据归0
                                            sbyte pwa_en = 0;
                                            byte cd_en = 0;
                                            ushort fd_en = 0;
                                            sbyte pwa_d = 0;
                                            byte cd_d = 0;
                                            ushort fd_d = 0;
                                            sbyte pwa_ca = 0;
                                            byte cd_ca = 0;
                                            ushort fd_ca = 0;
                                            sbyte pwa_cb = 0;
                                            byte cd_cb = 0;
                                            ushort fd_cb = 0;
                                            byte d_d_d = 0;
                                            byte en_d_d = 0;
                                            int den_d_c = 0;
                                            byte ca_d_d = 0;
                                            byte cb_d_d = 0;
                                            int cab_d_c = 0;

                                            ApplyCalibrationData(patternTiming.Period, driveA, patternTiming.StrobeA, ref cd_en, ref fd_en, ref cd_d, ref fd_d, ref cd_ca, ref fd_ca, ref cd_cb, ref fd_cb, ref d_d_d, ref en_d_d, ref den_d_c, ref ca_d_d, ref cb_d_d, ref cab_d_c, channelNum, slot);
                                            #endregion

                                            var fd_enBytes = BitConverter.GetBytes(fd_en).Reverse();
                                            var fd_dBytes = BitConverter.GetBytes(fd_d).Reverse();
                                            var fd_caBytes = BitConverter.GetBytes(fd_ca).Reverse();
                                            var fd_cbBytes = BitConverter.GetBytes(fd_cb).Reverse();
                                            var den_dcBytes = BitConverter.GetBytes(den_d_c).Reverse();
                                            var cab_dcBytes = BitConverter.GetBytes(cab_d_c).Reverse();

                                            var byteList = new List<byte>();
                                            byteList.Add((byte)channelNum);        //暂时按顺序下发通道（后续需要映射站点信息）
                                            byteList.AddRange(periodBytes);
                                            byteList.Add(formatByte);
                                            byteList.Add(strobeByte);
                                            byteList.AddRange(driveABytes);   //D0
                                            byteList.AddRange(driveBBytes);   //D1
                                            byteList.AddRange(driveCBytes);   //D2
                                            byteList.AddRange(driveDBytes);   //D3
                                            byteList.AddRange(r0PeriodBytes); //R0
                                            byteList.AddRange(r1PeriodBytes); //R1
                                            byteList.Add((byte)pwa_en);       //pwa_en
                                            byteList.Add(cd_en);              //cd_en
                                            byteList.AddRange(fd_enBytes);    //fd_en
                                            byteList.Add((byte)pwa_d);        //pwa_d
                                            byteList.Add(cd_d);               //cd_d
                                            byteList.AddRange(fd_dBytes);     //fd_d
                                            byteList.Add((byte)pwa_ca);       //pwa_ca
                                            byteList.Add(cd_ca);              //cd_ca
                                            byteList.AddRange(fd_caBytes);    //fd_ca
                                            byteList.Add((byte)pwa_cb);       //pwa_cb
                                            byteList.Add(cd_cb);              //cd_cb
                                            byteList.AddRange(fd_cbBytes);    //fd_cb
                                            byteList.Add(d_d_d);              //d_d_d
                                            byteList.Add(en_d_d);              //en_d_d
                                            byteList.AddRange(den_dcBytes);     //den_d_c
                                            byteList.Add(ca_d_d);              //ca_d_d
                                            byteList.Add(cb_d_d);              //cb_d_d
                                            byteList.AddRange(cab_dcBytes);    //cab_d_c
                                            var command = new CommandInfoModel()
                                            {
                                                CommandCode = "0x010A",
                                                CommandContent = byteList.ToArray(),
                                            };
                                            commandList.Add(command);

#if DEBUG
                                            var stringFormat = "CH_NUM:{0},Period:{1},Format:{2},Strobe:{3},D0:{4},D1:{5},D2:{6},D3:{7},R0:{8},R1:{9},PWA_EN:{10},CD_EN:{11},FD_EN:{12},PWA_D:{13},CD_D:{14},FD_D:{15},PWA_CA:{16},CD_CA:{17},FD_CA:{18},PWA_CB:{19},CD_CB:{20},FD_CB:{21},D_D_D:{22},EN_D_D:{23},DEN_D_C:{24},CA_D_D:{25},CB_D_D:{26},CAB_D_C:{27}";
                                            PrintResultLog.Message(string.Format(stringFormat, channelNum, periodUInt, formatByte, strobeByte, driveAUInt, driveBUInt, driveCUInt, driveDUInt, r0PeriodUInt, r1PeriodUInt, pwa_en, cd_en, fd_en, pwa_d, cd_d, fd_d, pwa_ca, cd_ca, fd_ca, pwa_cb, cd_cb, fd_cb, d_d_d, en_d_d, den_d_c, ca_d_d, cb_d_d, cab_d_c));
#endif
                                        }
                                    }
                                }

                                if (commandListDic.Any())
                                {
                                    foreach (var commandList in commandListDic)
                                    {
                                        var slotNum = $"0x{commandList.Key.ToString("x2")}";
                                        var message = CommandHelper.GetCommandBytes(0xFF, BoardType.PE, InstructionType.Configuration, commandList.Value);

                                        var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType((BoardType)Instance?.BoardType, slotNum);

                                        if (controlService != null && instrumentInfo != null)
                                            controlService.Send(instrumentInfo, message);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }

                }
            }
        }

    }
}
