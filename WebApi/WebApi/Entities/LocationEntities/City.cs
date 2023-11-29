using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.LocationEntities
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public AppLanguage CountryLang { get; set; }
        public string CityName { get; set; }
        public int CountryId { get; set; }
    }
}
