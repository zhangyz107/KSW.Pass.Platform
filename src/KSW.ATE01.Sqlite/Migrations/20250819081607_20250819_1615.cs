using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250819_1615 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestItemInfoId",
                table: "Limits");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectInfoId",
                table: "Limits",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "项目信息Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectInfoId",
                table: "Limits");

            migrationBuilder.AddColumn<Guid>(
                name: "TestItemInfoId",
                table: "Limits",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "测试项信息Id");
        }
    }
}
