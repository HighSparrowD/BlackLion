using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdventureLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CityCountryLang",
                table: "adventures",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CountryLang",
                table: "adventures",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_adventures_CityId_CityCountryLang",
                table: "adventures",
                columns: new[] { "CityId", "CityCountryLang" });

            migrationBuilder.CreateIndex(
                name: "IX_adventures_CountryId_CountryLang",
                table: "adventures",
                columns: new[] { "CountryId", "CountryLang" });

            migrationBuilder.AddForeignKey(
                name: "FK_adventures_cities_CityId_CityCountryLang",
                table: "adventures",
                columns: new[] { "CityId", "CityCountryLang" },
                principalTable: "cities",
                principalColumns: new[] { "Id", "CountryLang" });

            migrationBuilder.AddForeignKey(
                name: "FK_adventures_countries_CountryId_CountryLang",
                table: "adventures",
                columns: new[] { "CountryId", "CountryLang" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "Lang" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adventures_cities_CityId_CityCountryLang",
                table: "adventures");

            migrationBuilder.DropForeignKey(
                name: "FK_adventures_countries_CountryId_CountryLang",
                table: "adventures");

            migrationBuilder.DropIndex(
                name: "IX_adventures_CityId_CityCountryLang",
                table: "adventures");

            migrationBuilder.DropIndex(
                name: "IX_adventures_CountryId_CountryLang",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "CityCountryLang",
                table: "adventures");

            migrationBuilder.DropColumn(
                name: "CountryLang",
                table: "adventures");
        }
    }
}
