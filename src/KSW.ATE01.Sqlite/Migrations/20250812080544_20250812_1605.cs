using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250812_1605 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StrobeMode",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "选通模式",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComment: "选通模式");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionInfo",
                table: "TestItemInfo",
                type: "TEXT",
                nullable: true,
                comment: "附加信息",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldComment: "附加信息");

            migrationBuilder.AlterColumn<bool>(
                name: "StopOnFail",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "失败时停止",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldComment: "失败时停止");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveSummary",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "记录Summary",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldComment: "记录Summary");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveStdf",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "记录STDF",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldComment: "记录STDF");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveRealTimeText",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "记录RealTimeTxT",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldComment: "记录RealTimeTxT");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveCsv",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "记录CSV",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldComment: "记录CSV");

            migrationBuilder.AlterColumn<string>(
                name: "ReleasePath",
                table: "ProjectInfo",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                comment: "发布路径",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldComment: "发布路径");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectVersion",
                table: "ProjectInfo",
                type: "TEXT",
                maxLength: 200,
                nullable: true,
                comment: "项目版本",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldComment: "项目版本");

            migrationBuilder.AlterColumn<int>(
                name: "LoopCount",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "循环次数",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComment: "循环次数");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModificationTime",
                table: "ProjectInfo",
                type: "TEXT",
                nullable: true,
                comment: "最后修改时间",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldComment: "最后修改时间");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrintTime",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "是否打印时间",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldComment: "是否打印时间");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDoAll",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "是否DoAll",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldComment: "是否DoAll");

            migrationBuilder.AlterColumn<int>(
                name: "DelayBetweenLoops",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: true,
                comment: "循环间时延",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldComment: "循环间时延");

            migrationBuilder.AlterColumn<string>(
                name: "DatalogPath",
                table: "ProjectInfo",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                comment: "日志路径",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldComment: "日志路径");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "ProjectInfo",
                type: "TEXT",
                nullable: true,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<string>(
                name: "Units",
                table: "Limits",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                comment: "单位",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldComment: "单位");

            migrationBuilder.AlterColumn<string>(
                name: "LimitName",
                table: "Limits",
                type: "TEXT",
                nullable: true,
                comment: "门限名称",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldComment: "门限名称");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Level",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                comment: "注释",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldComment: "注释");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionInfo",
                table: "GlobalParameter",
                type: "TEXT",
                nullable: true,
                comment: "附加信息",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldComment: "附加信息");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StrobeMode",
                table: "Timing",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "选通模式",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "选通模式");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionInfo",
                table: "TestItemInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                comment: "附加信息",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "附加信息");

            migrationBuilder.AlterColumn<bool>(
                name: "StopOnFail",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                comment: "失败时停止",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "失败时停止");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveSummary",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                comment: "记录Summary",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "记录Summary");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveStdf",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                comment: "记录STDF",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "记录STDF");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveRealTimeText",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                comment: "记录RealTimeTxT",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "记录RealTimeTxT");

            migrationBuilder.AlterColumn<bool>(
                name: "SaveCsv",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                comment: "记录CSV",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "记录CSV");

            migrationBuilder.AlterColumn<string>(
                name: "ReleasePath",
                table: "ProjectInfo",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                comment: "发布路径",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "发布路径");

            migrationBuilder.AlterColumn<string>(
                name: "ProjectVersion",
                table: "ProjectInfo",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                comment: "项目版本",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "项目版本");

            migrationBuilder.AlterColumn<int>(
                name: "LoopCount",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "循环次数",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "循环次数");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModificationTime",
                table: "ProjectInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "最后修改时间",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "最后修改时间");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPrintTime",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                comment: "是否打印时间",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "是否打印时间");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDoAll",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                comment: "是否DoAll",
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "是否DoAll");

            migrationBuilder.AlterColumn<int>(
                name: "DelayBetweenLoops",
                table: "ProjectInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "循环间时延",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "循环间时延");

            migrationBuilder.AlterColumn<string>(
                name: "DatalogPath",
                table: "ProjectInfo",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                comment: "日志路径",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "日志路径");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateTime",
                table: "ProjectInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<string>(
                name: "Units",
                table: "Limits",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                comment: "单位",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "单位");

            migrationBuilder.AlterColumn<string>(
                name: "LimitName",
                table: "Limits",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                comment: "门限名称",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "门限名称");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Level",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                comment: "注释",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "注释");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionInfo",
                table: "GlobalParameter",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                comment: "附加信息",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "附加信息");
        }
    }
}
