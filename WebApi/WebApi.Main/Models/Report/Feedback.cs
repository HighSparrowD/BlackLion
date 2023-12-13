using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.Report;

namespace WebApi.Main.Models.Report;

public class Feedback
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("User")]
    public long UserId { get; set; }
    public long? AdminId { get; set; }
    public string Text { get; set; }
    public DateTime InsertedUtc { get; set; }
    public FeedbackReason Reason { get; set; }
    public virtual User.User User { get; set; }
}
