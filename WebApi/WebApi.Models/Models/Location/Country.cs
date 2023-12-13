using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Location;

public class Country
{
    [Key]
    public int Id { get; set; }
    [Key]
    public AppLanguage Lang { get; set; }
    public string CountryName { get; set; }
    [AllowNull]
    public short? Priority { get; set; }
    public virtual List<City> Cities { get; set; }
}
