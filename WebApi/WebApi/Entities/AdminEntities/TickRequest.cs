using WebApi.Entities.UserInfoEntities;
using WebApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities.AdminEntities
{
    #nullable enable
    public class TickRequest
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("User")]
        public long UserId { get; set; }
        public long? AdminId { get; set; }
        public TickRequestStatus? State { get; set; }
        public string? Photo { get; set; }
        public string? Video { get; set; }
        public string? Circle { get; set; }
        public string? Gesture { get; set; }
        public IdentityConfirmationType Type { get; set; }
        public virtual User? User{ get; set; }
    }
}
