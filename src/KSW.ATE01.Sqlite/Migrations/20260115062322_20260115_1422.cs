using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20260115_1422 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "StrobeB",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "选通结束时间",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "选通结束时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "StrobeA",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "选通开始时间",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "选通开始时间");

            migrationBuilder.AlterColumn<decimal>(
                name: "Period",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "周期",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "周期");

            migrationBuilder.AlterColumn<decimal>(
                name: "DriveD",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "关闭边缘",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "关闭边缘");

            migrationBuilder.AlterColumn<decimal>(
                name: "DriveC",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "返回边缘",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "返回边缘");

            migrationBuilder.AlterColumn<decimal>(
                name: "DriveB",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "起始边缘",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "起始边缘");

            migrationBuilder.AlterColumn<decimal>(
                name: "DriveA",
                table: "Timing",
                type: "TEXT",
                nullable: true,
                comment: "环绕边缘",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldComment: "环绕边缘");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StrobeB",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "选通结束时间",
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "选通结束时间");

            migrationBuilder.AlterColumn<int>(
                name: "StrobeA",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "选通开始时间",
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "选通开始时间");

            migrationBuilder.AlterColumn<int>(
                name: "Period",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "周期",
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "周期");

            migrationBuilder.AlterColumn<int>(
                name: "DriveD",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "关闭边缘",
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "关闭边缘");

            migrationBuilder.AlterColumn<int>(
                name: "DriveC",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "返回边缘",
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "返回边缘");

            migrationBuilder.AlterColumn<int>(
                name: "DriveB",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "起始边缘",
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "起始边缘");

            migrationBuilder.AlterColumn<int>(
                name: "DriveA",
                table: "Timing",
                type: "INTEGER",
                nullable: true,
                comment: "环绕边缘",
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true,
                oldComment: "环绕边缘");
        }
    }
}
