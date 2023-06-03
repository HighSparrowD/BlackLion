using WebApi.Entities.UserInfoEntities;
using WebApi.Enums;
using System;
using System.ComponentModel.DataAnnotations;

#nullable enable
namespace WebApi.Entities.UserActionEntities
{
    public class UserNotification
    {
        [Key]
        public Guid Id { get; set; }
        public long? UserId { get; set; }
        public long UserId1 { get; set; }
        public bool IsLikedBack { get; set; }
        public Severities Severity { get; set; }
        public Sections Section { get; set; }
        public string? Description { get; set; }
        //[ForeignKey("UserId")]
        //public virtual User? Sender { get; set; }
        //[ForeignKey("UserId1")]
        //public virtual User? Receiver { get; set; }
    }
}
