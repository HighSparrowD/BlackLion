using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.SecondaryEntities
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
