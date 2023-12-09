using System;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.Effect;

public class TheDetector : ActiveEffect
{
    public TheDetector(long userId) : base(userId)
    {
        Effect = Currency.TheDetector;
        Name = "TheDetector";
        ExpirationTime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(1), DateTimeKind.Utc);
    }
}
