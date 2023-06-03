using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.LocationEntities
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int CountryClassLocalisationId { get; set; }
        public string CityName { get; set; }
        public int CountryId { get; set; }
    }
}
