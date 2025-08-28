using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250827_1602 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "GlobalParameter",
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
                table: "GlobalParameter");
        }
    }
}
