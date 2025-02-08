using System.Text.RegularExpressions;

namespace KSW.ATE01.Project.Base.Models.Patterns
{
    public class InstrumentModel
    {
        public string StrInstrument { get; set; }

        internal int DataBlockLength { get; set; }

        public string InstrumentType { get; set; }

        public string DataBlockName { get; set; }

        public Dictionary<string, List<string>> DicPinItem { get; set; } = new Dictionary<string, List<string>>();

        public string DigitalMode { get; set; }

        public int InstrumentWidth { get; set; } = 1;

        public string InstrumentMode { get; set; }

        public string BitOrder { get; set; }

        public string Format { get; set; }

        public Regex RegularExpression { get; set; }

        public Dictionary<int, int> StartTrigBlockIndexAndCount { get; set; } = new Dictionary<int, int>();

        internal void CalculateStartTrigBlockMemberCountByInstrumentWidth()
        {
            for (int i = 0; i < StartTrigBlockIndexAndCount.Count; i++)
            {
                StartTrigBlockIndexAndCount[i] /= InstrumentWidth;
            }
        }
    }
}
