using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDailyTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDailyTasks");

            migrationBuilder.DropTable(
                name: "DailyTasks");

            migrationBuilder.AlterColumn<short>(
                name: "EffectId",
                table: "active_effects",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EffectId",
                table: "active_effects",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateTable(
                name: "DailyTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Condition = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Reward = table.Column<int>(type: "integer", nullable: false),
                    RewardCurrency = table.Column<short>(type: "smallint", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    TaskType = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTasks", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "UserDailyTasks",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DailyTaskId = table.Column<long>(type: "bigint", nullable: false),
                    DailyTaskClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    AcquireMessage = table.Column<string>(type: "text", nullable: true),
                    IsAcquired = table.Column<bool>(type: "boolean", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDailyTasks", x => new { x.UserId, x.DailyTaskId });
                    table.ForeignKey(
                        name: "FK_UserDailyTasks_DailyTasks_DailyTaskId_DailyTaskClassLocalis~",
                        columns: x => new { x.DailyTaskId, x.DailyTaskClassLocalisationId },
                        principalTable: "DailyTasks",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDailyTasks_DailyTaskId_DailyTaskClassLocalisationId",
                table: "UserDailyTasks",
                columns: new[] { "DailyTaskId", "DailyTaskClassLocalisationId" });
        }
    }
}
