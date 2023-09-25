using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RequestNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_users_SenderId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_users_UserId",
                table: "Requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Requests",
                table: "Requests");

            migrationBuilder.RenameTable(
                name: "Requests",
                newName: "requests");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_UserId",
                table: "requests",
                newName: "IX_requests_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Requests_SenderId",
                table: "requests",
                newName: "IX_requests_SenderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_requests",
                table: "requests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_requests_users_SenderId",
                table: "requests",
                column: "SenderId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_requests_users_UserId",
                table: "requests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_requests_users_SenderId",
                table: "requests");

            migrationBuilder.DropForeignKey(
                name: "FK_requests_users_UserId",
                table: "requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_requests",
                table: "requests");

            migrationBuilder.RenameTable(
                name: "requests",
                newName: "Requests");

            migrationBuilder.RenameIndex(
                name: "IX_requests_UserId",
                table: "Requests",
                newName: "IX_Requests_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_requests_SenderId",
                table: "Requests",
                newName: "IX_Requests_SenderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                table: "Requests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_users_SenderId",
                table: "Requests",
                column: "SenderId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_users_UserId",
                table: "Requests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
