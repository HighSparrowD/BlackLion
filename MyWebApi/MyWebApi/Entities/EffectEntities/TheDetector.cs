using System;
using WebApi.Enums;

namespace WebApi.Entities.EffectEntities
{
    public class TheDetector : ActiveEffect
    {
        public TheDetector(long userId) : base(userId)
        {
            EffectId = Currency.TheDetector;
            Name = "TheDetector";
            ExpirationTime = DateTime.SpecifyKind(DateTime.Now.AddHours(1), DateTimeKind.Utc);
        }
    }
}
