using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_user_settings_UserSettingsId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_UserSettingsId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "UserSettingsId",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_users_SettingsId",
                table: "users",
                column: "SettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_settings_SettingsId",
                table: "users",
                column: "SettingsId",
                principalTable: "user_settings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_user_settings_SettingsId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_SettingsId",
                table: "users");

            migrationBuilder.AddColumn<long>(
                name: "UserSettingsId",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_UserSettingsId",
                table: "users",
                column: "UserSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_settings_UserSettingsId",
                table: "users",
                column: "UserSettingsId",
                principalTable: "user_settings",
                principalColumn: "Id");
        }
    }
}
