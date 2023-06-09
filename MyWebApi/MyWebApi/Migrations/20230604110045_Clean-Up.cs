using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class CleanUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cities_countries_CountryId_CountryClassLocalisationId",
                table: "cities");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsor_languages_languages_LanguageClassLocalisationId_Lan~",
                table: "sponsor_languages");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_cities_CityId_CityCountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_countries_CountryId_CountryClassLocalisation~",
                table: "user_locations");

            migrationBuilder.DropTable(
                name: "UpdateCountry");

            migrationBuilder.DropTable(
                name: "UserReason");

            migrationBuilder.DropIndex(
                name: "IX_user_locations_CityId_CityCountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.DropIndex(
                name: "IX_user_locations_CountryId_CountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.DropIndex(
                name: "IX_sponsor_languages_LanguageClassLocalisationId_LanguageId",
                table: "sponsor_languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_languages",
                table: "languages");

            migrationBuilder.DropIndex(
                name: "IX_cities_CountryId_CountryClassLocalisationId",
                table: "cities");

            migrationBuilder.DropColumn(
                name: "ClassLocalisationId",
                table: "languages");

            migrationBuilder.RenameColumn(
                name: "ClassLocalisationId",
                table: "countries",
                newName: "Lang");

            migrationBuilder.RenameColumn(
                name: "CountryClassLocalisationId",
                table: "cities",
                newName: "Lang");

            migrationBuilder.AddColumn<byte>(
                name: "CityLang",
                table: "user_locations",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CountryLang",
                table: "user_locations",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "LanguageLang",
                table: "sponsor_languages",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Lang",
                table: "languages",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "CountryLang",
                table: "cities",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_languages",
                table: "languages",
                columns: new[] { "Id", "Lang" });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CityId_CityLang",
                table: "user_locations",
                columns: new[] { "CityId", "CityLang" });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CountryId_CountryLang",
                table: "user_locations",
                columns: new[] { "CountryId", "CountryLang" });

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_languages_LanguageId_LanguageLang",
                table: "sponsor_languages",
                columns: new[] { "LanguageId", "LanguageLang" });

            migrationBuilder.CreateIndex(
                name: "IX_cities_CountryId_CountryLang",
                table: "cities",
                columns: new[] { "CountryId", "CountryLang" });

            migrationBuilder.AddForeignKey(
                name: "FK_cities_countries_CountryId_CountryLang",
                table: "cities",
                columns: new[] { "CountryId", "CountryLang" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "Lang" });

            migrationBuilder.AddForeignKey(
                name: "FK_sponsor_languages_languages_LanguageId_LanguageLang",
                table: "sponsor_languages",
                columns: new[] { "LanguageId", "LanguageLang" },
                principalTable: "languages",
                principalColumns: new[] { "Id", "Lang" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_cities_CityId_CityLang",
                table: "user_locations",
                columns: new[] { "CityId", "CityLang" },
                principalTable: "cities",
                principalColumns: new[] { "Id", "Lang" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_countries_CountryId_CountryLang",
                table: "user_locations",
                columns: new[] { "CountryId", "CountryLang" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "Lang" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cities_countries_CountryId_CountryLang",
                table: "cities");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsor_languages_languages_LanguageId_LanguageLang",
                table: "sponsor_languages");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_cities_CityId_CityLang",
                table: "user_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_countries_CountryId_CountryLang",
                table: "user_locations");

            migrationBuilder.DropIndex(
                name: "IX_user_locations_CityId_CityLang",
                table: "user_locations");

            migrationBuilder.DropIndex(
                name: "IX_user_locations_CountryId_CountryLang",
                table: "user_locations");

            migrationBuilder.DropIndex(
                name: "IX_sponsor_languages_LanguageId_LanguageLang",
                table: "sponsor_languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_languages",
                table: "languages");

            migrationBuilder.DropIndex(
                name: "IX_cities_CountryId_CountryLang",
                table: "cities");

            migrationBuilder.DropColumn(
                name: "CityLang",
                table: "user_locations");

            migrationBuilder.DropColumn(
                name: "CountryLang",
                table: "user_locations");

            migrationBuilder.DropColumn(
                name: "LanguageLang",
                table: "sponsor_languages");

            migrationBuilder.DropColumn(
                name: "Lang",
                table: "languages");

            migrationBuilder.DropColumn(
                name: "CountryLang",
                table: "cities");

            migrationBuilder.RenameColumn(
                name: "Lang",
                table: "countries",
                newName: "ClassLocalisationId");

            migrationBuilder.RenameColumn(
                name: "Lang",
                table: "cities",
                newName: "CountryClassLocalisationId");

            migrationBuilder.AddColumn<int>(
                name: "ClassLocalisationId",
                table: "languages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_languages",
                table: "languages",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.CreateTable(
                name: "UpdateCountry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalizationId = table.Column<int>(type: "integer", nullable: true),
                    CountryName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateCountry", x => new { x.Id, x.ClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_UpdateCountry_class_localizations_ClassLocalizationId",
                        column: x => x.ClassLocalizationId,
                        principalTable: "class_localizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserReason",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    ReasonName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReason", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CityId_CityCountryClassLocalisationId",
                table: "user_locations",
                columns: new[] { "CityId", "CityCountryClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CountryId_CountryClassLocalisationId",
                table: "user_locations",
                columns: new[] { "CountryId", "CountryClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_languages_LanguageClassLocalisationId_LanguageId",
                table: "sponsor_languages",
                columns: new[] { "LanguageClassLocalisationId", "LanguageId" });

            migrationBuilder.CreateIndex(
                name: "IX_cities_CountryId_CountryClassLocalisationId",
                table: "cities",
                columns: new[] { "CountryId", "CountryClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_UpdateCountry_ClassLocalizationId",
                table: "UpdateCountry",
                column: "ClassLocalizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_cities_countries_CountryId_CountryClassLocalisationId",
                table: "cities",
                columns: new[] { "CountryId", "CountryClassLocalisationId" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sponsor_languages_languages_LanguageClassLocalisationId_Lan~",
                table: "sponsor_languages",
                columns: new[] { "LanguageClassLocalisationId", "LanguageId" },
                principalTable: "languages",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_cities_CityId_CityCountryClassLocalisationId",
                table: "user_locations",
                columns: new[] { "CityId", "CityCountryClassLocalisationId" },
                principalTable: "cities",
                principalColumns: new[] { "Id", "CountryClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_countries_CountryId_CountryClassLocalisation~",
                table: "user_locations",
                columns: new[] { "CountryId", "CountryClassLocalisationId" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "ClassLocalisationId" });
        }
    }
}
