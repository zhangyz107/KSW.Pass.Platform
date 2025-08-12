using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250812_1045 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalParameter",
                columns: table => new
                {
                    GlobalParameterId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "全局参数Id"),
                    ProjectInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "项目信息Id"),
                    PatternFile = table.Column<string>(type: "TEXT", nullable: false, comment: "向量文件名称"),
                    AdditionInfo = table.Column<string>(type: "TEXT", nullable: false, comment: "附加信息"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalParameter", x => x.GlobalParameterId);
                },
                comment: "全局参数");

            migrationBuilder.CreateTable(
                name: "GroupInfo",
                columns: table => new
                {
                    GroupInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "组信息Id"),
                    PinOverviewId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚总览Id"),
                    GroupName = table.Column<string>(type: "TEXT", nullable: false, comment: "组名称"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInfo", x => x.GroupInfoId);
                },
                comment: "组信息");

            migrationBuilder.CreateTable(
                name: "Level",
                columns: table => new
                {
                    LevelId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项电平Id"),
                    LevelGroupId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项电平组Id"),
                    GroupOrPinId = table.Column<Guid>(type: "TEXT", nullable: true, comment: "组或引脚Id"),
                    Vil = table.Column<decimal>(type: "TEXT", nullable: true, comment: "输入低电压"),
                    Vih = table.Column<decimal>(type: "TEXT", nullable: true, comment: "输入高电压"),
                    Vol = table.Column<decimal>(type: "TEXT", nullable: true, comment: "输出低电压"),
                    Voh = table.Column<decimal>(type: "TEXT", nullable: true, comment: "输出高电压"),
                    Iol = table.Column<decimal>(type: "TEXT", nullable: true, comment: "低电平输出灌电流"),
                    Ioh = table.Column<decimal>(type: "TEXT", nullable: true, comment: "高电平输出拉电流"),
                    Vt = table.Column<decimal>(type: "TEXT", nullable: true, comment: "电压基准"),
                    Vcl = table.Column<decimal>(type: "TEXT", nullable: true, comment: "错位低电压"),
                    Vch = table.Column<decimal>(type: "TEXT", nullable: true, comment: "错位高电压"),
                    Ps = table.Column<decimal>(type: "TEXT", nullable: true, comment: "供电电压"),
                    I = table.Column<decimal>(type: "TEXT", nullable: true, comment: "电流限制"),
                    Tdelay = table.Column<int>(type: "INTEGER", nullable: true, comment: "上电延迟时间"),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: true, comment: "上电顺序"),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, comment: "注释"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.LevelId);
                },
                comment: "测试项电平");

            migrationBuilder.CreateTable(
                name: "LevelGroup",
                columns: table => new
                {
                    LevelGroupId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项电平组Id"),
                    ProjectInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "项目信息Id"),
                    LevelGroupName = table.Column<string>(type: "TEXT", nullable: false, comment: "电平组名"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelGroup", x => x.LevelGroupId);
                },
                comment: "测试项电平组");

            migrationBuilder.CreateTable(
                name: "Limits",
                columns: table => new
                {
                    LimitsId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项门限Id"),
                    TestItemInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项信息Id"),
                    TestNumber = table.Column<int>(type: "INTEGER", nullable: false, comment: "测试编号"),
                    LowLimit = table.Column<decimal>(type: "TEXT", nullable: true, comment: "电压下限"),
                    HighLimit = table.Column<decimal>(type: "TEXT", nullable: true, comment: "电压上限"),
                    Units = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "单位"),
                    LimitName = table.Column<string>(type: "TEXT", nullable: false, comment: "门限名称"),
                    FailSoftwareBin = table.Column<int>(type: "INTEGER", nullable: true, comment: "软件失效分档"),
                    PassSoftwareBin = table.Column<int>(type: "INTEGER", nullable: true, comment: "软件成功分档"),
                    FailHardwareBin = table.Column<int>(type: "INTEGER", nullable: true, comment: "硬件失效分档"),
                    PassHardwareBin = table.Column<int>(type: "INTEGER", nullable: true, comment: "硬件成功分档"),
                    DutResult = table.Column<int>(type: "INTEGER", nullable: true, comment: "被测物结果"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limits", x => x.LimitsId);
                },
                comment: "测试项门限");

            migrationBuilder.CreateTable(
                name: "PinGroupRelationship",
                columns: table => new
                {
                    PinGroupRelationshipId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚与组关系Id"),
                    PinInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚信息Id"),
                    GroupInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "组信息Id"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinGroupRelationship", x => x.PinGroupRelationshipId);
                },
                comment: "引脚与组关系");

            migrationBuilder.CreateTable(
                name: "PinInfo",
                columns: table => new
                {
                    PinInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚信息Id"),
                    PinOverviewId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚总览Id"),
                    PinName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "引脚名称"),
                    PinType = table.Column<int>(type: "INTEGER", nullable: true, comment: "引脚类型"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinInfo", x => x.PinInfoId);
                },
                comment: "项目信息");

            migrationBuilder.CreateTable(
                name: "PinOverview",
                columns: table => new
                {
                    PinOverview = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚总览Id"),
                    ProjectInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "项目信息Id"),
                    SiteCount = table.Column<int>(type: "INTEGER", nullable: true, comment: "站点数"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinOverview", x => x.PinOverview);
                },
                comment: "引脚总览");

            migrationBuilder.CreateTable(
                name: "PinSiteInfo",
                columns: table => new
                {
                    PinSiteInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚站点信息Id"),
                    SiteInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "站点信息Id"),
                    PinInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚信息Id"),
                    ChannelName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "通道名称"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PinSiteInfo", x => x.PinSiteInfoId);
                },
                comment: "引脚站点信息");

            migrationBuilder.CreateTable(
                name: "ProjectInfo",
                columns: table => new
                {
                    ProjectInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "项目信息Id"),
                    ProjectName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "项目名称"),
                    ProjectPath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "项目路径"),
                    ProjectVersion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "项目版本"),
                    SaveRealTimeText = table.Column<bool>(type: "INTEGER", nullable: false, comment: "记录RealTimeTxT"),
                    SaveCsv = table.Column<bool>(type: "INTEGER", nullable: false, comment: "记录CSV"),
                    SaveSummary = table.Column<bool>(type: "INTEGER", nullable: false, comment: "记录Summary"),
                    SaveStdf = table.Column<bool>(type: "INTEGER", nullable: false, comment: "记录STDF"),
                    DatalogPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, comment: "日志路径"),
                    IsDoAll = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否DoAll"),
                    IsPrintTime = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否打印时间"),
                    LoopCount = table.Column<int>(type: "INTEGER", nullable: false, comment: "循环次数"),
                    DelayBetweenLoops = table.Column<int>(type: "INTEGER", nullable: false, comment: "循环间时延"),
                    StopOnFail = table.Column<bool>(type: "INTEGER", nullable: false, comment: "失败时停止"),
                    ReleasePath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, comment: "发布路径"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: false, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectInfo", x => x.ProjectInfoId);
                },
                comment: "项目信息");

            migrationBuilder.CreateTable(
                name: "SiteInfo",
                columns: table => new
                {
                    SiteInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "站点信息Id"),
                    PinOverviewId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "引脚总览Id"),
                    SortId = table.Column<int>(type: "INTEGER", nullable: false, comment: "序号"),
                    SiteName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "站点名称"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteInfo", x => x.SiteInfoId);
                },
                comment: "站点信息");

            migrationBuilder.CreateTable(
                name: "TestItemInfo",
                columns: table => new
                {
                    TestItemInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项信息Id"),
                    ProjectInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "项目信息Id"),
                    TestItemName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "测试项名称"),
                    FunctionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "方法名称"),
                    Force = table.Column<decimal>(type: "TEXT", nullable: false, comment: "激励"),
                    GroupOrPinId = table.Column<Guid>(type: "TEXT", nullable: true, comment: "组或引脚Id"),
                    LimitsId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项门限Id"),
                    LevelGroupId = table.Column<Guid>(type: "TEXT", nullable: true, comment: "测试项电平组Id"),
                    TimingGroupId = table.Column<Guid>(type: "TEXT", nullable: true, comment: "测试项时钟组Id"),
                    AdditionInfo = table.Column<string>(type: "TEXT", nullable: false, comment: "附加信息"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestItemInfo", x => x.TestItemInfoId);
                },
                comment: "测试项信息");

            migrationBuilder.CreateTable(
                name: "Timing",
                columns: table => new
                {
                    TimingId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项时钟Id"),
                    TimingGroupId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项时钟组Id"),
                    TimingName = table.Column<string>(type: "TEXT", nullable: false, comment: "时钟名称"),
                    Period = table.Column<int>(type: "INTEGER", nullable: true, comment: "周期"),
                    GroupOrPinId = table.Column<Guid>(type: "TEXT", nullable: true, comment: "组或引脚Id"),
                    WaveformFormat = table.Column<int>(type: "INTEGER", nullable: true, comment: "波形格式"),
                    DriveA = table.Column<int>(type: "INTEGER", nullable: true, comment: "环绕边缘"),
                    DriveB = table.Column<int>(type: "INTEGER", nullable: true, comment: "起始边缘"),
                    DriveC = table.Column<int>(type: "INTEGER", nullable: true, comment: "返回边缘"),
                    DriveD = table.Column<int>(type: "INTEGER", nullable: true, comment: "关闭边缘"),
                    StrobeMode = table.Column<int>(type: "INTEGER", nullable: false, comment: "选通模式"),
                    StrobeA = table.Column<int>(type: "INTEGER", nullable: true, comment: "选通开始时间"),
                    StrobeB = table.Column<int>(type: "INTEGER", nullable: true, comment: "选通结束时间"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timing", x => x.TimingId);
                },
                comment: "测试项时钟");

            migrationBuilder.CreateTable(
                name: "TimingGroup",
                columns: table => new
                {
                    TimingGroupId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "测试项时钟组Id"),
                    ProjectInfoId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "项目信息Id"),
                    TimingGroupName = table.Column<string>(type: "TEXT", nullable: false, comment: "时钟组名"),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "创建时间"),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true, comment: "最后修改时间"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否删除"),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false, comment: "版本号")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimingGroup", x => x.TimingGroupId);
                },
                comment: "测试项时钟组");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalParameter");

            migrationBuilder.DropTable(
                name: "GroupInfo");

            migrationBuilder.DropTable(
                name: "Level");

            migrationBuilder.DropTable(
                name: "LevelGroup");

            migrationBuilder.DropTable(
                name: "Limits");

            migrationBuilder.DropTable(
                name: "PinGroupRelationship");

            migrationBuilder.DropTable(
                name: "PinInfo");

            migrationBuilder.DropTable(
                name: "PinOverview");

            migrationBuilder.DropTable(
                name: "PinSiteInfo");

            migrationBuilder.DropTable(
                name: "ProjectInfo");

            migrationBuilder.DropTable(
                name: "SiteInfo");

            migrationBuilder.DropTable(
                name: "TestItemInfo");

            migrationBuilder.DropTable(
                name: "Timing");

            migrationBuilder.DropTable(
                name: "TimingGroup");
        }
    }
}
