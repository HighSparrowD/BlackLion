using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.SecondaryEntities
{
    public class UserReason
    {
        [Key]
        public short Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string ReasonName { get; set; }
    }
}
