using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Effect;

public class TheValentine : ActiveEffect
{
    public TheValentine(long userId, DateTime dateTimeNowUtc) : base(userId)
    {
        Effect = Currency.TheValentine;
        Name = "TheValentine";
        ExpirationTime = dateTimeNowUtc.AddHours(1);
    }
}
