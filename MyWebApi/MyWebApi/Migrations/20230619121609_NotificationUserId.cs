using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class NotificationUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_ReceiverId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications");

            migrationBuilder.DropIndex(
                name: "IX_notifications_ReceiverId",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "notifications");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "notifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "notifications",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ReceiverId",
                table: "notifications",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_notifications_ReceiverId",
                table: "notifications",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_ReceiverId",
                table: "notifications",
                column: "ReceiverId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
