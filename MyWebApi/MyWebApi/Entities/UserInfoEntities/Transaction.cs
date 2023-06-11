using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserInfoEntities
{
    public class Transaction
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime PointInTime{ get; set; }
        public int Amount{ get; set; }
        public string Description{ get; set; }
        public short Currency{ get; set; }
    }
}
