using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class VerificationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tick_requests");

            migrationBuilder.CreateTable(
                name: "verification_requests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: true),
                    Media = table.Column<string>(type: "text", nullable: false),
                    MediaType = table.Column<short>(type: "smallint", nullable: false),
                    Gesture = table.Column<string>(type: "text", nullable: true),
                    ConfirmationType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_verification_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_verification_requests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_verification_requests_UserId",
                table: "verification_requests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "verification_requests");

            migrationBuilder.CreateTable(
                name: "tick_requests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: true),
                    Circle = table.Column<string>(type: "text", nullable: true),
                    Gesture = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Video = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tick_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tick_requests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tick_requests_UserId",
                table: "tick_requests",
                column: "UserId");
        }
    }
}
