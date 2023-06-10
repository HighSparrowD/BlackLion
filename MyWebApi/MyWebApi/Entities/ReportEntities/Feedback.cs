using WebApi.Entities.ReasonEntities;
using WebApi.Entities.UserInfoEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.ReportEntities
{
    public class Feedback
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
        public short ReasonId { get; set; }
        public int ReasonClassLocalisationId { get; set; }
        public DateTime InsertedUtc { get; set; }
        public virtual FeedbackReason Reason { get; set; }
        public virtual User User { get; set; }
    }
}
