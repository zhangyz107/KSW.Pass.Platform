using KSW.ATE01.Results.STDF.Services.Converter;

namespace KSW.ATE01.Results.STDF.Models
{
    /// <summary>
    /// FAR 模型
    /// </summary>
    public class FARModel : STDFBaseModel
    {
        public FARModel() : base(0, 10)
        {

        }

        public FARModel(byte cpuType, byte stdfVersion) : this()
        {
            CpuType = cpuType;
            STDFVersion = stdfVersion;
        }

        /// <summary>
        /// CPU Type
        /// </summary>
        public byte CpuType { get; set; } = BitConverter.IsLittleEndian ? (byte)2 : (byte)1;

        /// <summary>
        /// STDF Version (默认版本V4)
        /// </summary>
        public byte STDFVersion { get; set; } = 4;

        public override ushort Length() => 6;

        public override ushort SetBytes(byte[] data, int offset = 0)
        {
            (data[offset + 0], data[offset + 1]) =
                           this.valueConverter.UshortToBytes(2); // Only two non-header bytes written.
            data[offset + 2] = this.RecordType;
            data[offset + 3] = this.RecordSubType;
            data[offset + 4] = this.CpuType;
            data[offset + 5] = this.STDFVersion;
            return 6;
        }
    }
}
