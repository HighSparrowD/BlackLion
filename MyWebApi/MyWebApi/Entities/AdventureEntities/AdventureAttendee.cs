using MyWebApi.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.AdventureEntities
{
    public class AdventureAttendee
    {
        [Key]
        public long UserId{ get; set; }
        [Key]
        public Guid AdventureId { get; set; }
        public string Username { get; set; }
        public AdventureRequestStatus Status { get; set; }
        public virtual Adventure Adventure { get; set; }
    }
}
