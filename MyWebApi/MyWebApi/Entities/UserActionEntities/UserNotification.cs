using WebApi.Entities.UserInfoEntities;
using WebApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace WebApi.Entities.UserActionEntities
{
    public class UserNotification
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Sender")]
        public long? SenderId { get; set; }
        [ForeignKey("Receiver")]
        public long ReceiverId { get; set; }
        public bool IsLikedBack { get; set; }
        public Severities Severity { get; set; }
        public Section Section { get; set; }
        public string? Description { get; set; }
        public virtual User? Sender { get; set; }
        public virtual User? Receiver { get; set; }
    }
}
