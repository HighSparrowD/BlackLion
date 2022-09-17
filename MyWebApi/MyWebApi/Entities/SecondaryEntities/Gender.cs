using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.SecondaryEntities
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
