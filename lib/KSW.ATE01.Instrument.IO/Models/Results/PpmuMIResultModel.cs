using KSW.ATE01.Instrument.IO.Enums.Ppmus;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    public  class PpmuMIResultModel<T> : ChannelResultModel<T>
    {
        public MIType IR { get; set; }      
    }
}
