using WebApi.Entities.UserInfoEntities;

namespace WebApi.Entities.UserActionEntities
{
    public class RecalculationResult
    {
        public UserPersonalityStats Stats { get; set; }
        public int TestResult { get; set; }
    }
}
