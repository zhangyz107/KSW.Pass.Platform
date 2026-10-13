using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Enums.ALPG
{
    /// <summary>
    /// FP功能
    /// </summary>
    public enum FPFunction
    {
        FIX0 = 0,

        MaskedParity = 1,

        Diagonal = 2,

        Diagonal2 = 3,

        InvertedDiagonal = 4,

        InvertedDiagonal2 = 5,

        MaskedDiagonal = 6,

        MaskedRowBar = 7,

        MaskedColumnBar = 8,

        CheckerBoard = 9,

        ANDRowColumnBar = 10,

        UniversalFP = 11,

        FIX1 = 12
    }
}
