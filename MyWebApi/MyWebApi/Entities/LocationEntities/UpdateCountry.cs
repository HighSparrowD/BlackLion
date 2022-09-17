using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.LocationEntities
{
    public class UpdateCountry
    {
        [Key]
        public long Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string CountryName { get; set; }
    }
}
