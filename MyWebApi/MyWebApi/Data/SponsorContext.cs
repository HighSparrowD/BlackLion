using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.UserInfoEntities;

namespace MyWebApi.Data
{
    public class SponsorContext : DbContext
    {
        public DbSet<Ad> SPONSOR_ADS { get; set; }
        public DbSet<Sponsor> SYSTEM_SPONSORS { get; set; }
        public DbSet<SponsorLanguage> SPONSOR_LANGUAGES { get; set; }
        public DbSet<ContactInfo> SPONSOR_CONTACT_INFO { get; set; }
        public DbSet<Event> SPONSOR_EVENTS { get; set; }
        public DbSet<User> SYSTEM_USERS { get; set; } //TODO: Remove after splitting contexts
        public DbSet<UserEvent> USER_EVENTS { get; set; }
        public DbSet<UserNotification> USER_NOTIFICATIONS { get; set; }
        public DbSet<SponsorNotification> SPONSOR_NOTIFICATIONS { get; set; }
        public DbSet<SponsorRating> SPONSOR_RATINGS { get; set; }

        public SponsorContext(DbContextOptions<SponsorContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            builder.Entity<Sponsor>().HasMany(s => s.SponsorAds);
            builder.Entity<Ad>();

            builder.Entity<UserEvent>().HasKey(e => new { e.UserId, e.EventId });
            builder.Entity<Country>().HasKey(e => new { e.Id, e.ClassLocalisationId });
            builder.Entity<City>().HasKey(e => new { e.Id, e.CountryClassLocalisationId });
            builder.Entity<UserReason>().HasKey(e => new { e.Id, e.ClassLocalisationId });
        }
    }
}
