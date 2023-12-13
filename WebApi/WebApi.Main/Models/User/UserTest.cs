using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.User;

#nullable enable
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
    public Enums.Enums.User.OceanStats? TestType { get; set; }
    public float Result { get; set; }
    public DateTime? PassedOn { get; set; }
    public virtual Test.Test Test { get; set; } = new Test.Test();

    public UserTest()
    {}

    public static explicit operator UserTest?(models.UserTest userTest)
    {
        if (userTest == null)
            return null;

        return new UserTest
        {
            TestId = userTest.TestId,
            TestType = userTest.TestType,
            PassedOn = userTest.PassedOn,
            Result = userTest.Result,
            Test = (Test.Test)userTest.Test!,
            TestLanguage = userTest.TestLanguage,
            UserId = userTest.UserId
        };
    }

    public static implicit operator models.UserTest?(UserTest userTest)
    {
        if (userTest == null)
            return null;

        return new models.UserTest
        {
            TestId = userTest.TestId,
            TestType = userTest.TestType,
            PassedOn = userTest.PassedOn,
            Result = userTest.Result,
            Test = (WebApi.Models.Models.Test.Test)userTest.Test!,
            TestLanguage = userTest.TestLanguage,
            UserId = userTest.UserId
        };
    }
}
