using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250821_1417 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "注释",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldComment: "注释");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Timing",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                comment: "注释",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "注释");
        }
    }
}
