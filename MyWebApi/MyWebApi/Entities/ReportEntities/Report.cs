using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.UserInfoEntities;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace MyWebApi.Entities.ReportEntities
{
    public class Report
    {
        [Key]
        public long Id { get; set; }
        public long UserBaseInfoId { get; set; }
        public long UserBaseInfoId1 { get; set; }
        public string? Text { get; set; }
        public short ReasonId { get; set; }
        public int ReasonClassLocalisationId { get; set; }
        public DateTime InsertedUtc { get; set; }
        public virtual ReportReason? Reason { get; set; }
        [ForeignKey("UserBaseInfoId")]
        public virtual UserBaseInfo? Sender { get; set; }
        [ForeignKey("UserBaseInfoId1")]
        public virtual UserBaseInfo? User { get; set; }
    }
}
