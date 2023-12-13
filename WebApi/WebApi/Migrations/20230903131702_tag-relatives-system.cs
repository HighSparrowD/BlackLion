using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class tagrelativessystem : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
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
