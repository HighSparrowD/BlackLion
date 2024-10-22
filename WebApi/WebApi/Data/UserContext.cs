﻿using Microsoft.EntityFrameworkCore;
using WebApi.Main.Entities.Achievement;
using WebApi.Main.Entities.Admin;
using WebApi.Main.Entities.Adventure;
using WebApi.Main.Entities.DailyReward;
using WebApi.Main.Entities.Effect;
using WebApi.Main.Entities.Hint;
using WebApi.Main.Entities.Language;
using WebApi.Main.Entities.Location;
using WebApi.Main.Entities.PromoCode;
using WebApi.Main.Entities.Report;
using WebApi.Main.Entities.Sponsor;
using WebApi.Main.Entities.Tag;
using WebApi.Main.Entities.Test;
using WebApi.Main.Entities.User;
using WebApi.Main.Models.Sponsor;

namespace WebApi.Data
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserData> UserData => Set<UserData>();
        public DbSet<Settings> UsersSettings => Set<Settings>();
        public DbSet<Location> UserLocations => Set<Location>();
        public DbSet<Statistics> UserStatistics => Set<Statistics>();
        public DbSet<Visit> UserVisits => Set<Visit>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Language> Languages => Set<Language>();
        public DbSet<BlackList> UserBlacklists => Set<BlackList>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<Report> UserReports => Set<Report>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
        public DbSet<Balance> Balances => Set<Balance>();
        public DbSet<Transaction> Transaction => Set<Transaction>();
        public DbSet<UserNotification> Notifications => Set<UserNotification>();
        public DbSet<Encounter> Encounters => Set<Encounter>();
        public DbSet<UserTag> UserTags => Set<UserTag>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        //Tests 
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<TestQuestion> TestsQuestions => Set<TestQuestion>();
        public DbSet<TestAnswer> TestsAnswers => Set<TestAnswer>();
        public DbSet<TestResult> TestsResults => Set<TestResult>();
        public DbSet<TestScale> TestsScales => Set<TestScale>();
        public DbSet<UserTest> UserTests => Set<UserTest>();

        //Sponsors
        public DbSet<Advertisement> Advertisements => Set<Advertisement>();
        public DbSet<AdvertisementStats> AdvertisementStatistics => Set<AdvertisementStats>();
        public DbSet<AdvertisementTag> AdvertisementTags => Set<AdvertisementTag>();
        //public DbSet<Sponsor> Sponsors => Set<Sponsor>();
        //public DbSet<SponsorLanguage> SponsorLanguages => Set<SponsorLanguage>();
        //public DbSet<SponsorContactInfo> SponsorContactInfo => Set<SponsorContactInfo>();
        //public DbSet<SponsorNotification> SponsorNotifications => Set<SponsorNotification>();
        //public DbSet<SponsorRating> SponsorRatings => Set<SponsorRating>();
        //public DbSet<Stats> SponsorStats => Set<Stats>();
        public DbSet<TrustLevel> TrustLevels => Set<TrustLevel>();
        public DbSet<DailyReward> DailyRewards => Set<DailyReward>();
        public DbSet<InvitationCredentials> InvitationCredentials => Set<InvitationCredentials>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<OceanStats> OceanStats => Set<OceanStats>();
        public DbSet<OceanPoints> OceanPoints => Set<OceanPoints>();
        public DbSet<ActiveEffect> ActiveEffects => Set<ActiveEffect>();
        public DbSet<VerificationRequest> TickRequests => Set<VerificationRequest>();

        //Adventures
        public DbSet<Adventure> Adventures => Set<Adventure>();
        public DbSet<AdventureTemplate> AdventureTemplates => Set<AdventureTemplate>();
        public DbSet<AdventureAttendee> AdventureAttendees => Set<AdventureAttendee>();
        public DbSet<AdventureTag> AdventureTags => Set<AdventureTag>();
        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
        public DbSet<Hint> Hints => Set<Hint>();

        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasPostgresExtension("fuzzystrmatch");

            ConfigureRelations(builder);
            ConfigureMapping(builder);

            // Hilo
            ConfigureAchievements(builder);
            ConfigureActiveEffects(builder);
            ConfigureAds(builder);
            ConfigureAdventures(builder);
            ConfigureAdventureTemplates(builder);
            ConfigureBalances(builder);
            ConfigureBlackLists(builder);
            ConfigureEncounters(builder);
            ConfigureFeedbacks(builder);
            ConfigureInvitations(builder);
            ConfigureNotifications(builder);
            ConfigureRequests(builder);
            ConfigureReports(builder);
            ConfigureTickRequests(builder);
            ConfigureTransactions(builder);
            ConfigureTests(builder);
            ConfigureTestQuestions(builder);
            ConfigureTestAnswers(builder);
            ConfigureTestResults(builder);
            ConfigureTestScales(builder);
            ConfigureTags(builder);
            ConfigureAdvertisementStatistics(builder);
        }

        private void ConfigureRelations(ModelBuilder builder)
        {
            builder.Entity<User>().HasOne(u => u.Data);
            builder.Entity<User>().HasOne(u => u.Settings);
            builder.Entity<User>().HasOne(u => u.Location);
            builder.Entity<User>().HasOne(u => u.Statistics);
            builder.Entity<User>().HasMany(u => u.BlackList);
            builder.Entity<User>().HasMany(u => u.Tags);
            builder.Entity<User>().HasMany(u => u.Encounters);
            builder.Entity<User>().HasMany(u => u.Notifications);
            builder.Entity<User>().HasMany(u => u.Requests);
            builder.Entity<User>().HasMany(u => u.UserRoles);
            builder.Entity<Location>().HasOne(u => u.Country);
            builder.Entity<Location>().HasOne(u => u.City);

            builder.Entity<Country>().HasMany(c => c.Cities);
            //builder.Entity<Sponsor>().HasMany(s => s.SponsorAds);
            builder.Entity<Test>().HasMany(t => t.Questions);
            builder.Entity<Test>().HasMany(t => t.Results);
            builder.Entity<Test>().HasMany(t => t.Scales);
            builder.Entity<Test>().HasKey(t => new { t.Id, t.Language });
            builder.Entity<TestScale>().HasKey(t => new { t.Id, t.TestId });
            builder.Entity<UserTest>().HasKey(t => new { t.TestId, t.UserId });
            builder.Entity<TestQuestion>().HasMany(q => q.Answers);

            builder.Entity<UserRole>().HasKey(c => new { c.UserId, c.Role });
            builder.Entity<Country>().HasKey(c => new { c.Id, c.Lang });
            builder.Entity<Language>().HasKey(l => new { l.Id, l.Lang });
            builder.Entity<City>().HasKey(c => new { c.Id, c.CountryLang });
            builder.Entity<Visit>().HasKey(g => new { g.UserId, g.Section });
            builder.Entity<Achievement>().HasKey(g => new { g.Id, g.Language });
            builder.Entity<UserAchievement>().HasKey(g => new { g.UserId, g.AchievementId });
            builder.Entity<BlackList>().HasKey(g => new { g.Id, g.UserId });
            builder.Entity<UserTag>().HasKey(t => new {t.TagId, t.UserId, t.TagType});
            builder.Entity<Hint>().HasKey(t => new { t.Id, t.Localization });

            builder.Entity<Tag>().HasKey(t => new { t.Id, t.Type });

            builder.Entity<Adventure>().HasOne(a => a.City);
            builder.Entity<Adventure>().HasOne(a => a.Country);
            builder.Entity<Adventure>().HasOne(a => a.Creator);
            builder.Entity<Adventure>().HasMany(a => a.Attendees);
            builder.Entity<AdventureAttendee>().HasKey(t => new { t.UserId, t.AdventureId });
            builder.Entity<AdventureTag>().HasKey(t => new {t.TagId, t.AdventureId, t.TagType});

            builder.Entity<Advertisement>().HasMany(a => a.AdvertisementStats);
            builder.Entity<AdvertisementStats>().HasOne(a => a.Advertisement);
            builder.Entity<AdvertisementTag>().HasKey(t => new {t.TagId, t.AdvertisementId, t.TagType});
            //builder.Entity<Sponsor>().HasMany(s => s.SponsorLanguages);
            //builder.Entity<SponsorLanguage>().HasOne(s => s.Language);

            builder.Entity<UserNotification>().HasOne(un => un.Receiver);
            
            builder.Entity<Request>().HasOne(un => un.Sender);
            builder.Entity<Request>().HasOne(un => un.Receiver);

            builder.Entity<UserAchievement>().HasOne(un => un.Achievement)
                .WithMany()
                .HasForeignKey(a => new {a.AchievementId, a.AchievementLanguage});
            builder.Entity<UserAchievement>().HasOne(un => un.User);

            builder.Entity<Encounter>().HasOne(un => un.User);
            builder.Entity<Encounter>().HasOne(un => un.EncounteredUser);

            builder.Entity<UserRole>().Property(ur => ur.Role).HasColumnType("varchar");
        }

        private void ConfigureMapping(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("users");
            builder.Entity<UserRole>().ToTable("user_roles");
            builder.Entity<UserData>().ToTable("user_data");
            builder.Entity<Settings>().ToTable("user_settings");
            builder.Entity<Location>().ToTable("user_locations");
            builder.Entity<Statistics>().ToTable("user_statistics");
            builder.Entity<Visit>().ToTable("user_visits");
            builder.Entity<Country>().ToTable("countries");
            builder.Entity<City>().ToTable("cities");
            builder.Entity<Language>().ToTable("languages");
            builder.Entity<BlackList>().ToTable("black_lists");
            builder.Entity<Feedback>().ToTable("feedbacks");
            builder.Entity<Report>().ToTable("user_reports");
            builder.Entity<Achievement>().ToTable("achievements");
            builder.Entity<UserAchievement>().ToTable("user_achievements");
            builder.Entity<Balance>().ToTable("balances");
            builder.Entity<Transaction>().ToTable("transactions");
            builder.Entity<UserNotification>().ToTable("notifications");
            builder.Entity<Encounter>().ToTable("encounters");
            builder.Entity<Test>().ToTable("tests");
            builder.Entity<TestQuestion>().ToTable("tests_questions");
            builder.Entity<TestAnswer>().ToTable("tests_answers");
            builder.Entity<TestResult>().ToTable("tests_results");
            builder.Entity<TestScale>().ToTable("tests_scales");
            builder.Entity<UserTest>().ToTable("user_tests");
            builder.Entity<UserTag>().ToTable("user_tags");
            builder.Entity<AdventureTag>().ToTable("adventure_tags");
            builder.Entity<AdvertisementTag>().ToTable("advertisement_tags");
            builder.Entity<Tag>().ToTable("tags");
            builder.Entity<Advertisement>().ToTable("advertisements");
            builder.Entity<AdvertisementStats>().ToTable("advertisement_statistics");
            //builder.Entity<Sponsor>().ToTable("sponsors");
            //builder.Entity<SponsorLanguage>().ToTable("sponsor_languages");
            //builder.Entity<SponsorContactInfo>().ToTable("sponsor_contact_info");
            //builder.Entity<SponsorNotification>().ToTable("sponsor_notifications");
            //builder.Entity<SponsorRating>().ToTable("sponsor_ratings");
            //builder.Entity<Stats>().ToTable("sponsor_stats");
            builder.Entity<TrustLevel>().ToTable("trust_levels");
            builder.Entity<DailyReward>().ToTable("daily_rewards");
            builder.Entity<InvitationCredentials>().ToTable("invitation_credentials");
            builder.Entity<Invitation>().ToTable("invitations");
            //builder.Entity<DailyTask>().ToTable("daily_tasks");
            builder.Entity<OceanStats>().ToTable("ocean_stats");
            builder.Entity<OceanPoints>().ToTable("ocean_points");
            builder.Entity<ActiveEffect>().ToTable("active_effects");
            builder.Entity<VerificationRequest>().ToTable("verification_requests");
            builder.Entity<Adventure>().ToTable("adventures");
            builder.Entity<AdventureTemplate>().ToTable("adventure_templates");
            builder.Entity<AdventureAttendee>().ToTable("adventure_attendees");
            builder.Entity<PromoCode>().ToTable("promocodes");
            builder.Entity<Hint>().ToTable("hints");
            builder.Entity<Request>().ToTable("requests");
        }

        //Hilo configuration
        private void ConfigureAchievements(ModelBuilder builder)
        {
            const string sequenceName = "achievements_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Achievement>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureActiveEffects(ModelBuilder builder)
        {
            const string sequenceName = "active_effects_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<ActiveEffect>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureAds(ModelBuilder builder)
        {
            const string sequenceName = "ads_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Advertisement>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureAdventures(ModelBuilder builder)
        {
            const string sequenceName = "adventures_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Adventure>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureAdventureTemplates(ModelBuilder builder)
        {
            const string sequenceName = "adventure_templates_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<AdventureTemplate>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureBalances(ModelBuilder builder)
        {
            const string sequenceName = "balances_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Balance>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureBlackLists(ModelBuilder builder)
        {
            const string sequenceName = "black_lists_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity <BlackList>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureEncounters(ModelBuilder builder)
        {
            const string sequenceName = "encounters_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Encounter>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureFeedbacks(ModelBuilder builder)
        {
            const string sequenceName = "feedbacks_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Feedback>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureInvitations(ModelBuilder builder)
        {
            const string sequenceName = "invitations_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Invitation>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureNotifications(ModelBuilder builder)
        {
            const string sequenceName = "notifications_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<UserNotification>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureRequests(ModelBuilder builder)
        {
            const string sequenceName = "requests_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Request>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureTickRequests(ModelBuilder builder)
        {
            const string sequenceName = "tick_requests_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<VerificationRequest>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureTransactions(ModelBuilder builder)
        {
            const string sequenceName = "transactions_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Transaction>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureReports(ModelBuilder builder)
        {
            const string sequenceName = "reports_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Report>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        //private void ConfigureUserTags(ModelBuilder builder)
        //{
        //    const string sequenceName = "user_tags_hilo";

        //    builder.HasSequence<int>(sequenceName)
        //        .StartsAt(1)
        //        .IncrementsBy(1);

        //    builder.Entity<UserTag>(b =>
        //    {
        //        b.Property(a => a.Id).UseHiLo(sequenceName);
        //    });
        //}

        private void ConfigureTests(ModelBuilder builder)
        {
            const string sequenceName = "tests_hilo";

            builder.HasSequence<long>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Test>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureTestQuestions(ModelBuilder builder)
        {
            const string sequenceName = "test_questions_hilo";

            builder.HasSequence<long>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<TestQuestion>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureTestAnswers(ModelBuilder builder)
        {
            const string sequenceName = "test_answers_hilo";

            builder.HasSequence<long>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<TestAnswer>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureTestResults(ModelBuilder builder)
        {
            const string sequenceName = "test_results_hilo";

            builder.HasSequence<long>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<TestResult>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureTestScales(ModelBuilder builder)
        {
            const string sequenceName = "test_scales_hilo";

            builder.HasSequence<int>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<TestScale>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureTags(ModelBuilder builder)
        {
            const string sequenceName = "tags_hilo";

            builder.HasSequence<long>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<Tag>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }

        private void ConfigureAdvertisementStatistics(ModelBuilder builder)
        {
            const string sequenceName = "advertisement_stats_hilo";

            builder.HasSequence<long>(sequenceName)
                .StartsAt(1)
                .IncrementsBy(1);

            builder.Entity<AdvertisementStats>(b =>
            {
                b.Property(a => a.Id).UseHiLo(sequenceName);
            });
        }
    }
}
