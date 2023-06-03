using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.SecondaryEntities
{
    public class Gender
    {
        [Key]
        public short Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string GenderName { get; set; }
    }
}
