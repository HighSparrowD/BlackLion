using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.Adventure;

namespace WebApi.Main.Entities.Adventure;

public class AdventureAttendee
{
    [Key]
    public long UserId { get; set; }
    [Key]
    public long AdventureId { get; set; }
    public string Username { get; set; }
    public AdventureAttendeeStatus Status { get; set; }
    public virtual Adventure Adventure { get; set; }
}
