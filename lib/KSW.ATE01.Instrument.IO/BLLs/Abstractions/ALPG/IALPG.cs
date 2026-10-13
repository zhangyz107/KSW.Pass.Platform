using KSW.ATE01.Instrument.IO.Enums.ALPG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.ALPG
{
    public interface IALPG
    {
        /// <summary>
        /// LVM DBM参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="startAddress"></param>
        /// <param name="readLength0"></param>
        /// <param name="readLength1"></param>
        /// <param name="dbmAddress"></param>
        /// <param name="dbmIncrease"></param>
        void SetLvmDBM(ushort startAddress, byte readLength0, byte readLength1, uint dbmAddress, uint dbmIncrease);

        /// <summary>
        /// ALPG程序下载
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="filePath">文件路径</param>
        void ALPGFileDownload(byte num, string filePath);

        /// <summary>
        /// ALPG程序启动
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="startAddress">起始地址</param>
        void ALPGProgramStart(ushort startAddress);

        /// <summary>
        /// 端口映射
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="inputChannel">输入通道</param>
        /// <param name="outputChannel">输出通道</param>
        void SetPortMapping(byte[] inputChannel, byte[] outputChannel);

        /// <summary>
        /// PRE预扰码模式
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="bgAddressMode">BG地址模式</param>
        /// <param name="baAddressMode">BA地址模式</param>
        /// <param name="rowAddressMode">行地址模式</param>
        /// <param name="colAddressMode">列地址模式</param>
        void SetPreScramb(AddressMode bgAddressMode, AddressMode baAddressMode, AddressMode rowAddressMode, AddressMode colAddressMode);

        /// <summary>
        /// PRE预扰码XOR
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="bgAddressXOR">BG地址异或值</param>
        /// <param name="bgAddressInvert">BG地址取反位</param>
        /// <param name="baAddressXOR">BA地址异或值</param>
        /// <param name="baAddressInvert">BA地址取反位</param>
        /// <param name="rowAddressXOR">行地址异或值</param>
        /// <param name="rowAddressInvert">行地址取反位</param>
        /// <param name="colAddressXOR">列地址异或值</param>
        /// <param name="colAddressInvert">列地址取反位</param>
        void SetPreScrambXOR(byte bgAddressXOR, byte bgAddressInvert, byte baAddressXOR, byte baAddressInvert, byte[] rowAddressXOR, byte[] rowAddressInvert, ushort colAddressXOR, ushort colAddressInvert);

        /// <summary>
        /// BG预扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据</param>
        void SetBGPreScramb(byte ramAddress, byte count, byte[] data);

        /// <summary>
        /// BA预扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据</param>
        void SetBAPreScramb(byte ramAddress, byte count, byte[] data);

        /// <summary>
        /// 行预扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据</param>
        void SetRowPreScramb(byte[] ramAddress, ushort count, byte[] data);

        /// <summary>
        /// 列预扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据</param>
        void SetColPreScramb(ushort ramAddress, ushort count, byte[] data);

        /// <summary>
        /// 解扰码模式
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="bgAddressMode">BG地址模式</param>
        /// <param name="baAddressMode">BA地址模式</param>
        /// <param name="rowAddressMode">行地址模式</param>
        /// <param name="colAddressMode">列地址模式</param>
        void SetDescramb(AddressMode bgAddressMode, AddressMode baAddressMode, AddressMode rowAddressMode, AddressMode colAddressMode);

        /// <summary>
        /// 解扰码XOR模式
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="bgAddressXOR">BG地址异或值</param>
        /// <param name="bgAddressInvert">BG地址取反位</param>
        /// <param name="baAddressXOR">BA地址异或值</param>
        /// <param name="baAddressInvert">BA地址取反位</param>
        /// <param name="rowAddressXOR">行地址异或值</param>
        /// <param name="rowAddressInvert">行地址取反位</param>
        /// <param name="colAddressXOR">列地址异或值</param>
        /// <param name="colAddressInvert">列地址取反位</param>
        void SetDescrambXOR(byte bgAddressXOR, byte bgAddressInvert, byte baAddressXOR, byte baAddressInvert, byte[] rowAddressXOR, byte[] rowAddressInvert, ushort colAddressXOR, ushort colAddressInvert);

        /// <summary>
        /// BG解扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据</param>
        void SetBGDescramb(byte ramAddress, byte count, byte[] data);

        /// <summary>
        /// BA解扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据</param>
        void SetBADescramb(byte ramAddress, byte count, byte[] data);

        /// <summary>
        /// 行解扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据（2*N）</param>
        void SetRowDescramb(byte[] ramAddress, ushort count, byte[] data);

        /// <summary>
        /// 列解扰码RAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据（2*N）</param>
        void SetColDescramb(ushort ramAddress, ushort count, byte[] data);

        /// <summary>
        /// DGU突发地址表
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据(2*N)</param>
        void SetDGUBurstAddress(ushort ramAddress, ushort count, byte[] data);

        /// <summary>
        /// DGU ARIRAM
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="ramAddress">RAM地址</param>
        /// <param name="count">配置数量</param>
        /// <param name="data">RAM数据(4*N)</param>
        void SetDGUAriram(byte[] ramAddress, ushort count, byte[] data);

        /// <summary>
        /// TIMEM 参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="counterId">计数器编号</param>
        /// <param name="counterEnable">计数器使能</param>
        /// <param name="targetValue">目标计数值</param>
        /// <param name="isLoop">计数模式是否循环</param>
        /// <param name="isHide">中断屏蔽是否屏蔽</param>
        /// <param name="isCounterClear">计数器清零是否清零</param>
        /// <param name="isInterruptClear">中断清零是否清零</param>
        void SetTimerParameter(byte counterId, byte counterEnable, byte[] targetValue, bool isLoop = false, bool isHide = false, bool isCounterClear = false, bool isInterruptClear = false);

        /// <summary>
        /// Lvm BM参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="address0">地址0</param>
        /// <param name="address1">地址1</param>
        /// <param name="address2">地址2</param>
        /// <param name="address3">地址3</param>
        /// <param name="address4">地址4</param>
        /// <param name="address5">地址5</param>
        /// <param name="address6">地址6</param>
        /// <param name="address7">地址7</param>
        void SetBMParameter(ushort address0, ushort address1, ushort address2, ushort address3, ushort address4, ushort address5, ushort address6, ushort address7);

        /// <summary>
        /// ALPG SCS参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="sh0">SH0</param>
        /// <param name="sh1">SH1</param>
        /// <param name="sh2">SH2</param>
        /// <param name="sh3">SH3</param>
        void SetSCSParameter(uint sh0, uint sh1, uint sh2, uint sh3);

        /// <summary>
        /// ALPG DGU开关
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="dcmr1Enable">DCMR1使能</param>
        /// <param name="dcmr2Enable">DCMR2使能</param>
        /// <param name="dflgEnable">DFLG使能</param>
        /// <param name="dsel">DSEL</param>
        /// <param name="dinvEnable">DINV使能</param>
        /// <param name="fpEnable">FP使能</param>
        /// <param name="ariramEnable">ARIRAM使能</param>
        void SetDGUSwitch(bool dcmr1Enable, bool dcmr2Enable, bool dflgEnable, byte dsel, bool dinvEnable, bool fpEnable, bool ariramEnable);

        /// <summary>
        /// ALPG DGU参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="function">FP功能</param>
        /// <param name="burstLength">Burst Length</param>
        /// <param name="xmask"></param>
        /// <param name="ymask"></param>
        /// <param name="diag2_val"></param>
        /// <param name="xval"></param>
        /// <param name="yval"></param>
        void SetDGUParameter(FPFunction function, BurstLength burstLength, byte[] xmask, byte[] ymask, byte[] diag2_val, byte[] xval, byte[] yval);

        /// <summary>
        /// ALPG AGU X寄存器
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="aguNum">AGU_Num</param>
        /// <param name="xh"></param>
        /// <param name="xos"></param>
        /// <param name="xt1"></param>
        /// <param name="xt2"></param>
        /// <param name="xt3"></param>
        /// <param name="xt4"></param>
        /// <param name="xt5"></param>
        /// <param name="xt6"></param>
        /// <param name="xt7"></param>
        /// <param name="xt8"></param>
        /// <param name="xta"></param>
        /// <param name="xtb"></param>
        /// <param name="xtc"></param>
        /// <param name="xtd"></param>
        /// <param name="xte"></param>
        /// <param name="xtf"></param>
        /// <param name="xmax"></param>
        /// <param name="xmsk"></param>
        /// <param name="xcmr"></param>
        void SetAGUXRegister(byte aguNum, byte[] xh, byte[] xos, byte[] xt1, byte[] xt2, byte[] xt3, byte[] xt4, byte[] xt5, byte[] xt6, byte[] xt7, byte[] xt8, byte[] xt9, byte[] xta, byte[] xtb, byte[] xtc, byte[] xtd, byte[] xte, byte[] xtf, byte[] xmax, byte[] xmsk, byte[] xcmr);

        /// <summary>
        /// ALPG AGU Y寄存器
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="aguNum">AGU_Num</param>
        /// <param name="yh"></param>
        /// <param name="yos"></param>
        /// <param name="yt1"></param>
        /// <param name="yt2"></param>
        /// <param name="yt3"></param>
        /// <param name="yt4"></param>
        /// <param name="yt5"></param>
        /// <param name="yt6"></param>
        /// <param name="yt7"></param>
        /// <param name="yt8"></param>
        /// <param name="yt9"></param>
        /// <param name="yta"></param>
        /// <param name="ytb"></param>
        /// <param name="ytc"></param>
        /// <param name="ytd"></param>
        /// <param name="yte"></param>
        /// <param name="ytf"></param>
        /// <param name="ymax"></param>
        /// <param name="ymsk"></param>
        /// <param name="xcmr"></param>
        void SetAGUYRegister(byte aguNum, byte[] yh, byte[] yos, byte[] yt1, byte[] yt2, byte[] yt3, byte[] yt4, byte[] yt5, byte[] yt6, byte[] yt7, byte[] yt8, byte[] yt9, byte[] yta, byte[] ytb, byte[] ytc, byte[] ytd, byte[] yte, byte[] ytf, byte[] ymax, byte[] ymsk, byte[] ycmr);

        /// <summary>
        /// ALPG AGU D寄存器
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="aguNum">AGU_Num</param>
        /// <param name="d1a"></param>
        /// <param name="d1b"></param>
        /// <param name="d1c"></param>
        /// <param name="d1d"></param>
        /// <param name="d1e"></param>
        /// <param name="d1f"></param>
        /// <param name="d1g"></param>
        /// <param name="d1h"></param>
        /// <param name="d2a"></param>
        /// <param name="d2b"></param>
        /// <param name="d2c"></param>
        /// <param name="d2d"></param>
        /// <param name="d3b"></param>
        /// <param name="d4b"></param>
        void SetAGUDRegister(byte aguNum, byte[] d1a, byte[] d1b, byte[] d1c, byte[] d1d, byte[] d1e, byte[] d1f, byte[] d1g, byte[] d1h, byte[] d2a, byte[] d2b, byte[] d2c, byte[] d2d, byte[] d3b, byte[] d4b);

        /// <summary>
        /// ALPG AGU N寄存器
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="aguNum">AGU_Num</param>
        /// <param name="nh"></param>
        /// <param name="nd1"></param>
        /// <param name="nd2"></param>
        /// <param name="nd3"></param>
        /// <param name="nd4"></param>
        /// <param name="nd5"></param>
        /// <param name="nd6"></param>
        /// <param name="nd7"></param>
        void SetAGUNRegister(byte aguNum, byte nh, byte nd1, byte nd2, byte nd3, byte nd4, byte nd5, byte nd6, byte nd7);

        /// <summary>
        /// ALPG AGU B寄存器
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="aguNum">AGU_Num</param>
        /// <param name="bh"></param>
        /// <param name="bd1"></param>
        /// <param name="bd2"></param>
        /// <param name="bd3"></param>
        /// <param name="bd4"></param>
        /// <param name="bd5"></param>
        /// <param name="bd6"></param>
        /// <param name="bd7"></param>
        void SetAGUBRegister(byte aguNum, byte bh, byte bd1, byte bd2, byte bd3, byte bd4, byte bd5, byte bd6, byte bd7);

        /// <summary>
        /// ALPG AGU G寄存器
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="aguNum">AGU_Num</param>
        /// <param name="gh"></param>
        /// <param name="gd1"></param>
        /// <param name="gd2"></param>
        /// <param name="gd3"></param>
        /// <param name="gd4"></param>
        /// <param name="gd5"></param>
        /// <param name="gd6"></param>
        /// <param name="gd7"></param>
        void SetAGUGRegister(byte aguNum, byte gh, byte gd1, byte gd2, byte gd3, byte gd4, byte gd5, byte gd6, byte gd7);

        /// <summary>
        /// ALPG DGU寄存器
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="aguNum">AGU_Num</param>
        /// <param name="tph1a"></param>
        /// <param name="tph1b"></param>
        /// <param name="tph1c"></param>
        /// <param name="tph1d"></param>
        /// <param name="d5a"></param>
        /// <param name="d5b"></param>
        /// <param name="d5c"></param>
        /// <param name="d5d"></param>
        /// <param name="tph2a"></param>
        /// <param name="tph2b"></param>
        /// <param name="tph2c"></param>
        /// <param name="tph2d"></param>
        /// <param name="d6a"></param>
        /// <param name="d6b"></param>
        /// <param name="d6c"></param>
        /// <param name="d6d"></param>
        /// <param name="dcmr1"></param>
        /// <param name="dcmr2"></param>
        /// <param name="dflg"></param>
        /// <param name="dsd"></param>
        void SetDGURegister(byte aguNum, uint tph1a, uint tph1b, uint tph1c, uint tph1d, uint d5a, uint d5b, uint d5c, uint d5d, uint tph2a, uint tph2b, uint tph2c, uint tph2d, uint d6a, uint d6b, uint d6c, uint d6d, uint dcmr1, uint dcmr2, uint dflg, uint dsd);

        /// <summary>
        /// ALPG DCU DD参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="chNum">通道号</param>
        /// <param name="db"></param>
        /// <param name="dc"></param>
        /// <param name="dl"></param>
        void SetDCUDDParameter(byte chNum, uint db, uint dc, uint dl);

        /// <summary>
        /// ALPG DCU PD参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="chNum">通道号</param>
        /// <param name="db"></param>
        /// <param name="dc"></param>
        /// <param name="dl"></param>
        void SetDCUPDParameter(byte chNum, uint db, uint dc, uint dl);

        /// <summary>
        /// ALPG PDS DDR参数
        /// </summary>
        /// <param name="num">ALPG_Num</param>
        /// <param name="mode2NEnable"></param>
        /// <param name="csGearDownModeEnable"></param>
        /// <param name="ds3CID3Enable"></param>
        /// <param name="burstLength"></param>
        /// <param name="readPreamble"></param>
        /// <param name="readPostamble"></param>
        /// <param name="writePreamble"></param>
        /// <param name="writePostamble"></param>
        void SetPDSDDRParameter(bool mode2NEnable, bool csGearDownModeEnable, bool ds3CID3Enable, BurstLength burstLength, ReadPreamble readPreamble, ReadPostamble readPostamble, WritePreamble writePreamble, WritePostamble writePostamble);
    }

}
