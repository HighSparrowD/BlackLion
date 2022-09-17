using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
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

            builder.Entity<Country>().HasMany(c => c.Cities);

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
        }
    }
}
