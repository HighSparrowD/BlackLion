using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities.AdminEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.UserInfoEntities;

namespace MyWebApi.Data
{
    public class AdminContext : DbContext
    {
        public DbSet<Admin> SYSTEM_ADMINS { get; set; }
        public DbSet<UpdateCountry> COUNTRIES { get; set; }
        public DbSet<City> CITIES { get; set; }
        public DbSet<Language> LANGUAGES { get; set; }
        public DbSet<Feedback> SYSTEM_FEEDBACKS { get; set; }
        public DbSet<FeedbackReason> FEEDBACK_REASONS { get; set; }
        public DbSet<UserBaseInfo> SYSTEM_USERS_BASES { get; set; }

        public AdminContext(DbContextOptions<AdminContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<Country>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<UpdateCountry>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<City>().HasKey(g => new { g.Id, g.CountryClassLocalisationId });
            builder.Entity<Language>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<FeedbackReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });

        }
    }
}
