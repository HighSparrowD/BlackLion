using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.User;

namespace WebApi.Models.Models.Test;

public class Test
{
    public long Id { get; set; }

    public AppLanguage Language { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public OceanStats? TestType { get; set; }

    public int CanBePassedInDays { get; set; }

    public virtual List<TestQuestion> Questions { get; set; }

    public virtual List<TestResult> Results { get; set; }

    public virtual List<TestScale> Scales { get; set; }
}
