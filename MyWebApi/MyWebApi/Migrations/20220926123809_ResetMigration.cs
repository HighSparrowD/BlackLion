using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyWebApi.Migrations
{
    public partial class ResetMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AGE_PREFERENCES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalisationId = table.Column<short>(type: "smallint", nullable: false),
                    AgePrefName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AGE_PREFERENCES", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "APP_LANGUAGES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LanguageName = table.Column<string>(type: "text", nullable: true),
                    LanguageNameShort = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_LANGUAGES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CLASS_LOCALISATIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LanguageName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLASS_LOCALISATIONS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "COMMUNICATION_PREFERENCES",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CommunicationPrefName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COMMUNICATION_PREFERENCES", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "COUNTRIES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_COUNTRIES", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "FEEDBACK_REASONS",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FEEDBACK_REASONS", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "INTELLECTUAL_TESTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INTELLECTUAL_TESTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LOCALISATIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    SectionName = table.Column<string>(type: "text", nullable: true),
                    LanguageName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOCALISATIONS", x => new { x.Id, x.SectionId });
                });

            migrationBuilder.CreateTable(
                name: "PSYCHOLOGICAL_TESTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSYCHOLOGICAL_TESTS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "REPORT_REASONS",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REPORT_REASONS", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_CONTACT_INFO",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    Tel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Instagram = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Facebook = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_CONTACT_INFO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_STATS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    AverageRating = table.Column<double>(type: "double precision", nullable: true),
                    ConductedEventsCount = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    LevelProgress = table.Column<double>(type: "double precision", nullable: false),
                    LevelGoal = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_STATS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_ACHIEVEMENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    ConditionValue = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_ACHIEVEMENTS", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_ADMINS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_ADMINS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_USERS_BASES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    UserRealName = table.Column<string>(type: "text", nullable: true),
                    UserDescription = table.Column<string>(type: "text", nullable: true),
                    UserPhoto = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_USERS_BASES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_USERS_PREFERENCES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserLanguagePreferences = table.Column<List<int>>(type: "integer[]", nullable: true),
                    UserLocationPreferences = table.Column<List<int>>(type: "integer[]", nullable: true),
                    AgePrefs = table.Column<List<int>>(type: "integer[]", nullable: true),
                    CommunicationPrefs = table.Column<int>(type: "integer", nullable: false),
                    UserGenderPrefs = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_USERS_PREFERENCES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER_ENCOUNTERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserId1 = table.Column<long>(type: "bigint", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    EncounterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ENCOUNTERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER_REASONS",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    ReasonName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_REASONS", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "USER_TRUST_LEVELS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Progress = table.Column<double>(type: "double precision", nullable: false),
                    Goal = table.Column<double>(type: "double precision", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_TRUST_LEVELS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER_VISITS",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_VISITS", x => new { x.UserId, x.SectionId });
                });

            migrationBuilder.CreateTable(
                name: "USER_WALLET_BALANCES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    PointInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_WALLET_BALANCES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER_WALLET_PURCHASES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PointInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_WALLET_PURCHASES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LANGUAGES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    LanguageName = table.Column<string>(type: "text", nullable: true),
                    LanguageNameNative = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LANGUAGES", x => new { x.Id, x.ClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_LANGUAGES_CLASS_LOCALISATIONS_ClassLocalisationId",
                        column: x => x.ClassLocalisationId,
                        principalTable: "CLASS_LOCALISATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_GENDERS",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    GenderName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_GENDERS", x => new { x.Id, x.ClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_SYSTEM_GENDERS_CLASS_LOCALISATIONS_ClassLocalisationId",
                        column: x => x.ClassLocalisationId,
                        principalTable: "CLASS_LOCALISATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UpdateCountry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateCountry", x => new { x.Id, x.ClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_UpdateCountry_CLASS_LOCALISATIONS_ClassLocalisationId",
                        column: x => x.ClassLocalisationId,
                        principalTable: "CLASS_LOCALISATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CITIES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CountryClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CityName = table.Column<string>(type: "text", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CITIES", x => new { x.Id, x.CountryClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_CITIES_COUNTRIES_CountryClassLocalisationId_CountryId",
                        columns: x => new { x.CountryClassLocalisationId, x.CountryId },
                        principalTable: "COUNTRIES",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
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
                name: "SECONDARY_LOCALISATIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocalisationId = table.Column<int>(type: "integer", nullable: false),
                    LocalisationSectionId = table.Column<int>(type: "integer", nullable: false),
                    ElementName = table.Column<string>(type: "text", nullable: true),
                    ElementText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECONDARY_LOCALISATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SECONDARY_LOCALISATIONS_LOCALISATIONS_LocalisationId_Locali~",
                        columns: x => new { x.LocalisationId, x.LocalisationSectionId },
                        principalTable: "LOCALISATIONS",
                        principalColumns: new[] { "Id", "SectionId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PsychologicalTestId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSYCHOLOGICAL_TESTS_QUESTIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PSYCHOLOGICAL_TESTS_QUESTIONS_PSYCHOLOGICAL_TESTS_Psycholog~",
                        column: x => x.PsychologicalTestId,
                        principalTable: "PSYCHOLOGICAL_TESTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_SPONSORS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: true),
                    CodeWord = table.Column<string>(type: "text", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    UserMaxAdCount = table.Column<int>(type: "integer", nullable: false),
                    UserMaxAdViewCount = table.Column<int>(type: "integer", nullable: false),
                    IsPostponed = table.Column<bool>(type: "boolean", nullable: false),
                    IsAwaiting = table.Column<bool>(type: "boolean", nullable: false),
                    UserAppLanguage = table.Column<int>(type: "integer", nullable: false),
                    UserCountryId = table.Column<int>(type: "integer", nullable: false),
                    UserCityId = table.Column<int>(type: "integer", nullable: false),
                    ContactInfoId = table.Column<long>(type: "bigint", nullable: true),
                    StatsId = table.Column<long>(type: "bigint", nullable: true),
                    HasBaseAccount = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_SPONSORS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SYSTEM_SPONSORS_SPONSOR_CONTACT_INFO_ContactInfoId",
                        column: x => x.ContactInfoId,
                        principalTable: "SPONSOR_CONTACT_INFO",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SYSTEM_SPONSORS_SPONSOR_STATS_StatsId",
                        column: x => x.StatsId,
                        principalTable: "SPONSOR_STATS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_FEEDBACKS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserBaseInfoId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    ReasonId = table.Column<short>(type: "smallint", nullable: false),
                    ReasonClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    InsertedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_FEEDBACKS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SYSTEM_FEEDBACKS_FEEDBACK_REASONS_ReasonId_ReasonClassLocal~",
                        columns: x => new { x.ReasonId, x.ReasonClassLocalisationId },
                        principalTable: "FEEDBACK_REASONS",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SYSTEM_FEEDBACKS_SYSTEM_USERS_BASES_UserBaseInfoId",
                        column: x => x.UserBaseInfoId,
                        principalTable: "SYSTEM_USERS_BASES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_ACHIEVEMENTS",
                columns: table => new
                {
                    AchievementId = table.Column<long>(type: "bigint", nullable: false),
                    UserBaseInfoId = table.Column<long>(type: "bigint", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    AcquireMessage = table.Column<string>(type: "text", nullable: true),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    IsAcquired = table.Column<bool>(type: "boolean", nullable: false),
                    AchievementClassLocalisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ACHIEVEMENTS", x => new { x.UserBaseInfoId, x.AchievementId });
                    table.ForeignKey(
                        name: "FK_USER_ACHIEVEMENTS_SYSTEM_ACHIEVEMENTS_AchievementId_Achieve~",
                        columns: x => new { x.AchievementId, x.AchievementClassLocalisationId },
                        principalTable: "SYSTEM_ACHIEVEMENTS",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_ACHIEVEMENTS_SYSTEM_USERS_BASES_UserBaseInfoId",
                        column: x => x.UserBaseInfoId,
                        principalTable: "SYSTEM_USERS_BASES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_REPORTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserBaseInfoId = table.Column<long>(type: "bigint", nullable: false),
                    UserBaseInfoId1 = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    ReasonId = table.Column<short>(type: "smallint", nullable: false),
                    ReasonClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    InsertedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_REPORTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_REPORTS_REPORT_REASONS_ReasonId_ReasonClassLocalisatio~",
                        columns: x => new { x.ReasonId, x.ReasonClassLocalisationId },
                        principalTable: "REPORT_REASONS",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_REPORTS_SYSTEM_USERS_BASES_UserBaseInfoId",
                        column: x => x.UserBaseInfoId,
                        principalTable: "SYSTEM_USERS_BASES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_REPORTS_SYSTEM_USERS_BASES_UserBaseInfoId1",
                        column: x => x.UserBaseInfoId1,
                        principalTable: "SYSTEM_USERS_BASES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_LOCATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: false),
                    CityCountryClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CountryClassLocalisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_LOCATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_LOCATIONS_CITIES_CityCountryClassLocalisationId_CityId",
                        columns: x => new { x.CityCountryClassLocalisationId, x.CityId },
                        principalTable: "CITIES",
                        principalColumns: new[] { "Id", "CountryClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_LOCATIONS_COUNTRIES_CountryClassLocalisationId_Country~",
                        columns: x => new { x.CountryClassLocalisationId, x.CountryId },
                        principalTable: "COUNTRIES",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "INTELLECTUAL_TESTS_ANSWERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IntellectualTestQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PSYCHOLOGICAL_TESTS_ANSWERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PsychologicalTestQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PSYCHOLOGICAL_TESTS_ANSWERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PSYCHOLOGICAL_TESTS_ANSWERS_PSYCHOLOGICAL_TESTS_QUESTIONS_P~",
                        column: x => x.PsychologicalTestQuestionId,
                        principalTable: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_ADS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Video = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_ADS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPONSOR_ADS_SYSTEM_SPONSORS_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "SYSTEM_SPONSORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_EVENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Languages = table.Column<List<int?>>(type: "integer[]", nullable: true),
                    MinAge = table.Column<short>(type: "smallint", nullable: false),
                    MaxAge = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    HasGroup = table.Column<bool>(type: "boolean", nullable: false),
                    GroupLink = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Bounty = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_EVENTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPONSOR_EVENTS_SYSTEM_SPONSORS_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "SYSTEM_SPONSORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_LANGUAGES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    LanguageClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_LANGUAGES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPONSOR_LANGUAGES_LANGUAGES_LanguageClassLocalisationId_Lan~",
                        columns: x => new { x.LanguageClassLocalisationId, x.LanguageId },
                        principalTable: "LANGUAGES",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPONSOR_LANGUAGES_SYSTEM_SPONSORS_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "SYSTEM_SPONSORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_NOTIFICATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    NotificationReason = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_NOTIFICATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPONSOR_NOTIFICATIONS_SYSTEM_SPONSORS_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "SYSTEM_SPONSORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_USERS_DATA",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserLanguages = table.Column<List<int>>(type: "integer[]", nullable: true),
                    ReasonId = table.Column<short>(type: "smallint", nullable: false),
                    UserAge = table.Column<int>(type: "integer", nullable: false),
                    UserGender = table.Column<short>(type: "smallint", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    ReasonClassLocalisationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_USERS_DATA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SYSTEM_USERS_DATA_USER_LOCATIONS_LocationId",
                        column: x => x.LocationId,
                        principalTable: "USER_LOCATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SYSTEM_USERS_DATA_USER_REASONS_ReasonId_ReasonClassLocalisa~",
                        columns: x => new { x.ReasonId, x.ReasonClassLocalisationId },
                        principalTable: "USER_REASONS",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_USERS",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserBaseInfoId = table.Column<long>(type: "bigint", nullable: false),
                    UserDataInfoId = table.Column<long>(type: "bigint", nullable: false),
                    UserPreferencesId = table.Column<long>(type: "bigint", nullable: false),
                    ShouldConsiderLanguages = table.Column<bool>(type: "boolean", nullable: true),
                    IsBusy = table.Column<bool>(type: "boolean", nullable: true),
                    IsBanned = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    HasPremium = table.Column<bool>(type: "boolean", nullable: true),
                    PremiumExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_USERS", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_SYSTEM_USERS_SYSTEM_USERS_BASES_UserBaseInfoId",
                        column: x => x.UserBaseInfoId,
                        principalTable: "SYSTEM_USERS_BASES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SYSTEM_USERS_SYSTEM_USERS_DATA_UserDataInfoId",
                        column: x => x.UserDataInfoId,
                        principalTable: "SYSTEM_USERS_DATA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SYSTEM_USERS_SYSTEM_USERS_PREFERENCES_UserPreferencesId",
                        column: x => x.UserPreferencesId,
                        principalTable: "SYSTEM_USERS_PREFERENCES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_RATINGS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CommentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_RATINGS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SPONSOR_RATINGS_SYSTEM_SPONSORS_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "SYSTEM_SPONSORS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPONSOR_RATINGS_SYSTEM_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "SYSTEM_USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_BLACKLISTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserId1 = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_BLACKLISTS", x => new { x.Id, x.UserId });
                    table.ForeignKey(
                        name: "FK_USER_BLACKLISTS_SYSTEM_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "SYSTEM_USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_EVENTS",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_EVENTS", x => new { x.UserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_USER_EVENTS_SPONSOR_EVENTS_EventId",
                        column: x => x.EventId,
                        principalTable: "SPONSOR_EVENTS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_EVENTS_SYSTEM_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "SYSTEM_USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_NOTIFICATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserId1 = table.Column<long>(type: "bigint", nullable: false),
                    IsLikedBack = table.Column<bool>(type: "boolean", nullable: false),
                    Severity = table.Column<short>(type: "smallint", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_NOTIFICATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_NOTIFICATIONS_SYSTEM_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "SYSTEM_USERS",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_USER_NOTIFICATIONS_SYSTEM_USERS_UserId1",
                        column: x => x.UserId1,
                        principalTable: "SYSTEM_USERS",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CITIES_CountryClassLocalisationId_CountryId",
                table: "CITIES",
                columns: new[] { "CountryClassLocalisationId", "CountryId" });

            migrationBuilder.CreateIndex(
                name: "IX_INTELLECTUAL_TESTS_ANSWERS_IntellectualTestQuestionId",
                table: "INTELLECTUAL_TESTS_ANSWERS",
                column: "IntellectualTestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_INTELLECTUAL_TESTS_QUESTIONS_IntellectualTestId",
                table: "INTELLECTUAL_TESTS_QUESTIONS",
                column: "IntellectualTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LANGUAGES_ClassLocalisationId",
                table: "LANGUAGES",
                column: "ClassLocalisationId");

            migrationBuilder.CreateIndex(
                name: "IX_PSYCHOLOGICAL_TESTS_ANSWERS_PsychologicalTestQuestionId",
                table: "PSYCHOLOGICAL_TESTS_ANSWERS",
                column: "PsychologicalTestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_PSYCHOLOGICAL_TESTS_QUESTIONS_PsychologicalTestId",
                table: "PSYCHOLOGICAL_TESTS_QUESTIONS",
                column: "PsychologicalTestId");

            migrationBuilder.CreateIndex(
                name: "IX_SECONDARY_LOCALISATIONS_LocalisationId_LocalisationSectionId",
                table: "SECONDARY_LOCALISATIONS",
                columns: new[] { "LocalisationId", "LocalisationSectionId" });

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_ADS_SponsorId",
                table: "SPONSOR_ADS",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_EVENTS_SponsorId",
                table: "SPONSOR_EVENTS",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_LANGUAGES_LanguageClassLocalisationId_LanguageId",
                table: "SPONSOR_LANGUAGES",
                columns: new[] { "LanguageClassLocalisationId", "LanguageId" });

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_LANGUAGES_SponsorId",
                table: "SPONSOR_LANGUAGES",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_NOTIFICATIONS_SponsorId",
                table: "SPONSOR_NOTIFICATIONS",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_RATINGS_SponsorId",
                table: "SPONSOR_RATINGS",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_RATINGS_UserId",
                table: "SPONSOR_RATINGS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_FEEDBACKS_ReasonId_ReasonClassLocalisationId",
                table: "SYSTEM_FEEDBACKS",
                columns: new[] { "ReasonId", "ReasonClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_FEEDBACKS_UserBaseInfoId",
                table: "SYSTEM_FEEDBACKS",
                column: "UserBaseInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_GENDERS_ClassLocalisationId",
                table: "SYSTEM_GENDERS",
                column: "ClassLocalisationId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_SPONSORS_ContactInfoId",
                table: "SYSTEM_SPONSORS",
                column: "ContactInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_SPONSORS_StatsId",
                table: "SYSTEM_SPONSORS",
                column: "StatsId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_USERS_UserBaseInfoId",
                table: "SYSTEM_USERS",
                column: "UserBaseInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_USERS_UserDataInfoId",
                table: "SYSTEM_USERS",
                column: "UserDataInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_USERS_UserPreferencesId",
                table: "SYSTEM_USERS",
                column: "UserPreferencesId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_USERS_DATA_LocationId",
                table: "SYSTEM_USERS_DATA",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_USERS_DATA_ReasonId_ReasonClassLocalisationId",
                table: "SYSTEM_USERS_DATA",
                columns: new[] { "ReasonId", "ReasonClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_UpdateCountry_ClassLocalisationId",
                table: "UpdateCountry",
                column: "ClassLocalisationId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ACHIEVEMENTS_AchievementId_AchievementClassLocalisatio~",
                table: "USER_ACHIEVEMENTS",
                columns: new[] { "AchievementId", "AchievementClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_BLACKLISTS_UserId",
                table: "USER_BLACKLISTS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_EVENTS_EventId",
                table: "USER_EVENTS",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_LOCATIONS_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS",
                columns: new[] { "CityCountryClassLocalisationId", "CityId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_LOCATIONS_CountryClassLocalisationId_CountryId",
                table: "USER_LOCATIONS",
                columns: new[] { "CountryClassLocalisationId", "CountryId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_NOTIFICATIONS_UserId",
                table: "USER_NOTIFICATIONS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_NOTIFICATIONS_UserId1",
                table: "USER_NOTIFICATIONS",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_USER_REPORTS_ReasonId_ReasonClassLocalisationId",
                table: "USER_REPORTS",
                columns: new[] { "ReasonId", "ReasonClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_REPORTS_UserBaseInfoId",
                table: "USER_REPORTS",
                column: "UserBaseInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_REPORTS_UserBaseInfoId1",
                table: "USER_REPORTS",
                column: "UserBaseInfoId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AGE_PREFERENCES");

            migrationBuilder.DropTable(
                name: "APP_LANGUAGES");

            migrationBuilder.DropTable(
                name: "COMMUNICATION_PREFERENCES");

            migrationBuilder.DropTable(
                name: "INTELLECTUAL_TESTS_ANSWERS");

            migrationBuilder.DropTable(
                name: "PSYCHOLOGICAL_TESTS_ANSWERS");

            migrationBuilder.DropTable(
                name: "SECONDARY_LOCALISATIONS");

            migrationBuilder.DropTable(
                name: "SPONSOR_ADS");

            migrationBuilder.DropTable(
                name: "SPONSOR_LANGUAGES");

            migrationBuilder.DropTable(
                name: "SPONSOR_NOTIFICATIONS");

            migrationBuilder.DropTable(
                name: "SPONSOR_RATINGS");

            migrationBuilder.DropTable(
                name: "SYSTEM_ADMINS");

            migrationBuilder.DropTable(
                name: "SYSTEM_FEEDBACKS");

            migrationBuilder.DropTable(
                name: "SYSTEM_GENDERS");

            migrationBuilder.DropTable(
                name: "UpdateCountry");

            migrationBuilder.DropTable(
                name: "USER_ACHIEVEMENTS");

            migrationBuilder.DropTable(
                name: "USER_BLACKLISTS");

            migrationBuilder.DropTable(
                name: "USER_ENCOUNTERS");

            migrationBuilder.DropTable(
                name: "USER_EVENTS");

            migrationBuilder.DropTable(
                name: "USER_NOTIFICATIONS");

            migrationBuilder.DropTable(
                name: "USER_REPORTS");

            migrationBuilder.DropTable(
                name: "USER_TRUST_LEVELS");

            migrationBuilder.DropTable(
                name: "USER_VISITS");

            migrationBuilder.DropTable(
                name: "USER_WALLET_BALANCES");

            migrationBuilder.DropTable(
                name: "USER_WALLET_PURCHASES");

            migrationBuilder.DropTable(
                name: "INTELLECTUAL_TESTS_QUESTIONS");

            migrationBuilder.DropTable(
                name: "PSYCHOLOGICAL_TESTS_QUESTIONS");

            migrationBuilder.DropTable(
                name: "LOCALISATIONS");

            migrationBuilder.DropTable(
                name: "LANGUAGES");

            migrationBuilder.DropTable(
                name: "FEEDBACK_REASONS");

            migrationBuilder.DropTable(
                name: "SYSTEM_ACHIEVEMENTS");

            migrationBuilder.DropTable(
                name: "SPONSOR_EVENTS");

            migrationBuilder.DropTable(
                name: "SYSTEM_USERS");

            migrationBuilder.DropTable(
                name: "REPORT_REASONS");

            migrationBuilder.DropTable(
                name: "INTELLECTUAL_TESTS");

            migrationBuilder.DropTable(
                name: "PSYCHOLOGICAL_TESTS");

            migrationBuilder.DropTable(
                name: "CLASS_LOCALISATIONS");

            migrationBuilder.DropTable(
                name: "SYSTEM_SPONSORS");

            migrationBuilder.DropTable(
                name: "SYSTEM_USERS_BASES");

            migrationBuilder.DropTable(
                name: "SYSTEM_USERS_DATA");

            migrationBuilder.DropTable(
                name: "SYSTEM_USERS_PREFERENCES");

            migrationBuilder.DropTable(
                name: "SPONSOR_CONTACT_INFO");

            migrationBuilder.DropTable(
                name: "SPONSOR_STATS");

            migrationBuilder.DropTable(
                name: "USER_LOCATIONS");

            migrationBuilder.DropTable(
                name: "USER_REASONS");

            migrationBuilder.DropTable(
                name: "CITIES");

            migrationBuilder.DropTable(
                name: "COUNTRIES");
        }
    }
}
