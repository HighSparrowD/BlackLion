using WebApi.Entities.UserInfoEntities;

namespace WebApi.Entities.UserActionEntities
{
    public class RecalculationResult
    {
        public OceanStats Stats { get; set; }
        public float TestResult { get; set; }
    }
}
