using System;

namespace MyWebApi.Entities.SponsorEntities
{
    public class PostponeEvent
    {
        public long EventId { get; set; }
        public DateTime PostponeUntil { get; set; }
        public string Comment { get; set; }
    }
}
