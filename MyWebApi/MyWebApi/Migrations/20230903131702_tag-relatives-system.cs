using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class tagrelativessystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FatherId",
                table: "tags",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FatherType",
                table: "tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MotherId",
                table: "tags",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MotherType",
                table: "tags",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tags_FatherId_FatherType",
                table: "tags",
                columns: new[] { "FatherId", "FatherType" });

            migrationBuilder.CreateIndex(
                name: "IX_tags_MotherId_MotherType",
                table: "tags",
                columns: new[] { "MotherId", "MotherType" });

            migrationBuilder.AddForeignKey(
                name: "FK_tags_tags_FatherId_FatherType",
                table: "tags",
                columns: new[] { "FatherId", "FatherType" },
                principalTable: "tags",
                principalColumns: new[] { "Id", "Type" });

            migrationBuilder.AddForeignKey(
                name: "FK_tags_tags_MotherId_MotherType",
                table: "tags",
                columns: new[] { "MotherId", "MotherType" },
                principalTable: "tags",
                principalColumns: new[] { "Id", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tags_tags_FatherId_FatherType",
                table: "tags");

            migrationBuilder.DropForeignKey(
                name: "FK_tags_tags_MotherId_MotherType",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "IX_tags_FatherId_FatherType",
                table: "tags");

            migrationBuilder.DropIndex(
                name: "IX_tags_MotherId_MotherType",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "FatherId",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "FatherType",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "MotherId",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "MotherType",
                table: "tags");
        }
    }
}
