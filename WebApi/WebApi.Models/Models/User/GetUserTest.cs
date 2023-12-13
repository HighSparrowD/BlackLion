#nullable enable
using WebApi.Models.Models.Test;
using WebApi.Models.Models.User;

namespace WebApi.Models.User;

public class GetUserTest
{
    public string? PassedOn { get; set; }
    public Test? Test { get; set; }

    public GetUserTest(UserTest testModel)
    {
        if (testModel.PassedOn != null)
            PassedOn = testModel.PassedOn.Value.ToString("dd.MM.yyyy");

        Test = testModel.Test;
    }
}
