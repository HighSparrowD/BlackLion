using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class StatisticsFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_user_statistics_StatisticsId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_StatisticsId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "StatisticsId",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_statistics",
                newName: "UserId");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "user_statistics",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_user_statistics_users_UserId",
                table: "user_statistics",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_statistics_users_UserId",
                table: "user_statistics");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_statistics",
                newName: "Id");

            migrationBuilder.AddColumn<long>(
                name: "StatisticsId",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "user_statistics",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.CreateIndex(
                name: "IX_users_StatisticsId",
                table: "users",
                column: "StatisticsId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_statistics_StatisticsId",
                table: "users",
                column: "StatisticsId",
                principalTable: "user_statistics",
                principalColumn: "Id");
        }
    }
}
