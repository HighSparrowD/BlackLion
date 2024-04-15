using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Main.Entities.User;

public class Invitation
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("InviterCredentials")]
    public Guid InviterCredentialsId { get; set; }
    public long InvitedUserId { get; set; }
    public DateTime InvitationTime { get; set; }
    public virtual InvitationCredentials InviterCredentials { get; set; }
}
