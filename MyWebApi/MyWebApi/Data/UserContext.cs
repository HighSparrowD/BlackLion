using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.AdminEntities;
using MyWebApi.Entities.DailyRewardEntities;
using MyWebApi.Entities.LocalisationEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.TestEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.UserInfoEntities;

namespace MyWebApi.Data
{
    public class UserContext : DbContext
    {
        public DbSet<User> SYSTEM_USERS { get; set; }
        public DbSet<UserBaseInfo> SYSTEM_USERS_BASES { get; set; }
        public DbSet<UserDataInfo> SYSTEM_USERS_DATA { get; set; }
        public DbSet<UserPreferences> SYSTEM_USERS_PREFERENCES { get; set; }
        public DbSet<Location> USER_LOCATIONS { get; set; }
        public DbSet<UserReason> USER_REASONS { get; set; }
        public DbSet<Visit> USER_VISITS { get; set; }
        public DbSet<Country> COUNTRIES { get; set; }
        public DbSet<City> CITIES { get; set; }
        public DbSet<Language> LANGUAGES { get; set; }
        public DbSet<BlackList> USER_BLACKLISTS { get; set; }
        public DbSet<Feedback> SYSTEM_FEEDBACKS { get; set; }
        public DbSet<FeedbackReason> FEEDBACK_REASONS { get; set; }
        public DbSet<Report> USER_REPORTS { get; set; }
        public DbSet<ReportReason> REPORT_REASONS { get; set; }
        public DbSet<Achievement> SYSTEM_ACHIEVEMENTS { get; set; }
        public DbSet<UserAchievement> USER_ACHIEVEMENTS { get; set; }
        public DbSet<Balance> USER_WALLET_BALANCES { get; set; }
        public DbSet<Purchase> USER_WALLET_PURCHASES { get; set; }
        public DbSet<UserNotification> USER_NOTIFICATIONS { get; set; }
        public DbSet<Encounter> USER_ENCOUNTERS { get; set; }
        public DbSet<Admin> SYSTEM_ADMINS { get; set; }
        public DbSet<Localisation> LOCALISATIONS { get; set; }
        public DbSet<SecondaryLocalisationModel> SECONDARY_LOCALISATIONS { get; set; }
        public DbSet<ClassLocalisation> CLASS_LOCALISATIONS { get; set; }
        public DbSet<AppLanguage> APP_LANGUAGES { get; set; }
        public DbSet<Gender> SYSTEM_GENDERS { get; set; }
        public DbSet<AgePreference> AGE_PREFERENCES { get; set; }
        public DbSet<CommunicationPreference> COMMUNICATION_PREFERENCES { get; set; }
        public DbSet<PsychologicalTest> PSYCHOLOGICAL_TESTS { get; set; }
        public DbSet<PsychologicalTestQuestion> PSYCHOLOGICAL_TESTS_QUESTIONS { get; set; }
        public DbSet<PsychologicalTestAnswer> PSYCHOLOGICAL_TESTS_ANSWERS { get; set; }
        public DbSet<IntellectualTest> INTELLECTUAL_TESTS { get; set; }
        public DbSet<IntellectualTestQuestion> INTELLECTUAL_TESTS_QUESTIONS { get; set; }
        public DbSet<IntellectualTestAnswer> INTELLECTUAL_TESTS_ANSWERS { get; set; }
        public DbSet<Ad> SPONSOR_ADS { get; set; }
        public DbSet<Sponsor> SYSTEM_SPONSORS { get; set; }
        public DbSet<SponsorLanguage> SPONSOR_LANGUAGES { get; set; }
        public DbSet<SponsorContactInfo> SPONSOR_CONTACT_INFO { get; set; }
        public DbSet<Event> SPONSOR_EVENTS { get; set; }
        public DbSet<EventTemplate> SPONSOR_EVENT_TEMPLATES { get; set; }
        public DbSet<UserEvent> USER_EVENTS { get; set; }
        public DbSet<SponsorNotification> SPONSOR_NOTIFICATIONS { get; set; }
        public DbSet<SponsorRating> SPONSOR_RATINGS { get; set; }
        public DbSet<Stats> SPONSOR_STATS { get; set; }
        public DbSet<UserTrustLevel> USER_TRUST_LEVELS { get; set; }
        public DbSet<DailyReward> DAILY_REWARDS { get; set; }



        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasOne(u => u.UserBaseInfo);
            builder.Entity<User>().HasOne(u => u.UserDataInfo);
            builder.Entity<User>().HasOne(u => u.UserPreferences);
            builder.Entity<UserDataInfo>().HasOne(u => u.Location);
            builder.Entity<UserDataInfo>().HasOne(u => u.Reason);
            //builder.Entity<UserDataInfo>().HasOne(u => u.Language);
            builder.Entity<Location>().HasOne(u => u.Country);
            builder.Entity<Location>().HasOne(u => u.City);

            builder.Entity<Localisation>().HasMany(l => l.Loc);
            builder.Entity<Country>().HasMany(c => c.Cities);
            builder.Entity<Sponsor>().HasMany(s => s.SponsorAds);
            builder.Entity<PsychologicalTest>().HasMany(t => t.Questions);
            builder.Entity<IntellectualTest>().HasMany(t => t.Questions);
            builder.Entity<PsychologicalTestQuestion>().HasMany(q => q.Answers);
            builder.Entity<IntellectualTestQuestion>().HasMany(q => q.Answers);

            builder.Entity<Country>().HasKey(c => new {c.Id, c.ClassLocalisationId});
            builder.Entity<Language>().HasKey(l => new {l.Id, l.ClassLocalisationId});
            builder.Entity<City>().HasKey(c => new {c.Id, c.CountryClassLocalisationId});
            builder.Entity<UserReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<FeedbackReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<ReportReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<Visit>().HasKey(g => new { g.UserId, g.SectionId });
            builder.Entity<Achievement>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<UserAchievement>().HasKey(g => new { g.UserBaseInfoId, g.AchievementId });
            builder.Entity<BlackList>().HasKey(g => new { g.Id, g.UserId });
            builder.Entity<UpdateCountry>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<UserEvent>().HasKey(e => new { e.UserId, e.EventId });
            builder.Entity<Localisation>().HasKey(m => new { m.Id, m.SectionId });
            builder.Entity<Gender>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<AgePreference>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<CommunicationPreference>().HasKey(g => new { g.Id, g.ClassLocalisationId });

            builder.Entity<Ad>();

            builder.Entity<Sponsor>().HasMany(s => s.SponsorLanguages);
            builder.Entity<SponsorLanguage>().HasOne(s => s.Language);
        }
    }
}
