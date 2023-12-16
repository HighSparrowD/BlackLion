using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class advertisemtnuserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_advertisements_sponsors_SponsorId",
                table: "advertisements");

            migrationBuilder.AlterColumn<long>(
                name: "SponsorId",
                table: "advertisements",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "advertisements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_advertisements_sponsors_SponsorId",
                table: "advertisements",
                column: "SponsorId",
                principalTable: "sponsors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_advertisements_sponsors_SponsorId",
                table: "advertisements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "advertisements");

            migrationBuilder.AlterColumn<long>(
                name: "SponsorId",
                table: "advertisements",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_advertisements_sponsors_SponsorId",
                table: "advertisements",
                column: "SponsorId",
                principalTable: "sponsors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
