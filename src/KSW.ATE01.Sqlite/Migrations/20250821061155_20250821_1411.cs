using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250821_1411 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Timing",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                comment: "注释");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Timing");
        }
    }
}
