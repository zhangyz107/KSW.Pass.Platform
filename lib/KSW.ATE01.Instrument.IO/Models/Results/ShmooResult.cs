using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Models.Results
{
    [Serializable]
    public class ShmooResult
    {
        public double XCoordinate { get; set; }

        public double YCoordinate { get; set; }

        public bool IsResultValid { get; set; }

        public static ShmooResult GetInvalidResult(double xCoordinate, double yCoordinate)
        {
            return new ShmooResult { XCoordinate = xCoordinate, YCoordinate = yCoordinate, IsResultValid = false };
        }
    }
}
