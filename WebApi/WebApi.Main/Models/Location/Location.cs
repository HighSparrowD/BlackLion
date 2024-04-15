using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Location;

namespace WebApi.Main.Entities.Location;

#nullable enable
public class Location
{
    [Key]
    public long Id { get; set; }
    public int? CityId { get; set; }
    public int? CountryId { get; set; }
    public AppLanguage? CountryLang { get; set; }
    public AppLanguage? CityCountryLang { get; set; }
    public virtual Country? Country { get; set; }
    public virtual City? City { get; set; }

    public Location()
    {}

    public static explicit operator Location? (models.Location? location)
    {
        if (location == null)
            return null;

        return new Location
        {
            Id = location.Id,
            CityId = location.CityId,
            CountryId = location.CountryId,
            CountryLang = location.CountryLang,
            CityCountryLang = location.CityCountryLang,
            City = (City?)location.City,
            Country = (Country?)location.Country
        };
    }

    public static implicit operator models.Location?(Location? location)
    {
        if (location == null)
            return null;

        return new models.Location
        {
            Id = location.Id,
            CityId = location.CityId,
            CountryId = location.CountryId,
            CountryLang = location.CountryLang,
            CityCountryLang = location.CityCountryLang,
            City = location.City,
            Country = location.Country
        };
    }
}
