using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdvertisementStatisticsEngagemenEconomy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClickCount",
                table: "advertisement_statistics",
                newName: "LinkClickCount");

            migrationBuilder.AddColumn<float>(
                name: "PeoplePercentage",
                table: "advertisement_statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeoplePercentage",
                table: "advertisement_statistics");

            migrationBuilder.RenameColumn(
                name: "LinkClickCount",
                table: "advertisement_statistics",
                newName: "ClickCount");
        }
    }
}
