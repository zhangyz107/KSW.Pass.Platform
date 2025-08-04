using System.Runtime.Serialization;

namespace KSW.ATE01.Results.STDF.Exception
{
    [Serializable]
    public class Stdf4ParserException : System.Exception
    {
        public Stdf4ParserException()
        {
        }

        public Stdf4ParserException(string message) : base(message)
        {
        }

        public Stdf4ParserException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected Stdf4ParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
