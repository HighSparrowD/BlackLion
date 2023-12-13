using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.User;
using models = WebApi.Models.Models.Test;

#nullable enable
namespace WebApi.Main.Models.Test;

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
    [NotNull]
    public string Description { get; set; } = default!;
    [NotNull]
    public OceanStats? TestType { get; set; }
    [NotNull]
    public int CanBePassedInDays { get; set; }
    [NotNull]
    public virtual List<TestQuestion> Questions { get; set; } = default!;
    [NotNull]
    public virtual List<TestResult> Results { get; set; } = default!;
    [NotNull]
    public virtual List<TestScale> Scales { get; set; } = default!;

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
            Name = test.Name,
            Questions = test.Questions.Select(q => (TestQuestion)q!).ToList(),
            Results = test.Results.Select(r => (TestResult)r!).ToList(),
            Scales = test.Scales.Select(s => (TestScale)s!).ToList()
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
            Questions = test.Questions.Select(q => (models.TestQuestion)q!).ToList(),
            Results = test.Results.Select(r => (models.TestResult)r!).ToList(),
            Scales = test.Scales.Select(s => (models.TestScale)s!).ToList()
        };
    }
}
