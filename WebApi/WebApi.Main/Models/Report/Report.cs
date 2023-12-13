using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.Report;

#nullable enable

namespace WebApi.Main.Models.Report;

public class Report
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("Sender")]
    public long SenderId { get; set; }
    [ForeignKey("User")]
    public long? UserId { get; set; }
    [ForeignKey("Adventure")]
    public long? AdventureId { get; set; }
    public long? AdminId { get; set; }
    public string? Text { get; set; }
    public ReportReason Reason { get; set; }
    public DateTime InsertedUtc { get; set; }
    public virtual User.User? Sender { get; set; }
    public virtual User.User? User { get; set; }
    public virtual Adventure.Adventure? Adventure { get; set; }
}
