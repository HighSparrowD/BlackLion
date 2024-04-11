using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Location;

#nullable enable
namespace WebApi.Main.Entities.Location;

public class Country
{
    [Key]
    public int Id { get; set; }
    [Key]
    public AppLanguage Lang { get; set; }
    public string CountryName { get; set; } = string.Empty;
    [AllowNull]
    public short? Priority { get; set; }
    public virtual List<City> Cities { get; set; } = new List<City>();

    public Country()
    { }

    public static explicit operator Country?(models.Country? country)
    {
        if (country == null)
            return null;

        return new Country
        {
            Id = country.Id,
            Lang = country.Lang,
            Priority = country.Priority,
            CountryName = country.CountryName,
            Cities = country.Cities.Select(c => (City)c!).ToList()!
        };
    }

    public static implicit operator models.Country?(Country? country)
    {
        if (country == null)
            return null;

        return new models.Country
        {
            Id = country.Id,
            Lang = country.Lang,
            Priority = country.Priority,
            CountryName = country.CountryName,
            Cities = country.Cities.Select(c => (models.City?) c).ToList()
        };
    }
}
