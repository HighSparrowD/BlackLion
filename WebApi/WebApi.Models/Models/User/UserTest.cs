using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.User;

public class UserTest
{
    [Key]
    [NotNull]
    public long UserId { get; set; }
    [Key]
    [NotNull]
    public long TestId { get; set; }
    [NotNull]
    public AppLanguage TestLanguage { get; set; }
    [NotNull]
    public Enums.Enums.User.OceanStats? TestType { get; set; }
    public float Result { get; set; }
    public DateTime? PassedOn { get; set; }
    public virtual Test.Test Test { get; set; }
}
