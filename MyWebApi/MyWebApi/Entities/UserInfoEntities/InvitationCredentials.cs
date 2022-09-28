using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class InvitationCredentials
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string Link { get; set; }
        public string QRCode { get; set; }
        public virtual User Invitor { get; set; }
    }
}
