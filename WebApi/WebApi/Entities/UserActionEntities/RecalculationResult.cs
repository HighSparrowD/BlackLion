using WebApi.Entities.UserInfoEntities;
using WebApi.Main.Models.User;

namespace WebApi.Entities.UserActionEntities
{
    public class RecalculationResult
    {
        public OceanStats Stats { get; set; }
        public float TestResult { get; set; }
    }
}
