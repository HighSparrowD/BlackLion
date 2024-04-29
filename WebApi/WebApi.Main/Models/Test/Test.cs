using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.User;
using models = WebApi.Models.Models.Test;

#nullable enable
namespace WebApi.Main.Entities.Test;

public class Test
{
    [Key]
    [NotNull]
    public long Id { get; set; }
    [Key]
    [NotNull]
    public AppLanguage Language { get; set; }
    [NotNull]
    public string Name { get; set; } = default!;

    public string? Description { get; set; } = default!;

    public OceanStats? TestType { get; set; }

    public int? CanBePassedInDays { get; set; }

    public virtual List<TestQuestion>? Questions { get; set; } = default!;

    public virtual List<TestResult>? Results { get; set; } = default!;

    public virtual List<TestScale>? Scales { get; set; } = default!;

    public Test()
    {}

    public static explicit operator Test? (models.Test? test)
    {
        if (test == null)
            return null;

        return new Test
        {
            Id = test.Id,
            CanBePassedInDays = test.CanBePassedInDays,
            Description = test.Description,
            TestType = test.TestType,
            Language = test.Language,
            Name = test.Name
        };
    }

    public static implicit operator models.Test?(Test? test)
    {
        if (test == null)
            return null;

        return new models.Test
        {
            Id = test.Id,
            CanBePassedInDays = test.CanBePassedInDays,
            Description = test.Description,
            TestType = test.TestType,
            Language = test.Language,
            Name = test.Name,
        };
    }
}
