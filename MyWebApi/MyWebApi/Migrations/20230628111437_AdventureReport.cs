using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AdventureReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_reports_users_UserId",
                table: "user_reports");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "user_reports",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "AdventureId",
                table: "user_reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_reports_AdventureId",
                table: "user_reports",
                column: "AdventureId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_reports_adventures_AdventureId",
                table: "user_reports",
                column: "AdventureId",
                principalTable: "adventures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_reports_users_UserId",
                table: "user_reports",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_reports_adventures_AdventureId",
                table: "user_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_user_reports_users_UserId",
                table: "user_reports");

            migrationBuilder.DropIndex(
                name: "IX_user_reports_AdventureId",
                table: "user_reports");

            migrationBuilder.DropColumn(
                name: "AdventureId",
                table: "user_reports");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "user_reports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_user_reports_users_UserId",
                table: "user_reports",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
