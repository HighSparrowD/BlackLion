using System;

namespace MyWebApi.Entities.AdminEntities
{
    public class ResolveTickRequest
    {
        public Guid Id { get; set; }
        public long AdminId { get; set; }
        public bool IsAccepted { get; set; }
        public string Comment { get; set; }
    }
}
