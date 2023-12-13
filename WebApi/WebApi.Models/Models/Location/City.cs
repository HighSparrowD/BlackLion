using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Location;

public class City
{
    [Key]
    public int Id { get; set; }
    [Key]
    public AppLanguage CountryLang { get; set; }
    public string CityName { get; set; }
    public int CountryId { get; set; }
}
