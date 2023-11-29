using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UserStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StatisticsId",
                table: "users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_statistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfileRegistrations = table.Column<int>(type: "integer", nullable: false),
                    TestsPassed = table.Column<int>(type: "integer", nullable: false),
                    DislikedProfiles = table.Column<int>(type: "integer", nullable: false),
                    DiscardedMatches = table.Column<int>(type: "integer", nullable: false),
                    LikesReceived = table.Column<int>(type: "integer", nullable: false),
                    Likes = table.Column<int>(type: "integer", nullable: false),
                    HighSimilarityMatches = table.Column<int>(type: "integer", nullable: false),
                    UseStreak = table.Column<int>(type: "integer", nullable: false),
                    IdeasGiven = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_statistics", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_user_statistics_StatisticsId",
                table: "users");

            migrationBuilder.DropTable(
                name: "user_statistics");

            migrationBuilder.DropIndex(
                name: "IX_users_StatisticsId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "StatisticsId",
                table: "users");
        }
    }
}
