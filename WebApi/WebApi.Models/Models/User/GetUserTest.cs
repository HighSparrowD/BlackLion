#nullable enable
using WebApi.Models.Models.Test;

namespace WebApi.Models.User;

public class GetUserTest
{
    public string? PassedOn { get; set; }
    public Test? Test { get; set; }

    public GetUserTest(DateTime? passedOn, Test? test)
    {
        if (passedOn != null)
            PassedOn = passedOn.Value.ToString("dd.MM.yyyy");

        Test = test;
    }
}
