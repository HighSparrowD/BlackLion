using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Location;

#nullable enable
namespace WebApi.Main.Models.Location;

public class City
{
    [Key]
    public int Id { get; set; }
    [Key]
    public AppLanguage CountryLang { get; set; }
    public string CityName { get; set; } = string.Empty;
    public int CountryId { get; set; }

    public City()
    {}

    public static explicit operator City?(models.City? city)
    {
        if (city == null)
            return null;

        return new City
        {
            Id = city.Id,
            CityName = city.CityName,
            CountryId = city.CountryId,
            CountryLang = city.CountryLang
        };
    }

    public static implicit operator models.City?(City? city)
    {
        if (city == null)
            return null;

        return new models.City
        {
            Id = city.Id,
            CityName = city.CityName,
            CountryId = city.CountryId,
            CountryLang = city.CountryLang
        };
    }
}
