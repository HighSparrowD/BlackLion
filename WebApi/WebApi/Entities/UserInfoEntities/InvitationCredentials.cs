using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities.UserInfoEntities
{
    public class InvitationCredentials
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("Inviter")]
        public long UserId { get; set; }
        public string Link { get; set; }
        public string QRCode { get; set; }
        public virtual User Inviter { get; set; }

        public InvitationCredentials()
        {

        }
    }
}
