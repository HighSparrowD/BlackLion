using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Cleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adventures_Users_UserId",
                table: "adventures");

            migrationBuilder.DropForeignKey(
                name: "FK_cities_countries_CountryClassLocalisationId_CountryId",
                table: "cities");

            migrationBuilder.DropForeignKey(
                name: "FK_LANGUAGES_CLASS_LOCALISATIONS_ClassLocalizationId",
                table: "LANGUAGES");

            migrationBuilder.DropForeignKey(
                name: "FK_SECONDARY_LOCALISATIONS_LOCALISATIONS_LocalizationId_Locali~",
                table: "SECONDARY_LOCALISATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_SPONSOR_ADS_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_ADS");

            migrationBuilder.DropForeignKey(
                name: "FK_SPONSOR_LANGUAGES_LANGUAGES_LanguageClassLocalisationId_Lan~",
                table: "SPONSOR_LANGUAGES");

            migrationBuilder.DropForeignKey(
                name: "FK_SPONSOR_LANGUAGES_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_LANGUAGES");

            migrationBuilder.DropForeignKey(
                name: "FK_SPONSOR_NOTIFICATIONS_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_NOTIFICATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_SPONSOR_RATINGS_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_RATINGS");

            migrationBuilder.DropForeignKey(
                name: "FK_SPONSOR_RATINGS_Users_UserId",
                table: "SPONSOR_RATINGS");

            migrationBuilder.DropForeignKey(
                name: "FK_SYSTEM_FEEDBACKS_FEEDBACK_REASONS_ReasonId_ReasonClassLocal~",
                table: "SYSTEM_FEEDBACKS");

            migrationBuilder.DropForeignKey(
                name: "FK_SYSTEM_FEEDBACKS_Users_UserId",
                table: "SYSTEM_FEEDBACKS");

            migrationBuilder.DropForeignKey(
                name: "FK_SYSTEM_SPONSORS_SPONSOR_CONTACT_INFO_ContactInfoId",
                table: "SYSTEM_SPONSORS");

            migrationBuilder.DropForeignKey(
                name: "FK_SYSTEM_SPONSORS_SPONSOR_STATS_StatsId",
                table: "SYSTEM_SPONSORS");

            migrationBuilder.DropForeignKey(
                name: "FK_tests_questions_tests_TestId_TestClassLocalisationId",
                table: "tests_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_tests_results_tests_TestId_TestClassLocalisationId",
                table: "tests_results");

            migrationBuilder.DropForeignKey(
                name: "FK_tick_requests_Users_UserId",
                table: "tick_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_UpdateCountry_CLASS_LOCALISATIONS_ClassLocalizationId",
                table: "UpdateCountry");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_ACHIEVEMENTS_SYSTEM_ACHIEVEMENTS_AchievementId_Achieve~",
                table: "USER_ACHIEVEMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_ACHIEVEMENTS_Users_UserId",
                table: "USER_ACHIEVEMENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_BLACKLISTS_Users_BannedUserId",
                table: "USER_BLACKLISTS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_DAILY_TASKS_DAILY_TASKS_DailyTaskId_DailyTaskClassLoca~",
                table: "USER_DAILY_TASKS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_ENCOUNTERS_Users_EncounteredUserId",
                table: "USER_ENCOUNTERS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_INVITATION_CREDENTIALS_Users_UserId",
                table: "USER_INVITATION_CREDENTIALS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_INVITATIONS_USER_INVITATION_CREDENTIALS_InvitorCredent~",
                table: "USER_INVITATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_LOCATIONS_cities_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_LOCATIONS_countries_CountryClassLocalisationId_Country~",
                table: "USER_LOCATIONS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_REPORTS_Users_UserId",
                table: "USER_REPORTS");

            migrationBuilder.DropForeignKey(
                name: "FK_USER_REPORTS_Users_UserId1",
                table: "USER_REPORTS");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tags_Users_UserId",
                table: "user_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tests_Users_UserId",
                table: "user_tests");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tests_tests_TestId_TestClassLocalisationId",
                table: "user_tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_USER_LOCATIONS_LocationId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_users_data_DataId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_users_settings_UserSettingsId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AGE_PREFERENCES");

            migrationBuilder.DropTable(
                name: "APP_LANGUAGES");

            migrationBuilder.DropTable(
                name: "COMMUNICATION_PREFERENCES");

            migrationBuilder.DropTable(
                name: "SPONSOR_EVENT_TEMPLATES");

            migrationBuilder.DropTable(
                name: "SYSTEM_GENDERS");

            migrationBuilder.DropTable(
                name: "USER_EVENTS");

            migrationBuilder.DropTable(
                name: "USER_WALLET_PURCHASES");

            migrationBuilder.DropTable(
                name: "SPONSOR_EVENTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_VISITS",
                table: "USER_VISITS");

            migrationBuilder.DropIndex(
                name: "IX_user_tests_TestId_TestClassLocalisationId",
                table: "user_tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_REPORTS",
                table: "USER_REPORTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_LOCATIONS",
                table: "USER_LOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_USER_LOCATIONS_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS");

            migrationBuilder.DropIndex(
                name: "IX_USER_LOCATIONS_CountryClassLocalisationId_CountryId",
                table: "USER_LOCATIONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_ACHIEVEMENTS",
                table: "USER_ACHIEVEMENTS");

            migrationBuilder.DropIndex(
                name: "IX_USER_ACHIEVEMENTS_AchievementId_AchievementClassLocalisatio~",
                table: "USER_ACHIEVEMENTS");

            migrationBuilder.DropIndex(
                name: "IX_tests_results_TestId_TestClassLocalisationId",
                table: "tests_results");

            migrationBuilder.DropIndex(
                name: "IX_tests_questions_TestId_TestClassLocalisationId",
                table: "tests_questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tests",
                table: "tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SPONSOR_STATS",
                table: "SPONSOR_STATS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SPONSOR_RATINGS",
                table: "SPONSOR_RATINGS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SPONSOR_NOTIFICATIONS",
                table: "SPONSOR_NOTIFICATIONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SPONSOR_LANGUAGES",
                table: "SPONSOR_LANGUAGES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SPONSOR_CONTACT_INFO",
                table: "SPONSOR_CONTACT_INFO");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LANGUAGES",
                table: "LANGUAGES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hints",
                table: "hints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FEEDBACK_REASONS",
                table: "FEEDBACK_REASONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DAILY_REWARDS",
                table: "DAILY_REWARDS");

            migrationBuilder.DropIndex(
                name: "IX_cities_CountryClassLocalisationId_CountryId",
                table: "cities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users_settings",
                table: "users_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_WALLET_BALANCES",
                table: "USER_WALLET_BALANCES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_TRUST_LEVELS",
                table: "USER_TRUST_LEVELS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_REASONS",
                table: "USER_REASONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_PERSONALITY_STATS",
                table: "USER_PERSONALITY_STATS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_PERSONALITY_POINTS",
                table: "USER_PERSONALITY_POINTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_NOTIFICATIONS",
                table: "USER_NOTIFICATIONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_INVITATIONS",
                table: "USER_INVITATIONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_INVITATION_CREDENTIALS",
                table: "USER_INVITATION_CREDENTIALS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_ENCOUNTERS",
                table: "USER_ENCOUNTERS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_DAILY_TASKS",
                table: "USER_DAILY_TASKS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_BLACKLISTS",
                table: "USER_BLACKLISTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER_ACTIVE_EFFECTS",
                table: "USER_ACTIVE_EFFECTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SYSTEM_SPONSORS",
                table: "SYSTEM_SPONSORS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SYSTEM_FEEDBACKS",
                table: "SYSTEM_FEEDBACKS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SYSTEM_ADMINS",
                table: "SYSTEM_ADMINS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SYSTEM_ACHIEVEMENTS",
                table: "SYSTEM_ACHIEVEMENTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SPONSOR_ADS",
                table: "SPONSOR_ADS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SECONDARY_LOCALISATIONS",
                table: "SECONDARY_LOCALISATIONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_promo_codes",
                table: "promo_codes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LOCALISATIONS",
                table: "LOCALISATIONS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DAILY_TASKS",
                table: "DAILY_TASKS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CLASS_LOCALISATIONS",
                table: "CLASS_LOCALISATIONS");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "users_data");

            migrationBuilder.DropColumn(
                name: "TestClassLocalisationId",
                table: "user_tests");

            migrationBuilder.DropColumn(
                name: "AchievementClassLocalisationId",
                table: "USER_ACHIEVEMENTS");

            migrationBuilder.DropColumn(
                name: "TestClassLocalisationId",
                table: "tests_results");

            migrationBuilder.DropColumn(
                name: "TestClassLocalisationId",
                table: "tests_questions");

            migrationBuilder.DropColumn(
                name: "ClassLocalisationId",
                table: "tests");

            migrationBuilder.DropColumn(
                name: "ClassLocalisationId",
                table: "hints");

            migrationBuilder.DropColumn(
                name: "ClassLocalisationId",
                table: "SYSTEM_ACHIEVEMENTS");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "USER_VISITS",
                newName: "user_visits");

            migrationBuilder.RenameTable(
                name: "USER_REPORTS",
                newName: "user_reports");

            migrationBuilder.RenameTable(
                name: "USER_LOCATIONS",
                newName: "user_locations");

            migrationBuilder.RenameTable(
                name: "USER_ACHIEVEMENTS",
                newName: "user_achievements");

            migrationBuilder.RenameTable(
                name: "SPONSOR_STATS",
                newName: "sponsor_stats");

            migrationBuilder.RenameTable(
                name: "SPONSOR_RATINGS",
                newName: "sponsor_ratings");

            migrationBuilder.RenameTable(
                name: "SPONSOR_NOTIFICATIONS",
                newName: "sponsor_notifications");

            migrationBuilder.RenameTable(
                name: "SPONSOR_LANGUAGES",
                newName: "sponsor_languages");

            migrationBuilder.RenameTable(
                name: "SPONSOR_CONTACT_INFO",
                newName: "sponsor_contact_info");

            migrationBuilder.RenameTable(
                name: "LANGUAGES",
                newName: "languages");

            migrationBuilder.RenameTable(
                name: "FEEDBACK_REASONS",
                newName: "feedback_reasons");

            migrationBuilder.RenameTable(
                name: "DAILY_REWARDS",
                newName: "daily_rewards");

            migrationBuilder.RenameTable(
                name: "users_settings",
                newName: "user_settings");

            migrationBuilder.RenameTable(
                name: "USER_WALLET_BALANCES",
                newName: "balances");

            migrationBuilder.RenameTable(
                name: "USER_TRUST_LEVELS",
                newName: "trust_levels");

            migrationBuilder.RenameTable(
                name: "USER_REASONS",
                newName: "UserReason");

            migrationBuilder.RenameTable(
                name: "USER_PERSONALITY_STATS",
                newName: "personality_stats");

            migrationBuilder.RenameTable(
                name: "USER_PERSONALITY_POINTS",
                newName: "personality_points");

            migrationBuilder.RenameTable(
                name: "USER_NOTIFICATIONS",
                newName: "notifications");

            migrationBuilder.RenameTable(
                name: "USER_INVITATIONS",
                newName: "invitation");

            migrationBuilder.RenameTable(
                name: "USER_INVITATION_CREDENTIALS",
                newName: "invitation_credentials");

            migrationBuilder.RenameTable(
                name: "USER_ENCOUNTERS",
                newName: "encounters");

            migrationBuilder.RenameTable(
                name: "USER_DAILY_TASKS",
                newName: "UserDailyTasks");

            migrationBuilder.RenameTable(
                name: "USER_BLACKLISTS",
                newName: "black_lists");

            migrationBuilder.RenameTable(
                name: "USER_ACTIVE_EFFECTS",
                newName: "active_effects");

            migrationBuilder.RenameTable(
                name: "SYSTEM_SPONSORS",
                newName: "sponsors");

            migrationBuilder.RenameTable(
                name: "SYSTEM_FEEDBACKS",
                newName: "feedbacks");

            migrationBuilder.RenameTable(
                name: "SYSTEM_ADMINS",
                newName: "admins");

            migrationBuilder.RenameTable(
                name: "SYSTEM_ACHIEVEMENTS",
                newName: "achievements");

            migrationBuilder.RenameTable(
                name: "SPONSOR_ADS",
                newName: "ads");

            migrationBuilder.RenameTable(
                name: "SECONDARY_LOCALISATIONS",
                newName: "secondary_localizations");

            migrationBuilder.RenameTable(
                name: "promo_codes",
                newName: "promocodes");

            migrationBuilder.RenameTable(
                name: "LOCALISATIONS",
                newName: "localizations");

            migrationBuilder.RenameTable(
                name: "DAILY_TASKS",
                newName: "DailyTasks");

            migrationBuilder.RenameTable(
                name: "CLASS_LOCALISATIONS",
                newName: "class_localizations");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserSettingsId",
                table: "users",
                newName: "IX_users_UserSettingsId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_LocationId",
                table: "users",
                newName: "IX_users_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_DataId",
                table: "users",
                newName: "IX_users_DataId");

            migrationBuilder.RenameIndex(
                name: "IX_USER_REPORTS_UserId1",
                table: "user_reports",
                newName: "IX_user_reports_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_USER_REPORTS_UserId",
                table: "user_reports",
                newName: "IX_user_reports_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_USER_ACHIEVEMENTS_UserId",
                table: "user_achievements",
                newName: "IX_user_achievements_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SPONSOR_RATINGS_UserId",
                table: "sponsor_ratings",
                newName: "IX_sponsor_ratings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SPONSOR_RATINGS_SponsorId",
                table: "sponsor_ratings",
                newName: "IX_sponsor_ratings_SponsorId");

            migrationBuilder.RenameIndex(
                name: "IX_SPONSOR_NOTIFICATIONS_SponsorId",
                table: "sponsor_notifications",
                newName: "IX_sponsor_notifications_SponsorId");

            migrationBuilder.RenameIndex(
                name: "IX_SPONSOR_LANGUAGES_SponsorId",
                table: "sponsor_languages",
                newName: "IX_sponsor_languages_SponsorId");

            migrationBuilder.RenameIndex(
                name: "IX_SPONSOR_LANGUAGES_LanguageClassLocalisationId_LanguageId",
                table: "sponsor_languages",
                newName: "IX_sponsor_languages_LanguageClassLocalisationId_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_LANGUAGES_ClassLocalizationId",
                table: "languages",
                newName: "IX_languages_ClassLocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_USER_INVITATIONS_InvitorCredentialsId",
                table: "invitation",
                newName: "IX_invitation_InvitorCredentialsId");

            migrationBuilder.RenameIndex(
                name: "IX_USER_INVITATION_CREDENTIALS_UserId",
                table: "invitation_credentials",
                newName: "IX_invitation_credentials_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_USER_ENCOUNTERS_EncounteredUserId",
                table: "encounters",
                newName: "IX_encounters_EncounteredUserId");

            migrationBuilder.RenameIndex(
                name: "IX_USER_DAILY_TASKS_DailyTaskId_DailyTaskClassLocalisationId",
                table: "UserDailyTasks",
                newName: "IX_UserDailyTasks_DailyTaskId_DailyTaskClassLocalisationId");

            migrationBuilder.RenameIndex(
                name: "IX_USER_BLACKLISTS_BannedUserId",
                table: "black_lists",
                newName: "IX_black_lists_BannedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_SYSTEM_SPONSORS_StatsId",
                table: "sponsors",
                newName: "IX_sponsors_StatsId");

            migrationBuilder.RenameIndex(
                name: "IX_SYSTEM_SPONSORS_ContactInfoId",
                table: "sponsors",
                newName: "IX_sponsors_ContactInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_SYSTEM_FEEDBACKS_UserId",
                table: "feedbacks",
                newName: "IX_feedbacks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_SYSTEM_FEEDBACKS_ReasonId_ReasonClassLocalisationId",
                table: "feedbacks",
                newName: "IX_feedbacks_ReasonId_ReasonClassLocalisationId");

            migrationBuilder.RenameIndex(
                name: "IX_SPONSOR_ADS_SponsorId",
                table: "ads",
                newName: "IX_ads_SponsorId");

            migrationBuilder.RenameIndex(
                name: "IX_SECONDARY_LOCALISATIONS_LocalizationId_LocalizationSectionId",
                table: "secondary_localizations",
                newName: "IX_secondary_localizations_LocalizationId_LocalizationSectionId");

            migrationBuilder.AlterColumn<byte>(
                name: "Reason",
                table: "users_data",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte>(
                name: "CommunicationPrefs",
                table: "users_data",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "users_data",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "user_tests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TestLanguage",
                table: "user_tests",
                type: "smallint",
                nullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "CountryClassLocalisationId",
                table: "user_locations",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "CityCountryClassLocalisationId",
                table: "user_locations",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "AchievementLanguage",
                table: "user_achievements",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "user_achievements",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "tests_results",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TestLanguage",
                table: "tests_results",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "tests_questions",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TestLanguage",
                table: "tests_questions",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "tests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Localization",
                table: "hints",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<byte>(
                name: "ClassLocalisationId",
                table: "countries",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte>(
                name: "CountryClassLocalisationId",
                table: "cities",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<byte>(
                name: "Language",
                table: "achievements",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_visits",
                table: "user_visits",
                columns: new[] { "UserId", "SectionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_reports",
                table: "user_reports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_locations",
                table: "user_locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_achievements",
                table: "user_achievements",
                columns: new[] { "UserBaseInfoId", "AchievementId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_tests",
                table: "tests",
                columns: new[] { "Id", "Language" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_sponsor_stats",
                table: "sponsor_stats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sponsor_ratings",
                table: "sponsor_ratings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sponsor_notifications",
                table: "sponsor_notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sponsor_languages",
                table: "sponsor_languages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sponsor_contact_info",
                table: "sponsor_contact_info",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_languages",
                table: "languages",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_hints",
                table: "hints",
                columns: new[] { "Id", "Localization" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_feedback_reasons",
                table: "feedback_reasons",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_daily_rewards",
                table: "daily_rewards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_settings",
                table: "user_settings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_balances",
                table: "balances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trust_levels",
                table: "trust_levels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserReason",
                table: "UserReason",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_personality_stats",
                table: "personality_stats",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_personality_points",
                table: "personality_points",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notifications",
                table: "notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invitation",
                table: "invitation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invitation_credentials",
                table: "invitation_credentials",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_encounters",
                table: "encounters",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDailyTasks",
                table: "UserDailyTasks",
                columns: new[] { "UserId", "DailyTaskId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_black_lists",
                table: "black_lists",
                columns: new[] { "Id", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_active_effects",
                table: "active_effects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sponsors",
                table: "sponsors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_feedbacks",
                table: "feedbacks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_admins",
                table: "admins",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_achievements",
                table: "achievements",
                columns: new[] { "Id", "Language" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ads",
                table: "ads",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_secondary_localizations",
                table: "secondary_localizations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_promocodes",
                table: "promocodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_localizations",
                table: "localizations",
                columns: new[] { "Id", "SectionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyTasks",
                table: "DailyTasks",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_class_localizations",
                table: "class_localizations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "transactions",
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
                    table.PrimaryKey("PK_transactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_tests_TestId_TestLanguage",
                table: "user_tests",
                columns: new[] { "TestId", "TestLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CityId_CityCountryClassLocalisationId",
                table: "user_locations",
                columns: new[] { "CityId", "CityCountryClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_locations_CountryId_CountryClassLocalisationId",
                table: "user_locations",
                columns: new[] { "CountryId", "CountryClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_user_achievements_AchievementId_AchievementLanguage",
                table: "user_achievements",
                columns: new[] { "AchievementId", "AchievementLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_tests_results_TestId_TestLanguage",
                table: "tests_results",
                columns: new[] { "TestId", "TestLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_tests_questions_TestId_TestLanguage",
                table: "tests_questions",
                columns: new[] { "TestId", "TestLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_cities_CountryId_CountryClassLocalisationId",
                table: "cities",
                columns: new[] { "CountryId", "CountryClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ads_sponsors_SponsorId",
                table: "ads",
                column: "SponsorId",
                principalTable: "sponsors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_adventures_users_UserId",
                table: "adventures",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_black_lists_users_BannedUserId",
                table: "black_lists",
                column: "BannedUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cities_countries_CountryId_CountryClassLocalisationId",
                table: "cities",
                columns: new[] { "CountryId", "CountryClassLocalisationId" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_encounters_users_EncounteredUserId",
                table: "encounters",
                column: "EncounteredUserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_feedback_reasons_ReasonId_ReasonClassLocalisation~",
                table: "feedbacks",
                columns: new[] { "ReasonId", "ReasonClassLocalisationId" },
                principalTable: "feedback_reasons",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feedbacks_users_UserId",
                table: "feedbacks",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_invitation_invitation_credentials_InvitorCredentialsId",
                table: "invitation",
                column: "InvitorCredentialsId",
                principalTable: "invitation_credentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_invitation_credentials_users_UserId",
                table: "invitation_credentials",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_languages_class_localizations_ClassLocalizationId",
                table: "languages",
                column: "ClassLocalizationId",
                principalTable: "class_localizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_secondary_localizations_localizations_LocalizationId_Locali~",
                table: "secondary_localizations",
                columns: new[] { "LocalizationId", "LocalizationSectionId" },
                principalTable: "localizations",
                principalColumns: new[] { "Id", "SectionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_sponsor_languages_languages_LanguageClassLocalisationId_Lan~",
                table: "sponsor_languages",
                columns: new[] { "LanguageClassLocalisationId", "LanguageId" },
                principalTable: "languages",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sponsor_languages_sponsors_SponsorId",
                table: "sponsor_languages",
                column: "SponsorId",
                principalTable: "sponsors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sponsor_notifications_sponsors_SponsorId",
                table: "sponsor_notifications",
                column: "SponsorId",
                principalTable: "sponsors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sponsor_ratings_sponsors_SponsorId",
                table: "sponsor_ratings",
                column: "SponsorId",
                principalTable: "sponsors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sponsor_ratings_users_UserId",
                table: "sponsor_ratings",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_sponsors_sponsor_contact_info_ContactInfoId",
                table: "sponsors",
                column: "ContactInfoId",
                principalTable: "sponsor_contact_info",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sponsors_sponsor_stats_StatsId",
                table: "sponsors",
                column: "StatsId",
                principalTable: "sponsor_stats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tests_questions_tests_TestId_TestLanguage",
                table: "tests_questions",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" });

            migrationBuilder.AddForeignKey(
                name: "FK_tests_results_tests_TestId_TestLanguage",
                table: "tests_results",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" });

            migrationBuilder.AddForeignKey(
                name: "FK_tick_requests_users_UserId",
                table: "tick_requests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UpdateCountry_class_localizations_ClassLocalizationId",
                table: "UpdateCountry",
                column: "ClassLocalizationId",
                principalTable: "class_localizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_achievements_achievements_AchievementId_AchievementLan~",
                table: "user_achievements",
                columns: new[] { "AchievementId", "AchievementLanguage" },
                principalTable: "achievements",
                principalColumns: new[] { "Id", "Language" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_achievements_users_UserId",
                table: "user_achievements",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_cities_CityId_CityCountryClassLocalisationId",
                table: "user_locations",
                columns: new[] { "CityId", "CityCountryClassLocalisationId" },
                principalTable: "cities",
                principalColumns: new[] { "Id", "CountryClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_locations_countries_CountryId_CountryClassLocalisation~",
                table: "user_locations",
                columns: new[] { "CountryId", "CountryClassLocalisationId" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_reports_users_UserId",
                table: "user_reports",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_reports_users_UserId1",
                table: "user_reports",
                column: "UserId1",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_tags_users_UserId",
                table: "user_tags",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_tests_tests_TestId_TestLanguage",
                table: "user_tests",
                columns: new[] { "TestId", "TestLanguage" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "Language" });

            migrationBuilder.AddForeignKey(
                name: "FK_user_tests_users_UserId",
                table: "user_tests",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDailyTasks_DailyTasks_DailyTaskId_DailyTaskClassLocalis~",
                table: "UserDailyTasks",
                columns: new[] { "DailyTaskId", "DailyTaskClassLocalisationId" },
                principalTable: "DailyTasks",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_locations_LocationId",
                table: "users",
                column: "LocationId",
                principalTable: "user_locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_user_settings_UserSettingsId",
                table: "users",
                column: "UserSettingsId",
                principalTable: "user_settings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_data_DataId",
                table: "users",
                column: "DataId",
                principalTable: "users_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ads_sponsors_SponsorId",
                table: "ads");

            migrationBuilder.DropForeignKey(
                name: "FK_adventures_users_UserId",
                table: "adventures");

            migrationBuilder.DropForeignKey(
                name: "FK_black_lists_users_BannedUserId",
                table: "black_lists");

            migrationBuilder.DropForeignKey(
                name: "FK_cities_countries_CountryId_CountryClassLocalisationId",
                table: "cities");

            migrationBuilder.DropForeignKey(
                name: "FK_encounters_users_EncounteredUserId",
                table: "encounters");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_feedback_reasons_ReasonId_ReasonClassLocalisation~",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_feedbacks_users_UserId",
                table: "feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_invitation_invitation_credentials_InvitorCredentialsId",
                table: "invitation");

            migrationBuilder.DropForeignKey(
                name: "FK_invitation_credentials_users_UserId",
                table: "invitation_credentials");

            migrationBuilder.DropForeignKey(
                name: "FK_languages_class_localizations_ClassLocalizationId",
                table: "languages");

            migrationBuilder.DropForeignKey(
                name: "FK_secondary_localizations_localizations_LocalizationId_Locali~",
                table: "secondary_localizations");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsor_languages_languages_LanguageClassLocalisationId_Lan~",
                table: "sponsor_languages");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsor_languages_sponsors_SponsorId",
                table: "sponsor_languages");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsor_notifications_sponsors_SponsorId",
                table: "sponsor_notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsor_ratings_sponsors_SponsorId",
                table: "sponsor_ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsor_ratings_users_UserId",
                table: "sponsor_ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsors_sponsor_contact_info_ContactInfoId",
                table: "sponsors");

            migrationBuilder.DropForeignKey(
                name: "FK_sponsors_sponsor_stats_StatsId",
                table: "sponsors");

            migrationBuilder.DropForeignKey(
                name: "FK_tests_questions_tests_TestId_TestLanguage",
                table: "tests_questions");

            migrationBuilder.DropForeignKey(
                name: "FK_tests_results_tests_TestId_TestLanguage",
                table: "tests_results");

            migrationBuilder.DropForeignKey(
                name: "FK_tick_requests_users_UserId",
                table: "tick_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_UpdateCountry_class_localizations_ClassLocalizationId",
                table: "UpdateCountry");

            migrationBuilder.DropForeignKey(
                name: "FK_user_achievements_achievements_AchievementId_AchievementLan~",
                table: "user_achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_user_achievements_users_UserId",
                table: "user_achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_cities_CityId_CityCountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_user_locations_countries_CountryId_CountryClassLocalisation~",
                table: "user_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_user_reports_users_UserId",
                table: "user_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_user_reports_users_UserId1",
                table: "user_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tags_users_UserId",
                table: "user_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tests_tests_TestId_TestLanguage",
                table: "user_tests");

            migrationBuilder.DropForeignKey(
                name: "FK_user_tests_users_UserId",
                table: "user_tests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDailyTasks_DailyTasks_DailyTaskId_DailyTaskClassLocalis~",
                table: "UserDailyTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_users_user_locations_LocationId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_user_settings_UserSettingsId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_users_data_DataId",
                table: "users");

            migrationBuilder.DropTable(
                name: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_visits",
                table: "user_visits");

            migrationBuilder.DropIndex(
                name: "IX_user_tests_TestId_TestLanguage",
                table: "user_tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_reports",
                table: "user_reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_locations",
                table: "user_locations");

            migrationBuilder.DropIndex(
                name: "IX_user_locations_CityId_CityCountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.DropIndex(
                name: "IX_user_locations_CountryId_CountryClassLocalisationId",
                table: "user_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_achievements",
                table: "user_achievements");

            migrationBuilder.DropIndex(
                name: "IX_user_achievements_AchievementId_AchievementLanguage",
                table: "user_achievements");

            migrationBuilder.DropIndex(
                name: "IX_tests_results_TestId_TestLanguage",
                table: "tests_results");

            migrationBuilder.DropIndex(
                name: "IX_tests_questions_TestId_TestLanguage",
                table: "tests_questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tests",
                table: "tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sponsor_stats",
                table: "sponsor_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sponsor_ratings",
                table: "sponsor_ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sponsor_notifications",
                table: "sponsor_notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sponsor_languages",
                table: "sponsor_languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sponsor_contact_info",
                table: "sponsor_contact_info");

            migrationBuilder.DropPrimaryKey(
                name: "PK_languages",
                table: "languages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hints",
                table: "hints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_feedback_reasons",
                table: "feedback_reasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_daily_rewards",
                table: "daily_rewards");

            migrationBuilder.DropIndex(
                name: "IX_cities_CountryId_CountryClassLocalisationId",
                table: "cities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserReason",
                table: "UserReason");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDailyTasks",
                table: "UserDailyTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_settings",
                table: "user_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trust_levels",
                table: "trust_levels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sponsors",
                table: "sponsors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_secondary_localizations",
                table: "secondary_localizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_promocodes",
                table: "promocodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_personality_stats",
                table: "personality_stats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_personality_points",
                table: "personality_points");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_localizations",
                table: "localizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invitation_credentials",
                table: "invitation_credentials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invitation",
                table: "invitation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_feedbacks",
                table: "feedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_encounters",
                table: "encounters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyTasks",
                table: "DailyTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_class_localizations",
                table: "class_localizations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_black_lists",
                table: "black_lists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_balances",
                table: "balances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ads",
                table: "ads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_admins",
                table: "admins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_active_effects",
                table: "active_effects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_achievements",
                table: "achievements");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "users_data");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "user_tests");

            migrationBuilder.DropColumn(
                name: "TestLanguage",
                table: "user_tests");

            migrationBuilder.DropColumn(
                name: "AchievementLanguage",
                table: "user_achievements");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "user_achievements");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "tests_results");

            migrationBuilder.DropColumn(
                name: "TestLanguage",
                table: "tests_results");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "tests_questions");

            migrationBuilder.DropColumn(
                name: "TestLanguage",
                table: "tests_questions");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "tests");

            migrationBuilder.DropColumn(
                name: "Localization",
                table: "hints");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "achievements");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "user_visits",
                newName: "USER_VISITS");

            migrationBuilder.RenameTable(
                name: "user_reports",
                newName: "USER_REPORTS");

            migrationBuilder.RenameTable(
                name: "user_locations",
                newName: "USER_LOCATIONS");

            migrationBuilder.RenameTable(
                name: "user_achievements",
                newName: "USER_ACHIEVEMENTS");

            migrationBuilder.RenameTable(
                name: "sponsor_stats",
                newName: "SPONSOR_STATS");

            migrationBuilder.RenameTable(
                name: "sponsor_ratings",
                newName: "SPONSOR_RATINGS");

            migrationBuilder.RenameTable(
                name: "sponsor_notifications",
                newName: "SPONSOR_NOTIFICATIONS");

            migrationBuilder.RenameTable(
                name: "sponsor_languages",
                newName: "SPONSOR_LANGUAGES");

            migrationBuilder.RenameTable(
                name: "sponsor_contact_info",
                newName: "SPONSOR_CONTACT_INFO");

            migrationBuilder.RenameTable(
                name: "languages",
                newName: "LANGUAGES");

            migrationBuilder.RenameTable(
                name: "feedback_reasons",
                newName: "FEEDBACK_REASONS");

            migrationBuilder.RenameTable(
                name: "daily_rewards",
                newName: "DAILY_REWARDS");

            migrationBuilder.RenameTable(
                name: "UserReason",
                newName: "USER_REASONS");

            migrationBuilder.RenameTable(
                name: "UserDailyTasks",
                newName: "USER_DAILY_TASKS");

            migrationBuilder.RenameTable(
                name: "user_settings",
                newName: "users_settings");

            migrationBuilder.RenameTable(
                name: "trust_levels",
                newName: "USER_TRUST_LEVELS");

            migrationBuilder.RenameTable(
                name: "sponsors",
                newName: "SYSTEM_SPONSORS");

            migrationBuilder.RenameTable(
                name: "secondary_localizations",
                newName: "SECONDARY_LOCALISATIONS");

            migrationBuilder.RenameTable(
                name: "promocodes",
                newName: "promo_codes");

            migrationBuilder.RenameTable(
                name: "personality_stats",
                newName: "USER_PERSONALITY_STATS");

            migrationBuilder.RenameTable(
                name: "personality_points",
                newName: "USER_PERSONALITY_POINTS");

            migrationBuilder.RenameTable(
                name: "notifications",
                newName: "USER_NOTIFICATIONS");

            migrationBuilder.RenameTable(
                name: "localizations",
                newName: "LOCALISATIONS");

            migrationBuilder.RenameTable(
                name: "invitation_credentials",
                newName: "USER_INVITATION_CREDENTIALS");

            migrationBuilder.RenameTable(
                name: "invitation",
                newName: "USER_INVITATIONS");

            migrationBuilder.RenameTable(
                name: "feedbacks",
                newName: "SYSTEM_FEEDBACKS");

            migrationBuilder.RenameTable(
                name: "encounters",
                newName: "USER_ENCOUNTERS");

            migrationBuilder.RenameTable(
                name: "DailyTasks",
                newName: "DAILY_TASKS");

            migrationBuilder.RenameTable(
                name: "class_localizations",
                newName: "CLASS_LOCALISATIONS");

            migrationBuilder.RenameTable(
                name: "black_lists",
                newName: "USER_BLACKLISTS");

            migrationBuilder.RenameTable(
                name: "balances",
                newName: "USER_WALLET_BALANCES");

            migrationBuilder.RenameTable(
                name: "ads",
                newName: "SPONSOR_ADS");

            migrationBuilder.RenameTable(
                name: "admins",
                newName: "SYSTEM_ADMINS");

            migrationBuilder.RenameTable(
                name: "active_effects",
                newName: "USER_ACTIVE_EFFECTS");

            migrationBuilder.RenameTable(
                name: "achievements",
                newName: "SYSTEM_ACHIEVEMENTS");

            migrationBuilder.RenameIndex(
                name: "IX_users_UserSettingsId",
                table: "Users",
                newName: "IX_Users_UserSettingsId");

            migrationBuilder.RenameIndex(
                name: "IX_users_LocationId",
                table: "Users",
                newName: "IX_Users_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_users_DataId",
                table: "Users",
                newName: "IX_Users_DataId");

            migrationBuilder.RenameIndex(
                name: "IX_user_reports_UserId1",
                table: "USER_REPORTS",
                newName: "IX_USER_REPORTS_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_user_reports_UserId",
                table: "USER_REPORTS",
                newName: "IX_USER_REPORTS_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_achievements_UserId",
                table: "USER_ACHIEVEMENTS",
                newName: "IX_USER_ACHIEVEMENTS_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_sponsor_ratings_UserId",
                table: "SPONSOR_RATINGS",
                newName: "IX_SPONSOR_RATINGS_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_sponsor_ratings_SponsorId",
                table: "SPONSOR_RATINGS",
                newName: "IX_SPONSOR_RATINGS_SponsorId");

            migrationBuilder.RenameIndex(
                name: "IX_sponsor_notifications_SponsorId",
                table: "SPONSOR_NOTIFICATIONS",
                newName: "IX_SPONSOR_NOTIFICATIONS_SponsorId");

            migrationBuilder.RenameIndex(
                name: "IX_sponsor_languages_SponsorId",
                table: "SPONSOR_LANGUAGES",
                newName: "IX_SPONSOR_LANGUAGES_SponsorId");

            migrationBuilder.RenameIndex(
                name: "IX_sponsor_languages_LanguageClassLocalisationId_LanguageId",
                table: "SPONSOR_LANGUAGES",
                newName: "IX_SPONSOR_LANGUAGES_LanguageClassLocalisationId_LanguageId");

            migrationBuilder.RenameIndex(
                name: "IX_languages_ClassLocalizationId",
                table: "LANGUAGES",
                newName: "IX_LANGUAGES_ClassLocalizationId");

            migrationBuilder.RenameIndex(
                name: "IX_UserDailyTasks_DailyTaskId_DailyTaskClassLocalisationId",
                table: "USER_DAILY_TASKS",
                newName: "IX_USER_DAILY_TASKS_DailyTaskId_DailyTaskClassLocalisationId");

            migrationBuilder.RenameIndex(
                name: "IX_sponsors_StatsId",
                table: "SYSTEM_SPONSORS",
                newName: "IX_SYSTEM_SPONSORS_StatsId");

            migrationBuilder.RenameIndex(
                name: "IX_sponsors_ContactInfoId",
                table: "SYSTEM_SPONSORS",
                newName: "IX_SYSTEM_SPONSORS_ContactInfoId");

            migrationBuilder.RenameIndex(
                name: "IX_secondary_localizations_LocalizationId_LocalizationSectionId",
                table: "SECONDARY_LOCALISATIONS",
                newName: "IX_SECONDARY_LOCALISATIONS_LocalizationId_LocalizationSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_invitation_credentials_UserId",
                table: "USER_INVITATION_CREDENTIALS",
                newName: "IX_USER_INVITATION_CREDENTIALS_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_invitation_InvitorCredentialsId",
                table: "USER_INVITATIONS",
                newName: "IX_USER_INVITATIONS_InvitorCredentialsId");

            migrationBuilder.RenameIndex(
                name: "IX_feedbacks_UserId",
                table: "SYSTEM_FEEDBACKS",
                newName: "IX_SYSTEM_FEEDBACKS_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_feedbacks_ReasonId_ReasonClassLocalisationId",
                table: "SYSTEM_FEEDBACKS",
                newName: "IX_SYSTEM_FEEDBACKS_ReasonId_ReasonClassLocalisationId");

            migrationBuilder.RenameIndex(
                name: "IX_encounters_EncounteredUserId",
                table: "USER_ENCOUNTERS",
                newName: "IX_USER_ENCOUNTERS_EncounteredUserId");

            migrationBuilder.RenameIndex(
                name: "IX_black_lists_BannedUserId",
                table: "USER_BLACKLISTS",
                newName: "IX_USER_BLACKLISTS_BannedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ads_SponsorId",
                table: "SPONSOR_ADS",
                newName: "IX_SPONSOR_ADS_SponsorId");

            migrationBuilder.AlterColumn<int>(
                name: "Reason",
                table: "users_data",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "CommunicationPrefs",
                table: "users_data",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "users_data",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestClassLocalisationId",
                table: "user_tests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CountryClassLocalisationId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CityCountryClassLocalisationId",
                table: "USER_LOCATIONS",
                type: "integer",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AchievementClassLocalisationId",
                table: "USER_ACHIEVEMENTS",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestClassLocalisationId",
                table: "tests_results",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestClassLocalisationId",
                table: "tests_questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassLocalisationId",
                table: "tests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassLocalisationId",
                table: "hints",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ClassLocalisationId",
                table: "countries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "CountryClassLocalisationId",
                table: "cities",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AddColumn<int>(
                name: "ClassLocalisationId",
                table: "SYSTEM_ACHIEVEMENTS",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_VISITS",
                table: "USER_VISITS",
                columns: new[] { "UserId", "SectionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_REPORTS",
                table: "USER_REPORTS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_LOCATIONS",
                table: "USER_LOCATIONS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_ACHIEVEMENTS",
                table: "USER_ACHIEVEMENTS",
                columns: new[] { "UserBaseInfoId", "AchievementId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_tests",
                table: "tests",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SPONSOR_STATS",
                table: "SPONSOR_STATS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SPONSOR_RATINGS",
                table: "SPONSOR_RATINGS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SPONSOR_NOTIFICATIONS",
                table: "SPONSOR_NOTIFICATIONS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SPONSOR_LANGUAGES",
                table: "SPONSOR_LANGUAGES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SPONSOR_CONTACT_INFO",
                table: "SPONSOR_CONTACT_INFO",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LANGUAGES",
                table: "LANGUAGES",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_hints",
                table: "hints",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_FEEDBACK_REASONS",
                table: "FEEDBACK_REASONS",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DAILY_REWARDS",
                table: "DAILY_REWARDS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_REASONS",
                table: "USER_REASONS",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_DAILY_TASKS",
                table: "USER_DAILY_TASKS",
                columns: new[] { "UserId", "DailyTaskId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_users_settings",
                table: "users_settings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_TRUST_LEVELS",
                table: "USER_TRUST_LEVELS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SYSTEM_SPONSORS",
                table: "SYSTEM_SPONSORS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SECONDARY_LOCALISATIONS",
                table: "SECONDARY_LOCALISATIONS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_promo_codes",
                table: "promo_codes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_PERSONALITY_STATS",
                table: "USER_PERSONALITY_STATS",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_PERSONALITY_POINTS",
                table: "USER_PERSONALITY_POINTS",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_NOTIFICATIONS",
                table: "USER_NOTIFICATIONS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LOCALISATIONS",
                table: "LOCALISATIONS",
                columns: new[] { "Id", "SectionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_INVITATION_CREDENTIALS",
                table: "USER_INVITATION_CREDENTIALS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_INVITATIONS",
                table: "USER_INVITATIONS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SYSTEM_FEEDBACKS",
                table: "SYSTEM_FEEDBACKS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_ENCOUNTERS",
                table: "USER_ENCOUNTERS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DAILY_TASKS",
                table: "DAILY_TASKS",
                columns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CLASS_LOCALISATIONS",
                table: "CLASS_LOCALISATIONS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_BLACKLISTS",
                table: "USER_BLACKLISTS",
                columns: new[] { "Id", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_WALLET_BALANCES",
                table: "USER_WALLET_BALANCES",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SPONSOR_ADS",
                table: "SPONSOR_ADS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SYSTEM_ADMINS",
                table: "SYSTEM_ADMINS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER_ACTIVE_EFFECTS",
                table: "USER_ACTIVE_EFFECTS",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SYSTEM_ACHIEVEMENTS",
                table: "SYSTEM_ACHIEVEMENTS",
                columns: new[] { "Id", "ClassLocalisationId" });

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
                name: "SPONSOR_EVENT_TEMPLATES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Bounty = table.Column<int>(type: "integer", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    Languages = table.Column<List<int>>(type: "integer[]", nullable: true),
                    MaxAge = table.Column<short>(type: "smallint", nullable: false),
                    MinAge = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPONSOR_EVENT_TEMPLATES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SPONSOR_EVENTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Bounty = table.Column<int>(type: "integer", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    GroupLink = table.Column<string>(type: "text", nullable: true),
                    HasGroup = table.Column<bool>(type: "boolean", nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    Languages = table.Column<List<int>>(type: "integer[]", nullable: true),
                    MaxAge = table.Column<short>(type: "smallint", nullable: false),
                    MinAge = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Photo = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    SponsorId = table.Column<long>(type: "bigint", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false)
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
                name: "SYSTEM_GENDERS",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false),
                    ClassLocalisationId = table.Column<int>(type: "integer", nullable: false),
                    ClassLocalizationId = table.Column<int>(type: "integer", nullable: true),
                    GenderName = table.Column<string>(type: "text", nullable: true)
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
                name: "USER_WALLET_PURCHASES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PointInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_WALLET_PURCHASES", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_user_tests_TestId_TestClassLocalisationId",
                table: "user_tests",
                columns: new[] { "TestId", "TestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_LOCATIONS_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS",
                columns: new[] { "CityCountryClassLocalisationId", "CityId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_LOCATIONS_CountryClassLocalisationId_CountryId",
                table: "USER_LOCATIONS",
                columns: new[] { "CountryClassLocalisationId", "CountryId" });

            migrationBuilder.CreateIndex(
                name: "IX_USER_ACHIEVEMENTS_AchievementId_AchievementClassLocalisatio~",
                table: "USER_ACHIEVEMENTS",
                columns: new[] { "AchievementId", "AchievementClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_tests_results_TestId_TestClassLocalisationId",
                table: "tests_results",
                columns: new[] { "TestId", "TestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_tests_questions_TestId_TestClassLocalisationId",
                table: "tests_questions",
                columns: new[] { "TestId", "TestClassLocalisationId" });

            migrationBuilder.CreateIndex(
                name: "IX_cities_CountryClassLocalisationId_CountryId",
                table: "cities",
                columns: new[] { "CountryClassLocalisationId", "CountryId" });

            migrationBuilder.CreateIndex(
                name: "IX_SPONSOR_EVENTS_SponsorId",
                table: "SPONSOR_EVENTS",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_SYSTEM_GENDERS_ClassLocalizationId",
                table: "SYSTEM_GENDERS",
                column: "ClassLocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_EVENTS_EventId",
                table: "USER_EVENTS",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_adventures_Users_UserId",
                table: "adventures",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cities_countries_CountryClassLocalisationId_CountryId",
                table: "cities",
                columns: new[] { "CountryClassLocalisationId", "CountryId" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LANGUAGES_CLASS_LOCALISATIONS_ClassLocalizationId",
                table: "LANGUAGES",
                column: "ClassLocalizationId",
                principalTable: "CLASS_LOCALISATIONS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SECONDARY_LOCALISATIONS_LOCALISATIONS_LocalizationId_Locali~",
                table: "SECONDARY_LOCALISATIONS",
                columns: new[] { "LocalizationId", "LocalizationSectionId" },
                principalTable: "LOCALISATIONS",
                principalColumns: new[] { "Id", "SectionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SPONSOR_ADS_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_ADS",
                column: "SponsorId",
                principalTable: "SYSTEM_SPONSORS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SPONSOR_LANGUAGES_LANGUAGES_LanguageClassLocalisationId_Lan~",
                table: "SPONSOR_LANGUAGES",
                columns: new[] { "LanguageClassLocalisationId", "LanguageId" },
                principalTable: "LANGUAGES",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SPONSOR_LANGUAGES_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_LANGUAGES",
                column: "SponsorId",
                principalTable: "SYSTEM_SPONSORS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SPONSOR_NOTIFICATIONS_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_NOTIFICATIONS",
                column: "SponsorId",
                principalTable: "SYSTEM_SPONSORS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SPONSOR_RATINGS_SYSTEM_SPONSORS_SponsorId",
                table: "SPONSOR_RATINGS",
                column: "SponsorId",
                principalTable: "SYSTEM_SPONSORS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SPONSOR_RATINGS_Users_UserId",
                table: "SPONSOR_RATINGS",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SYSTEM_FEEDBACKS_FEEDBACK_REASONS_ReasonId_ReasonClassLocal~",
                table: "SYSTEM_FEEDBACKS",
                columns: new[] { "ReasonId", "ReasonClassLocalisationId" },
                principalTable: "FEEDBACK_REASONS",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SYSTEM_FEEDBACKS_Users_UserId",
                table: "SYSTEM_FEEDBACKS",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SYSTEM_SPONSORS_SPONSOR_CONTACT_INFO_ContactInfoId",
                table: "SYSTEM_SPONSORS",
                column: "ContactInfoId",
                principalTable: "SPONSOR_CONTACT_INFO",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SYSTEM_SPONSORS_SPONSOR_STATS_StatsId",
                table: "SYSTEM_SPONSORS",
                column: "StatsId",
                principalTable: "SPONSOR_STATS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tests_questions_tests_TestId_TestClassLocalisationId",
                table: "tests_questions",
                columns: new[] { "TestId", "TestClassLocalisationId" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tests_results_tests_TestId_TestClassLocalisationId",
                table: "tests_results",
                columns: new[] { "TestId", "TestClassLocalisationId" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tick_requests_Users_UserId",
                table: "tick_requests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UpdateCountry_CLASS_LOCALISATIONS_ClassLocalizationId",
                table: "UpdateCountry",
                column: "ClassLocalizationId",
                principalTable: "CLASS_LOCALISATIONS",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ACHIEVEMENTS_SYSTEM_ACHIEVEMENTS_AchievementId_Achieve~",
                table: "USER_ACHIEVEMENTS",
                columns: new[] { "AchievementId", "AchievementClassLocalisationId" },
                principalTable: "SYSTEM_ACHIEVEMENTS",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ACHIEVEMENTS_Users_UserId",
                table: "USER_ACHIEVEMENTS",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_USER_BLACKLISTS_Users_BannedUserId",
                table: "USER_BLACKLISTS",
                column: "BannedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_DAILY_TASKS_DAILY_TASKS_DailyTaskId_DailyTaskClassLoca~",
                table: "USER_DAILY_TASKS",
                columns: new[] { "DailyTaskId", "DailyTaskClassLocalisationId" },
                principalTable: "DAILY_TASKS",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ENCOUNTERS_Users_EncounteredUserId",
                table: "USER_ENCOUNTERS",
                column: "EncounteredUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_INVITATION_CREDENTIALS_Users_UserId",
                table: "USER_INVITATION_CREDENTIALS",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_INVITATIONS_USER_INVITATION_CREDENTIALS_InvitorCredent~",
                table: "USER_INVITATIONS",
                column: "InvitorCredentialsId",
                principalTable: "USER_INVITATION_CREDENTIALS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_LOCATIONS_cities_CityCountryClassLocalisationId_CityId",
                table: "USER_LOCATIONS",
                columns: new[] { "CityCountryClassLocalisationId", "CityId" },
                principalTable: "cities",
                principalColumns: new[] { "Id", "CountryClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_USER_LOCATIONS_countries_CountryClassLocalisationId_Country~",
                table: "USER_LOCATIONS",
                columns: new[] { "CountryClassLocalisationId", "CountryId" },
                principalTable: "countries",
                principalColumns: new[] { "Id", "ClassLocalisationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_USER_REPORTS_Users_UserId",
                table: "USER_REPORTS",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_USER_REPORTS_Users_UserId1",
                table: "USER_REPORTS",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_tags_Users_UserId",
                table: "user_tags",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_tests_Users_UserId",
                table: "user_tests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_tests_tests_TestId_TestClassLocalisationId",
                table: "user_tests",
                columns: new[] { "TestId", "TestClassLocalisationId" },
                principalTable: "tests",
                principalColumns: new[] { "Id", "ClassLocalisationId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_USER_LOCATIONS_LocationId",
                table: "Users",
                column: "LocationId",
                principalTable: "USER_LOCATIONS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_users_data_DataId",
                table: "Users",
                column: "DataId",
                principalTable: "users_data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_users_settings_UserSettingsId",
                table: "Users",
                column: "UserSettingsId",
                principalTable: "users_settings",
                principalColumn: "Id");
        }
    }
}
