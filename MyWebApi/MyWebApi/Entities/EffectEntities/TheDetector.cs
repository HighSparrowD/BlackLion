using System;

namespace MyWebApi.Entities.EffectEntities
{
    public class TheDetector : ActiveEffect
    {
        public TheDetector(long userId) : base(userId)
        {
            EffectId = 7;
            Name = "TheDetector";
            ExpirationTime = DateTime.SpecifyKind(DateTime.Now.AddHours(1), DateTimeKind.Utc);
        }
    }
}
