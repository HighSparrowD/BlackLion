using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdvertisementAdventureVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AdminId",
                table: "advertisements",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AdminId",
                table: "adventures",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "advertisements");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "adventures");
        }
    }
}
