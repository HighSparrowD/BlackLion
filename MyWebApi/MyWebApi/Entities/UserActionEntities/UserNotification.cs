using MyWebApi.Entities.UserInfoEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace MyWebApi.Entities.UserActionEntities
{
    public class UserNotification
    {
        [Key]
        public Guid Id { get; set; }
        public long? UserId { get; set; }
        public long UserId1 { get; set; }
        public bool IsLikedBack { get; set; }
        public short Severity { get; set; }
        public int SectionId { get; set; }
        public string? Description { get; set; }
        //[ForeignKey("UserId")]
        //public virtual User? Sender { get; set; }
        //[ForeignKey("UserId1")]
        //public virtual User? Receiver { get; set; }
    }
}
