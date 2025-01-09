using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KSW.ATE01.Pattern.Domain.Projects.Patterns
{
    public class Instrument
    {
        private string _strInstrument;

        private int _dataBlockLength;

        private string _instrumentType;

        private string _dataBlockName;

        private Dictionary<string, List<string>> _dicPinItem = new Dictionary<string, List<string>>();

        private string _digitalMode;

        private int _instrumentWidth = 1;

        private string _instrumentMode;

        private string _bitOrder;

        private string _format;

        private Regex _regularExpression;

        private Dictionary<int, int> _startTrigBlockIndexAndCount = new Dictionary<int, int>();

        public string StrInstrument
        {
            get => _strInstrument;
            set => _strInstrument = value;
        }

        internal int DataBlockLength
        {
            get => _dataBlockLength;
            set => _dataBlockLength = value;
        }

        public string InstrumentType
        {
            get => _instrumentType;
            set => _instrumentType = value;
        }

        public string DataBlockName
        {
            get => _dataBlockName;
            set => _dataBlockName = value;
        }

        public Dictionary<string, List<string>> DicPinItem
        {
            get => _dicPinItem;
            set => _dicPinItem = value;
        }

        public string DigitalMode
        {
            get => _digitalMode;
            set => _digitalMode = value;
        }

        public int InstrumentWidth
        {
            get => _instrumentWidth;
            set => _instrumentWidth = value;
        }

        public string InstrumentMode
        {
            get => _instrumentMode;
            set => _instrumentMode = value;
        }

        public string BitOrder
        {
            get => _bitOrder;
            set => _bitOrder = value;
        }

        public string Format
        {
            get => _format;
            set => _format = value;
        }

        public Regex RegularExpression
        {
            get => _regularExpression;
            set => _regularExpression = value;
        }

        public Dictionary<int, int> StartTrigBlockIndexAndCount
        {
            get => _startTrigBlockIndexAndCount;
            set => _startTrigBlockIndexAndCount = value;
        }

        internal void CalculateStartTrigBlockMemberCountByInstrumentWidth()
        {
            for (int i = 0; i < StartTrigBlockIndexAndCount.Count; i++)
            {
                StartTrigBlockIndexAndCount[i] /= InstrumentWidth;
            }
        }
    }
}
