using System;

namespace MyWebApi.Entities.EffectEntities
{
    public class TheWhiteDetector : ActiveEffect
    {
        public TheWhiteDetector(long userId) : base(userId)
        {
            EffectId = 8;
            Name = "TheWhiteDetector";
            ExpirationTime = DateTime.SpecifyKind(DateTime.Now.AddHours(2), DateTimeKind.Utc);
        }
    }
}
