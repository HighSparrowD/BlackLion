using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TestScales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "test_answers_hilo");

            migrationBuilder.CreateSequence(
                name: "test_questions_hilo");

            migrationBuilder.CreateSequence(
                name: "test_results_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "test_scales_hilo");

            migrationBuilder.CreateSequence(
                name: "tests_hilo");

            migrationBuilder.AlterColumn<int>(
                name: "TestType",
                table: "user_tests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "tests_results",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "tests_questions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Scale",
                table: "tests_questions",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "tests_answers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "TestType",
                table: "tests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateTable(
                name: "tests_scales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    Scale = table.Column<string>(type: "text", nullable: true),
                    MinValue = table.Column<int>(type: "integer", nullable: false),
                    TestLanguage = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests_scales", x => new { x.Id, x.TestId });
                    table.ForeignKey(
                        name: "FK_tests_scales_tests_TestId_TestLanguage",
                        columns: x => new { x.TestId, x.TestLanguage },
                        principalTable: "tests",
                        principalColumns: new[] { "Id", "Language" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_tests_scales_TestId_TestLanguage",
                table: "tests_scales",
                columns: new[] { "TestId", "TestLanguage" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tests_scales");

            migrationBuilder.DropColumn(
                name: "Scale",
                table: "tests_questions");

            migrationBuilder.DropSequence(
                name: "test_answers_hilo");

            migrationBuilder.DropSequence(
                name: "test_questions_hilo");

            migrationBuilder.DropSequence(
                name: "test_results_hilo");

            migrationBuilder.DropSequence(
                name: "test_scales_hilo");

            migrationBuilder.DropSequence(
                name: "tests_hilo");

            migrationBuilder.AlterColumn<short>(
                name: "TestType",
                table: "user_tests",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "tests_results",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "tests_questions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "tests_answers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<short>(
                name: "TestType",
                table: "tests",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
