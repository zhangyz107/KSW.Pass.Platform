using System;

namespace KSW.ATE01.Extension.VS2022
{
    internal sealed partial class ATE01Guids
    {
        public const string GuidATE01PackageString = "8BF623B2-F831-4F3D-A729-88F0716F8954";
        public const string GuidATE01OutputPanelString = "9AF588D8-4C96-43E9-8A0E-B0206F5FE40D";
        public const string GuidATE01MenuSetString = "75503bc2-7ed6-4788-b072-83af7f587d68";

        public static Guid GuidATE01Package = new Guid(GuidATE01PackageString);
        public static Guid GuidATE01OutputPanel = new Guid(GuidATE01OutputPanelString);
        public static Guid GuidATE01MenuSet = new Guid(GuidATE01MenuSetString);
    }

    internal sealed partial class ATE01Ids
    {
        public const int TestPlanId = 0x2000;
        public const int PpmuDteId = 0x2100;
        public const int DigitalDteId = 0x2200;
        public const int DpsDteId = 0x2300;
        public const int UdbDteId = 0x2400;
    }
}
