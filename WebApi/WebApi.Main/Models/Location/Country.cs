using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.Location;

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
