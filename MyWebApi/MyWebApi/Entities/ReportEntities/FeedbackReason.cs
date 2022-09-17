using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.ReasonEntities
{
    public class FeedbackReason
    {
        [Key]
        public short Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string Description { get; set; }
    }
}
