using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Test;

#nullable enable
namespace WebApi.Main.Models.Test;

public class TestResult
{
    [Key]
    [NotNull]
    public long Id { get; set; }
    [NotNull]
    public long TestId { get; set; }
    [NotNull]
    public AppLanguage TestLanguage { get; set; }
    [NotNull]
    public int? Score { get; set; }
    [NotNull]
    public string Result { get; set; }
    public List<long> Tags { get; set; }

    public TestResult()
    {}

    public static explicit operator TestResult?(models.TestResult? result)
    {
        if (result == null)
            return null;

        return new TestResult
        {
            Id = result.Id,
            Score = result.Score,
            Result = result.Result,
            Tags = result.Tags,
            TestId = result.TestId,
            TestLanguage = result.TestLanguage
        };
    }

    public static explicit operator models.TestResult?(TestResult? result)
    {
        if (result == null)
            return null;

        return new models.TestResult
        {
            Id = result.Id,
            Score = result.Score,
            Result = result.Result,
            Tags = result.Tags,
            TestId = result.TestId,
            TestLanguage = result.TestLanguage
        };
    }
}
