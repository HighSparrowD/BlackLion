using WebApi.Enums.Enums.General;

namespace WebApi.Main.Entities.Effect;

public class TheDetector : ActiveEffect
{
    public TheDetector(long userId) : base(userId)
    {
        Effect = Currency.TheDetector;
        Name = "TheDetector";
        ExpirationTime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(1), DateTimeKind.Utc);
    }
}
