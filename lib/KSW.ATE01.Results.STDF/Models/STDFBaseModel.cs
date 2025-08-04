using KSW.ATE01.Results.STDF.Language;
using KSW.ATE01.Results.STDF.Services.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Results.STDF.Models
{
    public abstract class STDFBaseModel
    {
        protected DateTime baseDateTime = new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        protected LanguageManager L => LanguageManager.Instance;
        protected readonly StdfValueConverter valueConverter;
        private Encoding _encoding = Encoding.ASCII;

        public STDFBaseModel(byte recordType, byte recordSubType)
        {
            RecordType = recordType;
            RecordSubType = recordSubType;
            valueConverter = new StdfValueConverter();
        }

        protected Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value;
        }

        /// <summary>
        /// 记录类型
        /// </summary>
        public byte RecordType { get; private set; }

        /// <summary>
        /// 记录子类型
        /// </summary>
        public byte RecordSubType { get; private set; }


        /// <summary>
        /// 获取STDF字节数组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected byte[] GetStringBytes(string data)
        {
            if (string.IsNullOrEmpty(data))
                return [0];

            byte length = 0;
            var strBytes = Encoding.GetBytes(data);
            if (strBytes.Length > 255)
                length = 255;
            else
                length = (byte)strBytes.Length;

            var result = new List<byte>();
            result.Add(length);
            result.AddRange(strBytes.Take(length));
            return result.ToArray();
        }

        protected bool AddLengthBytes(List<byte> array)
        {
            var result = false;
            if (array != null && array.Any())
            {
                var ushortLength = (ushort)array.Count;
                var lengthBytes = BitConverter.GetBytes(ushortLength);
                array.InsertRange(0, lengthBytes);
            }
            result = true;
            return result;
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            var data = new byte[Length()];
            SetBytes(data, 0);

            return data;
        }

        public abstract ushort SetBytes(byte[] data, int offset = 0);

        public abstract ushort Length();

        public ushort LengthNoHeader() => (ushort)(this.Length() - 4);
    }
}
