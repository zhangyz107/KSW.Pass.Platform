using KSW.ATE01.Instrument.IO.BLLs.Abstractions.Shmoo;
using KSW.ATE01.Instrument.IO.Models.Results;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models;
using KSW.ATE01.Project.Base.Services.Loggers;
using System.IO;
using System.Text;

namespace KSW.ATE01.Instrument.IO.BLLs.Implements.Shmoo
{
    public class Print : IPrint
    {
        private string _testName;

        internal Print(string testName)
        {
            _testName = testName;

            ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.Open();
        }

        public void Text()
        {
            var shmooDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();

            if (shmooDic == null || !shmooDic.ContainsKey(_testName))
                return;

            var shmooModel = shmooDic[_testName];
            var shmooTest = shmooModel.TestInfo;
            var result = shmooModel.Results;

            if (result == null || !result.Any())
                return;

            var xAxisMode = shmooTest.XAxis == null ? shmooTest.YAxis.Mode : shmooTest.XAxis.Mode;
            var xUnit = shmooTest.XAxis == null ? GetAxisUnit(shmooTest.YAxis) : GetAxisUnit(shmooTest.XAxis);
            var yAxisMode = shmooTest.Direction == Enums.Shmoos.AxisDirection.None ? string.Empty : shmooTest.YAxis.Mode;
            var yUnit = shmooTest.Direction == Enums.Shmoos.AxisDirection.None ? string.Empty : GetAxisUnit(shmooTest.YAxis);

            var xaxis = shmooTest.XAxis;
            var yaxis = shmooTest.YAxis;
            CalculateAccuracy(xaxis != null ? xaxis.Step : 0.0, out int xStepAccuracy);
            CalculateAccuracy(xaxis != null ? xaxis.Begin : 0.0, out int xBeginAccuracy);
            CalculateAccuracy(xaxis != null ? xaxis.End : 0.0, out int xEndAccuracy);
            CalculateAccuracy(yaxis != null ? yaxis.Step : 0.0, out int ySteAccuracy);
            CalculateAccuracy(yaxis != null ? yaxis.Begin : 0.0, out int yBeginAccuracy);
            CalculateAccuracy(yaxis != null ? yaxis.End : 0.0, out int yEndAccuracy);

            int xAccuracy = new List<int>()
            {
                xStepAccuracy,
                xBeginAccuracy,
                xEndAccuracy
            }.Max();

            var yAccuracy = new List<int>()
            {
                ySteAccuracy,
                yBeginAccuracy,
                yEndAccuracy
            }.Max();

            var strBuild = PrintLog(result, shmooTest.Direction, xAxisMode, xUnit, yAxisMode, yUnit, xAccuracy, yAccuracy, shmooTest.XAxis != null);
            if (strBuild.Length > 0)
            {
                PrintResultLog.Message(strBuild.ToString());
                if (string.IsNullOrEmpty(shmooTest.PrintPath))
                    return;

                SaveFile(Path.Combine(shmooTest.PrintPath, string.Format("{0}_{1:yyyyMMdd_HHmmss_ffff}", shmooTest.TestName, DateTime.Now)), "txt", strBuild);
            }
        }

        public void Csv()
        {
            var shmooDic = ShmooShareMemory<Dictionary<string, ShmooModel>>.Instance.ReadObject();

            if (shmooDic == null || !shmooDic.ContainsKey(_testName))
                return;

            var shmooModel = shmooDic[_testName];
            var shmooTest = shmooModel.TestInfo;
            var result = shmooModel.Results;

            if (result == null || !result.Any())
                return;

            if (string.IsNullOrEmpty(shmooTest.PrintPath))
                return;

            var xAxisMode = shmooTest.XAxis == null ? shmooTest.YAxis.Mode : shmooTest.XAxis.Mode;
            var xUnit = shmooTest.XAxis == null ? GetAxisUnit(shmooTest.YAxis) : GetAxisUnit(shmooTest.XAxis);
            var yAxisMode = shmooTest.Direction == Enums.Shmoos.AxisDirection.None ? string.Empty : shmooTest.YAxis.Mode;
            var yUnit = shmooTest.Direction == Enums.Shmoos.AxisDirection.None ? string.Empty : GetAxisUnit(shmooTest.YAxis);

            var xaxis = shmooTest.XAxis;
            var yaxis = shmooTest.YAxis;
            CalculateAccuracy(xaxis != null ? xaxis.Step : 0.0, out int xStepAccuracy);
            CalculateAccuracy(xaxis != null ? xaxis.Begin : 0.0, out int xBeginAccuracy);
            CalculateAccuracy(xaxis != null ? xaxis.End : 0.0, out int xEndAccuracy);
            CalculateAccuracy(yaxis != null ? yaxis.Step : 0.0, out int ySteAccuracy);
            CalculateAccuracy(yaxis != null ? yaxis.Begin : 0.0, out int yBeginAccuracy);
            CalculateAccuracy(yaxis != null ? yaxis.End : 0.0, out int yEndAccuracy);

            int xAccuracy = new List<int>()
            {
                xStepAccuracy,
                xBeginAccuracy,
                xEndAccuracy
            }.Max();

            var yAccuracy = new List<int>()
            {
                ySteAccuracy,
                yBeginAccuracy,
                yEndAccuracy
            }.Max();

            var strBuilder = this.PrintLog(result, shmooTest.Direction, xAxisMode, xUnit, yAxisMode, yUnit, xAccuracy, yAccuracy, shmooTest.XAxis != null);
            strBuilder.Replace("\t", ",");
            strBuilder.Replace("\n", "\r\n");

            this.SaveFile(Path.Combine(shmooTest.PrintPath, string.Format("{0}_{1:yyyyMMdd_HHmmss_fff}", _testName, DateTime.Now)), "csv", strBuilder);
        }

        private void SaveFile(string filePath, string fileExtension, StringBuilder content)
        {
            StreamWriter streamWriter = new StreamWriter(filePath + "." + fileExtension, true, Encoding.ASCII);
            streamWriter.Write(content.ToString());
            streamWriter.Flush();
            streamWriter.Close();
        }

        private string GetAxisUnit(ShmooAxis axis)
        {
            if (axis == null)
                return string.Empty;

            var axisType = axis.AxisType;
            if (axisType == Enums.Shmoos.AxisType.Power)
            {
                var mode = axis.Mode;
                if (mode.ToLower().Equals("force_v"))
                    return "V";
                return "A";
            }
            else if (axisType == Enums.Shmoos.AxisType.Level)
            {
                var mode = axis.Mode;
                if (mode.ToLower().Equals("iol") || mode.ToLower().Equals("ioh"))
                    return "V";
                return "A";
            }
            else if (axisType == Enums.Shmoos.AxisType.Timing)
                return "ns";
            else
                return string.Empty;
        }

        private void CalculateAccuracy(double value, out int accuracy)
        {
            accuracy = 0;
            if (Math.Abs(value - Math.Round(value, 0)) < 5e-324)
                return;
            accuracy++;
            while (Math.Abs(value * 10.0 * (double)accuracy - Math.Round(value, accuracy) * 10.0 * (double)accuracy) > 5E-324)
            {
                accuracy++;
                if (accuracy == 5)
                {
                    break;
                }
            }
        }

        private StringBuilder PrintLog(List<ShmooResult> results, Enums.Shmoos.AxisDirection direction, string xAsixModel, string xUnit, string yAxisModel, string yUnit, int xAccuracy, int yAccuracy, bool isOnlyX = false)
        {
            var strBuild = new StringBuilder();
            var selectSites = CommonData.Instance.UseSiteName;
            if (!selectSites.Any())
                return strBuild;

            string str = isOnlyX ? "X" : "Y";
            foreach (var site in selectSites)
            {
                strBuild.Append("\n----------------------------------" + site + "Shmoo Result----------------------------------\n");
                int siteNo = int.Parse(site.Split(new[] { ' ' })[1]);
                if (!string.IsNullOrEmpty(yAxisModel))
                {
                    var verticalAxisStr = direction == Enums.Shmoos.AxisDirection.XY ? "Y" : "X";
                    var verticalAxisMode = direction == Enums.Shmoos.AxisDirection.XY ? (yAxisModel ?? "") : (xAsixModel ?? "");
                    strBuild.Append(verticalAxisStr + " Axis:\t" + verticalAxisMode + "\n");
                    var yGroups = from t in results
                                  group t by t.YCoordinate into t
                                  orderby t.Key descending
                                  select t;

                    foreach (var group in yGroups)
                    {
                        strBuild.Append("\t" + group.Key.ToString(string.Format("F{0}", yAccuracy)) + yUnit);

                        var orderResult = group.OrderBy(x => x.XCoordinate);
                        foreach (var shmooResult in orderResult)
                            strBuild.Append("\t" + GetResultSymbol(shmooResult, siteNo)); //打印结果值
                        strBuild.Append('\n');
                    }

                    var xValues = (from t in results
                                   group t by t.XCoordinate into t
                                   select t.Key into t
                                   orderby t
                                   select t).ToList();

                    strBuild.Append("\t\t");

                    foreach (double x in xValues)
                    {
                        strBuild.Append(x.ToString(string.Format("F{0}", xAccuracy)) + xUnit + "\t");
                    }
                    strBuild.Append("\n");
                    for (int i = 0; i < xValues.Count; i++)
                    {
                        strBuild.Append("\t");
                    }
                    var horizontalAxisStr = direction == Enums.Shmoos.AxisDirection.XY ? "X" : "Y";
                    var horizontalAxisMode = direction == Enums.Shmoos.AxisDirection.XY ? (xAsixModel ?? "") : (yAxisModel ?? "");
                    strBuild.Append(horizontalAxisStr + " Axis:\t" + horizontalAxisMode);
                }
                else
                {
                    strBuild.Append("\t");
                    var orderXResults = results.OrderBy(x => x.XCoordinate).ToList();
                    foreach (var shmooResult in orderXResults)
                        strBuild.Append("\t" + GetResultSymbol(shmooResult, siteNo));

                    strBuild.Append("\n");
                    strBuild.Append(str + " Axis:\t" + xAsixModel);
                    foreach (var shmooResult in orderXResults)
                        strBuild.Append("\t" + shmooResult.XCoordinate + xUnit);
                }
                strBuild.Append("\n\n");
            }
            return strBuild;
        }

        private string GetResultSymbol(ShmooResult shmooResult, int siteNo)
        {
            if (!shmooResult.IsResultValid)
            {
                return "NA";
            }
            //if (shmooResult.Result.SiteValue[siteNo] != 0.0)
            //{
            //    return "F";
            //}
            return "P";
        }
    }
}
