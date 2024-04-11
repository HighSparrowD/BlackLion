using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Test;

#nullable enable
namespace WebApi.Main.Entities.Test;

public class TestScale
{
    [Key]
    public int Id { get; set; }
    [Key]
    public long TestId { get; set; }
    public AppLanguage TestLanguage { get; set; }
    public string Scale { get; set; } = default!;
    // Used in "Lie" scales. Represents the value,
    // which has to be reached to determine that person has lied
    // during the test
    public int? MinValue { get; set; }

    public TestScale()
    {}

    public static explicit operator TestScale?(models.TestScale? scale)
    {
        if (scale == null)
            return null;

        return new TestScale
        {
            Id = scale.Id,
            Scale = scale.Scale,
            MinValue = scale.MinValue,
            TestId = scale.TestId,
            TestLanguage = scale.TestLanguage
        };
    }

    public static explicit operator models.TestScale?(TestScale? scale)
    {
        if (scale == null)
            return null;

        return new models.TestScale
        {
            Id = scale.Id,
            Scale = scale.Scale,
            MinValue = scale.MinValue,
            TestId = scale.TestId,
            TestLanguage = scale.TestLanguage
        };
    }
}
