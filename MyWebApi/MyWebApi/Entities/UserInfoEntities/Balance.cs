using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class Balance
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public int Points { get; set; }
        public int PersonalityPoints { get; set; }
        public DateTime PointInTime { get; set; }
    }
}
