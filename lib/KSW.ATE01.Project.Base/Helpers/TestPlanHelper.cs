using KSW.ATE01.Project.Base.Enums.Projects;
using KSW.ATE01.Project.Base.Enums.TestPlans;
using KSW.ATE01.Project.Base.Models.Projects;
using KSW.ATE01.Project.Base.Models.TestPlans;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace KSW.ATE01.Project.Base.Helpers
{
    /// <summary>
    /// 测试计划帮助类
    /// </summary>
    public class TestPlanHelper
    {
        private static readonly Lazy<TestPlanHelper> _instance = new Lazy<TestPlanHelper>(() => new TestPlanHelper());

        #region Fields
        private TestPlanModel _testPlan;

        private static readonly string _channelDataStartCell = "A3";
        private static readonly string _testItemDataStartCell = "A2";
        private static readonly string _limitsDataStartCell = "A2";
        private static readonly string _flowDataStartCell = "A2";
        private static readonly string _levelDataStartCell = "A2";
        private static readonly string _timingDataStartCell = "A2";
        private static readonly string _globalDataStartCell = "A2";
        private static readonly string _testPlanName = "TestPlan";

        #endregion

        public static string ExcelExtension { get; } = ".xlsx";

        /// <summary>
        /// 加载测试项
        /// </summary>
        public static TestPlanModel LoadTestPlan(ProjectInfo projectInfo)
        {
            TestPlanModel result = new TestPlanModel();
            try
            {
                switch (projectInfo.TestPlanType)
                {
                    case TestPlanType.Excel:
                        result = LoadTestPlanFromExcel(projectInfo);
                        break;
                    case TestPlanType.Csv:
                        result = LoadTestPlanFromCsv(projectInfo);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        private static TestPlanModel LoadTestPlanFromExcel(ProjectInfo projectInfo)
        {
            try
            {
                var testPlanPath = Path.Combine(projectInfo?.ReleasePath, projectInfo?.ProjectName + ExcelExtension);
                if (string.IsNullOrEmpty(testPlanPath) || !File.Exists(testPlanPath))
                    throw new FileNotFoundException("测试计划文件不存在");

                return LoadTestPlanFromExcel(testPlanPath);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static TestPlanModel LoadTestPlanFromExcel(string testPlanPath)
        {
            try
            {
                if (!File.Exists(testPlanPath))
                    throw new FileNotFoundException("测试计划文件不存在");

                var sheetNames = MiniExcel.GetSheetNames(testPlanPath);
                if (sheetNames == null || !sheetNames.Any())
                    return null;

                var result = new TestPlanModel();
                foreach (var sheetName in sheetNames)
                {
                    var rows = MiniExcel.Query(testPlanPath, sheetName: sheetName).ToList();
                    string sheetTypeStr = string.Empty;
                    if (rows.Any())
                        sheetTypeStr = System.Convert.ToString(rows[0].A);
                    if (Enum.TryParse(sheetTypeStr, out TestPlanSheetType sheetType))
                    {
                        switch (sheetType)
                        {
                            case TestPlanSheetType.Channel:
                                var siteCountStr = rows[1].A?.ToString();
                                if (!siteCountStr.ToLower().Equals("sitecount"))
                                    throw new Exception($"SiteCount关键字不匹配，当前是{siteCountStr}在单元格[1,0],请检查");

                                if (!int.TryParse(rows[1].B?.ToString(), out int siteCount))
                                    throw new Exception($"SiteCount值不正确，当前是{rows[1].B?.ToString()}在单元格[1,1],请检查");

                                GetChannelData(result, siteCount, testPlanPath, sheetName);
                                break;
                            case TestPlanSheetType.TestItem:
                                GetTestItemData(result, testPlanPath, sheetName);
                                break;
                            case TestPlanSheetType.Limits:
                                GetLimitsData(result, testPlanPath, sheetName);
                                break;
                            case TestPlanSheetType.Flow:
                                GetFlowData(result, testPlanPath, sheetName);
                                break;
                            case TestPlanSheetType.Level:
                                GetLevelData(result, testPlanPath, sheetName);
                                break;
                            case TestPlanSheetType.Timing:
                                GetTimingData(result, testPlanPath, sheetName);
                                break;
                            case TestPlanSheetType.Global:
                                GetGlobalData(result, testPlanPath, sheetName);
                                break;
                            default:
                                break;
                        }
                    }
                }

                _instance.Value._testPlan = result;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GetChannelData(TestPlanModel result, dynamic siteCount, string filePath, string sheetName)
        {
            try
            {
                var isCorrect = CheckChannelData(siteCount, filePath, sheetName);
                if (!isCorrect)
                    return;

                var rows = MiniExcel.QueryRange(filePath, useHeaderRow: true, sheetName: sheetName, startCell: _channelDataStartCell)?.Cast<IDictionary<string, object>>();
                if (!rows.Any())
                    return;

                if (result == null)
                    return;

                var channels = new List<ChannelModel>();
                var groups = new List<PinGroupModel>();
                var channelTypeDic = EnumHelper.GetEnumDescriptionDic<ChannelType>();
                var rowList = rows.ToList();
                PinGroupModel lastGroup = null;

                foreach (var row in rowList)
                {
                    var groupName = string.Empty;
                    var pinName = string.Empty;
                    var type = ChannelType.IO;
                    var index = rowList.IndexOf(row);

                    if (row.ContainsKey("GroupName") && row["GroupName"] != null)
                    {
                        groupName = row["GroupName"]?.ToString();
                        if (groups.Any(x => x.Name.ToLower().Equals(groupName.ToLower())))
                            throw new Exception($"GroupName重复,当前是在单元格[{index + 4},0],请检查。");

                        lastGroup = new PinGroupModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = groupName
                        };
                        groups.Add(lastGroup);
                    }

                    if (row.ContainsKey("PinName") && row["PinName"] != null)
                        pinName = row["PinName"]?.ToString();

                    if (row.ContainsKey("Type") && row["Type"] != null && channelTypeDic.Keys.Contains(row["Type"]?.ToString()))
                        type = channelTypeDic[row["Type"]?.ToString()];

                    ChannelModel tempChannel = null;
                    if (channels.Any(x => x.PinName.ToLower().Equals(pinName.ToLower())))
                    {
                        tempChannel = channels.FirstOrDefault(x => x.PinName.ToLower().Equals(pinName.ToLower()));
                        if (lastGroup != null && !tempChannel.Groups.Contains(lastGroup))
                        {
                            tempChannel.Groups.Add(lastGroup);
                        }
                    }
                    else
                    {
                        tempChannel = new ChannelModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            PinName = pinName,
                            Type = type,
                        };
                        if (lastGroup != null)
                            tempChannel.Groups.Add(lastGroup);
                        if (!string.IsNullOrEmpty(tempChannel.PinName))
                            channels.Add(tempChannel);
                    }

                    GetSites(siteCount, row, tempChannel);
                }
                result.Channel = channels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool CheckChannelData(int siteCount, string filePath, string sheetName)
        {
            var result = false;
            try
            {
                var cols = MiniExcel.GetColumns(filePath, useHeaderRow: true, sheetName: sheetName, excelType: ExcelType.XLSX, startCell: _channelDataStartCell);
                var colIndex = 0;

                if (cols.Count != 3 + siteCount)
                {
                    throw new Exception("单元列头数不正确，请检查。");
                }

                foreach (var col in cols)
                {
                    switch (colIndex)
                    {
                        case 0:
                            if (col?.ToLower()?.Equals("groupname") != true)
                                throw new Exception($"GroupName关键字不匹配,当前是{col}在单元格[2,0],请检查。");
                            break;
                        case 1:
                            if (col?.ToLower()?.Equals("pinname") != true)
                                throw new Exception($"PinName关键字不匹配,当前是{col}在单元格[2,1],请检查。");
                            break;
                        case 2:
                            if (col?.ToLower()?.Equals("type") != true)
                                throw new Exception($"Type关键字不匹配,当前是{col}在单元格[2,2],请检查。");
                            break;
                        default:
                            break;
                    }
                    if (colIndex > 2)
                    {
                        if (!col.Trim().StartsWith("Site "))
                            throw new Exception($"站点名称{col}没有以'Site '关键字为开头。");
                        if (col.Trim().Replace("Site ", "") != (colIndex - 3).ToString())
                            throw new Exception($"站点名称的站点编号不正确，站点编码应该为{colIndex - 3}。");
                    }

                    colIndex++;
                }
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static void GetSites(int siteCount, IDictionary<string, object> row, ChannelModel tempChannel)
        {
            if (tempChannel == null)
                return;

            var siteList = tempChannel.Sites;
            if (siteList == null)
                siteList = new List<SiteModel>();

            foreach (var col in row)
            {
                if (col.Key.ToString()?.StartsWith("Site ") == true)
                    siteList.Add(new SiteModel() { SiteName = col.Key, SiteValue = col.Value?.ToString() });
            }
            tempChannel.Sites = siteList;
        }

        private static void GetTestItemData(TestPlanModel result, string filePath, string sheetName)
        {
            try
            {
                var isCorrect = CheckTestItemData(filePath, sheetName);
                if (!isCorrect)
                    return;

                var rows = MiniExcel.QueryRange(filePath, useHeaderRow: true, sheetName: sheetName, startCell: _testItemDataStartCell)?.Cast<IDictionary<string, object>>();
                if (!rows.Any())
                    return;

                if (result == null)
                    return;

                var testItems = new List<TestItemModel>();
                foreach (var row in rows)
                {
                    var tempTestItem = new TestItemModel();
                    tempTestItem.SheetName = sheetName;
                    tempTestItem.Id = Guid.NewGuid().ToString();

                    if (row.ContainsKey("TestItemName") && row["TestItemName"] != null)
                        tempTestItem.TestItemName = row["TestItemName"]?.ToString();

                    if (row.ContainsKey("FunctionName") && row["FunctionName"] != null)
                        tempTestItem.FunctionName = row["FunctionName"]?.ToString();

                    if (row.ContainsKey("Force") && row["Force"] != null)
                        tempTestItem.Force = System.Convert.ToDecimal(row["Force"]?.ToString());

                    if (row.ContainsKey("Pins") && row["Pins"] != null)
                        tempTestItem.Pins = row["Pins"]?.ToString();

                    if (row.ContainsKey("Level") && row["Level"] != null)
                        tempTestItem.Level = row["Level"]?.ToString();

                    if (row.ContainsKey("Timing") && row["Timing"] != null)
                        tempTestItem.Timing = row["Timing"]?.ToString();

                    GetChannelArgs(row, tempTestItem);
                    if (!string.IsNullOrEmpty(tempTestItem.TestItemName))
                        testItems.Add(tempTestItem);
                }
                result.TestItem = testItems;

                if (result.Flow != null && result.Flow.Any())
                {
                    foreach (var flow in result.Flow)
                    {
                        var testItem = result.TestItem.FirstOrDefault(x => x.TestItemName.Equals(flow.TestItemName));
                        if (testItem != null)
                            flow.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                    }
                }

                if (result.Limits != null && result.Limits.Any())
                {
                    foreach (var limit in result.Limits)
                    {
                        var testItem = result.TestItem.FirstOrDefault(x => x.TestItemName.Equals(limit.TestItemName));
                        if (testItem != null)
                            limit.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool CheckTestItemData(string filePath, string sheetName)
        {
            var result = false;
            try
            {
                var cols = MiniExcel.GetColumns(filePath, useHeaderRow: true, sheetName: sheetName, excelType: ExcelType.XLSX, startCell: _testItemDataStartCell);
                var colIndex = 0;

                foreach (var col in cols)
                {
                    switch (colIndex)
                    {
                        case 0:
                            if (col?.ToLower()?.Equals("testitemname") != true)
                                throw new Exception($"TestItemName关键字不匹配,当前是{col}在单元格[2,0],请检查。");
                            break;
                        case 1:
                            if (col?.ToLower()?.Equals("functionname") != true)
                                throw new Exception($"FunctionName关键字不匹配,当前是{col}在单元格[2,1],请检查。");
                            break;
                        case 2:
                            if (col?.ToLower()?.Equals("force") != true)
                                throw new Exception($"force关键字不匹配,当前是{col}在单元格[2,2],请检查。");
                            break;
                        case 3:
                            if (col?.ToLower()?.Equals("pins") != true)
                                throw new Exception($"Pins关键字不匹配,当前是{col}在单元格[2,3],请检查。");
                            break;
                        case 4:
                            if (col?.ToLower()?.Equals("level") != true)
                                throw new Exception($"Level关键字不匹配,当前是{col}在单元格[2,4],请检查。");
                            break;
                        case 5:
                            if (col?.ToLower()?.Equals("timing") != true)
                                throw new Exception($"Timing关键字不匹配,当前是{col}在单元格[2,5],请检查。");
                            break;
                        default:
                            break;
                    }
                    colIndex++;
                }
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static void GetChannelArgs(IDictionary<string, object> row, TestItemModel testItem)
        {
            var args = new List<TestItemParamModel>();
            var colIndex = 0;
            foreach (var col in row)
            {
                if (colIndex > 5 && col.Value != null)
                {
                    var testItemParam = new TestItemParamModel();
                    testItemParam.ParamName = col.Key;
                    testItemParam.ParamValue = col.Value?.ToString();
                    args.Add(testItemParam);
                }
                colIndex++;
            }
            testItem.Args = args;
        }

        private static void GetLimitsData(TestPlanModel result, string filePath, string sheetName)
        {
            try
            {
                var isCorrect = CheckLimitsData(filePath, sheetName);
                if (!isCorrect)
                    return;

                var rows = MiniExcel.Query<LimitsModel>(filePath, sheetName: sheetName, startCell: _limitsDataStartCell);
                if (!rows.Any())
                    return;

                if (result == null)
                    return;
                var limits = rows.Where(x => !string.IsNullOrEmpty(x.TestItemName)).ToList();
                foreach (var limit in limits)
                {
                    limit.Id = Guid.NewGuid().ToString();
                    var testItem = result?.TestItem?.FirstOrDefault(x => x.TestItemName?.Equals(limit.TestItemName) == true);
                    limit.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                }
                result.Limits = limits;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool CheckLimitsData(string filePath, string sheetName)
        {
            var result = false;
            try
            {
                var cols = MiniExcel.GetColumns(filePath, useHeaderRow: true, sheetName: sheetName, excelType: ExcelType.XLSX, startCell: _limitsDataStartCell);
                var colIndex = 0;

                foreach (var col in cols)
                {
                    switch (colIndex)
                    {
                        case 0:
                            if (col?.ToLower()?.Equals("testitemname") != true)
                                throw new Exception($"TestItemName关键字不匹配,当前是{col}在单元格[2,0],请检查。");
                            break;
                        case 1:
                            if (col?.ToLower()?.Equals("testnumber") != true)
                                throw new Exception($"TestNumber关键字不匹配,当前是{col}在单元格[2,1],请检查。");
                            break;
                        case 2:
                            if (col?.ToLower()?.Equals("lowlimit") != true)
                                throw new Exception($"LowLimit关键字不匹配,当前是{col}在单元格[2,2],请检查。");
                            break;
                        case 3:
                            if (col?.ToLower()?.Equals("highlimit") != true)
                                throw new Exception($"HighLimit关键字不匹配,当前是{col}在单元格[2,3],请检查。");
                            break;
                        case 4:
                            if (col?.ToLower()?.Equals("units") != true)
                                throw new Exception($"Units关键字不匹配,当前是{col}在单元格[2,4],请检查。");
                            break;
                        case 5:
                            if (col?.ToLower()?.Equals("limitname") != true)
                                throw new Exception($"LimitName关键字不匹配,当前是{col}在单元格[2,5],请检查。");
                            break;
                        case 6:
                            if (col?.ToLower()?.Equals("failsoftwarebin") != true)
                                throw new Exception($"FailSoftwareBin关键字不匹配,当前是{col}在单元格[2,6],请检查。");
                            break;
                        case 7:
                            if (col?.ToLower()?.Equals("passsoftwarebin") != true)
                                throw new Exception($"PassSoftwareBin关键字不匹配,当前是{col}在单元格[2,7],请检查。");
                            break;
                        case 8:
                            if (col?.ToLower()?.Equals("failhardwarebin") != true)
                                throw new Exception($"FailHardwareBin关键字不匹配,当前是{col}在单元格[2,8],请检查。");
                            break;
                        case 9:
                            if (col?.ToLower()?.Equals("passhardwarebin") != true)
                                throw new Exception($"PassHardwareBin关键字不匹配,当前是{col}在单元格[2,9],请检查。");
                            break;
                        case 10:
                            if (col?.ToLower()?.Equals("dutresult") != true)
                                throw new Exception($"DUTResult关键字不匹配,当前是{col}在单元格[2,10],请检查。");
                            break;
                        default:
                            break;
                    }
                    colIndex++;
                }
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static void GetFlowData(TestPlanModel result, string filePath, string sheetName)
        {
            try
            {
                var isCorrect = CheckFlowData(filePath, sheetName);
                if (!isCorrect)
                    return;

                var rows = MiniExcel.Query<FlowModel>(filePath, sheetName: sheetName, startCell: _flowDataStartCell);
                if (!rows.Any())
                    return;

                if (result == null)
                    return;

                var index = 0;
                var flows = rows.Where(x => !string.IsNullOrEmpty(x.TestItemName)).ToList();
                foreach (var flow in flows)
                {
                    flow.Id = Guid.NewGuid().ToString();
                    var testItem = result?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(flow.TestItemName));
                    flow.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                    flow.IsSelected = string.IsNullOrEmpty(flow.Enable);
                    flow.SortId = ++index;
                }
                result.Flow = flows;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static bool CheckFlowData(string filePath, string sheetName)
        {
            var result = false;
            try
            {
                var cols = MiniExcel.GetColumns(filePath, useHeaderRow: true, sheetName: sheetName, excelType: ExcelType.XLSX, startCell: _flowDataStartCell);
                var colIndex = 0;

                foreach (var col in cols)
                {
                    switch (colIndex)
                    {
                        case 0:
                            if (col?.ToLower()?.Equals("testitemname") != true)
                                throw new Exception($"TestItemName关键字不匹配,当前是{col}在单元格[2,0],请检查。");
                            break;
                        case 1:
                            if (col?.ToLower()?.Equals("sheetname") != true)
                                throw new Exception($"SheetName关键字不匹配,当前是{col}在单元格[2,1],请检查。");
                            break;
                        case 2:
                            if (col?.ToLower()?.Equals("enable") != true)
                                throw new Exception($"Enable关键字不匹配,当前是{col}在单元格[2,2],请检查。");
                            break;
                        default:
                            break;
                    }
                    colIndex++;
                }
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static void GetLevelData(TestPlanModel result, string filePath, string sheetName)
        {
            try
            {
                var isCorrect = CheckLevelData(filePath, sheetName);
                if (!isCorrect)
                    return;

                var rows = MiniExcel.QueryRange(filePath, useHeaderRow: true, sheetName: sheetName, startCell: _levelDataStartCell)?.Cast<IDictionary<string, object>>();
                if (!rows.Any())
                    return;

                var testLevelModels = new List<LevelModel>();

                foreach (var row in rows)
                {
                    var tempLevelModel = new LevelModel();
                    tempLevelModel.Id = Guid.NewGuid().ToString();

                    if (row.ContainsKey("Pin/Group") && row["Pin/Group"] != null)
                        tempLevelModel.PinGroupName = row["Pin/Group"]?.ToString();

                    if (row.ContainsKey("Vil") && row["Vil"] != null)
                        tempLevelModel.Vil = System.Convert.ToDecimal(row["Vil"]?.ToString());

                    if (row.ContainsKey("Vih") && row["Vih"] != null)
                        tempLevelModel.Vih = System.Convert.ToDecimal(row["Vih"]?.ToString());

                    if (row.ContainsKey("Vol") && row["Vol"] != null)
                        tempLevelModel.Vol = System.Convert.ToDecimal(row["Vol"]?.ToString());

                    if (row.ContainsKey("Voh") && row["Voh"] != null)
                        tempLevelModel.Voh = System.Convert.ToDecimal(row["Voh"]?.ToString());

                    if (row.ContainsKey("Iol") && row["Iol"] != null)
                        tempLevelModel.Iol = System.Convert.ToDecimal(row["Iol"]?.ToString());

                    if (row.ContainsKey("Ioh") && row["Ioh"] != null)
                        tempLevelModel.Ioh = System.Convert.ToDecimal(row["Ioh"]?.ToString());

                    if (row.ContainsKey("Vt") && row["Vt"] != null)
                        tempLevelModel.Vt = System.Convert.ToDecimal(row["Vt"]?.ToString());

                    if (row.ContainsKey("Vcl") && row["Vcl"] != null)
                        tempLevelModel.Vcl = System.Convert.ToDecimal(row["Vcl"]?.ToString());

                    if (row.ContainsKey("Vch") && row["Vch"] != null)
                        tempLevelModel.Vch = System.Convert.ToDecimal(row["Vch"]?.ToString());

                    if (row.ContainsKey("PS(V)") && row["PS(V)"] != null)
                        tempLevelModel.PS = System.Convert.ToDecimal(row["PS(V)"]?.ToString());

                    if (row.ContainsKey("I(A)") && row["I(A)"] != null)
                        tempLevelModel.I = System.Convert.ToDecimal(row["I(A)"]?.ToString());

                    if (row.ContainsKey("Tdelay(S)") && row["Tdelay(S)"] != null)
                        tempLevelModel.Tdelay = System.Convert.ToDecimal(row["Tdelay(S)"]?.ToString());

                    if (row.ContainsKey("Sequence") && row["Sequence"] != null)
                        tempLevelModel.Sequence = row["Sequence"]?.ToString();

                    if (row.ContainsKey("Comment") && row["Comment"] != null)
                        tempLevelModel.Comment = row["Comment"]?.ToString();

                    if (!string.IsNullOrEmpty(tempLevelModel.PinGroupName))
                        testLevelModels.Add(tempLevelModel);
                }

                var testItems = result?.TestItem?.Where(x => x?.Level?.Equals(sheetName) == true).Select(x => x);
                foreach (var testItem in testItems)
                    testItem.Levels = testLevelModels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool CheckLevelData(string filePath, string sheetName)
        {
            var result = false;
            try
            {
                var cols = MiniExcel.GetColumns(filePath, useHeaderRow: true, sheetName: sheetName, excelType: ExcelType.XLSX, startCell: _levelDataStartCell);
                var colIndex = 0;

                foreach (var col in cols)
                {
                    switch (colIndex)
                    {
                        case 0:
                            if (col?.ToLower()?.Equals("pin/group") != true)
                                throw new Exception($"Pin/Group关键字不匹配,当前是{col}在单元格[2,0],请检查。");
                            break;
                        case 1:
                            if (col?.ToLower()?.Equals("vil") != true)
                                throw new Exception($"Vil关键字不匹配,当前是{col}在单元格[2,1],请检查。");
                            break;
                        case 2:
                            if (col?.ToLower()?.Equals("vih") != true)
                                throw new Exception($"Vih关键字不匹配,当前是{col}在单元格[2,2],请检查。");
                            break;
                        case 3:
                            if (col?.ToLower()?.Equals("vol") != true)
                                throw new Exception($"Vol关键字不匹配,当前是{col}在单元格[2,3],请检查。");
                            break;
                        case 4:
                            if (col?.ToLower()?.Equals("voh") != true)
                                throw new Exception($"Voh关键字不匹配,当前是{col}在单元格[2,4],请检查。");
                            break;
                        case 5:
                            if (col?.ToLower()?.Equals("iol") != true)
                                throw new Exception($"Iol关键字不匹配,当前是{col}在单元格[2,5],请检查。");
                            break;
                        case 6:
                            if (col?.ToLower()?.Equals("ioh") != true)
                                throw new Exception($"Ioh关键字不匹配,当前是{col}在单元格[2,6],请检查。");
                            break;
                        case 7:
                            if (col?.ToLower()?.Equals("vt") != true)
                                throw new Exception($"Vt关键字不匹配,当前是{col}在单元格[2,7],请检查。");
                            break;
                        case 8:
                            if (col?.ToLower()?.Equals("vcl") != true)
                                throw new Exception($"Vcl关键字不匹配,当前是{col}在单元格[2,8],请检查。");
                            break;
                        case 9:
                            if (col?.ToLower()?.Equals("vch") != true)
                                throw new Exception($"Vch关键字不匹配,当前是{col}在单元格[2,9],请检查。");
                            break;
                        default:
                            break;
                    }
                    colIndex++;
                }
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static void GetTimingData(TestPlanModel result, string filePath, string sheetName)
        {
            try
            {
                var isCorrect = CheckTimingData(filePath, sheetName);
                if (!isCorrect)
                    return;

                var rows = MiniExcel.Query<TimingModel>(filePath, sheetName: sheetName, startCell: _timingDataStartCell);
                if (!rows.Any())
                    return;

                if (result == null)
                    return;
                var timings = rows.Where(x => !string.IsNullOrEmpty(x.TimingName)).ToList();
                foreach (var timing in timings)
                    timing.Id = Guid.NewGuid().ToString();

                var testItems = result?.TestItem?.Where(x => x?.Timing?.Equals(sheetName) == true).Select(x => x);
                foreach (var testItem in testItems)
                    testItem.Timings = timings;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void GetGlobalData(TestPlanModel result, string filePath, string sheetName)
        {
            try
            {
                var rows = MiniExcel.QueryRange(filePath, useHeaderRow: true, sheetName: sheetName, startCell: _globalDataStartCell)?.Cast<IDictionary<string, object>>();
                if (!rows.Any())
                    return;

                if (result == null)
                    return;

                var globals = new List<GlobalModel>();
                var rowList = rows.ToList();

                foreach (var row in rowList)
                {
                    GlobalModel tempGlobal = null;

                    if (row.ContainsKey("PatternFile") && row["PatternFile"] != null)
                    {
                        var patternFileName = row["PatternFile"]?.ToString();
                        tempGlobal = new GlobalModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            PatternFile = patternFileName
                        };
                        globals.Add(tempGlobal);
                    }

                    GetGlobalParams(row, tempGlobal);
                }

                result.Global = globals;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void GetGlobalParams(IDictionary<string, object> row, GlobalModel? tempGlobal)
        {
            if (tempGlobal == null)
                return;

            var paramList = tempGlobal.Args;
            if (paramList == null)
                paramList = new List<GlobalParamModel>();

            foreach (var col in row)
            {
                if (col.Key.ToString()?.Equals("PatternFile") == false && !string.IsNullOrEmpty(col.Key?.ToString()) && !string.IsNullOrEmpty(col.Value?.ToString()))
                    paramList.Add(new GlobalParamModel() { ParamName = col.Key, ParamValue = col.Value?.ToString() });
            }
            tempGlobal.Args = paramList;
        }

        private static bool CheckTimingData(string filePath, string sheetName)
        {
            var result = false;
            try
            {
                var cols = MiniExcel.GetColumns(filePath, useHeaderRow: true, sheetName: sheetName, excelType: ExcelType.XLSX, startCell: _timingDataStartCell);
                var colIndex = 0;

                foreach (var col in cols)
                {
                    switch (colIndex)
                    {
                        case 0:
                            if (col?.ToLower()?.Equals("timingname") != true)
                                throw new Exception($"TimingName关键字不匹配,当前是{col}在单元格[2,0],请检查。");
                            break;
                        case 1:
                            if (col?.ToLower()?.Equals("period") != true)
                                throw new Exception($"Period关键字不匹配,当前是{col}在单元格[2,1],请检查。");
                            break;
                        case 2:
                            if (col?.ToLower()?.Equals("pinname") != true)
                                throw new Exception($"PinName关键字不匹配,当前是{col}在单元格[2,2],请检查。");
                            break;
                        case 3:
                            if (col?.ToLower()?.Equals("pinsetup") != true)
                                throw new Exception($"PinSetup关键字不匹配,当前是{col}在单元格[2,3],请检查。");
                            break;
                        case 4:
                            if (col?.ToLower()?.Equals("fmt") != true)
                                throw new Exception($"Fmt关键字不匹配,当前是{col}在单元格[2,4],请检查。");
                            break;
                        case 5:
                            if (col?.ToLower()?.Equals("drivea") != true)
                                throw new Exception($"DriveA关键字不匹配,当前是{col}在单元格[2,5],请检查。");
                            break;
                        case 6:
                            if (col?.ToLower()?.Equals("driveb") != true)
                                throw new Exception($"DriveB关键字不匹配,当前是{col}在单元格[2,6],请检查。");
                            break;
                        case 7:
                            if (col?.ToLower()?.Equals("drivec") != true)
                                throw new Exception($"DriveC关键字不匹配,当前是{col}在单元格[2,7],请检查。");
                            break;
                        case 8:
                            if (col?.ToLower()?.Equals("drived") != true)
                                throw new Exception($"DriveD关键字不匹配,当前是{col}在单元格[2,8],请检查。");
                            break;
                        case 9:
                            if (col?.ToLower()?.Equals("strobemode") != true)
                                throw new Exception($"StrobeMode关键字不匹配,当前是{col}在单元格[2,9],请检查。");
                            break;
                        case 10:
                            if (col?.ToLower()?.Equals("strobea") != true)
                                throw new Exception($"StrobeA关键字不匹配,当前是{col}在单元格[2,10],请检查。");
                            break;
                        case 11:
                            if (col?.ToLower()?.Equals("strobeb") != true)
                                throw new Exception($"strobeB关键字不匹配,当前是{col}在单元格[2,11],请检查。");
                            break;
                        case 12:
                            if (col?.ToLower()?.Equals("comment") != true)
                                throw new Exception($"Comment关键字不匹配,当前是{col}在单元格[2,12],请检查。");
                            break;
                        default:
                            break;
                    }
                    colIndex++;
                }
                result = true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static TestPlanModel LoadTestPlanFromCsv(ProjectInfo projectInfo)
        {
            var result = new TestPlanModel();

            try
            {
                var testPlanDir = Path.Combine(projectInfo?.ProjectPath, _testPlanName);

                result = LoadTestPlanFromCsv(testPlanDir);

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static TestPlanModel LoadTestPlanFromCsv(string testPlanDir)
        {
            var result = new TestPlanModel();

            try
            {
                if (string.IsNullOrEmpty(testPlanDir) || !Directory.Exists(testPlanDir))
                    throw new FileNotFoundException("测试计划文件夹不存在");

                var csvFiles = Directory.GetFiles(testPlanDir, "*.csv");
                if (csvFiles?.Any() == false)
                    throw new FileNotFoundException("测试计划文件不存在");

                var orderFiles = GetReorderCsvFiles(csvFiles);

                foreach (var csvFile in orderFiles)
                {
                    var sheetName = Path.GetFileNameWithoutExtension(csvFile.Key);
                    using (var reader = new StreamReader(csvFile.Key))
                    {
                        if (!reader.EndOfStream)
                        {
                            var row = reader.ReadLine();
                            switch (csvFile.Value)
                            {
                                case TestPlanSheetType.Channel:
                                    if (!reader.EndOfStream)
                                    {
                                        row = reader.ReadLine();
                                        var cols = row.Split(',');
                                        string[] colHeaders = null;
                                        var siteCount = string.IsNullOrEmpty(cols[1]) ? 0 : System.Convert.ToInt32(cols[1]);
                                        if (!reader.EndOfStream)
                                        {
                                            row = reader.ReadLine();
                                            colHeaders = row.Split(',');
                                        }
                                        GetChannelData(result, siteCount, reader, colHeaders);
                                    }
                                    break;
                                case TestPlanSheetType.TestItem:
                                    if (!reader.EndOfStream)
                                    {
                                        row = reader.ReadLine();
                                        var cols = row.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        GetTestItemData(result, reader, sheetName, cols);
                                    }
                                    break;
                                case TestPlanSheetType.Limits:
                                    if (!reader.EndOfStream)
                                        reader.ReadLine();
                                    GetLimitsData(result, reader, sheetName);
                                    break;
                                case TestPlanSheetType.Flow:
                                    if (!reader.EndOfStream)
                                        reader.ReadLine();
                                    GetFlowData(result, reader, sheetName);
                                    break;
                                case TestPlanSheetType.Level:
                                    if (!reader.EndOfStream)
                                        reader.ReadLine();
                                    GetLevelData(result, reader, sheetName);
                                    break;
                                case TestPlanSheetType.Timing:
                                    if (!reader.EndOfStream)
                                        reader.ReadLine();
                                    GetTimingData(result, reader, sheetName);
                                    break;
                                case TestPlanSheetType.Global:
                                    if (!reader.EndOfStream)
                                    {
                                        row = reader.ReadLine();
                                        var colHeaders = row.Split(',');
                                        GetGlobalData(result, reader, sheetName, colHeaders);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                _instance.Value._testPlan = result;
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Dictionary<string, TestPlanSheetType> GetReorderCsvFiles(string[] csvFiles)
        {
            try
            {
                var result = new Dictionary<string, TestPlanSheetType>();
                foreach (var csvFile in csvFiles)
                {
                    using (var reader = new StreamReader(csvFile))
                    {
                        if (!reader.EndOfStream)
                        {
                            var row = reader.ReadLine();
                            var cols = row.Split(',');
                            var sheetTypeStr = cols[0];
                            if (Enum.TryParse(sheetTypeStr, out TestPlanSheetType sheetType))
                                result.Add(csvFile, sheetType);
                        }
                    }
                }
                return result.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void GetChannelData(TestPlanModel result, int siteCount, StreamReader reader, string[] colHeaders)
        {
            var channels = new List<ChannelModel>();
            var groups = new List<PinGroupModel>();
            var channelTypeDic = EnumHelper.GetEnumDescriptionDic<ChannelType>();
            if (result == null)
                return;

            if (colHeaders == null || !colHeaders.Any())
                return;

            try
            {
                var index = 0;
                PinGroupModel lastGroup = null;
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    index++;
                    var cols = row.Split(',');
                    var colIndex = 0;
                    if (!cols.Any())
                        continue;

                    var groupName = string.Empty;
                    var pinName = string.Empty;
                    var type = ChannelType.IO;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                    {
                        groupName = cols[colIndex];
                        if (groups.Any(x => x.Name.ToLower().Equals(groupName.ToLower())))
                            throw new Exception($"GroupName重复,当前行{index + 3},请检查。");

                        lastGroup = new PinGroupModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = groupName
                        };
                        groups.Add(lastGroup);
                    }
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        pinName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        type = channelTypeDic[cols[colIndex]];
                    colIndex++;

                    ChannelModel tempChannel = null;
                    if (channels.Any(x => x.PinName.ToLower().Equals(pinName.ToLower())))
                    {
                        tempChannel = channels.FirstOrDefault(x => x.PinName.ToLower().Equals(pinName.ToLower()));
                        if (lastGroup != null && !tempChannel.Groups.Contains(lastGroup))
                        {
                            tempChannel.Groups.Add(lastGroup);
                        }
                    }
                    else
                    {
                        tempChannel = new ChannelModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            PinName = pinName,
                            Type = type,
                        };

                        if (lastGroup != null)
                            tempChannel.Groups.Add(lastGroup);
                        channels.Add(tempChannel);
                    }

                    if (colHeaders.Length > 2 + siteCount && cols.Length > 2 + siteCount && siteCount > 0)
                        GetSites(siteCount, colIndex, colHeaders, cols, tempChannel);
                }

                result.Channel = channels;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GetSites(int siteCount, int startColIndex, string[] colHeaders, string[] cols, ChannelModel tempChannel)
        {
            if (tempChannel == null)
                return;

            var siteList = tempChannel.Sites;
            if (siteList == null)
                siteList = new List<SiteModel>();

            for (int i = 0; i < siteCount; i++)
            {
                var siteName = string.Empty;
                var siteValue = string.Empty;

                if (colHeaders.Length > startColIndex + i && !string.IsNullOrEmpty(colHeaders[startColIndex + i]))
                    siteName = colHeaders[startColIndex + i];

                if (cols.Length > startColIndex + i && !string.IsNullOrEmpty(cols[startColIndex + i]))
                    siteValue = cols[startColIndex + i];

                if (!string.IsNullOrEmpty(siteName) && !string.IsNullOrEmpty(siteValue))
                    siteList.Add(new SiteModel() { SiteName = siteName, SiteValue = siteValue });
            }
            tempChannel.Sites = siteList;
        }

        private static void GetTestItemData(TestPlanModel result, StreamReader reader, string sheetName, string[] colHeaders)
        {
            if (result == null)
                return;

            var testItems = new List<TestItemModel>();
            try
            {
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(',');
                    var colIndex = 0;
                    if (!cols.Any())
                        continue;

                    var tempTestItem = new TestItemModel();
                    tempTestItem.SheetName = sheetName;
                    tempTestItem.Id = Guid.NewGuid().ToString();
                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        tempTestItem.TestItemName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        tempTestItem.FunctionName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal force))
                        tempTestItem.Force = force;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        tempTestItem.Pins = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        tempTestItem.Level = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        tempTestItem.Timing = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex)
                        GetChannelArgs(colIndex, cols, tempTestItem, colHeaders);

                    testItems.Add(tempTestItem);
                }
                result.TestItem = testItems;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void GetChannelArgs(int startColIndex, string[] cols, TestItemModel testItem, string[] colHeaders)
        {
            var args = new List<TestItemParamModel>();
            int argsCount = cols.Length - startColIndex;
            if (argsCount > 0)
                for (int i = 0; i < argsCount; i++)
                {
                    if (!string.IsNullOrEmpty(cols[startColIndex + i]))
                    {
                        var tempTestItemParam = new TestItemParamModel();
                        tempTestItemParam.ParamName = colHeaders[startColIndex + i];
                        tempTestItemParam.ParamValue = cols[startColIndex + i];
                        args.Add(tempTestItemParam);
                    }
                }

            testItem.Args = args;
        }

        private static void GetLimitsData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;
            try
            {
                var limits = new List<LimitsModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(',');
                    var colIndex = 0;

                    if (!cols.Any())
                        continue;

                    var limitModel = new LimitsModel();
                    limitModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        limitModel.TestItemName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && int.TryParse(cols[colIndex], out int testNumber))
                        limitModel.TestNumber = testNumber;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex], out decimal lowLimit))
                        limitModel.LowLimit = lowLimit;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex], out decimal highLimit))
                        limitModel.HighLimit = highLimit;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        limitModel.Units = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        limitModel.LimitName = cols[colIndex];

                    limits.Add(limitModel);
                }


                foreach (var limit in limits)
                {
                    limit.Id = Guid.NewGuid().ToString();
                    var testItem = result?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(limit.TestItemName));
                    limit.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                }

                result.Limits = limits;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GetFlowData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;

            try
            {
                var flows = new List<FlowModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(',');
                    var colIndex = 0;

                    if (!cols.Any())
                        continue;

                    var flowModel = new FlowModel();
                    flowModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        flowModel.TestItemName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        flowModel.SheetName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        flowModel.Enable = cols[colIndex];

                    flows.Add(flowModel);
                }

                var index = 0;
                foreach (var flow in flows)
                {
                    flow.Id = Guid.NewGuid().ToString();
                    var testItem = result?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(flow.TestItemName));
                    flow.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                    flow.IsSelected = string.IsNullOrEmpty(flow.Enable);
                    flow.SortId = ++index;
                }

                result.Flow = flows;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GetLevelData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;

            try
            {
                var levels = new List<LevelModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(',');
                    var colIndex = 0;

                    if (!cols.Any())
                        continue;

                    var levelModel = new LevelModel();
                    levelModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        levelModel.PinGroupName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal vil))
                        levelModel.Vil = vil;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal vih))
                        levelModel.Vih = vih;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal vol))
                        levelModel.Vol = vol;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal voh))
                        levelModel.Voh = voh;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal iol))
                        levelModel.Iol = iol;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal ioh))
                        levelModel.Ioh = ioh;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal vt))
                        levelModel.Vt = vt;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal vcl))
                        levelModel.Vcl = vcl;
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]) && decimal.TryParse(cols[colIndex]?.ToString(), out decimal vch))
                        levelModel.Vch = vch;

                    levels.Add(levelModel);
                }

                var testItems = result?.TestItem?.Where(x => x?.Level?.Equals(sheetName) == true).Select(x => x);
                if (testItems.Any())
                    foreach (var testItem in testItems)
                        testItem.Levels = levels;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GetTimingData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;

            try
            {
                var timings = new List<TimingModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(',');
                    var colIndex = 0;

                    if (!cols.Any())
                        continue;

                    var timingModel = new TimingModel();
                    timingModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.TimingName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.Period = System.Convert.ToInt32(cols[colIndex]?.ToString());
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.PinName = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.PinSetup = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.Fmt = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.DriveA = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.DriveB = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.DriveC = cols[colIndex];
                    colIndex++;

                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        timingModel.DriveD = cols[colIndex];

                    timings.Add(timingModel);
                }

                foreach (var timing in timings)
                    timing.Id = Guid.NewGuid().ToString();

                var testItems = result?.TestItem?.Where(x => x?.Timing?.Equals(sheetName) == true).Select(x => x);
                if (testItems.Any())
                    foreach (var testItem in testItems)
                        testItem.Timings = timings;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void GetGlobalData(TestPlanModel result, StreamReader reader, string sheetName, string[] colHeaders)
        {
            if (result == null)
                return;

            if (colHeaders == null || !colHeaders.Any())
                return;

            try
            {
                var globals = new List<GlobalModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(',');
                    var colIndex = 0;

                    if (!cols.Any())
                        continue;

                    var tempGlobal = new GlobalModel();
                    tempGlobal.Id = Guid.NewGuid().ToString();
                    if (cols.Length > colIndex && !string.IsNullOrEmpty(cols[colIndex]))
                        tempGlobal.PatternFile = cols[colIndex];
                    colIndex++;

                    globals.Add(tempGlobal);
                    GetGlobalParams(colIndex, colHeaders, cols, tempGlobal);
                }
                result.Global = globals;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void GetGlobalParams(int startColIndex, string[] colHeaders, string[] cols, GlobalModel? tempGlobal)
        {
            if (tempGlobal == null)
                return;

            var globalList = tempGlobal.Args;
            if (globalList == null)
                globalList = new List<GlobalParamModel>();

            for (int i = startColIndex; i < colHeaders.Length; i++)
            {
                var paramName = string.Empty;
                var paramValue = string.Empty;

                if (colHeaders.Length > startColIndex + i && !string.IsNullOrEmpty(colHeaders[startColIndex + i]))
                    paramName = colHeaders[startColIndex + i];

                if (cols.Length > startColIndex + i && !string.IsNullOrEmpty(cols[startColIndex + i]))
                    paramValue = cols[startColIndex + i];

                if (!string.IsNullOrEmpty(paramName) && !string.IsNullOrEmpty(paramValue))
                    globalList.Add(new GlobalParamModel() { ParamName = paramName, ParamValue = paramValue });
            }
            tempGlobal.Args = globalList;
        }

        public static TestPlanModel GetLoadedTestPlan()
        {
            return _instance.Value._testPlan;
        }
    }
}
