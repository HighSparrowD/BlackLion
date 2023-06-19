using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameUsersData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_users_data_DataId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users_data",
                table: "users_data");

            migrationBuilder.RenameTable(
                name: "users_data",
                newName: "user_data");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_data",
                table: "user_data",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_data_DataId",
                table: "users",
                column: "DataId",
                principalTable: "user_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_user_data_DataId",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_data",
                table: "user_data");

            migrationBuilder.RenameTable(
                name: "user_data",
                newName: "users_data");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users_data",
                table: "users_data",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_data_DataId",
                table: "users",
                column: "DataId",
                principalTable: "users_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
