using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class LocalizationCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageClassLocalisationId",
                table: "sponsor_languages");

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "sponsor_languages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<byte>(
                name: "Lang",
                table: "sponsor_languages",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lang",
                table: "sponsor_languages");

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "sponsor_languages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageClassLocalisationId",
                table: "sponsor_languages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
