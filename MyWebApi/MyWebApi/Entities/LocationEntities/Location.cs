using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.LocationEntities
{
    #nullable enable
    public class Location
    {
        [Key]
        public long Id { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public AppLanguage? CityCountryClassLocalisationId { get; set; }
        public AppLanguage? CountryClassLocalisationId { get; set; }
        public virtual Country? Country { get; set; }
        public virtual City? City { get; set; }
    }
}
