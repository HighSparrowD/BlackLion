using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.ReportEntities
{
    public class ReportReason
    {
        [Key]
        public short Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string Description { get; set; }
    }
}
