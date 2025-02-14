using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Models.Exceptions
{
    public class ATEException : ApplicationException
    {
        public ATEException(string? message) : base(message)
        {

        }

        protected ATEException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public ATEException(string? message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
