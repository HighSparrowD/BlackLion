using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TestLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tests_questions_tests_TestId_TestLanguage",
                table: "tests_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_tests_scales_tests_TestId_TestLanguage",
                table: "tests_scales");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "tests_questions");

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "tests_scales",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "tests_questions",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tests_questions_tests_TestId_TestLanguage",
                table: "tests_questions",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tests_scales_tests_TestId_TestLanguage",
                table: "tests_scales",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tests_questions_tests_TestId_TestLanguage",
                table: "tests_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_tests_scales_tests_TestId_TestLanguage",
                table: "tests_scales");

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "tests_scales",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<byte>(
                name: "TestLanguage",
                table: "tests_questions",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "tests_questions",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddForeignKey(
                name: "FK_tests_questions_tests_TestId_TestLanguage",
                table: "tests_questions",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" });

            migrationBuilder.AddForeignKey(
                name: "FK_tests_scales_tests_TestId_TestLanguage",
                table: "tests_scales",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" });
        }
    }
}
