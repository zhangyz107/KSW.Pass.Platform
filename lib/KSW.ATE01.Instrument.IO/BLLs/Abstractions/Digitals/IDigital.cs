using KSW.ATE01.Project.Base.Enums.TestPlans;

namespace KSW.ATE01.Instrument.IO.BLLs.Abstractions.Digitals
{
    public interface IDigital
    {
        void SetTimingDetail(double period, double driveA, double driveB, double driveC, double driveD, Timingformat fmt, StrobeModeType strobeMode, double strobeA, double strobeB);

        void SetTimingByPins();
    }
}
