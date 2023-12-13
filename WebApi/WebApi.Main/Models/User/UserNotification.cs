using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Notification;

#nullable enable
namespace WebApi.Main.Models.User;

public class UserNotification
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("Receiver")]
    public long UserId { get; set; }
    public NotificationType Type { get; set; }
    public Section Section { get; set; }
    public string? Description { get; set; }

    public virtual User? Receiver { get; set; }
}
