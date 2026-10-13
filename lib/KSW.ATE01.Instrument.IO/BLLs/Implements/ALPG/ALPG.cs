using KSW.ATE01.Instrument.IO.BLLs.Abstractions.ALPG;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Instruments;
using KSW.ATE01.Instrument.IO.Enums.ALPG;
using KSW.ATE01.Instrument.IO.Enums.Instruments;
using KSW.ATE01.Instrument.IO.Helpers;
using KSW.ATE01.Instrument.IO.Models.Instruments;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.Errors;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.ALPG
{
    public class ALPG : PE131CommandBase<ALPG>, IALPG
    {
        private int _alpgNum;

        public static IALPG Num(int alpgNum)
        {
            if (alpgNum < 0 || alpgNum > 3)
                ErrorMessages.ALPG.ALPGNumOutOfRange();

            Instance._alpgNum = alpgNum;
            return Instance;
        }

        public void SetLvmDBM(ushort startAddress, byte readLength0, byte readLength1, uint dbmAddress, uint dbmIncrease)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            BitConverterHelper.IsLittleEndian = false;
            byteList.AddRange(BitConverterHelper.GetBytes(startAddress));
            byteList.Add(readLength0);
            byteList.Add(readLength1);
            byteList.AddRange(BitConverterHelper.GetBytes(dbmAddress));
            byteList.AddRange(BitConverterHelper.GetBytes(dbmIncrease));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0010",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }


        }

        public void ALPGProgramStart(ushort startAddress)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            BitConverterHelper.IsLittleEndian = false;
            byteList.AddRange(BitConverterHelper.GetBytes(startAddress));
            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0013",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void ALPGFileDownload(byte num, string filePath)
        {
            throw new NotImplementedException();
        }

        public void SetPortMapping(byte[] inputChannel, byte[] outputChannel)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byte[] tempInputArray = AlignArray(inputChannel, 48);
            byteList.AddRange(tempInputArray);

            var tempOutputArray = AlignArray(outputChannel, 48);
            byteList.AddRange(tempOutputArray);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0014",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }


        public void SetPreScramb(AddressMode bgAddressMode, AddressMode baAddressMode, AddressMode rowAddressMode, AddressMode colAddressMode)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add((byte)bgAddressMode);
            byteList.Add((byte)baAddressMode);
            byteList.Add((byte)rowAddressMode);
            byteList.Add((byte)colAddressMode);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0015",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetPreScrambXOR(byte bgAddressXOR, byte bgAddressInvert, byte baAddressXOR, byte baAddressInvert, byte[] rowAddressXOR, byte[] rowAddressInvert, ushort colAddressXOR, ushort colAddressInvert)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(bgAddressXOR);
            byteList.Add(bgAddressInvert);
            byteList.Add(baAddressXOR);
            byteList.Add(baAddressInvert);

            var tempRowAddressXOR = AlignArray(rowAddressXOR, 3);
            byteList.AddRange(tempRowAddressXOR);

            var tempRowAddressInvert = AlignArray(rowAddressInvert, 3);
            byteList.AddRange(tempRowAddressInvert);

            byteList.AddRange(BitConverterHelper.GetBytes(colAddressXOR));
            byteList.AddRange(BitConverterHelper.GetBytes(colAddressInvert));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0016",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetBGPreScramb(byte ramAddress, byte count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(ramAddress);
            byteList.Add(count);

            var tempData = AlignArray(data, count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0017",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetBAPreScramb(byte ramAddress, byte count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(ramAddress);
            byteList.Add(count);

            var tempData = AlignArray(data, count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0018",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetRowPreScramb(byte[] ramAddress, ushort count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);

            var tempRamAddress = AlignArray(ramAddress, 3);
            byteList.AddRange(tempRamAddress);
            byteList.AddRange(BitConverterHelper.GetBytes(count));

            var tempData = AlignArray(data, 3 * count);
            byteList.AddRange(tempData);


            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0019",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetColPreScramb(ushort ramAddress, ushort count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.AddRange(BitConverterHelper.GetBytes(ramAddress));
            byteList.AddRange(BitConverterHelper.GetBytes(count));

            var tempData = AlignArray(data, 2 * count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x001A",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDescramb(AddressMode bgAddressMode, AddressMode baAddressMode, AddressMode rowAddressMode, AddressMode colAddressMode)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add((byte)bgAddressMode);
            byteList.Add((byte)baAddressMode);
            byteList.Add((byte)rowAddressMode);
            byteList.Add((byte)colAddressMode);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x001B",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDescrambXOR(byte bgAddressXOR, byte bgAddressInvert, byte baAddressXOR, byte baAddressInvert, byte[] rowAddressXOR, byte[] rowAddressInvert, ushort colAddressXOR, ushort colAddressInvert)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(bgAddressXOR);
            byteList.Add(bgAddressInvert);
            byteList.Add(baAddressXOR);
            byteList.Add(baAddressXOR);

            var tempRowAddressXOR = AlignArray(rowAddressXOR, 3);
            byteList.AddRange(tempRowAddressXOR);

            var tempRowAddressInvert = AlignArray(rowAddressInvert, 3);
            byteList.AddRange(tempRowAddressInvert);

            byteList.AddRange(BitConverterHelper.GetBytes(colAddressXOR));
            byteList.AddRange(BitConverterHelper.GetBytes(colAddressInvert));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x001C",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public void SetBGDescramb(byte ramAddress, byte count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(ramAddress);
            byteList.Add(count);

            var tempData = AlignArray(data, count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x001D",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetBADescramb(byte ramAddress, byte count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(ramAddress);
            byteList.Add(count);

            var tempData = AlignArray(data, count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x001E",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetRowDescramb(byte[] ramAddress, ushort count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);

            var tempRamAddress = AlignArray(ramAddress, 3);
            byteList.AddRange(tempRamAddress);
            byteList.AddRange(BitConverterHelper.GetBytes(count));

            var tempData = AlignArray(data, 3 * count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x001F",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetColDescramb(ushort ramAddress, ushort count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.AddRange(BitConverterHelper.GetBytes(ramAddress));
            byteList.AddRange(BitConverterHelper.GetBytes(count));

            var tempData = AlignArray(data, 2 * count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0020",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDGUBurstAddress(ushort ramAddress, ushort count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.AddRange(BitConverterHelper.GetBytes(ramAddress));
            byteList.AddRange(BitConverterHelper.GetBytes(count));

            var tempData = AlignArray(data, 2 * count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0021",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDGUAriram(byte[] ramAddress, ushort count, byte[] data)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);

            var tempRamAddress = AlignArray(ramAddress, 3);
            byteList.AddRange(tempRamAddress);
            byteList.AddRange(BitConverterHelper.GetBytes(count));

            var tempData = AlignArray(data, 4 * count);
            byteList.AddRange(tempData);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0022",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetTimerParameter(byte counterId, byte counterEnable, byte[] targetValue, bool isLoop = false, bool isHide = false, bool isCounterClear = false, bool isInterruptClear = false)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(counterId);
            byteList.Add(counterEnable);
            byteList.Add(BitConverterHelper.GetByte(isLoop));
            byteList.Add(BitConverterHelper.GetByte(isHide));
            byteList.Add(BitConverterHelper.GetByte(isCounterClear));
            byteList.Add(BitConverterHelper.GetByte(isInterruptClear));

            var tempTargetValue = AlignArray(targetValue, 5);
            byteList.AddRange(tempTargetValue);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0023",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetBMParameter(ushort address0, ushort address1, ushort address2, ushort address3, ushort address4, ushort address5, ushort address6, ushort address7)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.AddRange(BitConverterHelper.GetBytes(address0));
            byteList.AddRange(BitConverterHelper.GetBytes(address1));
            byteList.AddRange(BitConverterHelper.GetBytes(address2));
            byteList.AddRange(BitConverterHelper.GetBytes(address3));
            byteList.AddRange(BitConverterHelper.GetBytes(address4));
            byteList.AddRange(BitConverterHelper.GetBytes(address5));
            byteList.AddRange(BitConverterHelper.GetBytes(address6));
            byteList.AddRange(BitConverterHelper.GetBytes(address7));


            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0024",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetSCSParameter(uint sh0, uint sh1, uint sh2, uint sh3)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.AddRange(BitConverterHelper.GetBytes(sh0));
            byteList.AddRange(BitConverterHelper.GetBytes(sh1));
            byteList.AddRange(BitConverterHelper.GetBytes(sh2));
            byteList.AddRange(BitConverterHelper.GetBytes(sh3));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0025",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDGUSwitch(bool dcmr1Enable, bool dcmr2Enable, bool dflgEnable, byte dsel, bool dinvEnable, bool fpEnable, bool ariramEnable)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(BitConverterHelper.GetByte(dcmr1Enable));
            byteList.Add(BitConverterHelper.GetByte(dcmr2Enable));
            byteList.Add(BitConverterHelper.GetByte(dflgEnable));
            byteList.Add(dsel);
            byteList.Add(BitConverterHelper.GetByte(dinvEnable));
            byteList.Add(BitConverterHelper.GetByte(fpEnable));
            byteList.Add(BitConverterHelper.GetByte(ariramEnable));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0026",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDGUParameter(FPFunction function, BurstLength burstLength, byte[] xmask, byte[] ymask, byte[] diag2_val, byte[] xval, byte[] yval)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add((byte)function);
            byteList.Add((byte)burstLength);
            var tempXMask = AlignArray(xmask, 3);
            byteList.AddRange(tempXMask);

            var tempYMask = AlignArray(ymask, 3);
            byteList.AddRange(tempYMask);

            var tempDiag2val = AlignArray(diag2_val, 3);
            byteList.AddRange(tempDiag2val);

            var tempXVal = AlignArray(xval, 3);
            byteList.AddRange(tempXVal);

            var tempYVal = AlignArray(yval, 3);
            byteList.AddRange(tempYVal);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0027",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetAGUXRegister(byte aguNum, byte[] xh, byte[] xos, byte[] xt1, byte[] xt2, byte[] xt3, byte[] xt4, byte[] xt5, byte[] xt6, byte[] xt7, byte[] xt8, byte[] xt9, byte[] xta, byte[] xtb, byte[] xtc, byte[] xtd, byte[] xte, byte[] xtf, byte[] xmax, byte[] xmsk, byte[] xcmr)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(aguNum);

            var tempXH = AlignArray(xh, 3);
            byteList.AddRange(tempXH);

            var tempXOS = AlignArray(xos, 3);
            byteList.AddRange(tempXOS);

            var tempXT1 = AlignArray(xt1, 3);
            byteList.AddRange(tempXT1);

            var tempXT2 = AlignArray(xt2, 3);
            byteList.AddRange(tempXT2);

            var tempXT3 = AlignArray(xt3, 3);
            byteList.AddRange(tempXT3);

            var tempXT4 = AlignArray(xt4, 3);
            byteList.AddRange(tempXT4);

            var tempXT5 = AlignArray(xt5, 3);
            byteList.AddRange(tempXT5);

            var tempXT6 = AlignArray(xt6, 3);
            byteList.AddRange(tempXT6);

            var tempXT7 = AlignArray(xt7, 3);
            byteList.AddRange(tempXT7);

            var tempXT8 = AlignArray(xt8, 3);
            byteList.AddRange(tempXT8);

            var tempXT9 = AlignArray(xt9, 3);
            byteList.AddRange(tempXT9);

            var tempXTa = AlignArray(xta, 3);
            byteList.AddRange(tempXTa);

            var tempXTb = AlignArray(xtb, 3);
            byteList.AddRange(tempXTb);

            var tempXTc = AlignArray(xtc, 3);
            byteList.AddRange(tempXTc);

            var tempXTd = AlignArray(xtd, 3);
            byteList.AddRange(tempXTd);

            var tempXTe = AlignArray(xte, 3);
            byteList.AddRange(tempXTe);

            var tempXTf = AlignArray(xtf, 3);
            byteList.AddRange(tempXTf);

            var tempXmax = AlignArray(xmax, 3);
            byteList.AddRange(tempXmax);

            var tempXmsk = AlignArray(xmsk, 3);
            byteList.AddRange(tempXmsk);

            var tempXcmr = AlignArray(xcmr, 3);
            byteList.AddRange(tempXcmr);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0028",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public void SetAGUYRegister(byte aguNum, byte[] yh, byte[] yos, byte[] yt1, byte[] yt2, byte[] yt3, byte[] yt4, byte[] yt5, byte[] yt6, byte[] yt7, byte[] yt8, byte[] yt9, byte[] yta, byte[] ytb, byte[] ytc, byte[] ytd, byte[] yte, byte[] ytf, byte[] ymax, byte[] ymsk, byte[] ycmr)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(aguNum);

            var tempYH = AlignArray(yh, 3);
            byteList.AddRange(tempYH);

            var tempYOS = AlignArray(yos, 3);
            byteList.AddRange(tempYOS);

            var tempYT1 = AlignArray(yt1, 3);
            byteList.AddRange(tempYT1);

            var tempYT2 = AlignArray(yt2, 3);
            byteList.AddRange(tempYT2);

            var tempYT3 = AlignArray(yt3, 3);
            byteList.AddRange(tempYT3);

            var tempYT4 = AlignArray(yt4, 3);
            byteList.AddRange(tempYT4);

            var tempYT5 = AlignArray(yt5, 3);
            byteList.AddRange(tempYT5);

            var tempYT6 = AlignArray(yt6, 3);
            byteList.AddRange(tempYT6);

            var tempYT7 = AlignArray(yt7, 3);
            byteList.AddRange(tempYT7);

            var tempYT8 = AlignArray(yt8, 3);
            byteList.AddRange(tempYT8);

            var tempYT9 = AlignArray(yt9, 3);
            byteList.AddRange(tempYT9);

            var tempYTa = AlignArray(yta, 3);
            byteList.AddRange(tempYTa);

            var tempYTb = AlignArray(ytb, 3);
            byteList.AddRange(tempYTb);

            var tempYTc = AlignArray(ytc, 3);
            byteList.AddRange(tempYTc);

            var tempYTd = AlignArray(ytd, 3);
            byteList.AddRange(tempYTd);

            var tempYTe = AlignArray(yte, 3);
            byteList.AddRange(tempYTe);

            var tempYTf = AlignArray(ytf, 3);
            byteList.AddRange(tempYTf);

            var tempYmax = AlignArray(ymax, 3);
            byteList.AddRange(tempYmax);

            var tempYmsk = AlignArray(ymsk, 3);
            byteList.AddRange(tempYmsk);

            var tempYcmr = AlignArray(ycmr, 3);
            byteList.AddRange(tempYcmr);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0029",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetAGUDRegister(byte aguNum, byte[] d1a, byte[] d1b, byte[] d1c, byte[] d1d, byte[] d1e, byte[] d1f, byte[] d1g, byte[] d1h, byte[] d2a, byte[] d2b, byte[] d2c, byte[] d2d, byte[] d3b, byte[] d4b)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(aguNum);

            var tempD1A = AlignArray(d1a, 3);
            byteList.AddRange(tempD1A);

            var tempD1B = AlignArray(d1b, 3);
            byteList.AddRange(tempD1B);

            var tempD1C = AlignArray(d1c, 3);
            byteList.AddRange(tempD1C);

            var tempD1D = AlignArray(d1d, 3);
            byteList.AddRange(tempD1D);

            var tempD1E = AlignArray(d1e, 3);
            byteList.AddRange(tempD1E);

            var tempD1F = AlignArray(d1f, 3);
            byteList.AddRange(tempD1F);

            var tempD1G = AlignArray(d1g, 3);
            byteList.AddRange(tempD1G);

            var tempD1H = AlignArray(d1h, 3);
            byteList.AddRange(tempD1H);

            var tempD2A = AlignArray(d2a, 3);
            byteList.AddRange(tempD2A);

            var tempD2B = AlignArray(d2b, 3);
            byteList.AddRange(tempD2B);

            var tempD2C = AlignArray(d2c, 3);
            byteList.AddRange(tempD2C);

            var tempD2D = AlignArray(d2d, 3);
            byteList.AddRange(tempD2D);

            var tempD3B = AlignArray(d3b, 3);
            byteList.AddRange(tempD3B);

            var tempD4B = AlignArray(d4b, 3);
            byteList.AddRange(tempD4B);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x002A",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetAGUNRegister(byte aguNum, byte nh, byte nd1, byte nd2, byte nd3, byte nd4, byte nd5, byte nd6, byte nd7)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(aguNum);
            byteList.Add(nh);
            byteList.Add(nd1);
            byteList.Add(nd2);
            byteList.Add(nd3);
            byteList.Add(nd4);
            byteList.Add(nd5);
            byteList.Add(nd6);
            byteList.Add(nd7);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x002B",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        public void SetAGUBRegister(byte aguNum, byte bh, byte bd1, byte bd2, byte bd3, byte bd4, byte bd5, byte bd6, byte bd7)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(aguNum);
            byteList.Add(bh);
            byteList.Add(bd1);
            byteList.Add(bd2);
            byteList.Add(bd3);
            byteList.Add(bd4);
            byteList.Add(bd5);
            byteList.Add(bd6);
            byteList.Add(bd7);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x002C",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetAGUGRegister(byte aguNum, byte gh, byte gd1, byte gd2, byte gd3, byte gd4, byte gd5, byte gd6, byte gd7)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(aguNum);
            byteList.Add(gh);
            byteList.Add(gd1);
            byteList.Add(gd2);
            byteList.Add(gd3);
            byteList.Add(gd4);
            byteList.Add(gd5);
            byteList.Add(gd6);
            byteList.Add(gd7);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x002D",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDGURegister(byte aguNum, uint tph1a, uint tph1b, uint tph1c, uint tph1d, uint d5a, uint d5b, uint d5c, uint d5d, uint tph2a, uint tph2b, uint tph2c, uint tph2d, uint d6a, uint d6b, uint d6c, uint d6d, uint dcmr1, uint dcmr2, uint dflg, uint dsd)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(aguNum);
            byteList.AddRange(BitConverterHelper.GetBytes(tph1a));
            byteList.AddRange(BitConverterHelper.GetBytes(tph1b));
            byteList.AddRange(BitConverterHelper.GetBytes(tph1c));
            byteList.AddRange(BitConverterHelper.GetBytes(tph1d));
            byteList.AddRange(BitConverterHelper.GetBytes(d5a));
            byteList.AddRange(BitConverterHelper.GetBytes(d5b));
            byteList.AddRange(BitConverterHelper.GetBytes(d5c));
            byteList.AddRange(BitConverterHelper.GetBytes(d5d));
            byteList.AddRange(BitConverterHelper.GetBytes(tph2a));
            byteList.AddRange(BitConverterHelper.GetBytes(tph2b));
            byteList.AddRange(BitConverterHelper.GetBytes(tph2c));
            byteList.AddRange(BitConverterHelper.GetBytes(tph2d));
            byteList.AddRange(BitConverterHelper.GetBytes(d6a));
            byteList.AddRange(BitConverterHelper.GetBytes(d6b));
            byteList.AddRange(BitConverterHelper.GetBytes(d6c));
            byteList.AddRange(BitConverterHelper.GetBytes(d6d));
            byteList.AddRange(BitConverterHelper.GetBytes(dcmr1));
            byteList.AddRange(BitConverterHelper.GetBytes(dcmr2));
            byteList.AddRange(BitConverterHelper.GetBytes(dflg));
            byteList.AddRange(BitConverterHelper.GetBytes(dsd));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x002E",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        public void SetDCUDDParameter(byte chNum, uint db, uint dc, uint dl)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(chNum);
            byteList.AddRange(BitConverterHelper.GetBytes(db));
            byteList.AddRange(BitConverterHelper.GetBytes(dc));
            byteList.AddRange(BitConverterHelper.GetBytes(dl));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x002F",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDCUPDParameter(byte chNum, uint db, uint dc, uint dl)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(chNum);
            byteList.AddRange(BitConverterHelper.GetBytes(db));
            byteList.AddRange(BitConverterHelper.GetBytes(dc));
            byteList.AddRange(BitConverterHelper.GetBytes(dl));

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0030",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        public void SetPDSDDRParameter(bool mode2NEnable, bool csGearDownModeEnable, bool ds3CID3Enable, BurstLength burstLength, ReadPreamble readPreamble, ReadPostamble readPostamble, WritePreamble writePreamble, WritePostamble writePostamble)
        {
            var byteList = new List<byte>();
            byteList.Add((byte)_alpgNum);
            byteList.Add(BitConverterHelper.GetByte(mode2NEnable));
            byteList.Add(BitConverterHelper.GetByte(csGearDownModeEnable));
            byteList.Add(BitConverterHelper.GetByte(ds3CID3Enable));
            byteList.Add((byte)burstLength);
            byteList.Add((byte)readPreamble);
            byteList.Add((byte)readPostamble);
            byteList.Add((byte)writePreamble);
            byteList.Add((byte)writePostamble);

            try
            {
                var controlService = Instance?.ControlService;

                var command = new CommandInfoModel()
                {
                    CommandCode = "0x0031",
                    CommandContent = byteList.ToArray(),
                };

                var instrumentInfo = InstrumentManagerHelper.GetInstrumentInfoByBoardType(AllowedBoardType, null);
                if (instrumentInfo != null)
                {
                    var message = CommandHelper.GetCommandBytes(0xFF, instrumentInfo?.BoardInfo?.BoardType ?? BoardType.Unknown, InstructionType.Configuration, new List<CommandInfoModel>() { command });

                    if (controlService != null && instrumentInfo != null)
                        controlService.Send(instrumentInfo, message);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        private byte[] AlignArray(byte[] sourceArray, int targetlength)
        {
            if (targetlength < 0)
                targetlength = 0;

            var result = new byte[targetlength];
            if (sourceArray != null && sourceArray.Length > 0)
            {
                var length = sourceArray.Length > result.Length ? result.Length : sourceArray.Length;
                Array.Copy(sourceArray, result, length);
            }

            return result;
        }
    }
}
