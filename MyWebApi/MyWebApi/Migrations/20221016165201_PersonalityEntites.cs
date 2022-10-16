using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyWebApi.Migrations
{
    public partial class PersonalityEntites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "USER_WALLET_BALANCES",
                newName: "Points");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "USER_WALLET_PURCHASES",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<short>(
                name: "Currency",
                table: "USER_WALLET_PURCHASES",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "USER_WALLET_BALANCES",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "PersonalityPoints",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "BonusIndex",
                table: "SYSTEM_USERS",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "SYSTEM_USERS",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DAILY_REWARDS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PointReward = table.Column<int>(type: "integer", nullable: false),
                    Index = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DAILY_REWARDS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DAILY_TASKS",
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
                    table.PrimaryKey("PK_DAILY_TASKS", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "USER_INVITATION_CREDENTIALS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true),
                    QRCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_INVITATION_CREDENTIALS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_INVITATION_CREDENTIALS_SYSTEM_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "SYSTEM_USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_PERSONALITY_POINTS",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Personality = table.Column<int>(type: "integer", nullable: false),
                    PersonalityPercentage = table.Column<double>(type: "double precision", nullable: false),
                    EmotionalIntellect = table.Column<int>(type: "integer", nullable: false),
                    EmotionalIntellectPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Reliability = table.Column<int>(type: "integer", nullable: false),
                    ReliabilityPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Compassion = table.Column<int>(type: "integer", nullable: false),
                    CompassionPercentage = table.Column<double>(type: "double precision", nullable: false),
                    OpenMindedness = table.Column<int>(type: "integer", nullable: false),
                    OpenMindednessPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Agreeableness = table.Column<int>(type: "integer", nullable: false),
                    AgreeablenessPercentage = table.Column<double>(type: "double precision", nullable: false),
                    SelfAwareness = table.Column<int>(type: "integer", nullable: false),
                    SelfAwarenessPercentage = table.Column<double>(type: "double precision", nullable: false),
                    LevelsOfSense = table.Column<int>(type: "integer", nullable: false),
                    LevelsOfSensePercentage = table.Column<double>(type: "double precision", nullable: false),
                    Intellect = table.Column<int>(type: "integer", nullable: false),
                    IntellectPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Nature = table.Column<int>(type: "integer", nullable: false),
                    NaturePercentage = table.Column<double>(type: "double precision", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false),
                    CreativityPercentage = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_PERSONALITY_POINTS", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "USER_PERSONALITY_STATS",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Personality = table.Column<int>(type: "integer", nullable: false),
                    EmotionalIntellect = table.Column<int>(type: "integer", nullable: false),
                    Reliability = table.Column<int>(type: "integer", nullable: false),
                    Compassion = table.Column<int>(type: "integer", nullable: false),
                    OpenMindedness = table.Column<int>(type: "integer", nullable: false),
                    Agreeableness = table.Column<int>(type: "integer", nullable: false),
                    SelfAwareness = table.Column<int>(type: "integer", nullable: false),
                    LevelsOfSense = table.Column<int>(type: "integer", nullable: false),
                    Intellect = table.Column<int>(type: "integer", nullable: false),
                    Nature = table.Column<int>(type: "integer", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_PERSONALITY_STATS", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "USER_DAILY_TASKS",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DailyTaskId = table.Column<long>(type: "bigint", nullable: false),
                    DailyTaskClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    AcquireMessage = table.Column<string>(type: "text", nullable: true),
                    IsAcquired = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_DAILY_TASKS", x => new { x.UserId, x.DailyTaskId });
                    table.ForeignKey(
                        name: "FK_USER_DAILY_TASKS_DAILY_TASKS_DailyTaskId_DailyTaskClassLoca~",
                        columns: x => new { x.DailyTaskId, x.DailyTaskClassLocalisationId },
                        principalTable: "DAILY_TASKS",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_INVITATIONS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitorCredentialsId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedUserId = table.Column<long>(type: "bigint", nullable: false),
                    InvitationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_INVITATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_INVITATIONS_USER_INVITATION_CREDENTIALS_InvitorCredent~",
                        column: x => x.InvitorCredentialsId,
                        principalTable: "USER_INVITATION_CREDENTIALS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_DAILY_TASKS_DailyTaskId_DailyTaskClassLocalisationId",
                table: "USER_DAILY_TASKS",
                columns: new[] { "DailyTaskId", "DailyTaskClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_INVITATION_CREDENTIALS_UserId",
                table: "USER_INVITATION_CREDENTIALS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_INVITATIONS_InvitorCredentialsId",
                table: "USER_INVITATIONS",
                column: "InvitorCredentialsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DAILY_REWARDS");

            migrationBuilder.DropTable(
                name: "USER_DAILY_TASKS");

            migrationBuilder.DropTable(
                name: "USER_INVITATIONS");

            migrationBuilder.DropTable(
                name: "USER_PERSONALITY_POINTS");

            migrationBuilder.DropTable(
                name: "USER_PERSONALITY_STATS");

            migrationBuilder.DropTable(
                name: "DAILY_TASKS");

            migrationBuilder.DropTable(
                name: "USER_INVITATION_CREDENTIALS");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "USER_WALLET_PURCHASES");

            migrationBuilder.DropColumn(
                name: "PersonalityPoints",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "BonusIndex",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "SYSTEM_USERS");

            migrationBuilder.RenameColumn(
                name: "Points",
                table: "USER_WALLET_BALANCES",
                newName: "Amount");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "USER_WALLET_PURCHASES",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "USER_WALLET_BALANCES",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
