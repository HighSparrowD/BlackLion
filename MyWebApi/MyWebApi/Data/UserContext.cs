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
        public DbSet<User> users { get; set; }
        public DbSet<UserData> users_data { get; set; }
        public DbSet<UserSettings> users_settings { get; set; }
        public DbSet<Location> USER_LOCATIONS { get; set; }
        public DbSet<UserReason> USER_REASONS { get; set; }
        public DbSet<Visit> USER_VISITS { get; set; }
        public DbSet<Country> countries { get; set; }
        public DbSet<City> cities { get; set; }
        public DbSet<Language> LANGUAGES { get; set; }
        public DbSet<BlackList> USER_BLACKLISTS { get; set; }
        public DbSet<Feedback> SYSTEM_FEEDBACKS { get; set; }
        public DbSet<FeedbackReason> FEEDBACK_REASONS { get; set; }
        public DbSet<Report> USER_REPORTS { get; set; }
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
        public DbSet<Test> tests { get; set; }
        public DbSet<TestQuestion> tests_questions { get; set; }
        public DbSet<TestAnswer> tests_answers { get; set; }
        public DbSet<TestResult> tests_results { get; set; }
        public DbSet<UserTest> user_tests { get; set; }
        public DbSet<UserTag> user_tags { get; set; }
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
        public DbSet<InvitationCredentials> USER_INVITATION_CREDENTIALS { get; set; }
        public DbSet<Invitation> USER_INVITATIONS { get; set; }
        public DbSet<DailyTask> DAILY_TASKS { get; set; }
        public DbSet<UserDailyTask> USER_DAILY_TASKS { get; set; }
        public DbSet<UserPersonalityStats> USER_PERSONALITY_STATS { get; set; }
        public DbSet<UserPersonalityPoints> USER_PERSONALITY_POINTS { get; set; }
        public DbSet<AdminErrorLog> ADMIN_ERROR_LOGS { get; set; }
        public DbSet<ActiveEffect> USER_ACTIVE_EFFECTS { get; set; }
        public DbSet<TickRequest> tick_requests { get; set; }

        //Adventures
        public DbSet<Adventure> adventures { get; set; }
        public DbSet<AdventureTemplate> adventure_templates { get; set; }
        public DbSet<AdventureAttendee> adventure_attendees { get; set; }
        public DbSet<PromoCode> promo_codes { get; set; }
        public DbSet<Hint> hints { get; set; }



        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasOne(u => u.Data);
            builder.Entity<User>().HasOne(u => u.UserSettings);
            builder.Entity<User>().HasOne(u => u.Location);
            builder.Entity<User>().HasMany(u => u.UserBlackList);
            builder.Entity<User>().HasMany(u => u.Tags);
            //builder.Entity<UserDataInfo>().HasOne(u => u.Language);
            builder.Entity<Location>().HasOne(u => u.Country);
            builder.Entity<Location>().HasOne(u => u.City);

            builder.Entity<Localisation>().HasMany(l => l.Loc);
            builder.Entity<Country>().HasMany(c => c.Cities);
            builder.Entity<Sponsor>().HasMany(s => s.SponsorAds);
            builder.Entity<Test>().HasMany(t => t.Questions);
            builder.Entity<Test>().HasMany(t => t.Results);
            builder.Entity<Test>().HasKey(t => new {t.Id, t.ClassLocalisationId});
            builder.Entity<UserTest>().HasKey(t => new {t.TestId, t.UserId});
            builder.Entity<TestQuestion>().HasMany(q => q.Answers);

            builder.Entity<Country>().HasKey(c => new {c.Id, c.ClassLocalisationId});
            builder.Entity<Language>().HasKey(l => new {l.Id, l.ClassLocalisationId});
            builder.Entity<City>().HasKey(c => new {c.Id, c.CountryClassLocalisationId});
            builder.Entity<UserReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<FeedbackReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
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
            builder.Entity<DailyTask>().HasKey(t => new { t.Id, t.ClassLocalisationId });
            builder.Entity<UserDailyTask>().HasKey(t => new { t.UserId, t.DailyTaskId });
            builder.Entity<AdventureAttendee>().HasKey(t => new { t.UserId, t.AdventureId });
            builder.Entity<UserTag>().HasKey(t => new { t.UserId, t.Tag });
            builder.Entity<Hint>().HasKey(t => new { t.Id, t.ClassLocalisationId });

            builder.Entity<Ad>();

            builder.Entity<Sponsor>().HasMany(s => s.SponsorLanguages);
            builder.Entity<SponsorLanguage>().HasOne(s => s.Language);
        }
    }
}
