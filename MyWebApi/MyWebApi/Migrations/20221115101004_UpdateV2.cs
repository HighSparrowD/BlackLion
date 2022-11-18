using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyWebApi.Migrations
{
    public partial class UpdateV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PSYCHOLOGICAL_TESTS_ANSWERS_PSYCHOLOGICAL_TESTS_QUESTIONS_P~",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS");

            migrationBuilder.DropForeignKey(
                name: "FK_PSYCHOLOGICAL_TESTS_QUESTIONS_PSYCHOLOGICAL_TESTS_Psycholog~",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_LOCATIONS_CITIES_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_LOCATIONS_COUNTRIES_CountryClassLocalisationId_Country~",
                table: "USER_LOCATIONS");

            migrationBuilder.DropTable(
                name: "INTELLECTUAL_TESTS_ANSWERS");

            migrationBuilder.DropTable(
                name: "INTELLECTUAL_TESTS_QUESTIONS");

            migrationBuilder.DropTable(
                name: "INTELLECTUAL_TESTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PSYCHOLOGICAL_TESTS_QUESTIONS",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS");

            migrationBuilder.DropIndex(
                name: "IX_PSYCHOLOGICAL_TESTS_QUESTIONS_PsychologicalTestId",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PSYCHOLOGICAL_TESTS_ANSWERS",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS");

            migrationBuilder.DropIndex(
                name: "IX_PSYCHOLOGICAL_TESTS_ANSWERS_PsychologicalTestQuestionId",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PSYCHOLOGICAL_TESTS",
                table: "PSYCHOLOGICAL_TESTS");

            migrationBuilder.RenameTable(
                name: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                newName: "psychological_tests_questions");

            migrationBuilder.RenameTable(
                name: "PSYCHOLOGICAL_TESTS_ANSWERS",
                newName: "psychological_tests_answers");

            migrationBuilder.RenameTable(
                name: "PSYCHOLOGICAL_TESTS",
                newName: "psychological_tests");

            migrationBuilder.RenameColumn(
                name: "LevelsOfSense",
                table: "USER_PERSONALITY_STATS",
                newName: "LevelOfSense");

            migrationBuilder.RenameColumn(
                name: "LevelsOfSensePercentage",
                table: "USER_PERSONALITY_POINTS",
                newName: "LevelOfSensePercentage");

            migrationBuilder.RenameColumn(
                name: "LevelsOfSense",
                table: "USER_PERSONALITY_POINTS",
                newName: "LevelOfSense");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "USER_ENCOUNTERS",
                newName: "EncounteredUserId");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "USER_BLACKLISTS",
                newName: "BannedUserId");

            migrationBuilder.AddColumn<int>(
                name: "CardDecksMini",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CardDecksPlatinum",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Detectors",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecondChances",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThePersonalities",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Valentines",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WhiteDetectors",
                table: "USER_WALLET_BALANCES",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "USER_NOTIFICATIONS",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CountryClassLocalisationId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CityCountryClassLocalisationId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "USER_ENCOUNTERS",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldFilterUsersWithoutRealPhoto",
                table: "SYSTEM_USERS_PREFERENCES",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShouldUsePersonalityFunc",
                table: "SYSTEM_USERS_PREFERENCES",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AutoReplyText",
                table: "SYSTEM_USERS_DATA",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AutoReplyVoice",
                table: "SYSTEM_USERS_DATA",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Tags",
                table: "SYSTEM_USERS_DATA",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPhotoReal",
                table: "SYSTEM_USERS_BASES",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserRawDescription",
                table: "SYSTEM_USERS_BASES",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ShouldConsiderLanguages",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsBusy",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HasPremium",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HadReceivedReward",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "DailyRewardPoint",
                table: "SYSTEM_USERS",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "BonusIndex",
                table: "SYSTEM_USERS",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "InvitedUsersBonus",
                table: "SYSTEM_USERS",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "InvitedUsersCount",
                table: "SYSTEM_USERS",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIdentityConfirmed",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxProfileViewsCount",
                table: "SYSTEM_USERS",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProfileViewsCount",
                table: "SYSTEM_USERS",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TagSearchesCount",
                table: "SYSTEM_USERS",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "psychological_tests_questions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "PsychologicalTestClassLocalisationId",
                table: "psychological_tests_questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "PsychologicalTestQuestionId",
                table: "psychological_tests_answers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "psychological_tests_answers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ClassLocalisationId",
                table: "psychological_tests_answers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PsychologicalTestQuestionPsychologicalTestClassLocalisationId",
                table: "psychological_tests_answers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Value",
                table: "psychological_tests_answers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "psychological_tests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ClassLocalisationId",
                table: "psychological_tests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_psychological_tests_questions",
                table: "psychological_tests_questions",
                columns: new[] { "Id", "PsychologicalTestClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_psychological_tests_answers",
                table: "psychological_tests_answers",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_psychological_tests",
                table: "psychological_tests",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.CreateTable(
                name: "ADMIN_ERROR_LOGS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThrownByUser = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ADMIN_ERROR_LOGS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tick_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<bool>(type: "boolean", nullable: true),
                    Video = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Circle = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tick_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tick_requests_SYSTEM_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "SYSTEM_USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_ACTIVE_EFFECTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresIn = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ACTIVE_EFFECTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER_TESTS_RESULTS",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    Personality = table.Column<int>(type: "integer", nullable: false),
                    EmotionalIntellect = table.Column<int>(type: "integer", nullable: false),
                    Reliability = table.Column<int>(type: "integer", nullable: false),
                    Compassion = table.Column<int>(type: "integer", nullable: false),
                    OpenMindedness = table.Column<int>(type: "integer", nullable: false),
                    Agreeableness = table.Column<int>(type: "integer", nullable: false),
                    SelfAwareness = table.Column<int>(type: "integer", nullable: false),
                    LevelOfSense = table.Column<int>(type: "integer", nullable: false),
                    Intellect = table.Column<int>(type: "integer", nullable: false),
                    Nature = table.Column<int>(type: "integer", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_TESTS_RESULTS", x => new { x.UserId, x.TestId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_USER_ENCOUNTERS_EncounteredUserId",
                table: "USER_ENCOUNTERS",
                column: "EncounteredUserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_BLACKLISTS_BannedUserId",
                table: "USER_BLACKLISTS",
                column: "BannedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_psychological_tests_questions_PsychologicalTestId_Psycholog~",
                table: "psychological_tests_questions",
                columns: new[] { "PsychologicalTestId", "PsychologicalTestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_psychological_tests_answers_PsychologicalTestQuestionId_Psy~",
                table: "psychological_tests_answers",
                columns: new[] { "PsychologicalTestQuestionId", "PsychologicalTestQuestionPsychologicalTestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_tick_requests_UserId",
                table: "tick_requests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_psychological_tests_answers_psychological_tests_questions_P~",
                table: "psychological_tests_answers",
                columns: new[] { "PsychologicalTestQuestionId", "PsychologicalTestQuestionPsychologicalTestClassLocalisationId" },
                principalTable: "psychological_tests_questions",
                principalColumns: new[] { "Id", "PsychologicalTestClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_psychological_tests_questions_psychological_tests_Psycholog~",
                table: "psychological_tests_questions",
                columns: new[] { "PsychologicalTestId", "PsychologicalTestClassLocalisationId" },
                principalTable: "psychological_tests",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_BLACKLISTS_SYSTEM_USERS_BASES_BannedUserId",
                table: "USER_BLACKLISTS",
                column: "BannedUserId",
                principalTable: "SYSTEM_USERS_BASES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ENCOUNTERS_SYSTEM_USERS_BASES_EncounteredUserId",
                table: "USER_ENCOUNTERS",
                column: "EncounteredUserId",
                principalTable: "SYSTEM_USERS_BASES",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_LOCATIONS_CITIES_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS",
                columns: new[] { "CityCountryClassLocalisationId", "CityId" },
                principalTable: "CITIES",
                principalColumns: new[] { "Id", "CountryClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_USER_LOCATIONS_COUNTRIES_CountryClassLocalisationId_Country~",
                table: "USER_LOCATIONS",
                columns: new[] { "CountryClassLocalisationId", "CountryId" },
                principalTable: "COUNTRIES",
                principalColumns: new[] { "Id", "ClassLocalisationId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_psychological_tests_answers_psychological_tests_questions_P~",
                table: "psychological_tests_answers");

            migrationBuilder.DropForeignKey(
                name: "FK_psychological_tests_questions_psychological_tests_Psycholog~",
                table: "psychological_tests_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_BLACKLISTS_SYSTEM_USERS_BASES_BannedUserId",
                table: "USER_BLACKLISTS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_ENCOUNTERS_SYSTEM_USERS_BASES_EncounteredUserId",
                table: "USER_ENCOUNTERS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_LOCATIONS_CITIES_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_LOCATIONS_COUNTRIES_CountryClassLocalisationId_Country~",
                table: "USER_LOCATIONS");

            migrationBuilder.DropTable(
                name: "ADMIN_ERROR_LOGS");

            migrationBuilder.DropTable(
                name: "tick_requests");

            migrationBuilder.DropTable(
                name: "USER_ACTIVE_EFFECTS");

            migrationBuilder.DropTable(
                name: "USER_TESTS_RESULTS");

            migrationBuilder.DropIndex(
                name: "IX_USER_ENCOUNTERS_EncounteredUserId",
                table: "USER_ENCOUNTERS");

            migrationBuilder.DropIndex(
                name: "IX_USER_BLACKLISTS_BannedUserId",
                table: "USER_BLACKLISTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_psychological_tests_questions",
                table: "psychological_tests_questions");

            migrationBuilder.DropIndex(
                name: "IX_psychological_tests_questions_PsychologicalTestId_Psycholog~",
                table: "psychological_tests_questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_psychological_tests_answers",
                table: "psychological_tests_answers");

            migrationBuilder.DropIndex(
                name: "IX_psychological_tests_answers_PsychologicalTestQuestionId_Psy~",
                table: "psychological_tests_answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_psychological_tests",
                table: "psychological_tests");

            migrationBuilder.DropColumn(
                name: "CardDecksMini",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "CardDecksPlatinum",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "Detectors",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "SecondChances",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "ThePersonalities",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "Valentines",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "WhiteDetectors",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropColumn(
                name: "ShouldFilterUsersWithoutRealPhoto",
                table: "SYSTEM_USERS_PREFERENCES");

            migrationBuilder.DropColumn(
                name: "ShouldUsePersonalityFunc",
                table: "SYSTEM_USERS_PREFERENCES");

            migrationBuilder.DropColumn(
                name: "AutoReplyText",
                table: "SYSTEM_USERS_DATA");

            migrationBuilder.DropColumn(
                name: "AutoReplyVoice",
                table: "SYSTEM_USERS_DATA");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "SYSTEM_USERS_DATA");

            migrationBuilder.DropColumn(
                name: "IsPhotoReal",
                table: "SYSTEM_USERS_BASES");

            migrationBuilder.DropColumn(
                name: "UserRawDescription",
                table: "SYSTEM_USERS_BASES");

            migrationBuilder.DropColumn(
                name: "InvitedUsersBonus",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "InvitedUsersCount",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "IsIdentityConfirmed",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "MaxProfileViewsCount",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "ProfileViewsCount",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "TagSearchesCount",
                table: "SYSTEM_USERS");

            migrationBuilder.DropColumn(
                name: "PsychologicalTestClassLocalisationId",
                table: "psychological_tests_questions");

            migrationBuilder.DropColumn(
                name: "ClassLocalisationId",
                table: "psychological_tests_answers");

            migrationBuilder.DropColumn(
                name: "PsychologicalTestQuestionPsychologicalTestClassLocalisationId",
                table: "psychological_tests_answers");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "psychological_tests_answers");

            migrationBuilder.DropColumn(
                name: "ClassLocalisationId",
                table: "psychological_tests");

            migrationBuilder.RenameTable(
                name: "psychological_tests_questions",
                newName: "PSYCHOLOGICAL_TESTS_QUESTIONS");

            migrationBuilder.RenameTable(
                name: "psychological_tests_answers",
                newName: "PSYCHOLOGICAL_TESTS_ANSWERS");

            migrationBuilder.RenameTable(
                name: "psychological_tests",
                newName: "PSYCHOLOGICAL_TESTS");

            migrationBuilder.RenameColumn(
                name: "LevelOfSense",
                table: "USER_PERSONALITY_STATS",
                newName: "LevelsOfSense");

            migrationBuilder.RenameColumn(
                name: "LevelOfSensePercentage",
                table: "USER_PERSONALITY_POINTS",
                newName: "LevelsOfSensePercentage");

            migrationBuilder.RenameColumn(
                name: "LevelOfSense",
                table: "USER_PERSONALITY_POINTS",
                newName: "LevelsOfSense");

            migrationBuilder.RenameColumn(
                name: "EncounteredUserId",
                table: "USER_ENCOUNTERS",
                newName: "UserId1");

            migrationBuilder.RenameColumn(
                name: "BannedUserId",
                table: "USER_BLACKLISTS",
                newName: "UserId1");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "USER_NOTIFICATIONS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CountryClassLocalisationId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityCountryClassLocalisationId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "USER_ENCOUNTERS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<bool>(
                name: "ShouldConsiderLanguages",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBusy",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "HasPremium",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "HadReceivedReward",
                table: "SYSTEM_USERS",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<short>(
                name: "DailyRewardPoint",
                table: "SYSTEM_USERS",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<short>(
                name: "BonusIndex",
                table: "SYSTEM_USERS",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "PsychologicalTestQuestionId",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "PSYCHOLOGICAL_TESTS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PSYCHOLOGICAL_TESTS_QUESTIONS",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PSYCHOLOGICAL_TESTS_ANSWERS",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PSYCHOLOGICAL_TESTS",
                table: "PSYCHOLOGICAL_TESTS",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "INTELLECTUAL_TESTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INTELLECTUAL_TESTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "INTELLECTUAL_TESTS_QUESTIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IntellectualTestId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INTELLECTUAL_TESTS_QUESTIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_INTELLECTUAL_TESTS_QUESTIONS_INTELLECTUAL_TESTS_Intellectua~",
                        column: x => x.IntellectualTestId,
                        principalTable: "INTELLECTUAL_TESTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "INTELLECTUAL_TESTS_ANSWERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IntellectualTestQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INTELLECTUAL_TESTS_ANSWERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_INTELLECTUAL_TESTS_ANSWERS_INTELLECTUAL_TESTS_QUESTIONS_Int~",
                        column: x => x.IntellectualTestQuestionId,
                        principalTable: "INTELLECTUAL_TESTS_QUESTIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PSYCHOLOGICAL_TESTS_QUESTIONS_PsychologicalTestId",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                column: "PsychologicalTestId");

            migrationBuilder.CreateIndex(
                name: "IX_PSYCHOLOGICAL_TESTS_ANSWERS_PsychologicalTestQuestionId",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS",
                column: "PsychologicalTestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_INTELLECTUAL_TESTS_ANSWERS_IntellectualTestQuestionId",
                table: "INTELLECTUAL_TESTS_ANSWERS",
                column: "IntellectualTestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_INTELLECTUAL_TESTS_QUESTIONS_IntellectualTestId",
                table: "INTELLECTUAL_TESTS_QUESTIONS",
                column: "IntellectualTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PSYCHOLOGICAL_TESTS_ANSWERS_PSYCHOLOGICAL_TESTS_QUESTIONS_P~",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS",
                column: "PsychologicalTestQuestionId",
                principalTable: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PSYCHOLOGICAL_TESTS_QUESTIONS_PSYCHOLOGICAL_TESTS_Psycholog~",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                column: "PsychologicalTestId",
                principalTable: "PSYCHOLOGICAL_TESTS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_LOCATIONS_CITIES_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS",
                columns: new[] { "CityCountryClassLocalisationId", "CityId" },
                principalTable: "CITIES",
                principalColumns: new[] { "Id", "CountryClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_LOCATIONS_COUNTRIES_CountryClassLocalisationId_Country~",
                table: "USER_LOCATIONS",
                columns: new[] { "CountryClassLocalisationId", "CountryId" },
                principalTable: "COUNTRIES",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
