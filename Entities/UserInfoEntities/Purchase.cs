using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class Purchase
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime PointInTime{ get; set; }
        public int Amount{ get; set; }
        public string Description{ get; set; }
    }
}
