using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KSW.ATE01.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class _20250815_1405 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "TimingGroup",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "Timing",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "TestItemInfo",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "SiteInfo",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "ProjectInfo",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "PinOverview",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "PinInfo",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "PinGroupRelationship",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "Limits",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "LevelGroup",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "GroupInfo",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "GlobalParameter",
                newName: "CreationTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "TimingGroup",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Timing",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "TestItemInfo",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "SiteInfo",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "ProjectInfo",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "PinOverview",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "PinInfo",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "PinGroupRelationship",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Limits",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "LevelGroup",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "GroupInfo",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "GlobalParameter",
                newName: "CreateTime");
        }
    }
}
