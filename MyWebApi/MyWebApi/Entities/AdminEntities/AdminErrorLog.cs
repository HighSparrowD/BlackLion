using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.AdminEntities
{
    public class AdminErrorLog
    {
        [Key]
        public Guid Id { get; set; }
        public long? ThrownByUser { get; set; }
        public string Description { get; set; }
        public int SectionId { get; set; }
        public DateTime Time { get; set; }
    }
}
