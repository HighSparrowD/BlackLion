using MyWebApi.Entities.UserInfoEntities;

namespace MyWebApi.Entities.UserActionEntities
{
    public class RecalculationResult
    {
        public UserPersonalityStats Stats { get; set; }
        public int TestResult { get; set; }
    }
}
