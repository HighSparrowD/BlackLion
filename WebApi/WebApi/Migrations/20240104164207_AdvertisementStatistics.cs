using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdvertisementStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "advertisement_stats_hilo");

            migrationBuilder.CreateTable(
                name: "advertisement_statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    AdvertisementId = table.Column<long>(type: "bigint", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    AverageStayInSeconds = table.Column<int>(type: "integer", nullable: false),
                    Payback = table.Column<float>(type: "real", nullable: false),
                    PricePerClick = table.Column<float>(type: "real", nullable: false),
                    TotalPrice = table.Column<float>(type: "real", nullable: false),
                    Income = table.Column<float>(type: "real", nullable: false),
                    ClickCount = table.Column<float>(type: "real", nullable: false),
                    TargetAudience = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advertisement_statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_advertisement_statistics_advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_advertisement_statistics_AdvertisementId",
                table: "advertisement_statistics",
                column: "AdvertisementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "advertisement_statistics");

            migrationBuilder.DropSequence(
                name: "advertisement_stats_hilo");
        }
    }
}
