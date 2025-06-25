using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Enums.Calibrations
{
    /// <summary>
    /// 时钟校准类型
    /// </summary>
    public enum TimingCalibrationType
    {
        None = 0x00,
        Cd_d = 0x01,
        Cd_en = 0x02,
        Cd_ca = 0x04,
        Cd_cb = 0x08,
        Fd_d = 0x10,
        Fd_en = 0x20,
        Fd_ca = 0x40,
        Fd_cb = 0x80,
        DAT = 0x100,
        EN = 0x200,
        CA = 0x400,
        CB = 0x800,
        Cable = 0x1000,
    }
}
