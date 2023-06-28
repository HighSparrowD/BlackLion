using System;
using WebApi.Enums;

namespace WebApi.Entities.EffectEntities
{
    public class TheValentine : ActiveEffect
    {
        public TheValentine(long userId) : base(userId)
        {
            EffectId = Currency.TheValentine;
            Name = "TheValentine";
            ExpirationTime = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(1), DateTimeKind.Utc);
        }
    }
}
