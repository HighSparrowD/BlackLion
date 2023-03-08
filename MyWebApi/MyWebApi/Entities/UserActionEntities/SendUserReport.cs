using MyWebApi.Entities.ReportEntities;
using MyWebApi.Enums;
using System;

namespace MyWebApi.Entities.UserActionEntities
{
    public class SendUserReport
    {
        public long Sender { get; set; }
        public long ReportedUser { get; set; }
        public string Text { get; set; }
        public ReportReason Reason { get; set; }
    }
}
