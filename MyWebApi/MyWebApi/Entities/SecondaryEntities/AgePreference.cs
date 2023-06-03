using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.SecondaryEntities
{
    public class AgePreference
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public short ClassLocalisationId { get; set; }
        public string AgePrefName { get; set; }
    }
}
