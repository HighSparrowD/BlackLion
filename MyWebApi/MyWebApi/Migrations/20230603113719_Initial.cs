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
            migrationBuilder.CreateTable(
                name: "adventure_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => new { x.Id, x.ClassLocalisationId });
                });

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
                name: "hints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Section = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hints", x => new { x.Id, x.ClassLocalisationId });
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
                name: "promo_codes",
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
                    table.PrimaryKey("PK_promo_codes", x => x.Id);
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
                name: "SPONSOR_EVENT_TEMPLATES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Languages = table.Column<List<int?>>(type: "integer[]", nullable: true),
                    MinAge = table.Column<short>(type: "smallint", nullable: false),
                    MaxAge = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Bounty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_EVENT_TEMPLATES", x => x.Id);
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
                name: "tests",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TestType = table.Column<short>(type: "smallint", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    CanBePassedInDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests", x => new { x.Id, x.ClassLocalisationId });
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
                name: "USER_NOTIFICATIONS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    UserId1 = table.Column<long>(type: "bigint", nullable: false),
                    IsLikedBack = table.Column<bool>(type: "boolean", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Section = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_NOTIFICATIONS", x => x.Id);
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
                    LevelOfSense = table.Column<int>(type: "integer", nullable: false),
                    Intellect = table.Column<int>(type: "integer", nullable: false),
                    Nature = table.Column<int>(type: "integer", nullable: false),
                    Creativity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_PERSONALITY_STATS", x => x.UserId);
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_USER_WALLET_BALANCES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER_WALLET_PURCHASES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PointInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_WALLET_PURCHASES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users_data",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserLanguages = table.Column<List<int>>(type: "integer[]", nullable: true),
                    UserAge = table.Column<int>(type: "integer", nullable: false),
                    UserGender = table.Column<short>(type: "smallint", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    AutoReplyText = table.Column<string>(type: "text", nullable: true),
                    AutoReplyVoice = table.Column<string>(type: "text", nullable: true),
                    LanguagePreferences = table.Column<List<int>>(type: "integer[]", nullable: true),
                    LocationPreferences = table.Column<List<int>>(type: "integer[]", nullable: true),
                    AgePrefs = table.Column<List<int>>(type: "integer[]", nullable: true),
                    CommunicationPrefs = table.Column<int>(type: "integer", nullable: false),
                    UserGenderPrefs = table.Column<short>(type: "smallint", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
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
                name: "users_settings",
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
                    table.PrimaryKey("PK_users_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LANGUAGES",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    LanguageName = table.Column<string>(type: "text", nullable: true),
                    LanguageNameNative = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<short>(type: "smallint", nullable: true),
                    ClassLocalizationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LANGUAGES", x => new { x.Id, x.ClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_LANGUAGES_CLASS_LOCALISATIONS_ClassLocalizationId",
                        column: x => x.ClassLocalizationId,
                        principalTable: "CLASS_LOCALISATIONS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_GENDERS",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    GenderName = table.Column<string>(type: "text", nullable: true),
                    ClassLocalizationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SYSTEM_GENDERS", x => new { x.Id, x.ClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_SYSTEM_GENDERS_CLASS_LOCALISATIONS_ClassLocalizationId",
                        column: x => x.ClassLocalizationId,
                        principalTable: "CLASS_LOCALISATIONS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UpdateCountry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CountryName = table.Column<string>(type: "text", nullable: true),
                    ClassLocalizationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateCountry", x => new { x.Id, x.ClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_UpdateCountry_CLASS_LOCALISATIONS_ClassLocalizationId",
                        column: x => x.ClassLocalizationId,
                        principalTable: "CLASS_LOCALISATIONS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    CountryClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    CityName = table.Column<string>(type: "text", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => new { x.Id, x.CountryClassLocalisationId });
                    table.ForeignKey(
                        name: "FK_cities_countries_CountryClassLocalisationId_CountryId",
                        columns: x => new { x.CountryClassLocalisationId, x.CountryId },
                        principalTable: "countries",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
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
                name: "SECONDARY_LOCALISATIONS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocalisationId = table.Column<int>(type: "integer", nullable: false),
                    LocalisationSectionId = table.Column<int>(type: "integer", nullable: false),
                    ElementName = table.Column<string>(type: "text", nullable: true),
                    ElementText = table.Column<string>(type: "text", nullable: true),
                    LocalizationId = table.Column<int>(type: "integer", nullable: true),
                    LocalizationSectionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECONDARY_LOCALISATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SECONDARY_LOCALISATIONS_LOCALISATIONS_LocalizationId_Locali~",
                        columns: x => new { x.LocalizationId, x.LocalizationSectionId },
                        principalTable: "LOCALISATIONS",
                        principalColumns: new[] { "Id", "SectionId" });
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
                name: "tests_questions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    TestClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tests_questions_tests_TestId_TestClassLocalisationId",
                        columns: x => new { x.TestId, x.TestClassLocalisationId },
                        principalTable: "tests",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tests_results",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    TestClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tests_results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tests_results_tests_TestId_TestClassLocalisationId",
                        columns: x => new { x.TestId, x.TestClassLocalisationId },
                        principalTable: "tests",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_LOCATIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    CityCountryClassLocalisationId = table.Column<int>(type: "integer", nullable: true),
                    CountryClassLocalisationId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_LOCATIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_LOCATIONS_cities_CityCountryClassLocalisationId_CityId",
                        columns: x => new { x.CityCountryClassLocalisationId, x.CityId },
                        principalTable: "cities",
                        principalColumns: new[] { "Id", "CountryClassLocalisationId" });
                    table.ForeignKey(
                        name: "FK_USER_LOCATIONS_countries_CountryClassLocalisationId_Country~",
                        columns: x => new { x.CountryClassLocalisationId, x.CountryId },
                        principalTable: "countries",
                        principalColumns: new[] { "Id", "ClassLocalisationId" });
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
                name: "Users",
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
                    IsDecoy = table.Column<bool>(type: "boolean", nullable: false),
                    UserSettingsId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_USER_LOCATIONS_LocationId",
                        column: x => x.LocationId,
                        principalTable: "USER_LOCATIONS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_users_data_DataId",
                        column: x => x.DataId,
                        principalTable: "users_data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_users_settings_UserSettingsId",
                        column: x => x.UserSettingsId,
                        principalTable: "users_settings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "adventures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                        name: "FK_adventures_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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
                        name: "FK_SPONSOR_RATINGS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SYSTEM_FEEDBACKS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
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
                        name: "FK_SYSTEM_FEEDBACKS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tick_requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                        name: "FK_tick_requests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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
                    AchievementClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
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
                        name: "FK_USER_ACHIEVEMENTS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "USER_BLACKLISTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    BannedUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_BLACKLISTS", x => new { x.Id, x.UserId });
                    table.ForeignKey(
                        name: "FK_USER_BLACKLISTS_Users_BannedUserId",
                        column: x => x.BannedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_ENCOUNTERS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    EncounteredUserId = table.Column<long>(type: "bigint", nullable: false),
                    SectionId = table.Column<int>(type: "integer", nullable: false),
                    EncounterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ENCOUNTERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_ENCOUNTERS_Users_EncounteredUserId",
                        column: x => x.EncounteredUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
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
                        name: "FK_USER_EVENTS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_USER_INVITATION_CREDENTIALS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USER_REPORTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserId1 = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Reason = table.Column<short>(type: "smallint", nullable: false),
                    InsertedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_REPORTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_USER_REPORTS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_REPORTS_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tags",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Tag = table.Column<string>(type: "text", nullable: false),
                    TagType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tags", x => new { x.UserId, x.Tag });
                    table.ForeignKey(
                        name: "FK_user_tags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tests",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TestId = table.Column<long>(type: "bigint", nullable: false),
                    TestClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    TestType = table.Column<short>(type: "smallint", nullable: false),
                    Result = table.Column<double>(type: "double precision", nullable: false),
                    PassedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Tags = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tests", x => new { x.TestId, x.UserId });
                    table.ForeignKey(
                        name: "FK_user_tests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_tests_tests_TestId_TestClassLocalisationId",
                        columns: x => new { x.TestId, x.TestClassLocalisationId },
                        principalTable: "tests",
                        principalColumns: new[] { "Id", "ClassLocalisationId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "adventure_attendees",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AdventureId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "IX_adventure_attendees_AdventureId",
                table: "adventure_attendees",
                column: "AdventureId");

            migrationBuilder.CreateIndex(
                name: "IX_adventures_UserId",
                table: "adventures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_cities_CountryClassLocalisationId_CountryId",
                table: "cities",
                columns: new[] { "CountryClassLocalisationId", "CountryId" });

            migrationBuilder.CreateIndex(
                name: "IX_LANGUAGES_ClassLocalizationId",
                table: "LANGUAGES",
                column: "ClassLocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SECONDARY_LOCALISATIONS_LocalizationId_LocalizationSectionId",
                table: "SECONDARY_LOCALISATIONS",
                columns: new[] { "LocalizationId", "LocalizationSectionId" });

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
                name: "IX_SYSTEM_FEEDBACKS_UserId",
                table: "SYSTEM_FEEDBACKS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_GENDERS_ClassLocalizationId",
                table: "SYSTEM_GENDERS",
                column: "ClassLocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_SPONSORS_ContactInfoId",
                table: "SYSTEM_SPONSORS",
                column: "ContactInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_SPONSORS_StatsId",
                table: "SYSTEM_SPONSORS",
                column: "StatsId");

            migrationBuilder.CreateIndex(
                name: "IX_tests_answers_TestQuestionId",
                table: "tests_answers",
                column: "TestQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_tests_questions_TestId_TestClassLocalisationId",
                table: "tests_questions",
                columns: new[] { "TestId", "TestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_tests_results_TestId_TestClassLocalisationId",
                table: "tests_results",
                columns: new[] { "TestId", "TestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_tick_requests_UserId",
                table: "tick_requests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateCountry_ClassLocalizationId",
                table: "UpdateCountry",
                column: "ClassLocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ACHIEVEMENTS_AchievementId_AchievementClassLocalisatio~",
                table: "USER_ACHIEVEMENTS",
                columns: new[] { "AchievementId", "AchievementClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_ACHIEVEMENTS_UserId",
                table: "USER_ACHIEVEMENTS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_BLACKLISTS_BannedUserId",
                table: "USER_BLACKLISTS",
                column: "BannedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_DAILY_TASKS_DailyTaskId_DailyTaskClassLocalisationId",
                table: "USER_DAILY_TASKS",
                columns: new[] { "DailyTaskId", "DailyTaskClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_ENCOUNTERS_EncounteredUserId",
                table: "USER_ENCOUNTERS",
                column: "EncounteredUserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_EVENTS_EventId",
                table: "USER_EVENTS",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_INVITATION_CREDENTIALS_UserId",
                table: "USER_INVITATION_CREDENTIALS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_INVITATIONS_InvitorCredentialsId",
                table: "USER_INVITATIONS",
                column: "InvitorCredentialsId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_LOCATIONS_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS",
                columns: new[] { "CityCountryClassLocalisationId", "CityId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_LOCATIONS_CountryClassLocalisationId_CountryId",
                table: "USER_LOCATIONS",
                columns: new[] { "CountryClassLocalisationId", "CountryId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_REPORTS_UserId",
                table: "USER_REPORTS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_REPORTS_UserId1",
                table: "USER_REPORTS",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_user_tests_TestId_TestClassLocalisationId",
                table: "user_tests",
                columns: new[] { "TestId", "TestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_tests_UserId",
                table: "user_tests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DataId",
                table: "Users",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LocationId",
                table: "Users",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserSettingsId",
                table: "Users",
                column: "UserSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adventure_attendees");

            migrationBuilder.DropTable(
                name: "adventure_templates");

            migrationBuilder.DropTable(
                name: "AGE_PREFERENCES");

            migrationBuilder.DropTable(
                name: "APP_LANGUAGES");

            migrationBuilder.DropTable(
                name: "COMMUNICATION_PREFERENCES");

            migrationBuilder.DropTable(
                name: "DAILY_REWARDS");

            migrationBuilder.DropTable(
                name: "hints");

            migrationBuilder.DropTable(
                name: "promo_codes");

            migrationBuilder.DropTable(
                name: "SECONDARY_LOCALISATIONS");

            migrationBuilder.DropTable(
                name: "SPONSOR_ADS");

            migrationBuilder.DropTable(
                name: "SPONSOR_EVENT_TEMPLATES");

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
                name: "tests_answers");

            migrationBuilder.DropTable(
                name: "tests_results");

            migrationBuilder.DropTable(
                name: "tick_requests");

            migrationBuilder.DropTable(
                name: "UpdateCountry");

            migrationBuilder.DropTable(
                name: "USER_ACHIEVEMENTS");

            migrationBuilder.DropTable(
                name: "USER_ACTIVE_EFFECTS");

            migrationBuilder.DropTable(
                name: "USER_BLACKLISTS");

            migrationBuilder.DropTable(
                name: "USER_DAILY_TASKS");

            migrationBuilder.DropTable(
                name: "USER_ENCOUNTERS");

            migrationBuilder.DropTable(
                name: "USER_EVENTS");

            migrationBuilder.DropTable(
                name: "USER_INVITATIONS");

            migrationBuilder.DropTable(
                name: "USER_NOTIFICATIONS");

            migrationBuilder.DropTable(
                name: "USER_PERSONALITY_POINTS");

            migrationBuilder.DropTable(
                name: "USER_PERSONALITY_STATS");

            migrationBuilder.DropTable(
                name: "USER_REASONS");

            migrationBuilder.DropTable(
                name: "USER_REPORTS");

            migrationBuilder.DropTable(
                name: "user_tags");

            migrationBuilder.DropTable(
                name: "user_tests");

            migrationBuilder.DropTable(
                name: "USER_TRUST_LEVELS");

            migrationBuilder.DropTable(
                name: "USER_VISITS");

            migrationBuilder.DropTable(
                name: "USER_WALLET_BALANCES");

            migrationBuilder.DropTable(
                name: "USER_WALLET_PURCHASES");

            migrationBuilder.DropTable(
                name: "adventures");

            migrationBuilder.DropTable(
                name: "LOCALISATIONS");

            migrationBuilder.DropTable(
                name: "LANGUAGES");

            migrationBuilder.DropTable(
                name: "FEEDBACK_REASONS");

            migrationBuilder.DropTable(
                name: "tests_questions");

            migrationBuilder.DropTable(
                name: "SYSTEM_ACHIEVEMENTS");

            migrationBuilder.DropTable(
                name: "DAILY_TASKS");

            migrationBuilder.DropTable(
                name: "SPONSOR_EVENTS");

            migrationBuilder.DropTable(
                name: "USER_INVITATION_CREDENTIALS");

            migrationBuilder.DropTable(
                name: "CLASS_LOCALISATIONS");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "SYSTEM_SPONSORS");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SPONSOR_CONTACT_INFO");

            migrationBuilder.DropTable(
                name: "SPONSOR_STATS");

            migrationBuilder.DropTable(
                name: "USER_LOCATIONS");

            migrationBuilder.DropTable(
                name: "users_data");

            migrationBuilder.DropTable(
                name: "users_settings");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "countries");
        }
    }
}
