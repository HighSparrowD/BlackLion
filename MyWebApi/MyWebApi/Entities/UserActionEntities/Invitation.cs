using WebApi.Entities.UserInfoEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserActionEntities
{
    public class Invitation
    {
        [Key]
        public Guid Id { get; set; }
        public Guid InvitorCredentialsId { get; set; }
        public long InvitedUserId { get; set; }
        public DateTime InvitationTime { get; set; }
        public virtual InvitationCredentials InvitorCredentials { get; set; }
    }
}
