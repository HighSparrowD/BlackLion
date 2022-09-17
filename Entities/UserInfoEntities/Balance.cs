using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class Balance
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Amount { get; set; }
        public DateTime PointInTime { get; set; }
    }
}
