using MyWebApi.Entities.UserInfoEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserActionEntities
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
