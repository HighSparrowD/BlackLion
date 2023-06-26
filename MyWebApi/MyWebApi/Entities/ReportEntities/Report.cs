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
        public long Id { get; set; }
        [ForeignKey("Sender")]
        public long SenderId { get; set; }
        [ForeignKey("User")]
        public long UserId { get; set; }
        public long? AdminId { get; set; }
        public string? Text { get; set; }
        public ReportReason Reason { get; set; }
        public DateTime InsertedUtc { get; set; }
        public virtual User? Sender { get; set; }
        public virtual User? User { get; set; }
    }
}
