using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250827_1537 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "TimingGroup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "Timing",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "TestItemInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "PinInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "Limits",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "LevelGroup",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "Level",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "GroupInfo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                comment: "排序Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortId",
                table: "TimingGroup");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "Timing");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "TestItemInfo");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "PinInfo");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "Limits");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "LevelGroup");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "Level");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "GroupInfo");
        }
    }
}
