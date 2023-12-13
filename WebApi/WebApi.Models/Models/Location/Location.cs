using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Location;

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
}
