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
using KSW.ATE01.Domain.Projects.Core.Enums;
using KSW.ATE01.Domain.TestPlan.Core.Enums;
using KSW.ATE01.Domain.TestPlan.Entities;
using KSW.ATE01.Domain.TestPlan.Repositories;
using KSW.ATE01.Instrument.IO.BLLs.Implements.Results;
using KSW.ATE01.Project.Base.Enums.Results;
using KSW.ATE01.Project.Base.Enums.TestPlans;
using KSW.ATE01.Project.Base.Helpers;
using KSW.ATE01.Project.Base.Models.TestPlans;
using KSW.Helpers;
using NPOI.SS.UserModel;
using NPOI.Util;
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
        private readonly string _channelSheetName = "Channel";
        private readonly string _testItemSheetName = "TestItem";
        private readonly string _limitsSheetName = "Limits";
        private readonly string _flowSheetName = "Flow";
        private readonly string _levelSheetName = "Level";
        private readonly string _timingSheetName = "Timing";
        private readonly string _globalSheetName = "Global";

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

        #region 转换测试计划
        public async Task<TestPlanModel> ConversionTestPlanAsync(string projectId)
        {
            if (projectId.IsEmpty())
                return null;

            var groupIdDict = new Dictionary<Guid, string>();
            var pinIdDict = new Dictionary<Guid, string>();
            var levelGroupDict = new Dictionary<Guid, List<Project.Base.Models.TestPlans.LevelModel>>();
            var timingGroupDict = new Dictionary<Guid, List<Project.Base.Models.TestPlans.TimingModel>>();

            var result = new TestPlanModel();
            // 引脚信息
            await ConversionChannel(projectId, result, groupIdDict, pinIdDict);

            // 电平
            var levelGroups = await ConversionLevel(projectId, groupIdDict, pinIdDict, levelGroupDict);

            // 时钟
            var timingGroups = await ConversionTiming(projectId, groupIdDict, pinIdDict, timingGroupDict);

            // 测试项
            var testItems = await ConversionTestItem(projectId, result, groupIdDict, pinIdDict, levelGroupDict, timingGroupDict, levelGroups, timingGroups);

            await ConversionLimits(projectId, result, testItems);

            //全局参数
            await ConversionGlobalParameter(projectId, result);

            return result;
        }

        private List<PinGroupModel> GetPinGroups(List<GroupInfo> groupInfos, List<Guid> groupIds)
        {
            if (groupInfos.IsEmpty() || groupIds.IsEmpty())
                return null;

            var result = new List<PinGroupModel>();
            var groups = groupInfos.Where(x => groupIds.Contains(x.Id)).ToList();
            foreach (var group in groups)
            {
                var tempGroup = new PinGroupModel();
                tempGroup.Id = group.Id.SafeString();
                tempGroup.Name = group.GroupName;
                result.Add(tempGroup);
            }
            return result;
        }

        private List<SiteModel> GetPinSites(List<SiteInfo> siteInfos, List<PinSiteInfo> pinSiteInfos, List<Guid> pinSiteIds)
        {
            if (siteInfos.IsEmpty() || pinSiteInfos.IsEmpty() || pinSiteIds.IsEmpty())
                return null;


            var result = new List<SiteModel>();
            var sites = siteInfos.Where(x => pinSiteIds.Contains(x.Id)).ToList();

            foreach (var site in sites)
            {
                var pinSites = pinSiteInfos.FirstOrDefault(x => x.SiteInfoId.Equals(site.Id));
                var tempSite = new SiteModel();
                tempSite.SiteName = site.SiteName;
                tempSite.SiteValue = pinSites?.ChannelName;
                result.Add(tempSite);
            }
            return result;
        }

        private async Task<List<LevelGroup>> ConversionLevel(string projectId, Dictionary<Guid, string> groupIdDict, Dictionary<Guid, string> pinIdDict, Dictionary<Guid, List<Project.Base.Models.TestPlans.LevelModel>> levelGroupDict)
        {
            var levelGroups = await _levelGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            if (!levelGroups.IsEmpty())
            {
                foreach (var levelGroup in levelGroups)
                {
                    var tempLevels = new List<Project.Base.Models.TestPlans.LevelModel>();
                    var levels = await _levelRepository.FindAllAsync(x => x.LevelGroupId.Equals(levelGroup.Id));
                    if (!levels.IsEmpty())
                    {
                        foreach (var level in levels)
                        {
                            var tempLevel = new Project.Base.Models.TestPlans.LevelModel();
                            tempLevel.Id = level.Id.SafeString();
                            var groupOrPinId = level.GroupOrPinId ?? Guid.Empty;
                            if (groupIdDict.ContainsKey(groupOrPinId))
                                tempLevel.PinGroupName = groupIdDict[groupOrPinId];
                            else if (pinIdDict.ContainsKey(groupOrPinId))
                                tempLevel.PinGroupName = pinIdDict[groupOrPinId];
                            tempLevel.Vil = level.Vil ?? 0;
                            tempLevel.Vih = level.Vih ?? 0;
                            tempLevel.Vol = level.Vol ?? 0;
                            tempLevel.Voh = level.Voh ?? 0;
                            tempLevel.Iol = level.Iol ?? 0;
                            tempLevel.Ioh = level.Ioh ?? 0;
                            tempLevel.Vt = level.Vt ?? 0;
                            tempLevel.Vcl = level.Vcl ?? 0;
                            tempLevel.Vch = level.Vch ?? 0;
                            tempLevel.PS = level.Ps ?? 0;
                            tempLevel.I = level.I ?? 0;
                            tempLevel.Tdelay = level.Tdelay ?? 0;
                            tempLevel.Sequence = level.Sequence?.ToString();
                            tempLevel.Comment = level.Comment;
                            tempLevels.Add(tempLevel);
                        }
                    }

                    if (!levelGroupDict.ContainsKey(levelGroup.Id))
                        levelGroupDict.Add(levelGroup.Id, tempLevels);
                }
            }
            return levelGroups;
        }

        private async Task<List<TimingGroup>> ConversionTiming(string projectId, Dictionary<Guid, string> groupIdDict, Dictionary<Guid, string> pinIdDict, Dictionary<Guid, List<Project.Base.Models.TestPlans.TimingModel>> timingGroupDict)
        {
            var timingGroups = await _timingGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            if (!timingGroups.IsEmpty())
            {
                foreach (var timingGroup in timingGroups)
                {
                    var tempTimings = new List<Project.Base.Models.TestPlans.TimingModel>();
                    var timings = await _timingRepository.FindAllAsync(x => x.TimingGroupId.Equals(timingGroup.Id));
                    if (!timings.IsEmpty())
                    {
                        foreach (var timing in timings)
                        {
                            var tempTiming = new Project.Base.Models.TestPlans.TimingModel();
                            tempTiming.Id = timing.Id.SafeString();
                            tempTiming.TimingName = timing.TimingName;
                            tempTiming.Period = timing.Period ?? 0;
                            tempTiming.PinId = timing.GroupOrPinId ?? Guid.Empty;
                            var groupOrPinId = timing.GroupOrPinId ?? Guid.Empty;
                            if (groupIdDict.ContainsKey(groupOrPinId))
                                tempTiming.PinName = groupIdDict[groupOrPinId];
                            else if (pinIdDict.ContainsKey(groupOrPinId))
                                tempTiming.PinName = pinIdDict[groupOrPinId];
                            tempTiming.PinSetup = "PAT";
                            if (System.Enum.TryParse(timing.WaveformFormat.ToString(), out Timingformat timingformat))
                                tempTiming.Fmt = timingformat;
                            tempTiming.DriveA = timing.DriveA?.ToString();
                            tempTiming.DriveB = timing.DriveB?.ToString();
                            tempTiming.DriveC = timing.DriveC?.ToString();
                            tempTiming.DriveD = timing.DriveD?.ToString();
                            if (System.Enum.TryParse(timing.StrobeMode.ToString(), out Project.Base.Enums.TestPlans.StrobeModeType strobeMode))
                                tempTiming.StrobeMode = strobeMode;
                            tempTiming.StrobeA = timing.StrobeA ?? 0;
                            tempTiming.StrobeB = timing.StrobeB ?? 0;
                            tempTiming.Comment = timing.Comment;
                        }
                    }

                    if (!timingGroupDict.ContainsKey(timingGroup.Id))
                        timingGroupDict.Add(timingGroup.Id, tempTimings);
                }
            }
            return timingGroups;
        }

        private async Task<List<TestItemInfo>> ConversionTestItem(string projectId, TestPlanModel result, Dictionary<Guid, string> groupIdDict, Dictionary<Guid, string> pinIdDict, Dictionary<Guid, List<Project.Base.Models.TestPlans.LevelModel>> levelGroupDict, Dictionary<Guid, List<Project.Base.Models.TestPlans.TimingModel>> timingGroupDict, List<LevelGroup> levelGroups, List<TimingGroup> timingGroups)
        {
            var testItems = await _testItemInfoRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            if (!testItems.IsEmpty())
            {
                var testItemList = new List<TestItemModel>();
                var flowList = new List<FlowModel>();
                foreach (var testItem in testItems)
                {
                    var tempTestItem = new TestItemModel();
                    tempTestItem.Id = testItem.Id.SafeString();
                    tempTestItem.SheetName = "TestItem";
                    tempTestItem.TestItemName = testItem.TestItemName;
                    tempTestItem.FunctionName = testItem.FunctionName;
                    tempTestItem.Force = testItem.Force;
                    var groupOrPinId = testItem.GroupOrPinId ?? Guid.Empty;
                    if (groupIdDict.ContainsKey(groupOrPinId))
                        tempTestItem.Pins = groupIdDict[groupOrPinId];
                    else if (pinIdDict.ContainsKey(groupOrPinId))
                        tempTestItem.Pins = pinIdDict[groupOrPinId];

                    // 电平
                    var levelGroupId = testItem.LevelGroupId ?? Guid.Empty;
                    var levelGroup = levelGroups.FirstOrDefault(x => x.Id.Equals(levelGroupId));
                    tempTestItem.Level = levelGroup?.LevelGroupName;
                    if (levelGroupDict.ContainsKey(levelGroupId))
                        tempTestItem.Levels = levelGroupDict[levelGroupId];

                    // 时钟
                    var timingGroupId = testItem.TimingGroupId ?? Guid.Empty;
                    var timingGroup = timingGroups.FirstOrDefault(x => x.Id.Equals(timingGroupId));
                    tempTestItem.Timing = timingGroup?.TimingGroupName;
                    if (timingGroupDict.ContainsKey(timingGroupId))
                        tempTestItem.Timings = timingGroupDict[timingGroupId];

                    // 附加参数
                    if (!testItem.AdditionInfo.IsEmpty())
                    {
                        var args = testItem.AdditionInfo.Split(',');
                        var index = 0;
                        foreach (var arg in args)
                        {
                            var tempArg = new TestItemParamModel();
                            tempArg.ParamName = $"Arg{index++}";
                            tempArg.ParamValue = arg;
                            tempTestItem.Args.Add(tempArg);
                        }
                    }
                    testItemList.Add(tempTestItem);

                    var tempFlow = new FlowModel();
                    tempFlow.Id = Guid.NewGuid().ToString();
                    tempFlow.TestItemId = testItem.Id;
                    tempFlow.TestItemName = testItem.TestItemName;
                    tempFlow.SheetName = "TestItem";
                    tempFlow.SortId = testItem.FlowIndex ?? 0;
                    tempFlow.Enable = testItem.Enable == true ? null : "False";
                    tempFlow.IsSelected = testItem.Enable ?? false;
                    flowList.Add(tempFlow);
                }
                result.TestItem = testItemList;
                result.Flow = flowList.OrderBy(x => x.SortId).ToList();
            }

            return testItems;
        }

        private async Task ConversionLimits(string projectId, TestPlanModel result, List<TestItemInfo> testItems)
        {
            var limits = await _limitsRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            var limitsModels = new List<Project.Base.Models.TestPlans.LimitsModel>();
            foreach (var limit in limits)
            {
                var testItem = testItems.FirstOrDefault(x => x.LimitsId.Equals(limit.Id));
                var tempLimit = new Project.Base.Models.TestPlans.LimitsModel();
                tempLimit.Id = limit.Id.SafeString();
                if (testItem != null)
                {
                    tempLimit.TestItemId = testItem.Id;
                    tempLimit.TestItemName = testItem.TestItemName;
                }
                tempLimit.LimitName = limit.LimitName;
                tempLimit.TestNumber = (uint)limit.TestNumber;
                tempLimit.LowLimit = System.Convert.ToDouble(limit.LowLimit);
                tempLimit.HighLimit = System.Convert.ToDouble(limit.HighLimit);
                tempLimit.Units = limit.Units;
                tempLimit.FailHardwareBin = (uint)limit.FailHardwareBin;
                tempLimit.PassHardwareBin = (uint)limit.PassHardwareBin;
                tempLimit.FailSoftwareBin = (uint)limit.FailSoftwareBin;
                tempLimit.PassSoftwareBin = (uint)limit.PassSoftwareBin;
                tempLimit.DUTResult = Test.Fail;
                switch (limit.DutResult)
                {
                    case Domain.TestPlan.Core.Enums.DUTResultType.Pass:
                        tempLimit.DUTResult = Test.Pass;
                        break;
                    case Domain.TestPlan.Core.Enums.DUTResultType.Fail:
                        tempLimit.DUTResult = Test.Fail;
                        break;
                    case Domain.TestPlan.Core.Enums.DUTResultType.Error:
                        tempLimit.DUTResult = Test.Error;
                        break;
                }
                limitsModels.Add(tempLimit);
            }
            result.Limits = limitsModels;
        }

        private async Task ConversionChannel(string projectId, TestPlanModel testPlanModel, Dictionary<Guid, string> groupIdDict, Dictionary<Guid, string> pinIdDict)
        {
            var ovewview = (await _pinOverviewRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid())))?.FirstOrDefault();
            if (ovewview != null)
            {
                var pinInfos = await _pinInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(ovewview.Id));

                var groupInfos = await _groupInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(ovewview.Id));
                foreach (var groupInfo in groupInfos)
                {
                    if (!groupIdDict.ContainsKey(groupInfo.Id))
                        groupIdDict.Add(groupInfo.Id, groupInfo.GroupName);
                }

                var siteInfos = await _siteInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(ovewview.Id));

                var channels = new List<ChannelModel>();
                foreach (var pinInfo in pinInfos)
                {
                    if (!pinIdDict.ContainsKey(pinInfo.Id))
                        pinIdDict.Add(pinInfo.Id, pinInfo.PinName);

                    var pinGroupRelationships = await _pinGroupRelationshipRepositoy.FindAllAsync(x => x.PinInfoId.Equals(pinInfo.Id));
                    var groupIds = pinGroupRelationships.Select(x => x.GroupInfoId).ToList();
                    var groups = GetPinGroups(groupInfos, groupIds);

                    var pinSiteInfos = await _pinSiteInfoRepository.FindAllAsync(x => x.PinInfoId.Equals(pinInfo.Id));
                    var pinSiteIds = pinSiteInfos.Select(x => x.SiteInfoId).ToList();
                    var sites = GetPinSites(siteInfos, pinSiteInfos, pinSiteIds);

                    var tempChannel = new ChannelModel();
                    tempChannel.Id = pinInfo.Id.SafeString();
                    tempChannel.PinName = pinInfo.PinName;
                    if (System.Enum.TryParse(pinInfo.PinType.ToString(), out ChannelType channelType))
                    {
                        tempChannel.Type = channelType;
                    }
                    tempChannel.Groups = groups;
                    tempChannel.Sites = sites;
                    channels.Add(tempChannel);
                }
                testPlanModel.Channel = channels;
            }
        }

        private async Task ConversionGlobalParameter(string projectId, TestPlanModel result)
        {
            var globalParameters = await _globalParameterRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            if (!globalParameters.IsEmpty())
            {
                var global = new List<GlobalModel>();
                foreach (var globalParameter in globalParameters)
                {
                    var tempGlobalParameter = new GlobalModel();
                    tempGlobalParameter.Id = globalParameter.Id.SafeString();
                    tempGlobalParameter.PatternFile = globalParameter.PatternFile;
                    if (!globalParameter.AdditionInfo.IsEmpty())
                    {
                        var args = globalParameter.AdditionInfo.Split(',');
                        var index = 0;
                        foreach (var arg in args)
                        {
                            var tempArg = new GlobalParamModel();
                            tempArg.ParamName = $"Default {++index}";
                            tempArg.ParamValue = arg;
                            tempGlobalParameter.Args.Add(tempArg);
                        }

                    }
                    global.Add(tempGlobalParameter);
                }
                result.Global = global;
            }
        }
        #endregion

        #region 拷贝测试计划
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
                var pinInfos = (await _pinInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id))).OrderByDescending(x => x.CreationTime);
                var pinInfoList = new List<PinInfo>();
                foreach (var pinInfo in pinInfos)
                {
                    var pinInfoEntity = await CopyPinInfoAsync(pinInfoMapping, pinOverviewEntity, pinInfo);
                    pinInfoList.Add(pinInfoEntity);
                }
                await _pinInfoRepository.AddAsync(pinInfoList);

                // 组信息
                var groupInfos = (await _groupInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id))).OrderByDescending(x => x.CreationTime);
                var groupInfoList = new List<GroupInfo>();
                foreach (var groupInfo in groupInfos)
                {
                    var groupInfoEntity = await CopyGroupInfoAsync(groupInfoMapping, pinOverviewEntity, groupInfo);
                    groupInfoList.Add(groupInfoEntity);

                    // 组与引脚关系
                    var relationships = (await _pinGroupRelationshipRepositoy.FindAllAsync(x => x.GroupInfoId.Equals(groupInfo.Id))).OrderByDescending(x => x.CreationTime);
                    var relationshipList = new List<PinGroupRelationship>();
                    foreach (var relationship in relationships)
                    {
                        var relationshipsEntity = await CopyPinGroupRelationshipAsync(pinInfoMapping, groupInfoEntity, relationship);
                        relationshipList.Add(relationshipsEntity);
                    }
                    await _pinGroupRelationshipRepositoy.AddAsync(relationshipList);
                }
                await _groupInfoRepository.AddAsync(groupInfoList);

                // 站点信息
                var siteInfos = (await _siteInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id))).OrderByDescending(x => x.CreationTime);
                var siteInfoList = new List<SiteInfo>();
                foreach (var siteInfo in siteInfos)
                {
                    var siteInfoEntity = await CopySiteInfoAsync(pinOverviewEntity, siteInfo);
                    siteInfoList.Add(siteInfoEntity);

                    // 引脚站点信息
                    var pinSiteInfos = (await _pinSiteInfoRepository.FindAllAsync(x => x.SiteInfoId.Equals(siteInfo.Id))).OrderByDescending(x => x.CreationTime);
                    var pinSiteInfoList = new List<PinSiteInfo>();
                    foreach (var pinSiteInfo in pinSiteInfos)
                    {
                        var pinSiteInfoEntity = await CopyPinSiteInfoAsync(pinInfoMapping, siteInfoEntity, pinSiteInfo);
                        pinSiteInfoList.Add(pinSiteInfoEntity);
                    }
                    await _pinSiteInfoRepository.AddAsync(pinSiteInfoList);
                }
                await _siteInfoRepository.AddAsync(siteInfoList);
            }
            #endregion

            #region 门限
            var limits = (await _limitsRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()))).OrderByDescending(x => x.CreationTime);
            var limitList = new List<Limits>();
            foreach (var limit in limits)
            {
                var limitEntity = await CopyLimitAsync(limitsMapping, limit, newProjectId);
                limitList.Add(limitEntity);
            }
            await _limitsRepository.AddAsync(limitList);
            #endregion

            #region 电平组
            var levelGroups = (await _levelGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()))).OrderByDescending(x => x.CreationTime);
            var levelGroupList = new List<LevelGroup>();
            foreach (var levelGroup in levelGroups)
            {
                var levelGroupEntity = await CopyLevelGroupAsync(levelGroupMapping, levelGroup, newProjectId);
                levelGroupList.Add(levelGroupEntity);

                // 电平
                var levels = (await _levelRepository.FindAllAsync(x => x.LevelGroupId.Equals(levelGroup.Id))).OrderByDescending(x => x.CreationTime);
                var levelList = new List<Level>();
                foreach (var level in levels)
                {
                    var levelEntity = await CopyLevelAsync(pinInfoMapping, groupInfoMapping, levelGroupEntity, level);
                    levelList.Add(levelEntity);
                }
                await _levelRepository.AddAsync(levelList);
            }
            await _levelGroupRepository.AddAsync(levelGroupList);
            #endregion

            #region 时钟组
            var timingGroups = (await _timingGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()))).OrderByDescending(x => x.CreationTime);
            var timingGroupList = new List<TimingGroup>();
            foreach (var timingGroup in timingGroups)
            {
                var timingGroupEntity = await CopyTimingGroupAsync(timingGroupMapping, timingGroup, newProjectId);
                timingGroupList.Add(timingGroupEntity);

                // 时钟
                var timings = (await _timingRepository.FindAllAsync(x => x.TimingGroupId.Equals(timingGroup.Id))).OrderByDescending(x => x.CreationTime);
                var timingList = new List<Timing>();
                foreach (var timing in timings)
                {
                    var timingEntity = await CopyTimingAsync(pinInfoMapping, groupInfoMapping, timingGroupEntity, timing);
                    timingList.Add(timingEntity);
                }
                await _timingRepository.AddAsync(timingList);
            }
            await _timingGroupRepository.AddAsync(timingGroupList);
            #endregion

            #region 测试项
            var testItemInfos = (await _testItemInfoRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()))).OrderByDescending(x => x.CreationTime);
            var testItemList = new List<TestItemInfo>();
            foreach (var testItemInfo in testItemInfos)
            {
                var testItemInfoEntity = await CopyTestItemInfoAsync(pinInfoMapping, groupInfoMapping, limitsMapping, levelGroupMapping, timingGroupMapping, testItemInfo, newProjectId);
                testItemList.Add(testItemInfoEntity);
            }
            await _testItemInfoRepository.AddAsync(testItemList);
            #endregion

            #region 全局参数
            var globalParameters = (await _globalParameterRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()))).OrderByDescending(x => x.CreationTime);
            var globalParameterList = new List<GlobalParameter>();
            foreach (var globalParameter in globalParameters)
            {
                var globalParameterEntity = await CopyGlobalParameterAsync(globalParameter, newProjectId);
                globalParameterList.Add(globalParameterEntity);
            }
            await _globalParameterRepository.AddAsync(globalParameterList);
            #endregion
        }

        private async Task<PinOverview> CopyPinOverviewAsync(PinOverview old, string newProjectId)
        {
            var newPinOverview = old.Clone();
            var pinOverviewModel = newPinOverview.MapTo<PinOverviewModel>();
            pinOverviewModel.Id = Guid.NewGuid().ToString();
            pinOverviewModel.ProjectInfoId = newProjectId.ToGuid();
            pinOverviewModel.CreationTime = null;
            var pinOverviewEntity = pinOverviewModel.MapTo<PinOverview>();
            await _pinOverviewRepository.AddAsync(pinOverviewEntity);
            return pinOverviewEntity;
        }

        private async Task<PinInfo> CopyPinInfoAsync(Dictionary<Guid, Guid> pinInfoMapping, PinOverview newPinOverview, PinInfo old)
        {
            var newPinInfo = old.Clone();
            var pinInfoModel = newPinInfo.MapTo<PinInfoModel>();
            pinInfoModel.Id = Guid.NewGuid().ToString();
            pinInfoModel.PinOverviewId = newPinOverview.Id;
            pinInfoModel.CreationTime = null;
            var pinInfoEntity = pinInfoModel.MapTo<PinInfo>();

            if (!pinInfoMapping.ContainsKey(old.Id))
                pinInfoMapping.Add(old.Id, pinInfoEntity.Id);

            return pinInfoEntity;
        }

        private async Task<GroupInfo> CopyGroupInfoAsync(Dictionary<Guid, Guid> groupInfoMapping, PinOverview newPinOverview, GroupInfo old)
        {
            var newGroupInfo = old.Clone();
            var groupInfoModel = newGroupInfo.MapTo<GroupInfoModel>();
            groupInfoModel.Id = Guid.NewGuid().ToString();
            groupInfoModel.PinOverviewId = newPinOverview.Id;
            groupInfoModel.CreationTime = null;
            var groupInfoEntity = groupInfoModel.MapTo<GroupInfo>();

            if (!groupInfoMapping.ContainsKey(old.Id))
                groupInfoMapping.Add(old.Id, groupInfoEntity.Id);

            return groupInfoEntity;
        }

        private async Task<PinGroupRelationship> CopyPinGroupRelationshipAsync(Dictionary<Guid, Guid> pinInfoMapping, GroupInfo newGroupInfo, PinGroupRelationship old)
        {
            var newRelationship = old.Clone();
            var relationshipModel = newRelationship.MapTo<PinGroupRelationshipModel>();
            relationshipModel.Id = Guid.NewGuid().ToString();
            relationshipModel.GroupInfoId = newGroupInfo.Id;
            relationshipModel.CreationTime = null;
            var pinId = relationshipModel.PinInfoId ?? Guid.Empty;
            if (pinInfoMapping.ContainsKey(pinId))
                relationshipModel.PinInfoId = pinInfoMapping[pinId];
            var relationshipEntity = relationshipModel.MapTo<PinGroupRelationship>();
            return relationshipEntity;
        }

        private async Task<SiteInfo> CopySiteInfoAsync(PinOverview newPinOverview, SiteInfo old)
        {
            var newSiteInfo = old.Clone();
            var siteInfoModel = newSiteInfo.MapTo<SiteInfoModel>();
            siteInfoModel.Id = Guid.NewGuid().ToString();
            siteInfoModel.PinOverviewId = newPinOverview.Id;
            siteInfoModel.CreateTime = null;
            var siteInfoEntity = siteInfoModel.MapTo<SiteInfo>();

            return siteInfoEntity;
        }

        private async Task<PinSiteInfo> CopyPinSiteInfoAsync(Dictionary<Guid, Guid> pinInfoMapping, SiteInfo siteInfoEntity, PinSiteInfo old)
        {
            var newPinSiteInfo = old.Clone();
            var pinSiteModel = newPinSiteInfo.MapTo<PinSiteInfoModel>();
            pinSiteModel.Id = Guid.NewGuid().ToString();
            pinSiteModel.SiteInfoId = siteInfoEntity.Id;
            pinSiteModel.CreationTime = null;
            var pinId = pinSiteModel.PinInfoId;
            if (pinInfoMapping.ContainsKey(pinId))
                pinSiteModel.PinInfoId = pinInfoMapping[pinId];
            var pinSiteEntity = pinSiteModel.MapTo<PinSiteInfo>();
            return pinSiteEntity;
        }

        private async Task<Limits> CopyLimitAsync(Dictionary<Guid, Guid> limitsMapping, Limits old, string newProjectId)
        {
            var newLimit = old.Clone();
            var limitModel = newLimit.MapTo<Models.TestPlans.LimitsModel>();
            limitModel.Id = Guid.NewGuid().ToString();
            limitModel.ProjectInfoId = newProjectId.ToGuid();
            limitModel.CreationTime = null;
            var limitEntity = limitModel.MapTo<Limits>();

            if (!limitsMapping.ContainsKey(old.Id))
                limitsMapping.Add(old.Id, limitEntity.Id);
            return limitEntity;
        }

        private async Task<LevelGroup> CopyLevelGroupAsync(Dictionary<Guid, Guid> levelGroupMapping, LevelGroup old, string newProjectId)
        {
            var newLevelGroup = old.Clone();
            var levelGroupModel = newLevelGroup.MapTo<LevelGroupModel>();
            levelGroupModel.Id = Guid.NewGuid().ToString();
            levelGroupModel.ProjectInfoId = newProjectId.ToGuid();
            levelGroupModel.CreationTime = null;
            var levelGroupEntity = levelGroupModel.MapTo<LevelGroup>();
            await _levelGroupRepository.AddAsync(levelGroupEntity);

            if (!levelGroupMapping.ContainsKey(old.Id))
                levelGroupMapping.Add(old.Id, levelGroupEntity.Id);

            return levelGroupEntity;
        }

        private async Task<Level> CopyLevelAsync(Dictionary<Guid, Guid> pinInfoMapping, Dictionary<Guid, Guid> groupInfoMapping, LevelGroup newLevelGroup, Level old)
        {
            var newLevel = old.Clone();
            var levelModel = newLevel.MapTo<Models.TestPlans.LevelModel>();
            levelModel.Id = Guid.NewGuid().ToString();
            levelModel.LevelGroupId = newLevelGroup.Id;
            levelModel.CreationTime = null;
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
            return levelEntity;
        }

        private async Task<TimingGroup> CopyTimingGroupAsync(Dictionary<Guid, Guid> timingGroupMapping, TimingGroup old, string newProjectId)
        {
            var newTimingGroup = old.Clone();
            var timingGroupModel = newTimingGroup.MapTo<TimingGroupModel>();
            timingGroupModel.Id = Guid.NewGuid().ToString();
            timingGroupModel.ProjectInfoId = newProjectId.ToGuid();
            timingGroupModel.CreationTime = null;
            var timingGroupEntity = timingGroupModel.MapTo<TimingGroup>();
            await _timingGroupRepository.AddAsync(timingGroupEntity);

            if (!timingGroupMapping.ContainsKey(old.Id))
                timingGroupMapping.Add(old.Id, timingGroupEntity.Id);

            return timingGroupEntity;
        }

        private async Task<Timing> CopyTimingAsync(Dictionary<Guid, Guid> pinInfoMapping, Dictionary<Guid, Guid> groupInfoMapping, TimingGroup newTimingGroup, Timing old)
        {
            var newTiming = old.Clone();
            var timingModel = newTiming.MapTo<Models.TestPlans.TimingModel>();
            timingModel.Id = Guid.NewGuid().ToString();
            timingModel.TimingGroupId = newTimingGroup.Id;
            timingModel.CreationTime = null;
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
            return timingEntity;
        }

        private async Task<TestItemInfo> CopyTestItemInfoAsync(Dictionary<Guid, Guid> pinInfoMapping, Dictionary<Guid, Guid> groupInfoMapping, Dictionary<Guid, Guid> limitsMapping, Dictionary<Guid, Guid> levelGroupMapping, Dictionary<Guid, Guid> timingGroupMapping, TestItemInfo old, string newProjectId)
        {
            var newTestItemInfo = old.Clone();
            var testItemInfoModel = newTestItemInfo.MapTo<TestItemInfoModel>();
            testItemInfoModel.Id = Guid.NewGuid().ToString();
            testItemInfoModel.ProjectInfoId = newProjectId.ToGuidOrNull();
            testItemInfoModel.CreationTime = null;
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
            return testItemInfoEntity;
        }

        private async Task<GlobalParameter> CopyGlobalParameterAsync(GlobalParameter old, string newProjectId)
        {
            var newGlobalParameter = old.Clone();
            var globalParameterModel = newGlobalParameter.MapTo<GlobalParameterModel>();
            globalParameterModel.Id = Guid.NewGuid().SafeString();
            globalParameterModel.ProjectInfoId = newProjectId.ToGuidOrNull();
            globalParameterModel.CreationTime = null;
            var globalParameterEntity = globalParameterModel.MapTo<GlobalParameter>();
            return globalParameterEntity;
        }
        #endregion

        #region 删除测试计划
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
        #endregion

        #region 导入测试计划
        public async Task ImportTestPlanAsync(string filePath, string projectId)
        {
            if (projectId.IsEmpty())
                return;

            var testPlan = TestPlanHelper.LoadTestPlanFromExcel(filePath);
            if (testPlan != null)
            {
                // 删除项目已有的测试计划
                await DeleteTestPlanByProjectIdAsync(projectId);

                var groups = testPlan?.Channel?.SelectMany(x => x.Groups).DistinctBy(x => x.Name).ToList();
                var sites = testPlan?.Channel?.SelectMany(x => x.Sites).DistinctBy(x => x.SiteName).ToList();
                var limitDic = new Dictionary<string, Guid>();
                var levelGroups = testPlan?.TestItem?.Where(x => !x.Level.IsEmpty())?.Select(x => x.Level).Distinct().ToList();
                var timingGroups = testPlan?.TestItem?.Where(x => !x.Timing.IsEmpty())?.Select(x => x.Timing).Distinct().ToList();

                PinOverview pinOverview = await SavePinOverviewFromTestPlan(projectId, sites.Count);

                // 站点
                List<SiteInfo> siteList = await SaveSiteFromTestPlan(sites, pinOverview);

                // 引脚组
                List<GroupInfo> groupList = await SaveGroupFromTestPlan(groups, pinOverview);

                // 引脚
                List<PinInfo> pinList = await SavePinFromTestPlan(testPlan?.Channel, pinOverview, siteList, groupList);

                // 门限
                List<Limits> limitList = await SaveLimitsFromTestPlan(projectId, testPlan.Limits, limitDic);

                // 电平组
                List<LevelGroup> levelGroupList = await SaveLevelGroupFromTestPlan(projectId, levelGroups);

                // 时钟组
                List<TimingGroup> timingGroupList = await SaveTimingGroupFromTestPlan(projectId, timingGroups);

                // 测试项
                await SaveTestItemsFromTestPlan(projectId, testPlan, groupList, pinList, limitDic, limitList, levelGroupList, timingGroupList);

                // 全局参数
                await SaveGlobalParameterFromTestPlan(projectId, testPlan.Global);

            }
        }

        private async Task<PinOverview> SavePinOverviewFromTestPlan(string projectId, int siteCount)
        {
            var pinOverview = new PinOverview();
            pinOverview.Init();
            pinOverview.ProjectInfoId = projectId.ToGuid();
            pinOverview.SiteCount = siteCount;
            await _pinOverviewRepository.AddAsync(pinOverview);
            return pinOverview;
        }

        private async Task<List<SiteInfo>> SaveSiteFromTestPlan(List<SiteModel> sites, PinOverview pinOverview)
        {
            var siteList = new List<SiteInfo>();
            if (sites.IsEmpty())
                return siteList;

            for (int i = 0; i < sites.Count; i++)
            {
                var siteInfo = new SiteInfo();
                siteInfo.Init();
                siteInfo.PinOverviewId = pinOverview.Id;
                siteInfo.SortId = i;
                siteInfo.SiteName = sites[i].SiteName;
                siteList.Add(siteInfo);
            }
            await _siteInfoRepository.AddAsync(siteList);

            return siteList;
        }

        private async Task<List<GroupInfo>> SaveGroupFromTestPlan(List<PinGroupModel>? groups, PinOverview pinOverview)
        {
            var groupList = new List<GroupInfo>();
            if (groups.IsEmpty())
                return groupList;
            groups.Reverse();
            foreach (var group in groups)
            {
                var groupInfo = new GroupInfo();
                groupInfo.Init();
                groupInfo.PinOverviewId = pinOverview.Id;
                groupInfo.GroupName = group.Name;
                groupList.Add(groupInfo);
            }

            await _groupInfoRepository.AddAsync(groupList);

            return groupList;
        }

        private async Task<List<PinInfo>> SavePinFromTestPlan(List<ChannelModel>? channels, PinOverview pinOverview, List<SiteInfo> siteList, List<GroupInfo> groupList)
        {
            var pinList = new List<PinInfo>();
            if (channels.IsEmpty())
                return pinList;
            channels.Reverse();

            foreach (var channel in channels)
            {
                var pinInfo = new PinInfo();
                pinInfo.Init();
                pinInfo.PinOverviewId = pinOverview.Id;
                pinInfo.PinName = channel.PinName;
                if (System.Enum.TryParse(channel.Type.ToString(), out PinType type))
                    pinInfo.PinType = type;
                pinList.Add(pinInfo);

                if (!channel.Groups.IsEmpty())
                {
                    foreach (var group in channel.Groups)
                    {
                        var groupInfo = groupList.FirstOrDefault(x => x.GroupName.Equals(group.Name));
                        var pinGroup = new PinGroupRelationship();
                        pinGroup.Init();
                        pinGroup.PinInfoId = pinInfo.Id;
                        pinGroup.GroupInfoId = groupInfo.Id;
                        await _pinGroupRelationshipRepositoy.AddAsync(pinGroup);
                    }
                }

                if (!channel.Sites.IsEmpty())
                {
                    foreach (var site in channel.Sites)
                    {
                        var siteInfo = siteList.FirstOrDefault(x => x.SiteName.Equals(site.SiteName));
                        var pinSite = new PinSiteInfo();
                        pinSite.Init();
                        pinSite.SortId = channel.Sites.IndexOf(site);
                        pinSite.PinInfoId = pinInfo.Id;
                        pinSite.SiteInfoId = siteInfo.Id;
                        pinSite.ChannelName = site.SiteValue;
                        await _pinSiteInfoRepository.AddAsync(pinSite);
                    }
                }
            }
            await _pinInfoRepository.AddAsync(pinList);

            return pinList;
        }

        private async Task<List<Limits>> SaveLimitsFromTestPlan(string projectId, List<Project.Base.Models.TestPlans.LimitsModel> limits, Dictionary<string, Guid> limitDic)
        {
            var limitList = new List<Limits>();
            if (limits.IsEmpty())
                return limitList;
            limits.Reverse();
            foreach (var limit in limits)
            {
                var limitInfo = new Limits();
                limitInfo.Init();
                limitInfo.ProjectInfoId = projectId.ToGuid();
                limitInfo.TestNumber = (int)limit.TestNumber;
                limitInfo.LowLimit = System.Convert.ToDecimal(limit.LowLimit);
                limitInfo.HighLimit = System.Convert.ToDecimal(limit.HighLimit);
                limitInfo.Units = limit.Units;
                limitInfo.LimitName = limit.LimitName;
                limitInfo.FailSoftwareBin = (int)limit.FailSoftwareBin;
                limitInfo.PassSoftwareBin = (int)limit.PassSoftwareBin;
                limitInfo.FailHardwareBin = (int)limit.FailHardwareBin;
                limitInfo.PassHardwareBin = (int)limit.PassHardwareBin;
                limitInfo.DutResult = System.Enum.TryParse(limit.DUTResult.ToString(), out DUTResultType type) ? type : DUTResultType.None;

                if (!limit.TestItemName.IsEmpty() && !limitDic.ContainsKey(limit.TestItemName))
                    limitDic.Add(limit.TestItemName, limitInfo.Id);

                limitList.Add(limitInfo);
            }
            await _limitsRepository.AddAsync(limitList);

            return limitList;
        }

        private async Task<List<LevelGroup>> SaveLevelGroupFromTestPlan(string projectId, List<string>? levelGroups)
        {
            var levelGroupList = new List<LevelGroup>();
            if (levelGroups.IsEmpty())
                return levelGroupList;
            levelGroups.Reverse();
            foreach (var levelGroup in levelGroups)
            {
                var tempLevelGroup = new LevelGroup();
                tempLevelGroup.Init();
                tempLevelGroup.ProjectInfoId = projectId.ToGuid();
                tempLevelGroup.LevelGroupName = levelGroup;
                levelGroupList.Add(tempLevelGroup);
            }
            await _levelGroupRepository.AddAsync(levelGroupList);

            return levelGroupList;
        }

        private async Task<List<TimingGroup>> SaveTimingGroupFromTestPlan(string projectId, List<string>? timingGroups)
        {
            var timingGroupList = new List<TimingGroup>();
            if (timingGroups.IsEmpty())
                return timingGroupList;
            timingGroups.Reverse();
            foreach (var timingGroup in timingGroups)
            {
                var tempTimingGroup = new TimingGroup();
                tempTimingGroup.Init();
                tempTimingGroup.ProjectInfoId = projectId.ToGuid();
                tempTimingGroup.TimingGroupName = timingGroup;
                timingGroupList.Add(tempTimingGroup);
            }
            await _timingGroupRepository.AddAsync(timingGroupList);

            return timingGroupList;
        }

        private async Task SaveTestItemsFromTestPlan(string projectId, TestPlanModel testPlan, List<GroupInfo> groupList, List<PinInfo> pinList, Dictionary<string, Guid> limitDic, List<Limits> limitList, List<LevelGroup> levelGroupList, List<TimingGroup> timingGroupList)
        {
            var hasLevels = new List<string>();
            var hasTimings = new List<string>();
            var testItemList = new List<TestItemInfo>();
            var testItems = testPlan.TestItem;
            var flows = testPlan.Flow;
            var testItemDic = new Dictionary<string, Guid>();
            testItems.Reverse();
            foreach (var testItem in testItems)
            {
                var testItemInfo = new TestItemInfo();
                testItemInfo.Init();
                testItemInfo.ProjectInfoId = projectId.ToGuid();
                testItemInfo.TestItemName = testItem.TestItemName;
                testItemInfo.FunctionName = testItem.FunctionName;
                testItemInfo.Force = testItem.Force;
                testItemDic.Add(testItem.Id, testItemInfo.Id);
                var group = groupList.FirstOrDefault(x => x.GroupName.Equals(testItem.Pins));
                if (group == null)
                {
                    var pin = pinList.FirstOrDefault(x => x.PinName.Equals(testItem.Pins));
                    testItemInfo.GroupOrPinId = pin?.Id ?? Guid.Empty;
                }
                else
                    testItemInfo.GroupOrPinId = group.Id;

                // 测试项门限
                if (!limitDic.IsEmpty() && !limitList.IsEmpty() && limitDic.ContainsKey(testItem.TestItemName))
                {
                    var limit = limitList.FirstOrDefault(x => x.Id.Equals(limitDic[testItem.TestItemName]));
                    testItemInfo.LimitsId = limit?.Id ?? Guid.Empty;
                }

                // 测试项电平
                if (!testItem.Level.IsEmpty() && !testItem.Levels.IsEmpty())
                {
                    var levelGroup = levelGroupList.FirstOrDefault(x => x.LevelGroupName.Equals(testItem.Level));
                    testItemInfo.LevelGroupId = levelGroup?.Id;
                    testItem.Levels.Reverse();
                    foreach (var level in testItem.Levels)
                    {
                        if (!hasLevels.Contains(level.Id))
                        {
                            var tempLevel = new Level();
                            tempLevel.Init();
                            tempLevel.LevelGroupId = levelGroup?.Id ?? Guid.Empty;
                            var pinGroup = groupList.FirstOrDefault(x => x.GroupName.Equals(level.PinGroupName));
                            if (pinGroup == null)
                            {
                                var pin = pinList.FirstOrDefault(x => x.PinName.Equals(level.PinGroupName));
                                tempLevel.GroupOrPinId = pin?.Id ?? Guid.Empty;
                            }
                            else
                                tempLevel.GroupOrPinId = pinGroup.Id;
                            tempLevel.Vil = level.Vil;
                            tempLevel.Vih = level.Vih;
                            tempLevel.Vol = level.Vol;
                            tempLevel.Voh = level.Voh;
                            tempLevel.Iol = level.Iol;
                            tempLevel.Ioh = level.Ioh;
                            tempLevel.Vt = level.Vt;
                            tempLevel.Vcl = level.Vcl;
                            tempLevel.Vch = level.Vch;
                            tempLevel.Ps = level.PS;
                            tempLevel.I = level.I;
                            tempLevel.Tdelay = System.Convert.ToInt32(level.Tdelay);
                            tempLevel.Sequence = int.TryParse(level.Sequence, out int sequence) ? sequence : 0;
                            tempLevel.Comment = level.Comment;

                            hasLevels.Add(level.Id);
                            await _levelRepository.AddAsync(tempLevel);
                        }

                    }
                }

                // 测试项时钟
                if (!testItem.Timing.IsEmpty() && !testItem.Timings.IsEmpty())
                {
                    var timingGroup = timingGroupList.FirstOrDefault(x => x.TimingGroupName.Equals(testItem.Timing));
                    testItemInfo.TimingGroupId = timingGroup?.Id;
                    testItem.Timings.Reverse();
                    foreach (var timing in testItem.Timings)
                    {
                        if (!hasTimings.Contains(timing.Id))
                        {
                            var tempTiming = new Timing();
                            tempTiming.Init();
                            tempTiming.TimingGroupId = timingGroup?.Id ?? Guid.Empty;
                            tempTiming.TimingName = timing.TimingName;
                            tempTiming.Period = timing.Period;
                            var pinGroup = groupList.FirstOrDefault(x => x.GroupName.Equals(timing.PinName));
                            if (pinGroup == null)
                            {
                                var pin = pinList.FirstOrDefault(x => x.PinName.Equals(timing.PinName));
                                tempTiming.GroupOrPinId = pin?.Id ?? Guid.Empty;
                            }
                            else
                                tempTiming.GroupOrPinId = pinGroup.Id;
                            tempTiming.WaveformFormat = System.Enum.TryParse(timing.Fmt.ToString(), out TimingformatType fmt) ? fmt : TimingformatType.NR;
                            tempTiming.DriveA = int.TryParse(timing.DriveA?.ToString(), out int driveA) ? driveA : 0;
                            tempTiming.DriveB = int.TryParse(timing.DriveB?.ToString(), out int driveB) ? driveB : 0;
                            tempTiming.DriveC = int.TryParse(timing.DriveC?.ToString(), out int driveC) ? driveB : 0;
                            tempTiming.DriveD = int.TryParse(timing.DriveD?.ToString(), out int driveD) ? driveB : 0;
                            tempTiming.StrobeMode = System.Enum.TryParse(timing.StrobeMode.ToString(), out Domain.TestPlan.Core.Enums.StrobeModeType strobeMode) ? strobeMode : Domain.TestPlan.Core.Enums.StrobeModeType.OFF;
                            tempTiming.StrobeA = timing.StrobeA;
                            tempTiming.StrobeB = timing.StrobeB;
                            tempTiming.Comment = timing.Comment;
                            hasTimings.Add(timing.Id);
                            await _timingRepository.AddAsync(tempTiming);
                        }
                    }
                }

                if (!testItem.Args.IsEmpty())
                    testItemInfo.AdditionInfo = string.Join(",", testItem.Args.Select(x => x.ParamValue));

                testItemList.Add(testItemInfo);
            }

            var index = 0;
            foreach (var flow in flows)
            {
                var testItemId = flow.TestItemId.SafeString();
                if (testItemDic.ContainsKey(testItemId))
                {
                    var newTestItemId = testItemDic[testItemId];
                    var testItem = testItemList.FirstOrDefault(x => x.Id.Equals(newTestItemId));
                    testItem.FlowIndex = index++;
                    testItem.Enable = flow.Enable.IsEmpty();
                }
            }
            await _testItemInfoRepository.AddAsync(testItemList);
        }

        private async Task SaveGlobalParameterFromTestPlan(string projectId, List<GlobalModel> globals)
        {
            if (globals.IsEmpty())
                return;

            var globalList = new List<GlobalParameter>();
            globals.Reverse();
            foreach (var global in globals)
            {
                var globalInfo = new GlobalParameter();
                globalInfo.Init();
                globalInfo.ProjectInfoId = projectId.ToGuid();
                globalInfo.PatternFile = global.PatternFile;
                globalInfo.AdditionInfo = string.Join(",", global.Args.Select(x => x.ParamValue));
                globalList.Add(globalInfo);
            }
            await _globalParameterRepository.AddAsync(globalList);
        }
        #endregion

        #region 导出测试计划

        public async Task ExportTestPlanAsync(string filePath, string projectId)
        {
            if (projectId.IsEmpty())
                return;

            var testPlanDir = ConfigurationManager.AppSettings["TestPlanTemplateDir"] ?? throw new ArgumentNullException("TestPlanTemplateDir");
            var testPlanTemplateName = ConfigurationManager.AppSettings["TestPlanTemplateName"] ?? throw new ArgumentNullException("TestPlanTemplateName");
            var testPlanTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, testPlanDir, $"{testPlanTemplateName}.xlsx");

            if (File.Exists(filePath))
            {
                if (File.Exists(testPlanTemplatePath))
                {
                    File.Copy(testPlanTemplatePath, filePath, true);
                }
                else
                {
                    throw new FileNotFoundException(string.Format(L["FileNotFound"], testPlanTemplatePath));
                }
            }
            else
            {
                var parentDir = Path.GetDirectoryName(filePath);
                if (!parentDir.IsEmpty() && !Directory.Exists(parentDir))
                    Directory.CreateDirectory(parentDir);

                if (File.Exists(testPlanTemplatePath))
                {
                    File.Copy(testPlanTemplatePath, filePath);
                }
                else
                {
                    throw new FileNotFoundException(string.Format(L["FileNotFound"], testPlanTemplatePath));
                }
            }

            IWorkbook workbook = null;
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new XSSFWorkbook(stream); //创建一个新的工作簿

                    await SaveChannelSheet(workbook, _channelSheetName, projectId);

                    await SaveFlowSheet(workbook, _flowSheetName, projectId);

                    await SaveTestItemSheet(workbook, _testItemSheetName, projectId);

                    await SaveLimitsSheet(workbook, _limitsSheetName, projectId);

                    var levelAddCount = await SaveLevelSheets(workbook, projectId);

                    await SaveTimingSheets(workbook, projectId, levelAddCount);

                    await SaveGlobalSheets(workbook, projectId);
                }

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook?.Write(stream);
                    workbook?.Close();
                }

                return;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveChannelSheet(IWorkbook workbook, string channelSheetName, string projectId)
        {
            var pinOverviews = await _pinOverviewRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            var pinOverview = pinOverviews?.FirstOrDefault();
            var siteInfos = (await _siteInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id))).OrderBy(x => x.SortId);
            var groupInfos = (await _groupInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id))).OrderBy(x => x.CreationTime);
            var pinInfos = (await _pinInfoRepository.FindAllAsync(x => x.PinOverviewId.Equals(pinOverview.Id)))?.OrderBy(x => x.CreationTime);
            var siteInfoIds = siteInfos.Select(x => x.Id);
            var pinSiteInfos = await _pinSiteInfoRepository.FindAllAsync(x => siteInfoIds.Contains(x.SiteInfoId));
            var groupInfoIds = groupInfos.Select(x => x.Id);
            var pinGroupRelationships = await _pinGroupRelationshipRepositoy.FindAllAsync(x => groupInfoIds.Contains(x.GroupInfoId));
            try
            {
                var sheet = workbook?.GetSheet(channelSheetName);
                var rowIndex = 1;
                if (!siteInfos.IsEmpty())
                {
                    var siteCount = siteInfos.Count();
                    sheet?.GetRow(rowIndex++)?.GetCell(1).SetCellValue(siteCount);

                    var cellIndex = 3;
                    foreach (var siteInfo in siteInfos)
                    {
                        var row = sheet?.GetRow(rowIndex);
                        row?.GetCell(cellIndex++).SetCellValue(siteInfo.SiteName);
                    }
                }

                rowIndex = 3;
                if (!pinInfos.IsEmpty())
                {
                    var cellIndex = 1;
                    foreach (var pinInfo in pinInfos)
                    {
                        var row = sheet?.CreateRow(rowIndex);
                        row?.CreateCell(cellIndex++).SetCellValue(pinInfo.PinName);
                        row?.CreateCell(cellIndex++).SetCellValue(pinInfo.PinType.Description());

                        foreach (var siteInfo in siteInfos)
                        {
                            var pinSiteInfo = pinSiteInfos.FirstOrDefault(x => x.SiteInfoId.Equals(siteInfo.Id) && x.PinInfoId.Equals(pinInfo.Id));
                            row?.CreateCell(cellIndex++).SetCellValue(pinSiteInfo?.ChannelName);
                        }
                        rowIndex++;
                        cellIndex = 1;
                    }
                }

                if (!groupInfos.IsEmpty())
                {
                    var cellIndex = 0;
                    foreach (var groupInfo in groupInfos)
                    {
                        var row = sheet?.CreateRow(rowIndex);
                        row?.CreateCell(cellIndex++).SetCellValue(groupInfo.GroupName);

                        var relationships = pinGroupRelationships.Where(x => x.GroupInfoId.Equals(groupInfo.Id)).OrderBy(x => x.CreationTime);
                        if (relationships.Any())
                        {
                            var pinIds = relationships.Select(x => x.PinInfoId);
                            var pins = pinInfos.Where(x => pinIds.Contains(x.Id)).OrderBy(x => x.CreationTime);
                            var showType = true;
                            foreach (var pin in pins)
                            {
                                row?.CreateCell(cellIndex++).SetCellValue(pin.PinName);
                                if (showType)
                                {
                                    row?.CreateCell(cellIndex++).SetCellValue(pin.PinType.Description());
                                    showType = false;
                                }
                                cellIndex = 1;
                                row = sheet?.CreateRow(++rowIndex);
                            }
                        }
                        cellIndex = 0;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveFlowSheet(IWorkbook workbook, string flowSheetName, string projectId)
        {
            var testItems = (await _testItemInfoRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()))).OrderBy(x => x.FlowIndex);

            try
            {
                var sheet = workbook?.GetSheet(flowSheetName);
                var rowIndex = 2;
                var cellIndex = 0;
                foreach (var testItem in testItems)
                {
                    var row = sheet?.CreateRow(rowIndex);
                    row?.CreateCell(cellIndex++).SetCellValue(testItem.TestItemName);
                    row?.CreateCell(cellIndex++).SetCellValue(_testItemSheetName);
                    if (testItem.Enable != true)
                        row?.CreateCell(cellIndex++).SetCellValue("False");

                    cellIndex = 0;
                    rowIndex++;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveTestItemSheet(IWorkbook workbook, string testItemSheetName, string projectId)
        {
            var testItems = (await _testItemInfoRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid())))?.OrderBy(x => x.CreationTime);
            try
            {
                var sheet = workbook?.GetSheet(testItemSheetName);
                var rowIndex = 2;
                var cellIndex = 0;
                foreach (var testItem in testItems)
                {
                    var row = sheet?.CreateRow(rowIndex);
                    row?.CreateCell(cellIndex++).SetCellValue(testItem.TestItemName);
                    row?.CreateCell(cellIndex++).SetCellValue(testItem.FunctionName);
                    row?.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(testItem.Force));
                    row?.CreateCell(cellIndex++).SetCellValue(await GetPinOrGroupName(testItem.GroupOrPinId));

                    var levelGroup = await _levelGroupRepository.FindByIdAsync(testItem.LevelGroupId);
                    row?.CreateCell(cellIndex++).SetCellValue(levelGroup?.LevelGroupName);

                    var timingGroup = await _timingGroupRepository.FindByIdAsync(testItem.TimingGroupId);
                    row?.CreateCell(cellIndex++).SetCellValue(timingGroup?.TimingGroupName);

                    var args = testItem?.AdditionInfo?.Split(",");
                    if (!args.IsEmpty())
                    {
                        foreach (var arg in args)
                        {
                            row.CreateCell(cellIndex++).SetCellValue(arg);
                        }
                    }
                    cellIndex = 0;
                    rowIndex++;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<string> GetPinOrGroupName(Guid? pinOrGroupId)
        {
            var group = await _groupInfoRepository.FindByIdAsync(pinOrGroupId);
            if (group == null)
            {
                return (await _pinInfoRepository.FindByIdAsync(pinOrGroupId))?.PinName;
            }
            return group?.GroupName;
        }

        private async Task SaveLimitsSheet(IWorkbook workbook, string limitsSheetName, string projectId)
        {
            var limits = (await _limitsRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid())))?.OrderBy(x => x.CreationTime);

            try
            {
                var sheet = workbook?.GetSheet(limitsSheetName);
                var rowIndex = 2;
                var cellIndex = 0;
                foreach (var limit in limits)
                {
                    var row = sheet?.CreateRow(rowIndex);
                    var testItem = (await _testItemInfoRepository.FindAllAsync(x => x.LimitsId.Equals(limit.Id)))?.FirstOrDefault();
                    if (testItem != null)
                        row.CreateCell(cellIndex).SetCellValue(testItem.TestItemName);

                    cellIndex++;
                    row.CreateCell(cellIndex++).SetCellValue(limit.TestNumber);
                    row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(limit.LowLimit));
                    row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(limit.HighLimit));
                    row.CreateCell(cellIndex++).SetCellValue(limit.Units);
                    row.CreateCell(cellIndex++).SetCellValue(limit.LimitName);
                    row.CreateCell(cellIndex++).SetCellValue(limit.FailSoftwareBin?.ToString());
                    row.CreateCell(cellIndex++).SetCellValue(limit.PassSoftwareBin?.ToString());
                    row.CreateCell(cellIndex++).SetCellValue(limit.FailHardwareBin?.ToString());
                    row.CreateCell(cellIndex++).SetCellValue(limit.PassHardwareBin?.ToString());
                    row.CreateCell(cellIndex++).SetCellValue(limit.DutResult.Description());

                    cellIndex = 0;
                    rowIndex++;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task<int> SaveLevelSheets(IWorkbook workbook, string projectId)
        {
            var levelGroups = await _levelGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            var addCount = 0;
            try
            {
                var standSheet = workbook?.GetSheet(_levelSheetName);
                // 先拷贝Sheet
                foreach (var levelGroup in levelGroups)
                {
                    if (!levelGroup.LevelGroupName.Equals(_levelSheetName))
                    {
                        standSheet.CopySheet(levelGroup.LevelGroupName, true);
                        var index = workbook?.GetSheetIndex(_levelSheetName);
                        workbook.SetSheetOrder(levelGroup.LevelGroupName, (index ?? 0) + (++addCount));
                    }
                }

                foreach (var levelGroup in levelGroups)
                {
                    var sheet = workbook?.GetSheet(levelGroup.LevelGroupName);
                    var levels = (await _levelRepository.FindAllAsync(x => x.LevelGroupId.Equals(levelGroup.Id)))?.OrderBy(x => x.CreationTime);
                    var rowIndex = 2;
                    var cellIndex = 0;
                    if (levels.Any())
                    {
                        foreach (var level in levels)
                        {
                            var row = sheet?.CreateRow(rowIndex);

                            row.CreateCell(cellIndex++).SetCellValue(await GetPinOrGroupName(level.GroupOrPinId));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Vil));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Vih));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Vol));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Voh));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Iol));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Ioh));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Vt));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Vcl));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Vch));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Ps));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.I));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Tdelay));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Sequence));
                            row.CreateCell(cellIndex++).SetCellValue(System.Convert.ToDouble(level.Comment));

                            cellIndex = 0;
                            rowIndex++;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return addCount;
        }

        private async Task SaveTimingSheets(IWorkbook workbook, string projectId, int levelAddCount = 0)
        {
            var timingGroups = await _timingGroupRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid()));
            try
            {
                var standSheet = workbook?.GetSheet(_timingSheetName);
                // 先拷贝Sheet
                foreach (var timingGroup in timingGroups)
                {
                    if (!timingGroup.TimingGroupName.Equals(_timingSheetName))
                    {
                        standSheet.CopySheet(timingGroup.TimingGroupName, true);
                        var index = workbook?.GetSheetIndex(_timingSheetName);
                        workbook.SetSheetOrder(timingGroup.TimingGroupName, (index ?? 0) + levelAddCount);
                    }
                }

                foreach (var timingGroup in timingGroups)
                {
                    var sheet = workbook?.GetSheet(timingGroup.TimingGroupName);
                    var timings = (await _timingRepository.FindAllAsync(x => x.TimingGroupId.Equals(timingGroup.Id)))?.OrderBy(x => x.CreationTime);
                    var rowIndex = 2;
                    var cellIndex = 0;
                    if (timings.Any())
                    {
                        foreach (var timing in timings)
                        {
                            var row = sheet?.CreateRow(rowIndex);

                            row.CreateCell(cellIndex++).SetCellValue(timing.TimingName);
                            row.CreateCell(cellIndex++).SetCellValue(timing.Period?.ToString());
                            row.CreateCell(cellIndex++).SetCellValue(await GetPinOrGroupName(timing.GroupOrPinId));
                            row.CreateCell(cellIndex++).SetCellValue("PAT");
                            row.CreateCell(cellIndex++).SetCellValue(timing.WaveformFormat?.Description());
                            row.CreateCell(cellIndex++).SetCellValue(timing.DriveA?.ToString());
                            row.CreateCell(cellIndex++).SetCellValue(timing.DriveB?.ToString());
                            row.CreateCell(cellIndex++).SetCellValue(timing.DriveC?.ToString());
                            row.CreateCell(cellIndex++).SetCellValue(timing.DriveD?.ToString());
                            row.CreateCell(cellIndex++).SetCellValue(timing.StrobeMode?.Description());
                            row.CreateCell(cellIndex++).SetCellValue(timing.StrobeA?.ToString());
                            row.CreateCell(cellIndex++).SetCellValue(timing.StrobeB?.ToString());
                            row.CreateCell(cellIndex++).SetCellValue(timing.Comment);

                            cellIndex = 0;
                            rowIndex++;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async Task SaveGlobalSheets(IWorkbook workbook, string projectId)
        {
            var globals = (await _globalParameterRepository.FindAllAsync(x => x.ProjectInfoId.Equals(projectId.ToGuid())))?.OrderBy(x => x.CreationTime);
            try
            {
                var sheet = workbook?.GetSheet(_globalSheetName);
                var rowIndex = 2;
                var cellIndex = 0;
                foreach (var global in globals)
                {
                    var row = sheet?.CreateRow(rowIndex);
                    row.CreateCell(cellIndex++).SetCellValue(global.PatternFile);
                    var args = global?.AdditionInfo?.Split(",");
                    if (!args.IsEmpty())
                    {
                        foreach (var arg in args)
                            row.CreateCell(cellIndex++).SetCellValue(arg);
                    }
                    cellIndex = 0;
                    rowIndex++;
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
