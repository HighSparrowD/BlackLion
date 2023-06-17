using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UserDeleteDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityCountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.DropColumn(
                name: "CountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "users");

            migrationBuilder.AddColumn<byte>(
                name: "CityCountryClassLocalisationId",
                table: "user_locations",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CountryClassLocalisationId",
                table: "user_locations",
                type: "smallint",
                nullable: true);
        }
    }
}
