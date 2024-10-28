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
using KSW.ATE01.Application.Models.TestPlan;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.Projects.Entities;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Exceptions;
using MiniExcelLibs;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

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
        #endregion

        public TestPlanBLL(IContainerProvider containerProvider) : base(containerProvider)
        {
        }

        #region 加载测试项
        public async Task<TestPlanModel> LoadTestPlanAsync(ProjectInfoModel projectInfo)
        {
            var result = new TestPlanModel();
            var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");


            switch (projectInfo.TestPlanType)
            {
                case TestPlanType.Excel:
                    var filePath = Path.Combine(projectInfo.ReleasePath, projectInfo.ProjectName + projectInfo.TestPlanExtension);
                    result = await LoadTestPlanFromExcelAsync(filePath);
                    break;
                case TestPlanType.Csv:
                    var testPlanDir = Path.Combine(projectInfo.ProjectPath, testPlanDirName);
                    result = await LoadTestPlanFromCsvAsync(testPlanDir);
                    break;
            }
            return result;
        }

        private async Task<TestPlanModel> LoadTestPlanFromExcelAsync(string filePath)
        {
            var result = new TestPlanModel();
            try
            {
                if (!File.Exists(filePath))
                    throw new Warning("测试计划文件不存在");

                var sheetNames = MiniExcel.GetSheetNames(filePath);
                if (sheetNames.IsEmpty())
                    return result;

                foreach (var sheetName in sheetNames)
                {
                    var rows = (await MiniExcel.QueryAsync(filePath, sheetName: sheetName)).ToList();
                    var sheetTypeStr = Convert.ToString(rows[0].A);
                    if (Enum.TryParse<TestPlanSheetType>(sheetTypeStr, out TestPlanSheetType sheetType))
                    {
                        switch (sheetType)
                        {
                            case TestPlanSheetType.Channel:
                                var siteCount = Convert.ToInt32(rows[1].B);
                                GetChannelData(result, siteCount, filePath, sheetName);
                                break;
                            case TestPlanSheetType.TestItem:
                                GetTestItemData(result, filePath, sheetName);
                                break;
                            case TestPlanSheetType.Limits:
                                await GetLimitsData(result, filePath, sheetName);
                                break;
                            case TestPlanSheetType.Flow:
                                await GetFlowData(result, filePath, sheetName);
                                break;
                            case TestPlanSheetType.Level:
                                await GetLevelData(result, filePath, sheetName);
                                break;
                            case TestPlanSheetType.Timing:
                                await GetTimingData(result, filePath, sheetName);
                                break;
                            default:
                                break;
                        }
                    }
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GetChannelData(TestPlanModel result, int siteCount, string filePath, string sheetName)
        {
            var rows = MiniExcel.QueryRange(filePath, useHeaderRow: false, sheetName: sheetName, startCell: _channelDataStartCell)?.Cast<IDictionary<string, object>>();
            if (!rows.Any())
                return;

            if (result == null)
                return;

            var channels = new List<ChannelModel>();
            var channelTypeDic = GetEnumDescriptionDic<ChannelType>();
            foreach (var row in rows)
            {
                var tempChannel = new ChannelModel();
                tempChannel.Id = Guid.NewGuid().ToString();

                if (row.ContainsKey("A") && row["A"] != null)
                {
                    tempChannel.GroupName = row["A"]?.ToString();
                    tempChannel.GroupId = Guid.NewGuid();
                }

                if (row.ContainsKey("B") && row["B"] != null)
                    tempChannel.PinName = row["B"]?.ToString();

                if (row.ContainsKey("C") && row["C"] != null && channelTypeDic.Keys.Contains(row["C"]?.ToString()))
                    tempChannel.Type = channelTypeDic[row["C"]?.ToString()];

                if (siteCount > 0)
                    GetSites(siteCount, row, tempChannel);
                channels.Add(tempChannel);
            }
            result.Channel = channels;
        }

        private Dictionary<string, T> GetEnumDescriptionDic<T>() where T : Enum
        {
            var result = new Dictionary<string, T>();
            var values = Enum.GetValues(typeof(T));
            foreach (var value in values)
            {
                if (value is T enumValue)
                    result.TryAdd(enumValue.Description(), enumValue);
            }
            return result;
        }

        private void GetSites(int siteCount, IDictionary<string, object> row, ChannelModel tempChannel)
        {
            var col = 'D';
            var siteList = new List<SiteModel>();
            for (int i = 0; i < siteCount; i++)
            {
                var currentCol = ((char)(col + i)).ToString();
                if (row.ContainsKey(currentCol))
                    siteList.Add(new SiteModel() { SiteName = row[currentCol]?.ToString() });
            }
            tempChannel.Sites = siteList;
        }

        private void GetTestItemData(TestPlanModel result, string filePath, string sheetName)
        {
            var rows = MiniExcel.QueryRange(filePath, useHeaderRow: false, sheetName: sheetName, startCell: _testItemDataStartCell)?.Cast<IDictionary<string, object>>();
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
                if (row.ContainsKey("A"))
                    tempTestItem.TestItemName = row["A"]?.ToString();

                if (row.ContainsKey("B"))
                    tempTestItem.FunctionName = row["B"]?.ToString();

                if (row.ContainsKey("C"))
                    tempTestItem.Force = Convert.ToDecimal(row["C"]?.ToString());

                if (row.ContainsKey("D"))
                    tempTestItem.Pins = row["D"]?.ToString();

                if (row.ContainsKey("E"))
                    tempTestItem.Level = row["E"]?.ToString();

                if (row.ContainsKey("F"))
                    tempTestItem.Timing = row["F"]?.ToString();

                GetChannelArgs(row, tempTestItem);
                testItems.Add(tempTestItem);
            }
            result.TestItem = testItems;
        }

        private void GetChannelArgs(IDictionary<string, object> row, TestItemModel testItem)
        {
            int argsCount = 17;
            var col = 'G';
            var args = new List<string>();
            for (int i = 0; i < argsCount; i++)
            {
                var currentCol = ((char)(col + i)).ToString();
                if (row.ContainsKey(currentCol) && row[currentCol] != null)
                    args.Add(row[currentCol]?.ToString());
            }
            testItem.Args = args;
        }

        private async Task GetLimitsData(TestPlanModel result, string filePath, string sheetName)
        {
            try
            {
                var rows = await MiniExcel.QueryAsync<LimitsModel>(filePath, sheetName: sheetName, startCell: _limitsDataStartCell);
                if (!rows.Any())
                    return;

                if (result == null)
                    return;
                var limits = rows.ToList();
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

        private async Task GetFlowData(TestPlanModel result, string filePath, string sheetName)
        {
            var rows = await MiniExcel.QueryAsync<FlowModel>(filePath, sheetName: sheetName, startCell: _flowDataStartCell);
            if (!rows.Any())
                return;

            if (result == null)
                return;

            var index = 0;
            var flows = rows.ToList();
            foreach (var flow in flows)
            {
                flow.Id = Guid.NewGuid().ToString();
                var testItem = result?.TestItem?.FirstOrDefault(x => !x.TestItemName.IsEmpty() && x.TestItemName.Equals(flow.TestItemName));
                flow.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                flow.IsSelected = flow.Enable.IsEmpty();
                flow.SortId = ++index;
            }
            result.Flow = flows;
        }

        private async Task GetLevelData(TestPlanModel result, string filePath, string sheetName)
        {
            var rows = await MiniExcel.QueryAsync<LevelModel>(filePath, sheetName: sheetName, startCell: _levelDataStartCell);
            if (!rows.Any())
                return;

            if (result == null)
                return;
            var levels = rows.ToList();
            foreach (var level in levels)
            {
                level.Id = Guid.NewGuid().ToString();

                var tempChannel = result?.Channel?.FirstOrDefault(x => x?.GroupName?.Equals(level.GroupName) == true);
                level.ChannelGroupId = tempChannel?.GroupId ?? Guid.Empty;
            }

            var testItems = result?.TestItem?.Where(x => x?.Level?.Equals(sheetName) == true).Select(x => x);
            foreach (var testItem in testItems)
                testItem.Levels = levels;
        }

        private async Task GetTimingData(TestPlanModel result, string filePath, string sheetName)
        {
            var rows = await MiniExcel.QueryAsync<TimingModel>(filePath, sheetName: sheetName, startCell: _timingDataStartCell);
            if (!rows.Any())
                return;

            if (result == null)
                return;
            var timings = rows.ToList();
            foreach (var timing in timings)
                timing.Id = Guid.NewGuid().ToString();

            var testItems = result?.TestItem?.Where(x => x?.Timing?.Equals(sheetName) == true).Select(x => x);
            foreach (var testItem in testItems)
                testItem.Timings = timings;
        }

        private async Task<TestPlanModel> LoadTestPlanFromCsvAsync(string testPlanDir)
        {
            var result = new TestPlanModel();

            try
            {
                if (!Directory.Exists(testPlanDir))
                    throw new Warning("测试计划文件不存在");

                var csvFiles = Directory.GetFiles(testPlanDir, "*.csv");
                if (csvFiles?.IsEmpty() == true)
                    throw new Warning("测试计划文件不存在");

                foreach (var csvFile in csvFiles)
                {
                    var sheetName = Path.GetFileNameWithoutExtension(csvFile);
                    using (var reader = new StreamReader(csvFile))
                    {
                        if (!reader.EndOfStream)
                        {
                            var row = reader.ReadLine();
                            var cols = row.Split(",");
                            var sheetTypeStr = cols[0];
                            if (Enum.TryParse(sheetTypeStr, out TestPlanSheetType sheetType))
                            {
                                switch (sheetType)
                                {
                                    case TestPlanSheetType.Channel:
                                        if (!reader.EndOfStream)
                                        {
                                            row = reader.ReadLine();
                                            cols = row.Split(",");
                                            var siteCount = cols[1].IsEmpty() ? 0 : Convert.ToInt32(cols[1]);
                                            if (!reader.EndOfStream)
                                                reader.ReadLine();
                                            GetChannelData(result, siteCount, reader);
                                        }
                                        break;
                                    case TestPlanSheetType.TestItem:
                                        if (!reader.EndOfStream)
                                            reader.ReadLine();
                                        GetTestItemData(result, reader, sheetName);
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
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GetChannelData(TestPlanModel result, int siteCount, StreamReader reader)
        {
            var channels = new List<ChannelModel>();
            var channelTypeDic = GetEnumDescriptionDic<ChannelType>();
            if (result == null)
                return;

            try
            {
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(",");

                    if (cols.IsEmpty())
                        continue;

                    var tempChannel = new ChannelModel();
                    tempChannel.Id = Guid.NewGuid().ToString();

                    if (cols.Length > 0 && !cols[0].IsEmpty())
                    {
                        tempChannel.GroupName = cols[0];
                        tempChannel.GroupId = Guid.NewGuid();
                    }

                    if (cols.Length > 1 && !cols[1].IsEmpty())
                        tempChannel.PinName = cols[1];

                    if (cols.Length > 2 && !cols[2].IsEmpty())
                        tempChannel.Type = channelTypeDic[cols[2]];

                    if (cols.Length > 2 + siteCount && siteCount > 0)
                        GetSites(siteCount, cols, tempChannel);
                    channels.Add(tempChannel);
                }

                result.Channel = channels;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void GetSites(int siteCount, string[]? cols, ChannelModel tempChannel)
        {
            var siteList = new List<SiteModel>();
            for (int i = 0; i < siteCount; i++)
            {
                if (!cols[i].IsEmpty())
                    siteList.Add(new SiteModel() { SiteName = cols[i] });
            }
            tempChannel.Sites = siteList;
        }

        private void GetTestItemData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;

            var testItems = new List<TestItemModel>();
            int argsCount = 17;
            try
            {
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(",");

                    if (cols.IsEmpty())
                        continue;

                    var tempTestItem = new TestItemModel();
                    tempTestItem.SheetName = sheetName;
                    tempTestItem.Id = Guid.NewGuid().ToString();
                    if (cols.Length > 0 && !cols[0].IsEmpty())
                        tempTestItem.TestItemName = cols[0];

                    if (cols.Length > 1 && !cols[1].IsEmpty())
                        tempTestItem.FunctionName = cols[1];

                    if (cols.Length > 2 && !cols[2].IsEmpty())
                        tempTestItem.Force = Convert.ToDecimal(cols[2]?.ToString());

                    if (cols.Length > 3 && !cols[3].IsEmpty())
                        tempTestItem.Pins = cols[3];

                    if (cols.Length > 4 && !cols[4].IsEmpty())
                        tempTestItem.Level = cols[4];

                    if (cols.Length > 5 && cols[5].IsEmpty())
                        tempTestItem.Timing = cols[5];

                    if (cols.Length > 5 + argsCount)
                        GetChannelArgs(cols, tempTestItem);
                }
                result.TestItem = testItems;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GetChannelArgs(string[]? cols, TestItemModel testItem)
        {
            var args = new List<string>();
            int argsCount = 17;
            for (int i = 0; i < argsCount; i++)
            {
                if (!cols[i].IsEmpty())
                    args.Add(cols[i]);
            }
            testItem.Args = args;
        }

        private void GetLimitsData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;
            try
            {
                var limits = new List<LimitsModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(",");

                    if (cols.IsEmpty())
                        continue;

                    var limitModel = new LimitsModel();
                    limitModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > 0 && !cols[0].IsEmpty())
                        limitModel.TestItemName = cols[0];

                    if (cols.Length > 1 && !cols[1].IsEmpty())
                        limitModel.TestNumber = Convert.ToInt32(cols[1]?.ToString());

                    if (cols.Length > 2 && !cols[2].IsEmpty())
                        limitModel.LowLimit = Convert.ToDecimal(cols[2]?.ToString());

                    if (cols.Length > 3 && !cols[3].IsEmpty())
                        limitModel.HighLimit = Convert.ToDecimal(cols[3]?.ToString());

                    if (cols.Length > 4 && !cols[4].IsEmpty())
                        limitModel.Units = cols[4];

                    if (cols.Length > 5 && cols[5].IsEmpty())
                        limitModel.LimitName = cols[5];

                    limits.Add(limitModel);

                    foreach (var limit in limits)
                    {
                        limit.Id = Guid.NewGuid().ToString();
                        var testItem = result?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(limit.TestItemName));
                        limit.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                    }
                }
                result.Limits = limits;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GetFlowData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;

            try
            {
                var flows = new List<FlowModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(",");

                    if (cols.IsEmpty())
                        continue;

                    var flowModel = new FlowModel();
                    flowModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > 0 && !cols[0].IsEmpty())
                        flowModel.TestItemName = cols[0];

                    if (cols.Length > 1 && !cols[1].IsEmpty())
                        flowModel.SheetName = cols[1];

                    if (cols.Length > 2 && !cols[2].IsEmpty())
                        flowModel.Enable = cols[2];

                    flows.Add(flowModel);
                }

                var index = 0;
                foreach (var flow in flows)
                {
                    flow.Id = Guid.NewGuid().ToString();
                    var testItem = result?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(flow.TestItemName));
                    flow.TestItemId = testItem?.Id.ToGuid() ?? Guid.Empty;
                    flow.IsSelected = flow.Enable.IsEmpty();
                    flow.SortId = ++index;
                }

                result.Flow = flows;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void GetLevelData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;

            try
            {
                var levels = new List<LevelModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(",");

                    if (cols.IsEmpty())
                        continue;

                    var levelModel = new LevelModel();
                    levelModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > 0 && !cols[0].IsEmpty())
                        levelModel.GroupName = cols[0];

                    if (cols.Length > 1 && !cols[1].IsEmpty())
                        levelModel.Vil = Convert.ToDecimal(cols[1]?.ToString());

                    if (cols.Length > 2 && !cols[2].IsEmpty())
                        levelModel.Vih = Convert.ToDecimal(cols[2]?.ToString());

                    if (cols.Length > 3 && !cols[3].IsEmpty())
                        levelModel.Vol = Convert.ToDecimal(cols[3]?.ToString());

                    if (cols.Length > 4 && !cols[4].IsEmpty())
                        levelModel.Voh = Convert.ToDecimal(cols[4]?.ToString());

                    if (cols.Length > 5 && !cols[5].IsEmpty())
                        levelModel.Iol = Convert.ToDecimal(cols[5]?.ToString());

                    if (cols.Length > 6 && !cols[6].IsEmpty())
                        levelModel.Ioh = Convert.ToDecimal(cols[6]?.ToString());

                    if (cols.Length > 7 && !cols[7].IsEmpty())
                        levelModel.Vt = Convert.ToDecimal(cols[7]?.ToString());

                    if (cols.Length > 8 && !cols[8].IsEmpty())
                        levelModel.Vcl = Convert.ToDecimal(cols[8]?.ToString());

                    if (cols.Length > 9 && !cols[9].IsEmpty())
                        levelModel.Vch = Convert.ToDecimal(cols[9]?.ToString());

                    levels.Add(levelModel);
                }
                foreach (var level in levels)
                {
                    level.Id = Guid.NewGuid().ToString();

                    var tempChannel = result?.Channel?.FirstOrDefault(x => x?.GroupName?.Equals(level.GroupName) == true);
                    level.ChannelGroupId = tempChannel?.GroupId ?? Guid.Empty;
                }

                var testItems = result?.TestItem?.Where(x => x?.Level?.Equals(sheetName) == true).Select(x => x);
                foreach (var testItem in testItems)
                    testItem.Levels = levels;

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GetTimingData(TestPlanModel result, StreamReader reader, string sheetName)
        {
            if (result == null)
                return;

            try
            {
                var timings = new List<TimingModel>();
                while (!reader.EndOfStream)
                {
                    var row = reader.ReadLine();
                    var cols = row.Split(",");

                    if (cols.IsEmpty())
                        continue;

                    var timingModel = new TimingModel();
                    timingModel.Id = Guid.NewGuid().ToString();
                    if (cols.Length > 0 && !cols[0].IsEmpty())
                        timingModel.TimingName = cols[0];

                    if (cols.Length > 1 && !cols[1].IsEmpty())
                        timingModel.Period = Convert.ToInt32(cols[1]?.ToString());

                    if (cols.Length > 2 && !cols[2].IsEmpty())
                        timingModel.PinName = cols[2];

                    if (cols.Length > 3 && !cols[3].IsEmpty())
                        timingModel.PinSetup = cols[3];

                    if (cols.Length > 4 && !cols[4].IsEmpty())
                        timingModel.Fmt = cols[4];

                    if (cols.Length > 5 && !cols[5].IsEmpty())
                        timingModel.DriveA = cols[5];

                    if (cols.Length > 6 && !cols[6].IsEmpty())
                        timingModel.DriveB = cols[6];

                    if (cols.Length > 7 && !cols[7].IsEmpty())
                        timingModel.DriveC = cols[7];

                    if (cols.Length > 8 && !cols[8].IsEmpty())
                        timingModel.DriveD = cols[8];

                    timings.Add(timingModel);
                }
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
        #endregion

        #region 设置测试计划Flow
        public bool SetTestPlanFlow(TestPlanModel testPlan, TestPlanType testPlanType, string filePath)
        {
            var result = false;

            if (testPlan == null)
                return result;

            if (testPlan.Flow.IsEmpty())
                return result;

            if (!File.Exists(filePath))
                throw new Warning("文件不存在");

            IWorkbook workbook = null;
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(stream);
                    var sheet = workbook?.GetSheet("Flow");
                    var startRowIndex = 2;
                    var enableColIndex = 2;
                    foreach (var flow in testPlan.Flow)
                        sheet?.GetRow(startRowIndex++)?.CreateCell(enableColIndex)?.SetCellValue(flow.Enable);
                }

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook?.Write(stream);
                    workbook?.Close();
                }
                result = true;
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }
        #endregion

        #region 另存为测试计划
        public bool SaveAsTestPlan(TestPlanModel testPlan, TestPlanType testPlanType, string saveAsDir, string fileName)
        {
            var result = false;
            var testPlanDirName = ConfigurationManager.AppSettings["TestPlanDirName"] ?? throw new ArgumentNullException("TemplateDirName");
            var testPlanExtension = testPlanType == TestPlanType.Excel ? ".xlsx" : ".csv";

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
                foreach (var channel in testPlan.Channel)
                {
                    var col = 0;
                    var row = sheet?.CreateRow(rowNum++);
                    if (!channel.GroupName.IsEmpty())
                        row.CreateCell(col++).SetCellValue(channel.GroupName);

                    if (!channel.PinName.IsEmpty())
                        row.CreateCell(col++).SetCellValue(channel.PinName);

                    if (channel.Type != null)
                        row.CreateCell(col++).SetCellValue(channel.Type?.Description());

                    if (!channel.Sites.IsEmpty())
                        foreach (var site in channel.Sites)
                            row.CreateCell(col++).SetCellValue(site.SiteName);
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
                        row.CreateCell(col++).SetCellValue(testItem.TestItemName);

                    if (!testItem.FunctionName.IsEmpty())
                        row.CreateCell(col++).SetCellValue(testItem.FunctionName);

                    if (testItem.Force != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(testItem.Force));

                    if (testItem.Pins != null)
                        row.CreateCell(col++).SetCellValue(testItem.Pins);

                    if (testItem.Level != null)
                        row.CreateCell(col++).SetCellValue(testItem.Level);

                    if (testItem.Timing != null)
                        row.CreateCell(col++).SetCellValue(testItem.Timing);

                    if (!testItem.Args.IsEmpty())
                        foreach (var arg in testItem.Args)
                            row.CreateCell(col++).SetCellValue(arg);
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
                        row.CreateCell(col++).SetCellValue(limit.TestItemName);

                    if (limit.TestNumber != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(limit.TestNumber?.ToString()));

                    if (limit.LowLimit != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(limit.LowLimit));

                    if (limit.HighLimit != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(limit.HighLimit));

                    if (limit.Units != null)
                        row.CreateCell(col++).SetCellValue(limit.Units);

                    if (limit.LimitName != null)
                        row.CreateCell(col++).SetCellValue(limit.LimitName);
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
                        row.CreateCell(col++).SetCellValue(flow.TestItemName);

                    if (flow.SheetName != null)
                        row.CreateCell(col++).SetCellValue(flow.SheetName);

                    if (flow.Enable != null)
                        row.CreateCell(col++).SetCellValue(flow.Enable);
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
                if (!levelSheetName.IsEmpty())
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
                    if (!level.GroupName.IsEmpty())
                        row.CreateCell(col++).SetCellValue(level.GroupName);

                    if (level.Vil != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Vil));

                    if (level.Vih != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Vih));

                    if (level.Vol != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Vol));

                    if (level.Voh != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Voh));

                    if (level.Iol != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Iol));

                    if (level.Ioh != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Ioh));

                    if (level.Vt != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Vt));

                    if (level.Vcl != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Vcl));

                    if (level.Vch != null)
                        row.CreateCell(col++).SetCellValue(Convert.ToDouble(level.Vch));

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
                if (!timingSheetName.IsEmpty())
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

                    if (timing.Period != null)
                        row.CreateCell(col++).SetCellValue(timing.Period ?? 0);

                    if (timing.PinName != null)
                        row.CreateCell(col++).SetCellValue(timing.PinName);

                    if (timing.PinSetup != null)
                        row.CreateCell(col++).SetCellValue(timing.PinSetup);

                    if (timing.Fmt != null)
                        row.CreateCell(col++).SetCellValue(timing.Fmt);

                    if (timing.DriveA != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveA);

                    if (timing.DriveB != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveB);

                    if (timing.DriveC != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveC);

                    if (timing.DriveD != null)
                        row.CreateCell(col++).SetCellValue(timing.DriveD);

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



            return result;
        }
        #endregion
    }
}
