using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250829_1347 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enable",
                table: "TestItemInfo",
                type: "INTEGER",
                nullable: true,
                comment: "是否启用");

            migrationBuilder.AddColumn<int>(
                name: "FlowIndex",
                table: "TestItemInfo",
                type: "INTEGER",
                nullable: true,
                comment: "流程序号");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enable",
                table: "TestItemInfo");

            migrationBuilder.DropColumn(
                name: "FlowIndex",
                table: "TestItemInfo");
        }
    }
}
