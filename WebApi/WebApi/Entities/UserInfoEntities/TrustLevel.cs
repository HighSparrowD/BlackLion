using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserInfoEntities
{
    public class TrustLevel
    {
        [Key]
        public long Id { get; set; }
        public double Progress { get; set; }
        public double Goal { get; set; }
        public int Level { get; set; }
        public static TrustLevel CreateDefaultTrustLevel(long userId)
        {
            return new TrustLevel { Id = userId, Progress = 0, Goal = 800, Level = 1 };
        }
    }
}
