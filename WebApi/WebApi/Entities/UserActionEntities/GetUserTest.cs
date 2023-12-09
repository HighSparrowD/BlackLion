using WebApi.Entities.TestEntities;
using WebApi.Entities.UserInfoEntities;
using WebApi.Main.Models.Test;
using WebApi.Main.Models.User;

#nullable enable
namespace WebApi.Entities.UserActionEntities
{
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
}
