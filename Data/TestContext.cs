using Microsoft.EntityFrameworkCore;
using MyWebApi.Entities.LocalisationEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.TestEntities;
using System.Collections.Generic;

namespace MyWebApi.Data
{
    public class TestContext : DbContext
    {
        public DbSet<Localisation> LOCALISATIONS { get; set; }
        public DbSet<SecondaryLocalisationModel> SECONDARY_LOCALISATIONS { get; set; }
        public DbSet<ClassLocalisation> CLASS_LOCALISATIONS { get; set; }
        public DbSet<AppLanguage> APP_LANGUAGES { get; set; }
        public DbSet<Gender> SYSTEM_GENDERS { get; set; }
        public DbSet<UserReason> USER_REASONS { get; set; }
        public DbSet<AgePreference> AGE_PREFERENCES { get; set; }
        public DbSet<CommunicationPreference> COMMUNICATION_PREFERENCES { get; set; }

        public DbSet<PsychologicalTest> PSYCHOLOGICAL_TESTS { get; set; }
        public DbSet<PsychologicalTestQuestion> PSYCHOLOGICAL_TESTS_QUESTIONS { get; set; }
        public DbSet<PsychologicalTestAnswer> PSYCHOLOGICAL_TESTS_ANSWERS { get; set; }       

        public DbSet<IntellectualTest> INTELLECTUAL_TESTS { get; set; }
        public DbSet<IntellectualTestQuestion> INTELLECTUAL_TESTS_QUESTIONS { get; set; }
        public DbSet<IntellectualTestAnswer> INTELLECTUAL_TESTS_ANSWERS { get; set; }

        public DbSet<UpdateCountry> COUNTRIES { get; set; }
        public DbSet<City> CITIES { get; set; }
        public DbSet<Language> LANGUAGES { get; set; }


        public TestContext(DbContextOptions<TestContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            builder.Entity<Localisation>().HasMany(l => l.Loc);

            builder.Entity<PsychologicalTest>().HasMany(t => t.Questions);
            builder.Entity<IntellectualTest>().HasMany(t => t.Questions);
            builder.Entity<PsychologicalTestQuestion>().HasMany(q => q.Answers);
            builder.Entity<IntellectualTestQuestion>().HasMany(q => q.Answers);

            //builder.Entity<SecondaryLocalisationModel>().HasOne(l => l.Localisation);

            builder.Entity<Localisation>().HasKey(m => new { m.Id, m.SectionId });
            builder.Entity<Country>().HasKey(m => new { m.Id, m.ClassLocalisationId });
            builder.Entity<UpdateCountry>().HasKey(m => new { m.Id, m.ClassLocalisationId });
            builder.Entity<City>().HasKey(m => new { m.Id, m.CountryClassLocalisationId });
            builder.Entity<Gender>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<UserReason>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<AgePreference>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<CommunicationPreference>().HasKey(g => new { g.Id, g.ClassLocalisationId });
            builder.Entity<Language>().HasKey(g => new { g.Id, g.ClassLocalisationId });
        }
    }
}
