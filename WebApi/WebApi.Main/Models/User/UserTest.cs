using WebApi.Entities.TestEntities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.User;

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
    public Enums.User.OceanStats? TestType { get; set; }
    public float Result { get; set; }
    public DateTime? PassedOn { get; set; }
    public virtual Test.Test Test { get; set; }
}
