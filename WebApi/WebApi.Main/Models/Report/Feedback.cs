using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.Report;
using models = WebApi.Models.Models.Report;

namespace WebApi.Main.Entities.Report;

#nullable enable
public class Feedback
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("User")]
    public long UserId { get; set; }

    public long? AdminId { get; set; }

    public string? Text { get; set; }

    public DateTime InsertedUtc { get; set; }

    public FeedbackReason Reason { get; set; }

    public virtual User.User? User { get; set; }

    public static explicit operator Feedback?(models.Feedback? feedback)
    {
        if (feedback == null)
            return null;

        return new Feedback
        {
            Id = feedback.Id,
            AdminId = feedback.AdminId,
            InsertedUtc = feedback.InsertedUtc,
            Reason = feedback.Reason,
            Text = feedback.Text,
            UserId = feedback.UserId
        };
    }

    public static implicit operator models.Feedback?(Feedback? feedback)
    {
        if (feedback == null)
            return null;

        return new models.Feedback
        {
            Id = feedback.Id,
            AdminId = feedback.AdminId,
            InsertedUtc = feedback.InsertedUtc,
            Reason = feedback.Reason,
            Text = feedback.Text,
            UserId = feedback.UserId
        };
    }
}
