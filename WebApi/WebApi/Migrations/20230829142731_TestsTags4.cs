using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TestsTags4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_tags",
                table: "user_tags");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "user_tags");

            migrationBuilder.DropSequence(
                name: "user_tags_hilo");

            migrationBuilder.AddColumn<long>(
                name: "TagId",
                table: "user_tags",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_tags",
                table: "user_tags",
                columns: new[] { "Id", "UserId", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_user_tags_TagId_TagType",
                table: "user_tags",
                columns: new[] { "TagId", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_user_tags_UserId",
                table: "user_tags",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_tags_tags_TagId_TagType",
                table: "user_tags",
                columns: new[] { "TagId", "TagType" },
                principalTable: "tags",
                principalColumns: new[] { "Id", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_tags_tags_TagId_TagType",
                table: "user_tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_tags",
                table: "user_tags");

            migrationBuilder.DropIndex(
                name: "IX_user_tags_TagId_TagType",
                table: "user_tags");

            migrationBuilder.DropIndex(
                name: "IX_user_tags_UserId",
                table: "user_tags");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "user_tags");

            migrationBuilder.CreateSequence<int>(
                name: "user_tags_hilo");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "user_tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_tags",
                table: "user_tags",
                columns: new[] { "UserId", "Tag" });
        }
    }
}
