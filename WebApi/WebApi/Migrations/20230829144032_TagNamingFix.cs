using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class TagNamingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_tags_tags_TagId_TagType",
                table: "user_tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_tags",
                table: "user_tags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "user_tags");

            migrationBuilder.AlterColumn<long>(
                name: "TagId",
                table: "user_tags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_tags",
                table: "user_tags",
                columns: new[] { "TagId", "UserId", "TagType" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_tags_tags_TagId_TagType",
                table: "user_tags",
                columns: new[] { "TagId", "TagType" },
                principalTable: "tags",
                principalColumns: new[] { "Id", "Type" },
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.AlterColumn<long>(
                name: "TagId",
                table: "user_tags",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "user_tags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_tags",
                table: "user_tags",
                columns: new[] { "Id", "UserId", "TagType" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_tags_tags_TagId_TagType",
                table: "user_tags",
                columns: new[] { "TagId", "TagType" },
                principalTable: "tags",
                principalColumns: new[] { "Id", "Type" });
        }
    }
}
