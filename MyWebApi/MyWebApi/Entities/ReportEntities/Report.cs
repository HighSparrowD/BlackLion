using WebApi.Entities.ReasonEntities;
using WebApi.Entities.UserInfoEntities;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums;

#nullable enable

namespace WebApi.Entities.ReportEntities
{
    public class Report
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long UserId1 { get; set; }
        public string? Text { get; set; }
        public ReportReason Reason { get; set; }
        public DateTime InsertedUtc { get; set; }
        [ForeignKey("UserId")]
        public virtual User? Sender { get; set; }
        [ForeignKey("UserId1")]
        public virtual User? User { get; set; }
    }
}
