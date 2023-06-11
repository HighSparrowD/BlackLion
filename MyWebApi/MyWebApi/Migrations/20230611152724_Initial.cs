using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:fuzzystrmatch", ",,");

            migrationBuilder.CreateSequence<int>(
                name: "achievements_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "active_effects_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "ads_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "adventure_templates_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "adventures_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "balances_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "black_lists_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "encounters_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "feedbacks_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "invitations_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "notifications_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "reports_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "tick_requests_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "transactions_hilo");

            migrationBuilder.CreateSequence<int>(
                name: "user_tags_hilo");

            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Language = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    ConditionValue = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => new { x.Id, x.Language });
                });

            migrationBuilder.CreateTable(
                name: "active_effects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EffectId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresIn = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_active_effects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "adventure_templates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsOffline = table.Column<bool>(type: "boolean", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    Media = table.Column<string>(type: "text", nullable: true),
                    IsMediaPhoto = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Experience = table.Column<string>(type: "text", nullable: true),
                    AttendeesDescription = table.Column<string>(type: "text", nullable: true),
                    UnwantedAttendeesDescription = table.Column<string>(type: "text", nullable: true),
                    Gratitude = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<string>(type: "text", nullable: true),
                    Application = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    IsAutoReplyText = table.Column<bool>(type: "boolean", nullable: true),
                    AutoReply = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adventure_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "balances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    PersonalityPoints = table.Column<int>(type: "integer", nullable: false),
                    SecondChances = table.Column<int>(type: "integer", nullable: false),
                    Valentines = table.Column<int>(type: "integer", nullable: false),
                    Detectors = table.Column<int>(type: "integer", nullable: false),
                    Nullifiers = table.Column<int>(type: "integer", nullable: false),
                    CardDecksMini = table.Column<int>(type: "integer", nullable: false),
                    CardDecksPlatinum = table.Column<int>(type: "integer", nullable: false),
                    ThePersonalities = table.Column<int>(type: "integer", nullable: false),
                    PointInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Lang = table.Column<byte>(type: "smallint", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => new { x.Id, x.Lang });
                });

            migrationBuilder.CreateTable(
                name: "daily_rewards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PointReward = table.Column<int>(type: "integer", nullable: false),
                    Index = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_rewards", x => x.Id);
                });

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
                name: "feedback_reasons",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedback_reasons", x => new { x.Id, x.ClassLocalisationId });
                });

            migrationBuilder.CreateTable(
                name: "hints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Localization = table.Column<byte>(type: "smallint", nullable: false),
                    Section = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hints", x => new { x.Id, x.Localization });
                });

            migrationBuilder.CreateTable(
                name: "languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Lang = table.Column<byte>(type: "smallint", nullable: false),
                    LanguageName = table.Column<string>(type: "text", nullable: true),
                    LanguageNameNative = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_languages", x => new { x.Id, x.Lang });
                });

            migrationBuilder.CreateTable(
                name: "personality_points",
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
                    LevelOfSense = table.Column<int>(type: "integer", nullable: false),
                    LevelOfSensePercentage = table.Column<double>(type: "double precision", nullable: false),
                    Intellect = table.Column<int>(type: "integer", nullable: false),
                    IntellectPercentage = table.Column<double>(type: "double precision", nullable: false),
                    Nature = table.Column<int>(type: "integer", nullable: false),
                    NaturePercentage = table.Column<double>(type: "double precision", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false),
                    CreativityPercentage = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personality_points", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "personality_stats",
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
                    LevelOfSense = table.Column<int>(type: "integer", nullable: false),
                    Intellect = table.Column<int>(type: "integer", nullable: false),
                    Nature = table.Column<int>(type: "integer", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personality_stats", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "promocodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsedOnlyInRegistration = table.Column<bool>(type: "boolean", nullable: false),
                    Promo = table.Column<string>(type: "text", nullable: true),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    PersonalityPoints = table.Column<int>(type: "integer", nullable: false),
                    SecondChance = table.Column<int>(type: "integer", nullable: false),
                    TheValentine = table.Column<int>(type: "integer", nullable: false),
                    TheDetector = table.Column<int>(type: "integer", nullable: false),
                    Nullifier = table.Column<int>(type: "integer", nullable: false),
                    CardDeckMini = table.Column<int>(type: "integer", nullable: false),
                    CardDeckPlatinum = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promocodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sponsor_contact_info",
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
                    table.PrimaryKey("PK_sponsor_contact_info", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sponsor_stats",
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
                    table.PrimaryKey("PK_sponsor_stats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Language = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TestType = table.Column<short>(type: "smallint", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CanBePassedInDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests", x => new { x.Id, x.Language });
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PointInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "trust_levels",
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
                    table.PrimaryKey("PK_trust_levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_settings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShouldUsePersonalityFunc = table.Column<bool>(type: "boolean", nullable: false),
                    ShouldFilterUsersWithoutRealPhoto = table.Column<bool>(type: "boolean", nullable: false),
                    ShouldConsiderLanguages = table.Column<bool>(type: "boolean", nullable: false),
                    ShouldComment = table.Column<bool>(type: "boolean", nullable: false),
                    ShouldSendHints = table.Column<bool>(type: "boolean", nullable: false),
                    IncreasedFamiliarity = table.Column<bool>(type: "boolean", nullable: false),
                    IsFree = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_visits",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_visits", x => new { x.UserId, x.SectionId });
                });

            migrationBuilder.CreateTable(
                name: "users_data",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserLanguages = table.Column<List<int>>(type: "integer[]", nullable: true),
                    UserAge = table.Column<int>(type: "integer", nullable: false),
                    UserGender = table.Column<byte>(type: "smallint", nullable: false),
                    Language = table.Column<byte>(type: "smallint", nullable: false),
                    AutoReplyText = table.Column<string>(type: "text", nullable: true),
                    AutoReplyVoice = table.Column<string>(type: "text", nullable: true),
                    LanguagePreferences = table.Column<List<int>>(type: "integer[]", nullable: true),
                    LocationPreferences = table.Column<List<int>>(type: "integer[]", nullable: true),
                    AgePrefs = table.Column<List<int>>(type: "integer[]", nullable: true),
                    CommunicationPrefs = table.Column<byte>(type: "smallint", nullable: false),
                    UserGenderPrefs = table.Column<byte>(type: "smallint", nullable: false),
                    Reason = table.Column<byte>(type: "smallint", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    UserRealName = table.Column<string>(type: "text", nullable: true),
                    UserDescription = table.Column<string>(type: "text", nullable: true),
                    UserRawDescription = table.Column<string>(type: "text", nullable: true),
                    UserMedia = table.Column<string>(type: "text", nullable: true),
                    IsMediaPhoto = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_data", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CountryLang = table.Column<byte>(type: "smallint", nullable: false),
                    CityName = table.Column<string>(type: "text", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => new { x.Id, x.CountryLang });
                    table.ForeignKey(
                        name: "FK_cities_countries_CountryId_CountryLang",
                        columns: x => new { x.CountryId, x.CountryLang },
                        principalTable: "countries",
                        principalColumns: new[] { "Id", "Lang" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDailyTasks",
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
                    table.PrimaryKey("PK_UserDailyTasks", x => new { x.UserId, x.DailyTaskId });
                    table.ForeignKey(
                        name: "FK_UserDailyTasks_DailyTasks_DailyTaskId_DailyTaskClassLocalis~",
                        columns: x => new { x.DailyTaskId, x.DailyTaskClassLocalisationId },
                        principalTable: "DailyTasks",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sponsors",
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
                    table.PrimaryKey("PK_sponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sponsors_sponsor_contact_info_ContactInfoId",
                        column: x => x.ContactInfoId,
                        principalTable: "sponsor_contact_info",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_sponsors_sponsor_stats_StatsId",
                        column: x => x.StatsId,
                        principalTable: "sponsor_stats",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tests_questions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    Language = table.Column<byte>(type: "smallint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    TestLanguage = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tests_questions_tests_TestId_TestLanguage",
                        columns: x => new { x.TestId, x.TestLanguage },
                        principalTable: "tests",
                        principalColumns: new[] { "Id", "Language" });
                });

            migrationBuilder.CreateTable(
                name: "tests_results",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    Language = table.Column<byte>(type: "smallint", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    TestLanguage = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tests_results_tests_TestId_TestLanguage",
                        columns: x => new { x.TestId, x.TestLanguage },
                        principalTable: "tests",
                        principalColumns: new[] { "Id", "Language" });
                });

            migrationBuilder.CreateTable(
                name: "user_locations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    CityCountryClassLocalisationId = table.Column<byte>(type: "smallint", nullable: true),
                    CountryClassLocalisationId = table.Column<byte>(type: "smallint", nullable: true),
                    CountryLang = table.Column<byte>(type: "smallint", nullable: true),
                    CityCountryLang = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_locations_cities_CityId_CityCountryLang",
                        columns: x => new { x.CityId, x.CityCountryLang },
                        principalTable: "cities",
                        principalColumns: new[] { "Id", "CountryLang" });
                    table.ForeignKey(
                        name: "FK_user_locations_countries_CountryId_CountryLang",
                        columns: x => new { x.CountryId, x.CountryLang },
                        principalTable: "countries",
                        principalColumns: new[] { "Id", "Lang" });
                });

            migrationBuilder.CreateTable(
                name: "ads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Video = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ads_sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sponsor_languages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    Lang = table.Column<byte>(type: "smallint", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    LanguageLang = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsor_languages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sponsor_languages_languages_LanguageId_LanguageLang",
                        columns: x => new { x.LanguageId, x.LanguageLang },
                        principalTable: "languages",
                        principalColumns: new[] { "Id", "Lang" });
                    table.ForeignKey(
                        name: "FK_sponsor_languages_sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sponsor_notifications",
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
                    table.PrimaryKey("PK_sponsor_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sponsor_notifications_sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tests_answers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    TestQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests_answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tests_answers_tests_questions_TestQuestionId",
                        column: x => x.TestQuestionId,
                        principalTable: "tests_questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataId = table.Column<long>(type: "bigint", nullable: false),
                    SettingsId = table.Column<long>(type: "bigint", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    IsBusy = table.Column<bool>(type: "boolean", nullable: false),
                    IsBanned = table.Column<bool>(type: "boolean", nullable: false),
                    BanDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    HasPremium = table.Column<bool>(type: "boolean", nullable: false),
                    HadReceivedReward = table.Column<bool>(type: "boolean", nullable: false),
                    PremiumDuration = table.Column<short>(type: "smallint", nullable: true),
                    IdentityType = table.Column<int>(type: "integer", nullable: false),
                    ShouldEnhance = table.Column<bool>(type: "boolean", nullable: false),
                    ReportCount = table.Column<short>(type: "smallint", nullable: false),
                    DailyRewardPoint = table.Column<short>(type: "smallint", nullable: false),
                    BonusIndex = table.Column<double>(type: "double precision", nullable: false),
                    InvitedUsersCount = table.Column<int>(type: "integer", nullable: false),
                    InvitedUsersBonus = table.Column<double>(type: "double precision", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    ProfileViewsCount = table.Column<int>(type: "integer", nullable: false),
                    RTViewsCount = table.Column<int>(type: "integer", nullable: false),
                    MaxProfileViewsCount = table.Column<int>(type: "integer", nullable: false),
                    MaxRTViewsCount = table.Column<int>(type: "integer", nullable: false),
                    MaxTagSearchCount = table.Column<int>(type: "integer", nullable: false),
                    TagSearchesCount = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<short>(type: "smallint", nullable: true),
                    PremiumExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EnteredPromoCodes = table.Column<string>(type: "text", nullable: true),
                    IsUpdated = table.Column<bool>(type: "boolean", nullable: false),
                    IsDecoy = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_user_locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "user_locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_user_settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "user_settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_users_data_DataId",
                        column: x => x.DataId,
                        principalTable: "users_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "adventures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsOffline = table.Column<bool>(type: "boolean", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    Media = table.Column<string>(type: "text", nullable: true),
                    IsMediaPhoto = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Experience = table.Column<string>(type: "text", nullable: true),
                    AttendeesDescription = table.Column<string>(type: "text", nullable: true),
                    UnwantedAttendeesDescription = table.Column<string>(type: "text", nullable: true),
                    Gratitude = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<string>(type: "text", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<string>(type: "text", nullable: true),
                    Application = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    IsAutoReplyText = table.Column<bool>(type: "boolean", nullable: true),
                    AutoReply = table.Column<string>(type: "text", nullable: true),
                    UniqueLink = table.Column<string>(type: "text", nullable: true),
                    IsAwaiting = table.Column<bool>(type: "boolean", nullable: false),
                    GroupLink = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adventures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_adventures_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "black_lists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BannedUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_black_lists", x => new { x.Id, x.UserId });
                    table.ForeignKey(
                        name: "FK_black_lists_users_BannedUserId",
                        column: x => x.BannedUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "encounters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    EncounteredUserId = table.Column<long>(type: "bigint", nullable: false),
                    Section = table.Column<int>(type: "integer", nullable: false),
                    EncounterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_encounters_users_EncounteredUserId",
                        column: x => x.EncounteredUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_encounters_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    ReasonId = table.Column<short>(type: "smallint", nullable: false),
                    ReasonClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    InsertedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_feedbacks_feedback_reasons_ReasonId_ReasonClassLocalisation~",
                        columns: x => new { x.ReasonId, x.ReasonClassLocalisationId },
                        principalTable: "feedback_reasons",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_feedbacks_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitation_credentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true),
                    QRCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invitation_credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invitation_credentials_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserId1 = table.Column<long>(type: "bigint", nullable: false),
                    IsLikedBack = table.Column<bool>(type: "boolean", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Section = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notifications_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_notifications_users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sponsor_ratings",
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
                    table.PrimaryKey("PK_sponsor_ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sponsor_ratings_sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sponsor_ratings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tick_requests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Video = table.Column<string>(type: "text", nullable: true),
                    Circle = table.Column<string>(type: "text", nullable: true),
                    Gesture = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "user_achievements",
                columns: table => new
                {
                    AchievementId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: false),
                    AcquireMessage = table.Column<string>(type: "text", nullable: true),
                    ShortDescription = table.Column<string>(type: "text", nullable: true),
                    IsAcquired = table.Column<bool>(type: "boolean", nullable: false),
                    AchievementLanguage = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_achievements", x => new { x.UserId, x.AchievementId });
                    table.ForeignKey(
                        name: "FK_user_achievements_achievements_AchievementId_AchievementLan~",
                        columns: x => new { x.AchievementId, x.AchievementLanguage },
                        principalTable: "achievements",
                        principalColumns: new[] { "Id", "Language" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_achievements_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_reports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    SenderId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<short>(type: "smallint", nullable: false),
                    InsertedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_reports_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_reports_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tags",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Tag = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TagType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tags", x => new { x.UserId, x.Tag });
                    table.ForeignKey(
                        name: "FK_user_tags_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tests",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    Language = table.Column<byte>(type: "smallint", nullable: false),
                    TestType = table.Column<short>(type: "smallint", nullable: false),
                    Result = table.Column<double>(type: "double precision", nullable: false),
                    PassedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: true),
                    TestLanguage = table.Column<byte>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tests", x => new { x.TestId, x.UserId });
                    table.ForeignKey(
                        name: "FK_user_tests_tests_TestId_TestLanguage",
                        columns: x => new { x.TestId, x.TestLanguage },
                        principalTable: "tests",
                        principalColumns: new[] { "Id", "Language" });
                    table.ForeignKey(
                        name: "FK_user_tests_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "adventure_attendees",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AdventureId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adventure_attendees", x => new { x.UserId, x.AdventureId });
                    table.ForeignKey(
                        name: "FK_adventure_attendees_adventures_AdventureId",
                        column: x => x.AdventureId,
                        principalTable: "adventures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "invitation",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    InviterCredentialsId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedUserId = table.Column<long>(type: "bigint", nullable: false),
                    InvitationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_invitation_invitation_credentials_InviterCredentialsId",
                        column: x => x.InviterCredentialsId,
                        principalTable: "invitation_credentials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_invitation_users_InvitedUserId",
                        column: x => x.InvitedUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ads_SponsorId",
                table: "ads",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_adventure_attendees_AdventureId",
                table: "adventure_attendees",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_adventures_UserId",
                table: "adventures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_black_lists_BannedUserId",
                table: "black_lists",
                column: "BannedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_cities_CountryId_CountryLang",
                table: "cities",
                columns: new[] { "CountryId", "CountryLang" });

            migrationBuilder.CreateIndex(
                name: "IX_encounters_EncounteredUserId",
                table: "encounters",
                column: "EncounteredUserId");

            migrationBuilder.CreateIndex(
                name: "IX_encounters_UserId",
                table: "encounters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_ReasonId_ReasonClassLocalisationId",
                table: "feedbacks",
                columns: new[] { "ReasonId", "ReasonClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_feedbacks_UserId",
                table: "feedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_InvitedUserId",
                table: "invitation",
                column: "InvitedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_InviterCredentialsId",
                table: "invitation",
                column: "InviterCredentialsId");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_credentials_UserId",
                table: "invitation_credentials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserId",
                table: "notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserId1",
                table: "notifications",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_languages_LanguageId_LanguageLang",
                table: "sponsor_languages",
                columns: new[] { "LanguageId", "LanguageLang" });

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_languages_SponsorId",
                table: "sponsor_languages",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_notifications_SponsorId",
                table: "sponsor_notifications",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_ratings_SponsorId",
                table: "sponsor_ratings",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_sponsor_ratings_UserId",
                table: "sponsor_ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_sponsors_ContactInfoId",
                table: "sponsors",
                column: "ContactInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_sponsors_StatsId",
                table: "sponsors",
                column: "StatsId");

            migrationBuilder.CreateIndex(
                name: "IX_tests_answers_TestQuestionId",
                table: "tests_answers",
                column: "TestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_tests_questions_TestId_TestLanguage",
                table: "tests_questions",
                columns: new[] { "TestId", "TestLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_tests_results_TestId_TestLanguage",
                table: "tests_results",
                columns: new[] { "TestId", "TestLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_tick_requests_UserId",
                table: "tick_requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_AchievementId_AchievementLanguage",
                table: "user_achievements",
                columns: new[] { "AchievementId", "AchievementLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CityId_CityCountryLang",
                table: "user_locations",
                columns: new[] { "CityId", "CityCountryLang" });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CountryId_CountryLang",
                table: "user_locations",
                columns: new[] { "CountryId", "CountryLang" });

            migrationBuilder.CreateIndex(
                name: "IX_user_reports_SenderId",
                table: "user_reports",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_user_reports_UserId",
                table: "user_reports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_tests_TestId_TestLanguage",
                table: "user_tests",
                columns: new[] { "TestId", "TestLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_user_tests_UserId",
                table: "user_tests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDailyTasks_DailyTaskId_DailyTaskClassLocalisationId",
                table: "UserDailyTasks",
                columns: new[] { "DailyTaskId", "DailyTaskClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_users_DataId",
                table: "users",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_users_LocationId",
                table: "users",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_users_SettingsId",
                table: "users",
                column: "SettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "active_effects");

            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "ads");

            migrationBuilder.DropTable(
                name: "adventure_attendees");

            migrationBuilder.DropTable(
                name: "adventure_templates");

            migrationBuilder.DropTable(
                name: "balances");

            migrationBuilder.DropTable(
                name: "black_lists");

            migrationBuilder.DropTable(
                name: "daily_rewards");

            migrationBuilder.DropTable(
                name: "encounters");

            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "hints");

            migrationBuilder.DropTable(
                name: "invitation");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "personality_points");

            migrationBuilder.DropTable(
                name: "personality_stats");

            migrationBuilder.DropTable(
                name: "promocodes");

            migrationBuilder.DropTable(
                name: "sponsor_languages");

            migrationBuilder.DropTable(
                name: "sponsor_notifications");

            migrationBuilder.DropTable(
                name: "sponsor_ratings");

            migrationBuilder.DropTable(
                name: "tests_answers");

            migrationBuilder.DropTable(
                name: "tests_results");

            migrationBuilder.DropTable(
                name: "tick_requests");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropTable(
                name: "trust_levels");

            migrationBuilder.DropTable(
                name: "user_achievements");

            migrationBuilder.DropTable(
                name: "user_reports");

            migrationBuilder.DropTable(
                name: "user_tags");

            migrationBuilder.DropTable(
                name: "user_tests");

            migrationBuilder.DropTable(
                name: "user_visits");

            migrationBuilder.DropTable(
                name: "UserDailyTasks");

            migrationBuilder.DropTable(
                name: "adventures");

            migrationBuilder.DropTable(
                name: "feedback_reasons");

            migrationBuilder.DropTable(
                name: "invitation_credentials");

            migrationBuilder.DropTable(
                name: "languages");

            migrationBuilder.DropTable(
                name: "sponsors");

            migrationBuilder.DropTable(
                name: "tests_questions");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "DailyTasks");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "sponsor_contact_info");

            migrationBuilder.DropTable(
                name: "sponsor_stats");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "user_locations");

            migrationBuilder.DropTable(
                name: "user_settings");

            migrationBuilder.DropTable(
                name: "users_data");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropSequence(
                name: "achievements_hilo");

            migrationBuilder.DropSequence(
                name: "active_effects_hilo");

            migrationBuilder.DropSequence(
                name: "ads_hilo");

            migrationBuilder.DropSequence(
                name: "adventure_templates_hilo");

            migrationBuilder.DropSequence(
                name: "adventures_hilo");

            migrationBuilder.DropSequence(
                name: "balances_hilo");

            migrationBuilder.DropSequence(
                name: "black_lists_hilo");

            migrationBuilder.DropSequence(
                name: "encounters_hilo");

            migrationBuilder.DropSequence(
                name: "feedbacks_hilo");

            migrationBuilder.DropSequence(
                name: "invitations_hilo");

            migrationBuilder.DropSequence(
                name: "notifications_hilo");

            migrationBuilder.DropSequence(
                name: "reports_hilo");

            migrationBuilder.DropSequence(
                name: "tick_requests_hilo");

            migrationBuilder.DropSequence(
                name: "transactions_hilo");

            migrationBuilder.DropSequence(
                name: "user_tags_hilo");
        }
    }
}
