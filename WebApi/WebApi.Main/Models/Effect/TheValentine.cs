using System;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.Effect;

public class TheValentine : ActiveEffect
{
    public TheValentine(long userId) : base(userId)
    {
        Effect = Currency.TheValentine;
        Name = "TheValentine";
        ExpirationTime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(1), DateTimeKind.Utc);
    }
}
