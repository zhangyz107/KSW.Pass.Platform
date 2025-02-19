using KSW.ATE01.Instrument.IO.Enums.Ppmus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Ppmus
{
    public class PpmuCurrentRange
    {
        #region Fields
        private Ppmu _ppmu;
        internal IMType _imType;
        internal double _iforce;
        internal double _vcl;
        internal double _vch;
        #endregion

        public PpmuCurrentRange(Ppmu ppmu)
        {
            _ppmu = ppmu;
        }

        public void _4uA()
        {
            var imType = IMType.IM0;
            if (imType != _imType)
                _ppmu?.SetFIMV(imType, _iforce, _vcl, _vch);
        }

        public void _40uA()
        {
            var imType = IMType.IM1;
            if (imType != _imType)
                _ppmu?.SetFIMV(imType, _iforce, _vcl, _vch);
        }

        public void _400uA()
        {
            var imType = IMType.IM2;
            if (imType != _imType)
                _ppmu?.SetFIMV(imType, _iforce, _vcl, _vch);
        }

        public void _4mA()
        {
            var imType = IMType.IM3;
            if (imType != _imType)
                _ppmu?.SetFIMV(imType, _iforce, _vcl, _vch);
        }

        public void _40mA()
        {
            var imType = IMType.IM4;
            if (imType != _imType)
                _ppmu?.SetFIMV(imType, _iforce, _vcl, _vch);
        }
    }
}
