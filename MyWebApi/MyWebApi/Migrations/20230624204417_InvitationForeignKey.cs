using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InvitationForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invitation_users_InvitedUserId",
                table: "invitation");

            migrationBuilder.DropIndex(
                name: "IX_invitation_InvitedUserId",
                table: "invitation");

            migrationBuilder.AlterColumn<short>(
                name: "Status",
                table: "adventure_attendees",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "adventure_attendees",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_InvitedUserId",
                table: "invitation",
                column: "InvitedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_invitation_users_InvitedUserId",
                table: "invitation",
                column: "InvitedUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
