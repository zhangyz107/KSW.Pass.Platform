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
using KSW.ATE01.Application.Managers.Abstractions.TestPlans;
using KSW.ATE01.Application.Models.Projects;
using KSW.ATE01.Application.Models.TestPlans;
using KSW.ATE01.Data;
using KSW.ATE01.Data.Repositories.TestPlans;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.Exceptions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Configuration;
using System.Text;

namespace KSW.ATE01.Application.Managers.Implements.TestPlans
{
    /// <summary>
    /// 测试计划业务逻辑层
    /// </summary>
    public class TestPlanManager : ServiceBase, ITestPlanManager
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

        private readonly IPinOverviewRepository _pinOverviewRepository;
        private readonly ISiteInfoRepository _siteInfoRepository;
        private readonly IPinInfoRepository _pinInfoRepository;
        private readonly IGroupInfoRepository _groupInfoRepository;
        private readonly IPinGroupRelationshipRepositoy _pinGroupRelationshipRepositoy;
        private readonly IPinSiteInfoRepository _pinSiteInfoRepository;
        private readonly ITestItemInfoRepository _testItemInfoRepository;
        private readonly ILimitsRepository _limitsRepository;
        private readonly ILevelGroupRepository _levelGroupRepository;
        private readonly ITimingGroupRepository _timingGroupRepository;
        private readonly ILevelRepository _levelRepository;
        private readonly ITimingRepository _timingRepository;
        private readonly IGlobalParameterRepository _globalParameterRepository;
        #endregion

        public TestPlanManager(
            IContainerProvider containerProvider,
            IPinOverviewRepository pinOverviewRepository,
            ISiteInfoRepository siteInfoRepository,
            IPinInfoRepository pinInfoRepository,
            IGroupInfoRepository groupInfoRepository,
            IPinGroupRelationshipRepositoy pinGroupRelationshipRepositoy,
            IPinSiteInfoRepository pinSiteInfoRepository,
            ITestItemInfoRepository testItemInfoRepository,
            ILimitsRepository limitsRepository,
            ILevelGroupRepository levelGroupRepository,
            ITimingGroupRepository timingGroupRepository,
            ILevelRepository levelRepository,
            ITimingRepository timingRepository,
            IGlobalParameterRepository globalParameterRepository) : base(containerProvider)
        {
            _excelExtension = ConfigurationManager.AppSettings["ExcelExtension"];

            _pinOverviewRepository = pinOverviewRepository;
            _siteInfoRepository = siteInfoRepository;
            _pinInfoRepository = pinInfoRepository;
            _groupInfoRepository = groupInfoRepository;
            _pinGroupRelationshipRepositoy = pinGroupRelationshipRepositoy;
            _pinSiteInfoRepository = pinSiteInfoRepository;
            _testItemInfoRepository = testItemInfoRepository;
            _limitsRepository = limitsRepository;
            _levelGroupRepository = levelGroupRepository;
            _timingGroupRepository = timingGroupRepository;
            _levelRepository = levelRepository;
            _timingRepository = timingRepository;
            _globalParameterRepository = globalParameterRepository;
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
                var levelDic = new Dictionary<TestItemModel, List<Project.Base.Models.TestPlans.LevelModel>>();
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

        private void SetLevelSheet(IWorkbook workbook, string levelSheetName, List<Project.Base.Models.TestPlans.LevelModel> levels)
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
                var timingDic = new Dictionary<TestItemModel, List<Project.Base.Models.TestPlans.TimingModel>>();
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

        private void SetTimingSheet(IWorkbook workbook, string timingSheetName, List<Project.Base.Models.TestPlans.TimingModel> timings)
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
                        nameof(Project.Base.Models.TestPlans.LimitsModel.TestItemName),
                        nameof(Project.Base.Models.TestPlans.LimitsModel.TestNumber),
                        nameof(Project.Base.Models.TestPlans.LimitsModel.LowLimit),
                        nameof(Project.Base.Models.TestPlans.LimitsModel.HighLimit),
                        nameof(Project.Base.Models.TestPlans.LimitsModel.Units),
                        nameof(Project.Base.Models.TestPlans.LimitsModel.LimitName),
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
                var levelDic = new Dictionary<TestItemModel, List<Project.Base.Models.TestPlans.LevelModel>>();
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

        private void SetLevelCsv(string testPlanDirPath, string sheetName, List<Project.Base.Models.TestPlans.LevelModel> value)
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
                        nameof(Project.Base.Models.TestPlans.LevelModel.Vil),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Vih),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Vol),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Voh),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Iol),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Ioh),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Vt),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Vcl),
                        nameof(Project.Base.Models.TestPlans.LevelModel.Vch)
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
                var timingDic = new Dictionary<TestItemModel, List<Project.Base.Models.TestPlans.TimingModel>>();
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

        private void SetTimingCsv(string testPlanDirPath, string sheetName, List<Project.Base.Models.TestPlans.TimingModel> value)
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
                        nameof(Project.Base.Models.TestPlans.TimingModel.TimingName),
                        nameof(Project.Base.Models.TestPlans.TimingModel.Period),
                        nameof(Project.Base.Models.TestPlans.TimingModel.PinName),
                        nameof(Project.Base.Models.TestPlans.TimingModel.PinSetup),
                        nameof(Project.Base.Models.TestPlans.TimingModel.Fmt),
                        nameof(Project.Base.Models.TestPlans.TimingModel.DriveA),
                        nameof(Project.Base.Models.TestPlans.TimingModel.DriveB),
                        nameof(Project.Base.Models.TestPlans.TimingModel.DriveC),
                        nameof(Project.Base.Models.TestPlans.TimingModel.DriveD)
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

        public async Task CopyTestPlanByProjectIdAsync(string projectId, string newProjectId)
        {
            var pinInfoMapping = new Dictionary<Guid, Guid>();
            var groupInfoMapping = new Dictionary<Guid, Guid>();
            var limitsMapping = new Dictionary<Guid, Guid>();
            var levelGroupMapping = new Dictionary<Guid, Guid>();
            var timingGroupMapping = new Dictionary<Guid, Guid>();

            #region 引脚总览
            var pinOverviews = await _pinOverviewRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            foreach (var pinOverview in pinOverviews)
            {
                var pinOverviewEntity = await CopyPinOverviewAsync(pinOverview, newProjectId);

                // 引脚信息
                var pinInfos = await _pinInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id));
                foreach (var pinInfo in pinInfos)
                    await CopyPinInfoAsync(pinInfoMapping, pinOverviewEntity, pinInfo);

                // 组信息
                var groupInfos = await _groupInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id));
                foreach (var groupInfo in groupInfos)
                {
                    var groupInfoEntity = await CopyGroupInfoAsync(groupInfoMapping, pinOverviewEntity, groupInfo);

                    // 组与引脚关系
                    var relationships = await _pinGroupRelationshipRepositoy.FindAllAsync(x => x.GroupInfoId.Equals(groupInfo.Id));
                    foreach (var relationship in relationships)
                        await CopyPinGroupRelationshipAsync(pinInfoMapping, groupInfoEntity, relationship);
                }

                // 站点信息
                var siteInfos = await _siteInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id));
                foreach (var siteInfo in siteInfos)
                {
                    var siteInfoEntity = await CopySiteInfoAsync(pinOverviewEntity, siteInfo);

                    // 引脚站点信息
                    var pinSiteInfos = await _pinSiteInfoRepository.FindAllAsync(x => x.SiteInfoId.Equals(siteInfo.Id));
                    foreach (var pinSiteInfo in pinSiteInfos)
                        await CopyPinSiteInfoAsync(pinInfoMapping, siteInfoEntity, pinSiteInfo);
                }

            }
            #endregion

            #region 门限
            var limits = await _limitsRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            foreach (var limit in limits)
                await CopyLimitAsync(limitsMapping, limit, newProjectId);
            #endregion

            #region 电平组
            var levelGroups = await _levelGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            foreach (var levelGroup in levelGroups)
            {
                var levelGroupEntity = await CopyLevelGroupAsync(levelGroupMapping, levelGroup, newProjectId);

                // 电平
                var levels = await _levelRepository.FindAllAsync(x => x.LevelGroupId.Equals(levelGroup.Id));
                foreach (var level in levels)
                    await CopyLevelAsync(pinInfoMapping, groupInfoMapping, levelGroupEntity, level);
            }
            #endregion

            #region 时钟组
            var timingGroups = await _timingGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            foreach (var timingGroup in timingGroups)
            {
                var timingGroupEntity = await CopyTimingGroupAsync(timingGroupMapping, timingGroup, newProjectId);

                // 时钟
                var timings = await _timingRepository.FindAllAsync(x => x.TimingGroupId.Equals(timingGroup.Id));
                foreach (var timing in timings)
                    await CopyTimingAsync(pinInfoMapping, groupInfoMapping, timingGroupEntity, timing);
            }
            #endregion

            #region 测试项
            var testItemInfos = await _testItemInfoRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            foreach (var testItemInfo in testItemInfos)
                await CopyTestItemInfoAsync(pinInfoMapping, groupInfoMapping, limitsMapping, levelGroupMapping, timingGroupMapping, testItemInfo, newProjectId);
            #endregion

            #region 全局参数
            var globalParameters = await _globalParameterRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            foreach (var globalParameter in globalParameters)
                await CopyGlobalParameterAsync(globalParameter, newProjectId);
            #endregion
        }

        private async Task<PinOverview> CopyPinOverviewAsync(PinOverview old, string newProjectId)
        {
            var newPinOverview = old.Clone();
            var pinOverviewModel = newPinOverview.MapTo<PinOverviewModel>();
            pinOverviewModel.Id = Guid.NewGuid().ToString();
            pinOverviewModel.ProjectInfoId = newProjectId.ToGuid();
            var pinOverviewEntity = pinOverviewModel.MapTo<PinOverview>();
            await _pinOverviewRepository.AddAsync(pinOverviewEntity);
            return pinOverviewEntity;
        }

        private async Task CopyPinInfoAsync(Dictionary<Guid, Guid> pinInfoMapping, PinOverview newPinOverview, PinInfo old)
        {
            var newPinInfo = old.Clone();
            var pinInfoModel = newPinInfo.MapTo<PinInfoModel>();
            pinInfoModel.Id = Guid.NewGuid().ToString();
            pinInfoModel.PinOverviewId = newPinOverview.Id;
            var pinInfoEntity = pinInfoModel.MapTo<PinInfo>();
            await _pinInfoRepository.AddAsync(pinInfoEntity);

            if (!pinInfoMapping.ContainsKey(old.Id))
                pinInfoMapping.Add(old.Id, pinInfoEntity.Id);
        }

        private async Task<GroupInfo> CopyGroupInfoAsync(Dictionary<Guid, Guid> groupInfoMapping, PinOverview newPinOverview, GroupInfo old)
        {
            var newGroupInfo = old.Clone();
            var groupInfoModel = newGroupInfo.MapTo<GroupInfoModel>();
            groupInfoModel.Id = Guid.NewGuid().ToString();
            groupInfoModel.PinOverviewId = newPinOverview.Id;
            var groupInfoEntity = groupInfoModel.MapTo<GroupInfo>();
            await _groupInfoRepository.AddAsync(groupInfoEntity);

            if (!groupInfoMapping.ContainsKey(old.Id))
                groupInfoMapping.Add(old.Id, groupInfoEntity.Id);

            return groupInfoEntity;
        }

        private async Task CopyPinGroupRelationshipAsync(Dictionary<Guid, Guid> pinInfoMapping, GroupInfo newGroupInfo, PinGroupRelationship old)
        {
            var newRelationship = old.Clone();
            var relationshipModel = newRelationship.MapTo<PinGroupRelationshipModel>();
            relationshipModel.Id = Guid.NewGuid().ToString();
            relationshipModel.GroupInfoId = newGroupInfo.Id;
            var pinId = relationshipModel.PinInfoId ?? Guid.Empty;
            if (pinInfoMapping.ContainsKey(pinId))
                relationshipModel.PinInfoId = pinInfoMapping[pinId];
            var relationshipEntity = relationshipModel.MapTo<PinGroupRelationship>();
            await _pinGroupRelationshipRepositoy.AddAsync(relationshipEntity);
        }

        private async Task<SiteInfo> CopySiteInfoAsync(PinOverview newPinOverview, SiteInfo old)
        {
            var newSiteInfo = old.Clone();
            var siteInfoModel = newSiteInfo.MapTo<SiteInfoModel>();
            siteInfoModel.Id = Guid.NewGuid().ToString();
            siteInfoModel.PinOverviewId = newPinOverview.Id;
            var siteInfoEntity = siteInfoModel.MapTo<SiteInfo>();
            await _siteInfoRepository.AddAsync(siteInfoEntity);

            return siteInfoEntity;
        }

        private async Task CopyPinSiteInfoAsync(Dictionary<Guid, Guid> pinInfoMapping, SiteInfo siteInfoEntity, PinSiteInfo old)
        {
            var newPinSiteInfo = old.Clone();
            var pinSiteModel = newPinSiteInfo.MapTo<PinSiteInfoModel>();
            pinSiteModel.Id = Guid.NewGuid().ToString();
            pinSiteModel.SiteInfoId = siteInfoEntity.Id;
            var pinId = pinSiteModel.PinInfoId;
            if (pinInfoMapping.ContainsKey(pinId))
                pinSiteModel.PinInfoId = pinInfoMapping[pinId];
            var pinSiteEntity = pinSiteModel.MapTo<PinSiteInfo>();
            await _pinSiteInfoRepository.AddAsync(pinSiteEntity);
        }

        private async Task CopyLimitAsync(Dictionary<Guid, Guid> limitsMapping, Limits old, string newProjectId)
        {
            var newLimit = old.Clone();
            var limitModel = newLimit.MapTo<Models.TestPlans.LimitsModel>();
            limitModel.Id = Guid.NewGuid().ToString();
            limitModel.ProjectInfoId = newProjectId.ToGuid();
            var limitEntity = limitModel.MapTo<Limits>();
            await _limitsRepository.AddAsync(limitEntity);

            if (!limitsMapping.ContainsKey(old.Id))
                limitsMapping.Add(old.Id, limitEntity.Id);
        }

        private async Task<LevelGroup> CopyLevelGroupAsync(Dictionary<Guid, Guid> levelGroupMapping, LevelGroup old, string newProjectId)
        {
            var newLevelGroup = old.Clone();
            var levelGroupModel = newLevelGroup.MapTo<LevelGroupModel>();
            levelGroupModel.Id = Guid.NewGuid().ToString();
            levelGroupModel.ProjectInfoId = newProjectId.ToGuid();
            var levelGroupEntity = levelGroupModel.MapTo<LevelGroup>();
            await _levelGroupRepository.AddAsync(levelGroupEntity);

            if (!levelGroupMapping.ContainsKey(old.Id))
                levelGroupMapping.Add(old.Id, levelGroupEntity.Id);

            return levelGroupEntity;
        }

        private async Task CopyLevelAsync(Dictionary<Guid, Guid> pinInfoMapping, Dictionary<Guid, Guid> groupInfoMapping, LevelGroup newLevelGroup, Level old)
        {
            var newLevel = old.Clone();
            var levelModel = newLevel.MapTo<Models.TestPlans.LevelModel>();
            levelModel.Id = Guid.NewGuid().ToString();
            levelModel.LevelGroupId = newLevelGroup.Id;
            var pinGroupId = old.GroupOrPinId ?? Guid.Empty;
            if (groupInfoMapping.ContainsKey(pinGroupId))
            {
                levelModel.GroupOrPinId = groupInfoMapping[pinGroupId];
            }
            else if (pinInfoMapping.ContainsKey(pinGroupId))
            {
                levelModel.GroupOrPinId = pinInfoMapping[pinGroupId];
            }
            else
                levelModel.GroupOrPinId = Guid.Empty;
            var levelEntity = levelModel.MapTo<Level>();
            await _levelRepository.AddAsync(levelEntity);

        }

        private async Task<TimingGroup> CopyTimingGroupAsync(Dictionary<Guid, Guid> timingGroupMapping, TimingGroup old, string newProjectId)
        {
            var newTimingGroup = old.Clone();
            var timingGroupModel = newTimingGroup.MapTo<TimingGroupModel>();
            timingGroupModel.Id = Guid.NewGuid().ToString();
            timingGroupModel.ProjectInfoId = newProjectId.ToGuid();
            var timingGroupEntity = timingGroupModel.MapTo<TimingGroup>();
            await _timingGroupRepository.AddAsync(timingGroupEntity);

            if (!timingGroupMapping.ContainsKey(old.Id))
                timingGroupMapping.Add(old.Id, timingGroupEntity.Id);

            return timingGroupEntity;
        }

        private async Task CopyTimingAsync(Dictionary<Guid, Guid> pinInfoMapping, Dictionary<Guid, Guid> groupInfoMapping, TimingGroup newTimingGroup, Timing old)
        {
            var newTiming = old.Clone();
            var timingModel = newTiming.MapTo<Models.TestPlans.TimingModel>();
            timingModel.Id = Guid.NewGuid().ToString();
            timingModel.TimingGroupId = newTimingGroup.Id;
            var pinGroupId = old.GroupOrPinId ?? Guid.Empty;
            if (groupInfoMapping.ContainsKey(pinGroupId))
            {
                timingModel.GroupOrPinId = groupInfoMapping[pinGroupId];
            }
            else if (pinInfoMapping.ContainsKey(pinGroupId))
            {
                timingModel.GroupOrPinId = pinInfoMapping[pinGroupId];
            }
            else
                timingModel.GroupOrPinId = Guid.Empty;
            var timingEntity = timingModel.MapTo<Timing>();
            await _timingRepository.AddAsync(timingEntity);
        }

        private async Task CopyTestItemInfoAsync(Dictionary<Guid, Guid> pinInfoMapping, Dictionary<Guid, Guid> groupInfoMapping, Dictionary<Guid, Guid> limitsMapping, Dictionary<Guid, Guid> levelGroupMapping, Dictionary<Guid, Guid> timingGroupMapping, TestItemInfo old, string newProjectId)
        {
            var newTestItemInfo = old.Clone();
            var testItemInfoModel = newTestItemInfo.MapTo<TestItemInfoModel>();
            testItemInfoModel.Id = Guid.NewGuid().ToString();
            testItemInfoModel.ProjectInfoId = newProjectId.ToGuidOrNull();
            var pinGroupId = old.GroupOrPinId ?? Guid.Empty;
            if (groupInfoMapping.ContainsKey(pinGroupId))
            {
                testItemInfoModel.GroupOrPinId = groupInfoMapping[pinGroupId];
            }
            else if (pinInfoMapping.ContainsKey(pinGroupId))
            {
                testItemInfoModel.GroupOrPinId = pinInfoMapping[pinGroupId];
            }
            else
                testItemInfoModel.GroupOrPinId = Guid.Empty;

            var limitId = testItemInfoModel.LimitsId ?? Guid.Empty;
            if (limitsMapping.ContainsKey(limitId))
            {
                testItemInfoModel.LimitsId = limitsMapping[limitId];
            }
            else
                testItemInfoModel.LimitsId = Guid.Empty;

            var levelGroupId = testItemInfoModel.LevelGroupId ?? Guid.Empty;
            if (levelGroupMapping.ContainsKey(levelGroupId))
            {
                testItemInfoModel.LevelGroupId = levelGroupMapping[levelGroupId];
            }
            else
                testItemInfoModel.LevelGroupId = Guid.Empty;

            var timingGroupId = testItemInfoModel.TimingGroupId ?? Guid.Empty;
            if (timingGroupMapping.ContainsKey(timingGroupId))
            {
                testItemInfoModel.TimingGroupId = timingGroupMapping[timingGroupId];
            }
            else
                testItemInfoModel.TimingGroupId = Guid.Empty;
            var testItemInfoEntity = testItemInfoModel.MapTo<TestItemInfo>();
            await _testItemInfoRepository.AddAsync(testItemInfoEntity);
        }

        private async Task CopyGlobalParameterAsync(GlobalParameter old, string newProjectId)
        {
            var newGlobalParameter = old.Clone();
            var globalParameterModel = newGlobalParameter.MapTo<GlobalParameterModel>();
            globalParameterModel.Id = Guid.NewGuid().SafeString();
            globalParameterModel.ProjectInfoId = newProjectId.ToGuidOrNull();
            var globalParameterEntity = globalParameterModel.MapTo<GlobalParameter>();
            await _globalParameterRepository.AddAsync(globalParameterEntity);
        }

        public async Task DeleteTestPlanByProjectIdAsync(string projectId)
        {
            #region 引脚总览
            var pinOverviews = await _pinOverviewRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            var pinOverviewIds = pinOverviews?.Select(x => x.Id);
            await _pinOverviewRepository.RemoveAsync(pinOverviews);
            #endregion

            #region 组信息
            var groupInfos = await _groupInfoRepository.FindAllAsync(x => pinOverviewIds.Contains(x.PinOverviewId));
            var groupInfoIds = groupInfos?.Select(x => x.Id);
            await _groupInfoRepository.RemoveAsync(groupInfos);
            #endregion

            #region 引脚与组关系
            var pinGroupRelationships = await _pinGroupRelationshipRepositoy.FindAllAsync(x => groupInfoIds.Contains(x.GroupInfoId));
            await _pinGroupRelationshipRepositoy.RemoveAsync(pinGroupRelationships);
            #endregion

            #region 引脚信息
            var pinInfos = await _pinInfoRepository.FindAllAsync(x => pinOverviewIds.Contains(x.PinOverviewId));
            var pinInfoIds = pinInfos?.Select(x => x.Id);
            await _pinInfoRepository.RemoveAsync(pinInfos);
            #endregion

            #region 站点信息
            var siteInfos = await _siteInfoRepository.FindAllAsync(x => pinOverviewIds.Contains(x.PinOverviewId));
            var siteInfoIds = siteInfos?.Select(x => x.Id);
            await _siteInfoRepository.RemoveAsync(siteInfos);
            #endregion

            #region 引脚站点信息
            var pinSiteInfos = await _pinSiteInfoRepository.FindAllAsync(x => siteInfoIds.Contains(x.SiteInfoId));
            await _pinSiteInfoRepository.RemoveAsync(pinSiteInfos);
            #endregion

            #region 测试项信息
            var testItems = await _testItemInfoRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            await _testItemInfoRepository.RemoveAsync(testItems);
            #endregion

            #region 测试项门限
            var limits = await _limitsRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            await _limitsRepository.RemoveAsync(limits);
            #endregion

            #region 测试项电平组
            var levelGroups = await _levelGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            var levelGroupIds = levelGroups?.Select(x => x.Id);
            await _levelGroupRepository.RemoveAsync(levelGroups);
            #endregion

            #region 测试项电平
            var levels = await _levelRepository.FindAllAsync(x => levelGroupIds.Contains(x.LevelGroupId));
            await _levelRepository.RemoveAsync(levels);
            #endregion

            #region 测试项时钟组
            var timingGroups = await _timingGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            var timingGroupIds = timingGroups?.Select(x => x.Id);
            await _timingGroupRepository.RemoveAsync(timingGroups);
            #endregion

            #region 测试项时钟
            var timings = await _timingRepository.FindAllAsync(x => timingGroupIds.Contains(x.TimingGroupId));
            await _timingRepository.RemoveAsync(timings);
            #endregion

            #region 全局参数
            var globalParameters = await _globalParameterRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            await _globalParameterRepository.RemoveAsync(globalParameters);
            #endregion
        }
    }
}
