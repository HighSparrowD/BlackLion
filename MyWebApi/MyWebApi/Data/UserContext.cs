using Microsoft.EntityFrameworkCore;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
using WebApi.Entities.AdventureEntities;
using WebApi.Entities.DailyRewardEntities;
using WebApi.Entities.DailyTaskEntities;
using WebApi.Entities.EffectEntities;
using WebApi.Entities.HintEntities;
using WebApi.Entities.LocalisationEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.ReasonEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.SponsorEntities;
using WebApi.Entities.TestEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities.UserInfoEntities;

namespace WebApi.Data
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserData> UsersData => Set<UserData>();
        public DbSet<UserSettings> UsersSettings => Set<UserSettings>();
        public DbSet<Location> UserLocations => Set<Location>();
        public DbSet<Visit> UserVisits => Set<Visit>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Language> Languages => Set<Language>();
        public DbSet<BlackList> UserBlacklists => Set<BlackList>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<FeedbackReason> FeedbackReasons => Set<FeedbackReason>();
        public DbSet<Report> UserReports => Set<Report>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
        public DbSet<Balance> Balances => Set<Balance>();
        public DbSet<Transaction> Transaction => Set<Transaction>();
        public DbSet<UserNotification> Notifications => Set<UserNotification>();
        public DbSet<Encounter> Encounters => Set<Encounter>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Localization> Localizations => Set<Localization>();
        public DbSet<SecondaryLocalizationModel> SecondaryLocalizations => Set<SecondaryLocalizationModel>();
        public DbSet<ClassLocalization> ClassLocalizations => Set<ClassLocalization>();
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<TestQuestion> TestsQuestions => Set<TestQuestion>();
        public DbSet<TestAnswer> TestsAnswers => Set<TestAnswer>();
        public DbSet<TestResult> TestsResults => Set<TestResult>();
        public DbSet<UserTest> UserTests => Set<UserTest>();
        public DbSet<UserTag> UserTags => Set<UserTag>();
        public DbSet<Ad> Ads => Set<Ad>();
        public DbSet<Sponsor> Sponsors => Set<Sponsor>();
        public DbSet<SponsorLanguage> SponsorLanguages => Set<SponsorLanguage>();
        public DbSet<SponsorContactInfo> SponsorContactInfo => Set<SponsorContactInfo>();
        public DbSet<SponsorNotification> SponsorNotifications => Set<SponsorNotification>();
        public DbSet<SponsorRating> SponsorRatings => Set<SponsorRating>();
        public DbSet<Stats> SponsorStats => Set<Stats>();
        public DbSet<UserTrustLevel> TrustLevels => Set<UserTrustLevel>();
        public DbSet<DailyReward> DailyRewards => Set<DailyReward>();
        public DbSet<InvitationCredentials> InvitationCredentials => Set<InvitationCredentials>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<DailyTask> DailyTasks => Set<DailyTask>(); //TODO: Remove
        public DbSet<UserDailyTask> UserDailyTasks => Set<UserDailyTask>(); //TODO: Remove
        public DbSet<UserPersonalityStats> PersonalityStats => Set<UserPersonalityStats>();
        public DbSet<UserPersonalityPoints> PersonalityPoints => Set<UserPersonalityPoints>();
        public DbSet<ActiveEffect> ActiveEffects => Set<ActiveEffect>();
        public DbSet<TickRequest> TickRequests => Set<TickRequest>();

        //Adventures
        public DbSet<Adventure> Adventures => Set<Adventure>();
        public DbSet<AdventureTemplate> AdventureTemplates => Set<AdventureTemplate>();
        public DbSet<AdventureAttendee> AdventureAttendees => Set<AdventureAttendee>();
        public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
        public DbSet<Hint> Hints => Set<Hint>();



        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ConfigureRelations(builder);
            ConfigureMapping(builder);
        }

        private void ConfigureRelations(ModelBuilder builder)
        {
            builder.Entity<User>().HasOne(u => u.Data);
            builder.Entity<User>().HasOne(u => u.UserSettings);
            builder.Entity<User>().HasOne(u => u.Location);
            builder.Entity<User>().HasMany(u => u.UserBlackList);
            builder.Entity<User>().HasMany(u => u.Tags);
            builder.Entity<Location>().HasOne(u => u.Country);
            builder.Entity<Location>().HasOne(u => u.City);

            builder.Entity<Localization>().HasMany(l => l.Loc);
            builder.Entity<Country>().HasMany(c => c.Cities);
            builder.Entity<Sponsor>().HasMany(s => s.SponsorAds);
            builder.Entity<Test>().HasMany(t => t.Questions);
            builder.Entity<Test>().HasMany(t => t.Results);
            builder.Entity<Test>().HasKey(t => new { t.Id, t.Language });
            builder.Entity<UserTest>().HasKey(t => new { t.TestId, t.UserId });
            builder.Entity<TestQuestion>().HasMany(q => q.Answers);

            builder.Entity<DailyTask>().HasKey(q => new {q.Id, q.ClassLocalisationId});
            builder.Entity<UserDailyTask>().HasKey(q => new {q.UserId, q.DailyTaskId});

            builder.Entity<Country>().HasKey(c => new { c.Id, c.Lang });
            builder.Entity<Language>().HasKey(l => new { l.Id, l.ClassLocalisationId });
            builder.Entity<City>().HasKey(c => new { c.Id, c.Lang });
            builder.Entity<FeedbackReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<Visit>().HasKey(g => new { g.UserId, g.SectionId });
            builder.Entity<Achievement>().HasKey(g => new { g.Id, g.Language });
            builder.Entity<UserAchievement>().HasKey(g => new { g.UserBaseInfoId, g.AchievementId });
            builder.Entity<BlackList>().HasKey(g => new { g.Id, g.UserId });
            builder.Entity<Localization>().HasKey(m => new { m.Id, m.SectionId });
            builder.Entity<AdventureAttendee>().HasKey(t => new { t.UserId, t.AdventureId });
            builder.Entity<UserTag>().HasKey(t => new { t.UserId, t.Tag });
            builder.Entity<Hint>().HasKey(t => new { t.Id, t.Localization });

            builder.Entity<Sponsor>().HasMany(s => s.SponsorLanguages);
            builder.Entity<SponsorLanguage>().HasOne(s => s.Language);
        }

        private void ConfigureMapping(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("users");
            builder.Entity<UserData>().ToTable("users_data");
            builder.Entity<UserSettings>().ToTable("user_settings");
            builder.Entity<Location>().ToTable("user_locations");
            builder.Entity<Visit>().ToTable("user_visits");
            builder.Entity<Country>().ToTable("countries");
            builder.Entity<City>().ToTable("cities");
            builder.Entity<Language>().ToTable("languages");
            builder.Entity<BlackList>().ToTable("black_lists");
            builder.Entity<Feedback>().ToTable("feedbacks");
            builder.Entity<FeedbackReason>().ToTable("feedback_reasons");
            builder.Entity<Report>().ToTable("user_reports");
            builder.Entity<Achievement>().ToTable("achievements");
            builder.Entity<UserAchievement>().ToTable("user_achievements");
            builder.Entity<Balance>().ToTable("balances");
            builder.Entity<Transaction>().ToTable("transactions");
            builder.Entity<UserNotification>().ToTable("notifications");
            builder.Entity<Encounter>().ToTable("encounters");
            builder.Entity<Admin>().ToTable("admins");
            builder.Entity<Localization>().ToTable("localizations"); //TODO: Remove
            builder.Entity<SecondaryLocalizationModel>().ToTable("secondary_localizations"); //TODO: Remove
            builder.Entity<ClassLocalization>().ToTable("class_localizations"); //TODO: Remove
            //builder.Entity<AppLanguage>().ToTable("app_languages"); //TODO: Remove
            builder.Entity<Test>().ToTable("tests");
            builder.Entity<TestQuestion>().ToTable("tests_questions");
            builder.Entity<TestAnswer>().ToTable("tests_answers");
            builder.Entity<TestResult>().ToTable("tests_results");
            builder.Entity<UserTest>().ToTable("user_tests");
            builder.Entity<UserTag>().ToTable("user_tags");
            builder.Entity<Ad>().ToTable("ads");
            builder.Entity<Sponsor>().ToTable("sponsors");
            builder.Entity<SponsorLanguage>().ToTable("sponsor_languages");
            builder.Entity<SponsorContactInfo>().ToTable("sponsor_contact_info");
            builder.Entity<SponsorNotification>().ToTable("sponsor_notifications");
            builder.Entity<SponsorRating>().ToTable("sponsor_ratings");
            builder.Entity<Stats>().ToTable("sponsor_stats");
            builder.Entity<UserTrustLevel>().ToTable("trust_levels");
            builder.Entity<DailyReward>().ToTable("daily_rewards");
            builder.Entity<InvitationCredentials>().ToTable("invitation_credentials");
            builder.Entity<Invitation>().ToTable("invitation");
            //builder.Entity<DailyTask>().ToTable("daily_tasks");
            builder.Entity<UserPersonalityStats>().ToTable("personality_stats");
            builder.Entity<UserPersonalityPoints>().ToTable("personality_points");
            builder.Entity<ActiveEffect>().ToTable("active_effects");
            builder.Entity<TickRequest>().ToTable("tick_requests");
            builder.Entity<Adventure>().ToTable("adventures");
            builder.Entity<AdventureTemplate>().ToTable("adventure_templates");
            builder.Entity<AdventureAttendee>().ToTable("adventure_attendees");
            builder.Entity<PromoCode>().ToTable("promocodes");
            builder.Entity<Hint>().ToTable("hints");

        }
    }
}
