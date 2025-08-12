/*--------------------------------------------------------------
// Copyright (C) KSW-Tech
// 版权所有。
//
// 文件名称：TestPlanBLL.cs
// 功能描述：测试计划业务逻辑层接口
//
// 作者：zhangyingzhong
// 日期：2024/10/22 15:42
// 修改记录(Revision History)
//
//------------------------------------------------------------*/


using KSW.Application;
using KSW.ATE01.Application.BLLs.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.Exceptions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Configuration;
using System.Text;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 测试计划业务逻辑层
    /// </summary>
    public class TestPlanBLL : ServiceBase, ITestPlanBLL
    {
        #region Fields
        private readonly string _channelDataStartCell = "A4";
        private readonly string _testItemDataStartCell = "A3";
        private readonly string _limitsDataStartCell = "A2";
        private readonly string _flowDataStartCell = "A2";
        private readonly string _levelDataStartCell = "A2";
        private readonly string _timingDataStartCell = "A2";
        private readonly string _releaseDirName = "Release";

        private readonly string _channelSheetName = "Channel";
        private readonly string _testItemSheetName = "TestItem";
        private readonly string _limitsSheetName = "Limits";
        private readonly string _flowSheetName = "Flow";
        private readonly string _levelSheetName = "Level";
        private readonly string _timingSheetName = "Timing";
        private readonly string _excelExtension;
        #endregion

        public TestPlanBLL(IContainerProvider containerProvider) : base(containerProvider)
        {
            _excelExtension = ConfigurationManager.AppSettings["ExcelExtension"];
        }

        #region 加载测试项
        public async Task<TestPlanModel> LoadTestPlanAsync(ProjectInfoModel projectInfo)
        {
            var result = new TestPlanModel();

            try
            {
                var path = GetTestPlanFilePathFromProject(projectInfo);
                //switch (projectInfo.TestPlanType)
                //{
                //    case TestPlanType.Excel:
                //        result = await LoadTestPlanFromExcelAsync(path);
                //        break;
                //    case TestPlanType.Csv:
                //        result = await LoadTestPlanFromCsvAsync(path);
                //        break;
                //}
                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task<TestPlanModel> LoadTestPlanFromExcelAsync(string filePath)
        {
            var result = new TestPlanModel();
            try
            {
                if (!File.Exists(filePath))
                    throw new Warning(L["FileDoesNotExist"]);

                result = TestPlanHelper.LoadTestPlanFromExcel(filePath);

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<TestPlanModel> LoadTestPlanFromCsvAsync(string testPlanDir)
        {
            var result = new TestPlanModel();

            try
            {
                if (!Directory.Exists(testPlanDir))
                    throw new Warning(L["FileDoesNotExist"]);

                var csvFiles = Directory.GetFiles(testPlanDir, "*.csv");
                if (csvFiles?.IsEmpty() == true)
                    throw new Warning(L["FileDoesNotExist"]);

                result = TestPlanHelper.LoadTestPlanFromCsv(testPlanDir);

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 从项目信息中获取测试计划路径

        public string GetTestPlanFilePathFromProject(ProjectInfoModel projectInfo)
        {
            var result = string.Empty;
            try
            {
                var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");
                //switch (projectInfo.TestPlanType)
                //{
                //    case TestPlanType.Excel:
                //        result = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + projectInfo.TestPlanExtension);
                //        break;
                //    case TestPlanType.Csv:
                //        result = Path.Combine(projectInfo.ProjectPath, testPlanDirName);
                //        break;
                //}
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 设置测试计划Flow
        public bool SetTestPlanFlow(IList<FlowInfoModel> flows, ProjectInfoModel projectInfo)
        {
            var result = false;

            if (flows.IsEmpty())
                return result;

            try
            {
                var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");

                //var filePath = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + projectInfo.TestPlanExtension);
                //if (projectInfo.TestPlanType == TestPlanType.Csv)
                //    filePath = Path.Combine(projectInfo.ProjectPath, testPlanDirName, _flowSheetName + projectInfo.TestPlanExtension);

                //if (!File.Exists(filePath))
                //    throw new Warning(L["FileDoesNotExist"]);

                //switch (projectInfo.TestPlanType)
                //{
                //    case TestPlanType.Excel:
                //        SetTestPlanFlowToExcel(flows, filePath);
                //        break;
                //    case TestPlanType.Csv:
                //        SetTestPlanFlowToCsv(flows, filePath);
                //        break;
                //    default:
                //        break;
                //}
                result = true;
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private void SetTestPlanFlowToExcel(IList<FlowInfoModel> flows, string filePath)
        {
            IWorkbook workbook = null;
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(stream);
                    var sheet = workbook?.GetSheet("Flow");
                    var startRowIndex = 2;
                    var enableColIndex = 2;
                    foreach (var flow in flows)
                        sheet?.GetRow(startRowIndex++)?.CreateCell(enableColIndex)?.SetCellValue(flow.Enable);
                }

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook?.Write(stream);
                    workbook?.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetTestPlanFlowToCsv(IList<FlowInfoModel> flows, string filePath)
        {
            if (flows.IsEmpty())
                return;

            try
            {
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    #region 数据类型头
                    writer.WriteLine(string.Join(",", _flowSheetName));
                    #endregion

                    #region 通道头
                    var header = new List<string>
                    {
                        nameof(FlowModel.TestItemName),
                        nameof(FlowModel.SheetName),
                        nameof(FlowModel.Enable)
                    };

                    writer.WriteLine(string.Join(",", header.ToArray()));
                    #endregion

                    foreach (var flow in flows)
                    {
                        var flowData = new List<string>();
                        var testItemName = flow.TestItemName.IsEmpty() ? "" : flow.TestItemName;
                        flowData.Add(testItemName);
                        var flowSheetName = flow.SheetName == null ? "" : flow.SheetName;
                        flowData.Add(flowSheetName);
                        var enable = flow.Enable == null ? "" : flow.Enable;
                        flowData.Add(enable);

                        writer.WriteLine(string.Join(",", flowData.ToArray()));
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 另存为测试计划
        public bool SaveAsTestPlan(TestPlanModel testPlan, TestPlanType testPlanType, string saveAsDir, string fileName)
        {
            var result = false;
            var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");
            var testPlanExtension = testPlanType == TestPlanType.Excel ? _excelExtension : ".csv";

            try
            {
                switch (testPlanType)
                {
                    case TestPlanType.Excel:
                        var releaseDirPath = Path.Combine(saveAsDir, _releaseDirName);
                        result = SaveExcelTestPlan(testPlan, releaseDirPath, fileName, testPlanExtension);
                        break;
                    case TestPlanType.Csv:
                        var testPlanDirPath = Path.Combine(saveAsDir, testPlanDirName);
                        result = SaveCsvTestPlan(testPlan, testPlanDirPath, testPlanExtension);
                        break;
                    default:
                        break;
                }
                result = true;
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        private bool SaveExcelTestPlan(TestPlanModel testPlan, string releaseDirPath, string fileName, string extension)
        {
            var result = false;
            if (testPlan == null)
                return result;

            var filePath = Path.Combine(releaseDirPath, fileName + extension);
            if (!File.Exists(filePath))
                return result;

            IWorkbook workbook = null;
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(stream); //创建一个新的工作簿

                    SetChannelSheet(workbook, _channelSheetName, testPlan);

                    SetTestItemSheet(workbook, _testItemSheetName, testPlan);

                    SetLimitsSheet(workbook, _limitsSheetName, testPlan);

                    SetFlowSheet(workbook, _flowSheetName, testPlan);

                    SetLevelSheets(workbook, testPlan);

                    SetTimingSheets(workbook, testPlan);
                }

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook?.Write(stream);
                    workbook?.Close();
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void SetChannelSheet(IWorkbook workbook, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.Channel.IsEmpty())
                return;

            try
            {
                var sheet = workbook?.GetSheet(sheetName);
                var rowNum = 3;
                var groupNames = new Dictionary<string, List<ChannelModel>>();
                foreach (var channel in testPlan.Channel)
                {
                    var col = 0;
                    var row = sheet?.CreateRow(rowNum++);
                    if (!channel.Groups.IsEmpty())
                    {
                        foreach (var group in channel.Groups)
                        {
                            if (groupNames.ContainsKey(group.Name))
                                groupNames[group.Name].Add(channel);
                            else
                            {
                                groupNames[group.Name] = new List<ChannelModel>();
                                groupNames[group.Name].Add(channel);
                            }
                        }
                    }
                    col++;

                    if (!channel.PinName.IsEmpty())
                        row.CreateCell(col).SetCellValue(channel.PinName);
                    col++;

                    row.CreateCell(col).SetCellValue(channel.Type.Description());
                    col++;

                    if (!channel.Sites.IsEmpty())
                        foreach (var site in channel.Sites)
                            row.CreateCell(col++).SetCellValue(site.SiteName);
                }

                var orderGroups = groupNames.OrderBy(x => x.Value.Count).ToDictionary();
                foreach (var group in orderGroups)
                {
                    var groupIndex = 0;
                    foreach (var channel in group.Value)
                    {
                        var col = 0;
                        var row = sheet?.CreateRow(rowNum++);
                        if (groupIndex == 0)
                            row.CreateCell(col).SetCellValue(group.Key);
                        col++;

                        if (!channel.PinName.IsEmpty())
                            row.CreateCell(col).SetCellValue(channel.PinName);
                        col++;

                        if (groupIndex == 0)
                            row.CreateCell(col).SetCellValue(channel.Type.Description());
                        col++;

                        groupIndex++;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void SetTestItemSheet(IWorkbook workbook, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.TestItem.IsEmpty())
                return;

            try
            {
                var sheet = workbook?.GetSheet(sheetName);
                var rowNum = 2;

                foreach (var testItem in testPlan.TestItem)
                {
                    var col = 0;
                    var row = sheet?.CreateRow(rowNum++);
                    if (!testItem.TestItemName.IsEmpty())
                        row.CreateCell(col).SetCellValue(testItem.TestItemName);
                    col++;

                    if (!testItem.FunctionName.IsEmpty())
                        row.CreateCell(col).SetCellValue(testItem.FunctionName);
                    col++;

                    if (testItem.Force != null)
                        row.CreateCell(col).SetCellValue(System.Convert.ToDouble(testItem.Force));
                    col++;

                    if (testItem.Pins != null)
                        row.CreateCell(col).SetCellValue(testItem.Pins);
                    col++;

                    if (testItem.Level != null)
                        row.CreateCell(col).SetCellValue(testItem.Level);
                    col++;

                    if (testItem.Timing != null)
                        row.CreateCell(col).SetCellValue(testItem.Timing);
                    col++;

                    if (!testItem.Args.IsEmpty())
                        foreach (var arg in testItem.Args)
                            row.CreateCell(col++).SetCellValue(arg.ParamValue);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetLimitsSheet(IWorkbook workbook, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.Limits.IsEmpty())
                return;
            try
            {
                var sheet = workbook?.GetSheet(sheetName);
                var rowNum = 2;

                foreach (var limit in testPlan.Limits)
                {
                    var col = 0;
                    var row = sheet?.CreateRow(rowNum++);
                    if (!limit.TestItemName.IsEmpty())
                        row.CreateCell(col).SetCellValue(limit.TestItemName);
                    col++;

                    if (limit.TestNumber != null)
                        row.CreateCell(col).SetCellValue(System.Convert.ToDouble(limit.TestNumber.ToString()));
                    col++;

                    if (limit.LowLimit != null)
                        row.CreateCell(col).SetCellValue(System.Convert.ToDouble(limit.LowLimit));
                    col++;

                    if (limit.HighLimit != null)
                        row.CreateCell(col).SetCellValue(System.Convert.ToDouble(limit.HighLimit));
                    col++;

                    if (limit.Units != null)
                        row.CreateCell(col).SetCellValue(limit.Units);
                    col++;

                    if (limit.LimitName != null)
                        row.CreateCell(col).SetCellValue(limit.LimitName);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetFlowSheet(IWorkbook workbook, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.Flow.IsEmpty())
                return;

            try
            {
                var sheet = workbook?.GetSheet(sheetName);
                var rowNum = 2;

                foreach (var flow in testPlan.Flow)
                {
                    var col = 0;
                    var row = sheet?.CreateRow(rowNum++);
                    if (!flow.TestItemName.IsEmpty())
                        row.CreateCell(col).SetCellValue(flow.TestItemName);
                    col++;

                    if (flow.SheetName != null)
                        row.CreateCell(col).SetCellValue(flow.SheetName);
                    col++;

                    if (flow.Enable != null)
                        row.CreateCell(col).SetCellValue(flow.Enable);
                    col++;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetLevelSheets(IWorkbook workbook, TestPlanModel testPlan)
        {
            if (testPlan.TestItem.IsEmpty())
                return;
            try
            {
                var levelDic = new Dictionary<TestItemModel, List<LevelModel>>();
                var testItems = testPlan.TestItem.Where(x => !x.Levels.IsEmpty()).Select(x => x);
                foreach (var testItem in testItems)
                    levelDic.Add(testItem, testItem.Levels);

                var distinceLevels = levelDic.DistinctBy(x => x.Key.Level).ToDictionary();
                foreach (var distinceLevel in distinceLevels)
                    SetLevelSheet(workbook, distinceLevel.Key.Level, distinceLevel.Value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetLevelSheet(IWorkbook workbook, string levelSheetName, List<LevelModel> levels)
        {

            levelSheetName = _levelSheetName;

            if (levels.IsEmpty())
                return;

            if (workbook == null)
                return;

            try
            {
                var sheet = workbook?.GetSheet(_levelSheetName);
                var levelSheetIndex = workbook.GetSheetIndex(sheet);
                var existsSheet = workbook?.GetSheet(levelSheetName);
                if (existsSheet == null && !levelSheetName.IsEmpty())
                {
                    sheet = workbook?.CloneSheet(levelSheetIndex);
                    var currentSheetIndex = workbook.GetSheetIndex(sheet);
                    workbook?.SetSheetName(currentSheetIndex, levelSheetName);
                }
                var rowNum = 2;
                foreach (var level in levels)
                {
                    var col = 0;
                    var row = sheet?.CreateRow(rowNum++);
                    if (!level.PinGroupName.IsEmpty())
                        row.CreateCell(col++).SetCellValue(level.PinGroupName);

                    if (level.Vil != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Vil));

                    if (level.Vih != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Vih));

                    if (level.Vol != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Vol));

                    if (level.Voh != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Voh));

                    if (level.Iol != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Iol));

                    if (level.Ioh != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Ioh));

                    if (level.Vt != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Vt));

                    if (level.Vcl != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Vcl));

                    if (level.Vch != null)
                        row.CreateCell(col++).SetCellValue(System.Convert.ToDouble(level.Vch));

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetTimingSheets(IWorkbook workbook, TestPlanModel testPlan)
        {
            if (testPlan.TestItem.IsEmpty())
                return;
            try
            {
                var timingDic = new Dictionary<TestItemModel, List<TimingModel>>();
                var testItems = testPlan.TestItem.Where(x => !x.Timings.IsEmpty()).Select(x => x);
                foreach (var testItem in testItems)
                    timingDic.Add(testItem, testItem.Timings);

                var distinceTimings = timingDic.DistinctBy(x => x.Key.Timing).ToDictionary();
                foreach (var distinceTiming in distinceTimings)
                    SetTimingSheet(workbook, distinceTiming.Key.Timing, distinceTiming.Value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetTimingSheet(IWorkbook workbook, string timingSheetName, List<TimingModel> timings)
        {
            timingSheetName = _timingSheetName;

            if (timings.IsEmpty())
                return;

            if (workbook == null)
                return;

            try
            {
                var sheet = workbook?.GetSheet(_timingSheetName);
                var timingSheetIndex = workbook.GetSheetIndex(sheet);
                var existsSheet = workbook?.GetSheet(timingSheetName);
                if (existsSheet == null && !timingSheetName.IsEmpty())
                {
                    sheet = workbook?.CloneSheet(timingSheetIndex);
                    var currentSheetIndex = workbook.GetSheetIndex(sheet);
                    workbook?.SetSheetName(currentSheetIndex, timingSheetName);
                }
                var rowNum = 2;
                foreach (var timing in timings)
                {
                    var col = 0;
                    var row = sheet?.CreateRow(rowNum++);
                    if (!timing.TimingName.IsEmpty())
                        row.CreateCell(col++).SetCellValue(timing.TimingName);

                    row.CreateCell(col++).SetCellValue(timing.Period);

                    if (timing.PinName != null)
                        row.CreateCell(col++).SetCellValue(timing.PinName);

                    if (timing.PinSetup != null)
                        row.CreateCell(col++).SetCellValue(timing.PinSetup);

                    if (timing.Fmt != null)
                        row.CreateCell(col++).SetCellValue(timing.Fmt.Description());

                    if (timing.DriveA != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveA);

                    if (timing.DriveB != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveB);

                    if (timing.DriveC != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveC);

                    if (timing.DriveD != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveD);

                    if (timing.StrobeMode != null)
                        row.CreateCell(col++).SetCellValue(timing.StrobeMode.Description());

                    if (timing.StrobeA != null)
                        row.CreateCell(col++).SetCellValue(timing.StrobeA);

                    if (timing.StrobeB != null)
                        row.CreateCell(col++).SetCellValue(timing.StrobeB);

                    if (timing.Comment != null)
                        row.CreateCell(col++).SetCellValue(timing.Comment);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool SaveCsvTestPlan(TestPlanModel testPlan, string testPlanDirPath, string extension)
        {
            var result = false;
            if (testPlan == null)
                return result;

            if (!Directory.Exists(testPlanDirPath))
                Directory.CreateDirectory(testPlanDirPath);

            try
            {
                SetChannelCsv(testPlanDirPath, _channelSheetName, testPlan);

                SetTestItemCsv(testPlanDirPath, _testItemSheetName, testPlan);

                SetLimitsCsv(testPlanDirPath, _limitsSheetName, testPlan);

                SetFlowCsv(testPlanDirPath, _flowSheetName, testPlan);

                SetLevelCsvs(testPlanDirPath, testPlan);

                SetTimingCsvs(testPlanDirPath, testPlan);
            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        private void SetChannelCsv(string testPlanDirPath, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.Channel.IsEmpty())
                return;

            try
            {
                var filePath = Path.Combine(testPlanDirPath, sheetName + ".csv");

                using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    #region 数据类型头
                    writer.WriteLine(string.Join(",", _channelSheetName));
                    #endregion

                    #region 通道头
                    var siteCount = testPlan.Channel.Max(x => x.Sites.Count);
                    writer.WriteLine(string.Join(",", "SiteCount", siteCount));

                    var channelHeader = new List<string>
                    {
                        "GroupName",
                        nameof(ChannelModel.PinName),
                        nameof(ChannelModel.Type)
                    };
                    for (int i = 0; i < siteCount; i++)
                    {
                        channelHeader.Add($"Site{i}");
                    }
                    writer.WriteLine(string.Join(",", channelHeader.ToArray()));
                    #endregion
                    var groupNames = new Dictionary<string, List<ChannelModel>>();

                    foreach (var channel in testPlan.Channel)
                    {
                        var channelData = new List<string>();
                        if (!channel.Groups.IsEmpty())
                        {
                            foreach (var group in channel.Groups)
                            {
                                if (groupNames.ContainsKey(group.Name))
                                    groupNames[group.Name].Add(channel);
                                else
                                {
                                    groupNames[group.Name] = new List<ChannelModel>();
                                    groupNames[group.Name].Add(channel);
                                }
                            }
                        }
                        channelData.Add("");
                        var pinName = channel.PinName.IsEmpty() ? "" : channel.PinName;
                        channelData.Add(pinName);
                        var type = channel.Type == null ? "" : channel.Type.Description();
                        channelData.Add(type);

                        if (!channel.Sites.IsEmpty())
                            foreach (var site in channel.Sites)
                            {
                                var siteStr = site.SiteName.IsEmpty() ? "" : site.SiteName;
                                channelData.Add(siteStr);
                            }

                        writer.WriteLine(string.Join(",", channelData.ToArray()));
                    }

                    var orderGroups = groupNames.OrderBy(x => x.Value.Count).ToDictionary();
                    foreach (var group in orderGroups)
                    {
                        var groupIndex = 0;
                        foreach (var channel in group.Value)
                        {
                            var channelData = new List<string>();
                            if (groupIndex == 0)
                                channelData.Add(group.Key);
                            var pinName = channel.PinName.IsEmpty() ? "" : channel.PinName;
                            channelData.Add(pinName);
                            var type = channel.Type == null ? "" : channel.Type.Description();
                            channelData.Add(type);

                            groupIndex++;
                            writer.WriteLine(string.Join(",", channelData.ToArray()));
                        }
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetTestItemCsv(string testPlanDirPath, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.TestItem.IsEmpty())
                return;

            try
            {
                var filePath = Path.Combine(testPlanDirPath, sheetName + ".csv");

                using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    #region 数据类型头
                    writer.WriteLine(string.Join(",", _testItemSheetName));
                    #endregion

                    #region 通道头
                    var header = new List<string>
                    {
                        nameof(TestItemModel.TestItemName),
                        nameof(TestItemModel.FunctionName),
                        nameof(TestItemModel.Force),
                        nameof(TestItemModel.Pins),
                        nameof(TestItemModel.Level),
                        nameof(TestItemModel.Timing),
                    };

                    writer.WriteLine(string.Join(",", header.ToArray()));
                    #endregion

                    foreach (var testItem in testPlan.TestItem)
                    {
                        var testItemData = new List<string>();
                        var testItemName = testItem.TestItemName.IsEmpty() ? "" : testItem.TestItemName;
                        testItemData.Add(testItemName);
                        var functionName = testItem.FunctionName.IsEmpty() ? "" : testItem.FunctionName;
                        testItemData.Add(functionName);
                        var force = testItem.Force == null ? "" : testItem.Force.ToString();
                        testItemData.Add(force);
                        var pins = testItem.Pins.IsEmpty() ? "" : testItem.Pins;
                        testItemData.Add(pins);
                        var level = testItem.Level.IsEmpty() ? "" : testItem.Level;
                        testItemData.Add(level);
                        var timing = testItem.Timing.IsEmpty() ? "" : testItem.Timing;
                        testItemData.Add(timing);

                        writer.WriteLine(string.Join(",", testItemData.ToArray()));
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetLimitsCsv(string testPlanDirPath, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.Limits.IsEmpty())
                return;

            try
            {
                var filePath = Path.Combine(testPlanDirPath, sheetName + ".csv");

                using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    #region 数据类型头
                    writer.WriteLine(string.Join(",", _limitsSheetName));
                    #endregion

                    #region 通道头
                    var header = new List<string>
                    {
                        nameof(LimitsModel.TestItemName),
                        nameof(LimitsModel.TestNumber),
                        nameof(LimitsModel.LowLimit),
                        nameof(LimitsModel.HighLimit),
                        nameof(LimitsModel.Units),
                        nameof(LimitsModel.LimitName),
                    };

                    writer.WriteLine(string.Join(",", header.ToArray()));
                    #endregion

                    foreach (var limit in testPlan.Limits)
                    {
                        var limitData = new List<string>();
                        var testItemName = limit.TestItemName.IsEmpty() ? "" : limit.TestItemName;
                        limitData.Add(testItemName);
                        var testNumber = limit.TestNumber == null ? "" : limit.TestNumber.ToString();
                        limitData.Add(testNumber);
                        var lowLimit = limit.LowLimit == null ? "" : limit.LowLimit.ToString();
                        limitData.Add(lowLimit);
                        var highLimit = limit.HighLimit == null ? "" : limit.HighLimit.ToString();
                        limitData.Add(highLimit);
                        var units = limit.Units.IsEmpty() ? "" : limit.Units;
                        limitData.Add(units);
                        var limitName = limit.LimitName.IsEmpty() ? "" : limit.LimitName;
                        limitData.Add(limitName);

                        writer.WriteLine(string.Join(",", limitData.ToArray()));
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetFlowCsv(string testPlanDirPath, string sheetName, TestPlanModel testPlan)
        {
            if (testPlan.Flow.IsEmpty())
                return;

            try
            {
                var filePath = Path.Combine(testPlanDirPath, sheetName + ".csv");

                using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    #region 数据类型头
                    writer.WriteLine(string.Join(",", _flowSheetName));
                    #endregion

                    #region 通道头
                    var header = new List<string>
                    {
                        nameof(FlowModel.TestItemName),
                        nameof(FlowModel.SheetName),
                        nameof(FlowModel.Enable)
                    };

                    writer.WriteLine(string.Join(",", header.ToArray()));
                    #endregion

                    foreach (var flow in testPlan.Flow)
                    {
                        var flowData = new List<string>();
                        var testItemName = flow.TestItemName.IsEmpty() ? "" : flow.TestItemName;
                        flowData.Add(testItemName);
                        var flowSheetName = flow.SheetName == null ? "" : flow.SheetName;
                        flowData.Add(flowSheetName);
                        var enable = flow.Enable == null ? "" : flow.Enable;
                        flowData.Add(enable);

                        writer.WriteLine(string.Join(",", flowData.ToArray()));
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetLevelCsvs(string testPlanDirPath, TestPlanModel testPlan)
        {

            if (testPlan.TestItem.IsEmpty())
                return;
            try
            {
                var levelDic = new Dictionary<TestItemModel, List<LevelModel>>();
                var testItems = testPlan.TestItem.Where(x => !x.Levels.IsEmpty()).Select(x => x);
                foreach (var testItem in testItems)
                    levelDic.Add(testItem, testItem.Levels);

                var distinceLevels = levelDic.DistinctBy(x => x.Key.Level).ToDictionary();
                foreach (var distinceLevel in distinceLevels)
                    SetLevelCsv(testPlanDirPath, distinceLevel.Key.Level, distinceLevel.Value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetLevelCsv(string testPlanDirPath, string sheetName, List<LevelModel> value)
        {
            if (value.IsEmpty())
                return;

            try
            {
                var filePath = Path.Combine(testPlanDirPath, sheetName + ".csv");

                using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    #region 数据类型头
                    writer.WriteLine(string.Join(",", sheetName));
                    #endregion

                    #region 通道头
                    var header = new List<string>
                    {
                        "Pin/Group",
                        nameof(LevelModel.Vil),
                        nameof(LevelModel.Vih),
                        nameof(LevelModel.Vol),
                        nameof(LevelModel.Voh),
                        nameof(LevelModel.Iol),
                        nameof(LevelModel.Ioh),
                        nameof(LevelModel.Vt),
                        nameof(LevelModel.Vcl),
                        nameof(LevelModel.Vch)
                    };

                    writer.WriteLine(string.Join(",", header.ToArray()));
                    #endregion

                    foreach (var level in value)
                    {
                        var levelData = new List<string>();
                        var groupName = level.PinGroupName.IsEmpty() ? "" : level.PinGroupName;
                        levelData.Add(groupName);
                        var vil = level.Vil == null ? "" : level.Vil.ToString();
                        levelData.Add(vil);
                        var vih = level.Vih == null ? "" : level.Vih.ToString();
                        levelData.Add(vih);
                        var vol = level.Vol == null ? "" : level.Vol.ToString();
                        levelData.Add(vol);
                        var voh = level.Voh == null ? "" : level.Voh.ToString();
                        levelData.Add(voh);
                        var iol = level.Iol == null ? "" : level.Iol.ToString();
                        levelData.Add(iol);
                        var ioh = level.Ioh == null ? "" : level.Ioh.ToString();
                        levelData.Add(ioh);
                        var vt = level.Vt == null ? "" : level.Vt.ToString();
                        levelData.Add(vt);
                        var vcl = level.Vcl == null ? "" : level.Vcl.ToString();
                        levelData.Add(vcl);
                        var vch = level.Vch == null ? "" : level.Vch.ToString();
                        levelData.Add(vch);

                        writer.WriteLine(string.Join(",", levelData.ToArray()));
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetTimingCsvs(string testPlanDirPath, TestPlanModel testPlan)
        {
            if (testPlan.TestItem.IsEmpty())
                return;
            try
            {
                var timingDic = new Dictionary<TestItemModel, List<TimingModel>>();
                var testItems = testPlan.TestItem.Where(x => !x.Timings.IsEmpty()).Select(x => x);
                foreach (var testItem in testItems)
                    timingDic.Add(testItem, testItem.Timings);

                var distinceTimings = timingDic.DistinctBy(x => x.Key.Timing).ToDictionary();
                foreach (var distinceTiming in distinceTimings)
                    SetTimingCsv(testPlanDirPath, distinceTiming.Key.Timing, distinceTiming.Value);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SetTimingCsv(string testPlanDirPath, string sheetName, List<TimingModel> value)
        {
            if (value.IsEmpty())
                return;

            try
            {
                var filePath = Path.Combine(testPlanDirPath, sheetName + ".csv");

                using (var writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    #region 数据类型头
                    writer.WriteLine(string.Join(",", sheetName));
                    #endregion

                    #region 通道头
                    var header = new List<string>
                    {
                        nameof(TimingModel.TimingName),
                        nameof(TimingModel.Period),
                        nameof(TimingModel.PinName),
                        nameof(TimingModel.PinSetup),
                        nameof(TimingModel.Fmt),
                        nameof(TimingModel.DriveA),
                        nameof(TimingModel.DriveB),
                        nameof(TimingModel.DriveC),
                        nameof(TimingModel.DriveD)
                    };

                    writer.WriteLine(string.Join(",", header.ToArray()));
                    #endregion

                    foreach (var timing in value)
                    {
                        var timingData = new List<string>();
                        var timingName = timing.TimingName.IsEmpty() ? "" : timing.TimingName;
                        timingData.Add(timingName);
                        var period = timing.Period == null ? "" : timing.Period.ToString();
                        timingData.Add(period);
                        var pinName = timing.PinName.IsEmpty() ? "" : timing.PinName;
                        timingData.Add(pinName);
                        var pinSetup = timing.PinSetup.IsEmpty() ? "" : timing.PinSetup;
                        timingData.Add(pinSetup);
                        var fmt = timing.Fmt.Description();
                        timingData.Add(fmt);
                        var driveA = timing.DriveA.IsEmpty() ? "" : timing.DriveA;
                        timingData.Add(driveA);
                        var driveB = timing.DriveB.IsEmpty() ? "" : timing.DriveB;
                        timingData.Add(driveB);
                        var driveC = timing.DriveC.IsEmpty() ? "" : timing.DriveC;
                        timingData.Add(driveC);
                        var driveD = timing.DriveD.IsEmpty() ? "" : timing.DriveD;
                        timingData.Add(driveD);
                        var strobeMode = timing.StrobeMode.Description();
                        timingData.Add(strobeMode);
                        var strobeA = timing.StrobeA.ToString().IsEmpty() ? "" : timing.StrobeA.ToString();
                        timingData.Add(strobeA);
                        var strobeB = timing.StrobeB.ToString().IsEmpty() ? "" : timing.StrobeB.ToString();
                        timingData.Add(strobeB);
                        var comment = timing.Comment.IsEmpty() ? "" : timing.Comment;
                        timingData.Add(comment);

                        writer.WriteLine(string.Join(",", timingData.ToArray()));
                    }

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
