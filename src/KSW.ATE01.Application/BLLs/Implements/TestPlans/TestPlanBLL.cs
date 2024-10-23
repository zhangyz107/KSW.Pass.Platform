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
using KSW.ATE01.Application.Models.TestPlan;
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.Exceptions;
using Microsoft.Office.Interop.Excel;
using MiniExcelLibs;

namespace KSW.ATE01.Application.BLLs.Implements.TestPlans
{
    /// <summary>
    /// 测试计划业务逻辑层
    /// </summary>
    public class TestPlanBLL : ServiceBase, ITestPlanBLL
    {
        private readonly string _channelDataStartCell = "A4";
        private readonly string _testItemDataStartCell = "A3";
        private readonly string _limitsDataStartCell = "A2";
        private readonly string _flowDataStartCell = "A2";
        private readonly string _levelDataStartCell = "A2";
        private readonly string _timingDataStartCell = "A2";

        public TestPlanBLL(IContainerProvider containerProvider) : base(containerProvider)
        {
        }

        public async Task<TestPlanModel> LoadTestPlanAsync(TestPlanType testPlanType, string filePath)
        {
            var result = new TestPlanModel();
            switch (testPlanType)
            {
                case TestPlanType.Excel:
                    result = await LoadTestPlanFromExcelAsync(filePath);
                    break;
                case TestPlanType.Csv:
                    break;
            }
            return result;
        }

        private async Task<TestPlanModel> LoadTestPlanFromExcelAsync(string filePath)
        {
            var result = new TestPlanModel();
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

        private static void GetChannelArgs(IDictionary<string, object> row, TestItemModel testItem)
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
                var testItem = result?.TestItem?.FirstOrDefault(x => x.TestItemName.Equals(flow.TestItemName));
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

        public void SetTestPlanFlow(TestPlanModel testPlan, TestPlanType testPlanType, string filePath)
        {
            if (testPlan == null)
                return;

            if (testPlan.Flow.IsEmpty())
                return;

            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            excelApp.Visible = false;
            var workbook = excelApp.Workbooks.Open(filePath);
            var worksheet = (Worksheet)workbook.Worksheets["Flow"];
            var startRowIndex = 3;
            foreach (var flow in testPlan.Flow)
            {
                worksheet.Cells[startRowIndex++,3] = flow.Enable;
            }

            workbook.Save();
            workbook.Close();
            excelApp.Quit();
        }
    }
}
