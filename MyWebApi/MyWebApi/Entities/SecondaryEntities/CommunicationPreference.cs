using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.SecondaryEntities
{
    public class CommunicationPreference
    {
        [Key]
        public short Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string CommunicationPrefName { get; set; }
    }
}
