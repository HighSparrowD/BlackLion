using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.User;

#nullable enable
namespace WebApi.Models.Models.Test;

public class Test
{
    public long Id { get; set; }

    public AppLanguage Language { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public OceanStats? TestType { get; set; }

    public int? CanBePassedInDays { get; set; }
}
