using KSW.ATE01.Instrument.IO.Enums.Shmoos;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSW.ATE01.Instrument.IO.Helpers
{
    internal class ShmooTestManagerHelper
    {
        private static ShmooTestManagerHelper _instance;
        private static object _lock = new object();

        private ShmooTestManagerHelper()
        {

        }

        public static ShmooTestManagerHelper Instance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new ShmooTestManagerHelper();
                return _instance;
            }
        }

        public Dictionary<string, ShmooTest> Tests { get; set; } = new Dictionary<string, ShmooTest>();

        public Dictionary<string, List<ShmooResult>> Results { get; set; } = new Dictionary<string, List<ShmooResult>>();

        public void TryAdd(string testName, ShmooTest test)
        {
            if (Tests.ContainsKey(testName))
                Tests[testName] = test;
            else
                Tests.Add(testName, test);

            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
            var shmooTestDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();
            if (shmooTestDic.ContainsKey(testName))
                shmooTestDic[testName].TestInfo = test;
            else
                shmooTestDic.Add(testName, new ShmooModel() { TestInfo = new ShmooTest() });

            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.WriteObject(shmooTestDic);
        }

        public void TrySetX(string testName, string axisType, string mode, double begin, double end, double stepSize, string pinList, string timingName = "", double delayS = 0.0)
        {
            if (!Tests.ContainsKey(testName))
                Tests.Add(testName, new ShmooTest());

            if (Tests[testName].XAxis == null)
                Tests[testName].XAxis = new ShmooAxis();

            InternalSetAxis(Tests[testName].XAxis, axisType, mode, begin, end, stepSize, pinList, timingName, delayS);

            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
            var shmooTestDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();
            if (!shmooTestDic.ContainsKey(testName))
                shmooTestDic.Add(testName, new ShmooModel() { TestInfo = new ShmooTest() });

            if (shmooTestDic[testName].TestInfo.XAxis == null)
                shmooTestDic[testName].TestInfo.XAxis = new ShmooAxis();

            InternalSetAxis(shmooTestDic[testName].TestInfo.XAxis, axisType, mode, begin, end, stepSize, pinList, timingName, delayS);
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.WriteObject(shmooTestDic);
        }

        public void TrySetY(string testName, string axisType, string mode, double begin, double end, double stepSize, string pinList, string timingName = "", double delayS = 0.0)
        {
            if (!Tests.ContainsKey(testName))
                Tests.Add(testName, new ShmooTest());

            if (Tests[testName].YAxis == null)
                Tests[testName].YAxis = new ShmooAxis();

            InternalSetAxis(Tests[testName].YAxis, axisType, mode, begin, end, stepSize, pinList, timingName, delayS);
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
            var shmooTestDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();
            if (!shmooTestDic.ContainsKey(testName))
                shmooTestDic.Add(testName, new ShmooModel() { TestInfo = new ShmooTest() });

            if (shmooTestDic[testName].TestInfo.YAxis == null)
                shmooTestDic[testName].TestInfo.YAxis = new ShmooAxis();

            InternalSetAxis(shmooTestDic[testName].TestInfo.YAxis, axisType, mode, begin, end, stepSize, pinList, timingName, delayS);
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.WriteObject(shmooTestDic);
        }

        public void TrySetMode(string testName,AxisDirection direction)
        {
            if (!Tests.ContainsKey(testName))
                return;

            Tests[testName].Direction = direction;
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
            var shmooTestDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();

            if (!shmooTestDic.ContainsKey(testName))
                return;
            shmooTestDic[testName].TestInfo.Direction = direction;
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.WriteObject(shmooTestDic);
        }

        public void TrySetPrint(string testName,string printPath)
        {
            if (!Tests.ContainsKey(testName))
                return;

            Tests[testName].PrintPath = printPath;
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
            var shmooTestDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();

            if (!shmooTestDic.ContainsKey(testName))
                return;
            shmooTestDic[testName].TestInfo.PrintPath = printPath;
            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.WriteObject(shmooTestDic);
        }

        private void InternalSetAxis(ShmooAxis sourceAxis, string axisType, string mode, double begin, double end, double step, string pinList, string timingName = "", double delayS = 0.0)
        {
            if (Enum.TryParse(axisType, out AxisType type))
                sourceAxis.AxisType = type;

            sourceAxis.Begin = begin;
            sourceAxis.Delay = delayS;
            sourceAxis.End = end;
            sourceAxis.Mode = mode;
            sourceAxis.PinList = pinList;
            sourceAxis.Step = step;
            sourceAxis.TimingName = timingName;
        }
    }
}
