using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.LocationEntities
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public AppLanguage CountryClassLocalisationId { get; set; }
        public string CityName { get; set; }
        public int CountryId { get; set; }
    }
}
