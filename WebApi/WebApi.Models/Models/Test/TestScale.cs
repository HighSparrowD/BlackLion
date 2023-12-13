using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Test;

public class TestScale
{
    [Key]
    public int Id { get; set; }
    [Key]
    public long TestId { get; set; }
    public AppLanguage TestLanguage { get; set; }
    public string Scale { get; set; }
    // Used in "Lie" scales. Represents the value,
    // which has to be reached to determine that person has lied
    // during the test
    public int? MinValue { get; set; }
}
