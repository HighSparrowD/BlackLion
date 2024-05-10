using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.Report;
using models = WebApi.Models.Models.Report;

#nullable enable

namespace WebApi.Main.Entities.Report;

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

    public static explicit operator Report?(models.Report? report)
    {
        if (report == null)
            return null;

        return new Report
        {
            Id = report.Id,
            AdminId = report.AdminId,
            InsertedUtc = report.InsertedUtc,
            Reason = report.Reason,
            Text = report.Text,
            UserId = report.UserId,
            AdventureId = report.AdventureId,
            SenderId = report.SenderId
        };
    }

    public static implicit operator models.Report?(Report? report)
    {
        if (report == null)
            return null;

        return new models.Report
        {
            Id = report.Id,
            AdminId = report.AdminId,
            InsertedUtc = report.InsertedUtc,
            Reason = report.Reason,
            Text = report.Text,
            UserId = report.UserId,
            SenderId = report.SenderId,
            AdventureId = report.AdventureId
        };
    }
}
