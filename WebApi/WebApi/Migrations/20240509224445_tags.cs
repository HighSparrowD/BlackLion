using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class tags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adventure_tags",
                columns: table => new
                {
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    AdventureId = table.Column<long>(type: "bigint", nullable: false),
                    TagType = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adventure_tags", x => new { x.TagId, x.AdventureId, x.TagType });
                    table.ForeignKey(
                        name: "FK_adventure_tags_tags_TagId_TagType",
                        columns: x => new { x.TagId, x.TagType },
                        principalTable: "tags",
                        principalColumns: new[] { "Id", "Type" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "advertisement_tags",
                columns: table => new
                {
                    TagId = table.Column<long>(type: "bigint", nullable: false),
                    AdvertisementId = table.Column<long>(type: "bigint", nullable: false),
                    TagType = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advertisement_tags", x => new { x.TagId, x.AdvertisementId, x.TagType });
                    table.ForeignKey(
                        name: "FK_advertisement_tags_advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_advertisement_tags_tags_TagId_TagType",
                        columns: x => new { x.TagId, x.TagType },
                        principalTable: "tags",
                        principalColumns: new[] { "Id", "Type" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_adventure_tags_TagId_TagType",
                table: "adventure_tags",
                columns: new[] { "TagId", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_advertisement_tags_AdvertisementId",
                table: "advertisement_tags",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_advertisement_tags_TagId_TagType",
                table: "advertisement_tags",
                columns: new[] { "TagId", "TagType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adventure_tags");

            migrationBuilder.DropTable(
                name: "advertisement_tags");
        }
    }
}
