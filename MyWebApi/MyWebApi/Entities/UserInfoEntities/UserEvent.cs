using MyWebApi.Entities.SponsorEntities;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserEvent
    {
        [Key]
        public long UserId { get; set; }
        [Key]
        public long EventId { get; set; }
        public virtual User Attendee { get; set; }
        public virtual Event Event { get; set; }
    }
}
