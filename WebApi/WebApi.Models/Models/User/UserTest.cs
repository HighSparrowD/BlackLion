using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;

#nullable enable
namespace WebApi.Models.Models.User;

public class UserTest
{
    [NotNull]
    public long UserId { get; set; }

    [NotNull]
    public long TestId { get; set; }
    [NotNull]
    public AppLanguage TestLanguage { get; set; }
    
    [NotNull]
    public Enums.Enums.User.OceanStats? TestType { get; set; }
    
    public float Result { get; set; }
    
    public DateTime? PassedOn { get; set; }

    public Test.Test? Test { get; set; }
}
