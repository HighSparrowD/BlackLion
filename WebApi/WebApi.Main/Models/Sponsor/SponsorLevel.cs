using System.ComponentModel.DataAnnotations;

#nullable enable
namespace WebApi.Main.Entities.Sponsor;

public class SponsorLevel
{
    [Key]
    public long SponsorId { get; set; }
    public int Level { get; set; }
    public double Progress { get; set; }
    public double Goal { get; set; }
}
