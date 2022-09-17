using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.UserInfoEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.ReportEntities
{
    public class Feedback
    {
        [Key]
        public long Id { get; set; }
        public long UserBaseInfoId { get; set; }
        public string Text { get; set; }
        public short ReasonId { get; set; }
        public int ReasonClassLocalisationId { get; set; }
        public DateTime InsertedUtc { get; set; }
        public virtual FeedbackReason Reason { get; set; }
        public virtual UserBaseInfo User { get; set; }
    }
}
