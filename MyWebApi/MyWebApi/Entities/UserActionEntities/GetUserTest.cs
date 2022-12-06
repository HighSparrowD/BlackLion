using MyWebApi.Entities.TestEntities;
using MyWebApi.Entities.UserInfoEntities;
    
#nullable enable
namespace MyWebApi.Entities.UserActionEntities
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
