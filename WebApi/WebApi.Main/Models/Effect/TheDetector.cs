using WebApi.Enums.Enums.General;

namespace WebApi.Main.Entities.Effect;

public class TheDetector : ActiveEffect
{
    public TheDetector(long userId, DateTime dateTimeNowUtc) : base(userId)
    {
        Effect = Currency.TheDetector;
        Name = "TheDetector";
        ExpirationTime = dateTimeNowUtc.AddHours(1);
    }
}
