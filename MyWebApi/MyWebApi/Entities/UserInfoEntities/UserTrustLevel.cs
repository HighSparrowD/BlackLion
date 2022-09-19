using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserTrustLevel
    {
        [Key]
        public long UserId { get; set; }
        public double Progress { get; set; }
        public double Goal { get; set; }
        public int Level { get; set; }
    }
}
