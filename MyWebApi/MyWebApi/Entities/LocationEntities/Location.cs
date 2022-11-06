using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.LocationEntities
{
    #nullable enable
    public class Location
    {
        [Key]
        public long Id { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public int? CityCountryClassLocalisationId { get; set; }
        public int? CountryClassLocalisationId { get; set; }
        public virtual Country? Country { get; set; }
        public virtual City? City { get; set; }
    }
}
