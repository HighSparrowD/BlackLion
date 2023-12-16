using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class sponsorentititesremoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "advertisements");

            migrationBuilder.DropTable(
                name: "sponsor_languages");

            migrationBuilder.DropTable(
                name: "sponsor_notifications");

            migrationBuilder.DropTable(
                name: "sponsor_ratings");

            migrationBuilder.DropTable(
                name: "sponsors");

            migrationBuilder.DropTable(
                name: "sponsor_contact_info");

            migrationBuilder.DropTable(
                name: "sponsor_stats");

            migrationBuilder.DropSequence(
                name: "ads_hilo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "ads_hilo");

            migrationBuilder.CreateTable(
                name: "sponsor_contact_info",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Facebook = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Instagram = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    Tel = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
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
                    AverageRating = table.Column<double>(type: "double precision", nullable: true),
                    ConductedEventsCount = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    LevelGoal = table.Column<double>(type: "double precision", nullable: false),
                    LevelProgress = table.Column<double>(type: "double precision", nullable: false),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sponsor_stats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sponsors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactInfoId = table.Column<long>(type: "bigint", nullable: true),
                    StatsId = table.Column<long>(type: "bigint", nullable: true),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    CodeWord = table.Column<string>(type: "text", nullable: true),
                    HasBaseAccount = table.Column<bool>(type: "boolean", nullable: false),
                    IsAwaiting = table.Column<bool>(type: "boolean", nullable: false),
                    IsPostponed = table.Column<bool>(type: "boolean", nullable: false),
                    UserAppLanguage = table.Column<int>(type: "integer", nullable: false),
                    UserCityId = table.Column<int>(type: "integer", nullable: false),
                    UserCountryId = table.Column<int>(type: "integer", nullable: false),
                    UserMaxAdCount = table.Column<int>(type: "integer", nullable: false),
                    UserMaxAdViewCount = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: true)
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
                name: "advertisements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Media = table.Column<string>(type: "text", nullable: true),
                    MediaType = table.Column<short>(type: "smallint", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Show = table.Column<bool>(type: "boolean", nullable: false),
                    SponsorId = table.Column<long>(type: "bigint", nullable: true),
                    TargetAudience = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: true),
                    Updated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_advertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_advertisements_sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "sponsors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_advertisements_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sponsor_languages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    LanguageLang = table.Column<byte>(type: "smallint", nullable: true),
                    Lang = table.Column<byte>(type: "smallint", nullable: false),
                    Level = table.Column<short>(type: "smallint", nullable: false),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false)
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
                    Description = table.Column<string>(type: "text", nullable: true),
                    NotificationReason = table.Column<short>(type: "smallint", nullable: false)
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
                name: "sponsor_ratings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CommentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rating = table.Column<short>(type: "smallint", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_advertisements_SponsorId",
                table: "advertisements",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_advertisements_UserId",
                table: "advertisements",
                column: "UserId");

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
        }
    }
}
