using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class advertisemtnreferencefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_advertisements_UserId",
                table: "advertisements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_advertisements_users_UserId",
                table: "advertisements",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_advertisements_users_UserId",
                table: "advertisements");

            migrationBuilder.DropIndex(
                name: "IX_advertisements_UserId",
                table: "advertisements");
        }
    }
}
