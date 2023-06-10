using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class cityLangFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cities_countries_CountryId_CountryLang",
                table: "cities");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_cities_CityId_CityLang",
                table: "user_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cities",
                table: "cities");

            migrationBuilder.DropColumn(
                name: "Lang",
                table: "cities");

            migrationBuilder.RenameColumn(
                name: "CityLang",
                table: "user_locations",
                newName: "CityCountryLang");

            migrationBuilder.RenameIndex(
                name: "IX_user_locations_CityId_CityLang",
                table: "user_locations",
                newName: "IX_user_locations_CityId_CityCountryLang");

            migrationBuilder.AlterColumn<byte>(
                name: "CountryLang",
                table: "cities",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cities",
                table: "cities",
                columns: new[] { "Id", "CountryLang" });

            migrationBuilder.AddForeignKey(
                name: "FK_cities_countries_CountryId_CountryLang",
                table: "cities",
                columns: new[] { "CountryId", "CountryLang" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "Lang" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_cities_CityId_CityCountryLang",
                table: "user_locations",
                columns: new[] { "CityId", "CityCountryLang" },
                principalTable: "cities",
                principalColumns: new[] { "Id", "CountryLang" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cities_countries_CountryId_CountryLang",
                table: "cities");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_cities_CityId_CityCountryLang",
                table: "user_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cities",
                table: "cities");

            migrationBuilder.RenameColumn(
                name: "CityCountryLang",
                table: "user_locations",
                newName: "CityLang");

            migrationBuilder.RenameIndex(
                name: "IX_user_locations_CityId_CityCountryLang",
                table: "user_locations",
                newName: "IX_user_locations_CityId_CityLang");

            migrationBuilder.AlterColumn<byte>(
                name: "CountryLang",
                table: "cities",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<byte>(
                name: "Lang",
                table: "cities",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cities",
                table: "cities",
                columns: new[] { "Id", "Lang" });

            migrationBuilder.AddForeignKey(
                name: "FK_cities_countries_CountryId_CountryLang",
                table: "cities",
                columns: new[] { "CountryId", "CountryLang" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "Lang" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_cities_CityId_CityLang",
                table: "user_locations",
                columns: new[] { "CityId", "CityLang" },
                principalTable: "cities",
                principalColumns: new[] { "Id", "Lang" });
        }
    }
}
