using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_SenderId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_SenderId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "IsLikedBack",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "notifications");

            migrationBuilder.CreateSequence<int>(
                name: "requests_hilo");

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SenderId = table.Column<long>(type: "bigint", nullable: false),
                    IsMatch = table.Column<bool>(type: "boolean", nullable: false),
                    SystemMessage = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Requests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SenderId",
                table: "Requests",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                table: "Requests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropSequence(
                name: "requests_hilo");

            migrationBuilder.AddColumn<bool>(
                name: "IsLikedBack",
                table: "notifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "SenderId",
                table: "notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_SenderId",
                table: "notifications",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_SenderId",
                table: "notifications",
                column: "SenderId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
