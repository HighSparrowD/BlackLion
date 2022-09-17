using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.UserInfoEntities;

namespace MyWebApi.Data
{
    public class StoreContext : DbContext
    {
        public DbSet<User> SYSTEM_USERS { get; set; } // Represents table in a database
        public DbSet<FriendModel> USER_FRIENDS { get; set; }
        public DbSet<UserRegistrationModel> USER_REGISTRATIONS { get; set; }
        //public DbSet<TestingModel> TESTS { get; set; }

        private IConfiguration config { get; set; }

        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
            
        }

        //protected override void OnConfiguring() //DbContextOptionsBuilder builder )
        //{
        //    //builder.UseNpgsql(config.GetConnectionString("DefaultConnectionString"));
        //}

    }
}
