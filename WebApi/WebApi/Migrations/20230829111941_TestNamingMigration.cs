using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TestNamingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tests_results_tests_TestId_TestLanguage",
                table: "tests_results");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "tests_results");

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "tests_results",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tests_results_tests_TestId_TestLanguage",
                table: "tests_results",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tests_results_tests_TestId_TestLanguage",
                table: "tests_results");

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "tests_results",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "tests_results",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddForeignKey(
                name: "FK_tests_results_tests_TestId_TestLanguage",
                table: "tests_results",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" });
        }
    }
}
