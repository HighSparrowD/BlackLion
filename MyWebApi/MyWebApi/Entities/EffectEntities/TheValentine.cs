using System;

namespace WebApi.Entities.EffectEntities
{
    public class TheValentine : ActiveEffect
    {
        public TheValentine(long userId) : base(userId)
        {
            EffectId = 6;
            Name = "TheValentine";
            ExpirationTime = DateTime.SpecifyKind(DateTime.Now.AddHours(1), DateTimeKind.Utc);
        }
    }
}
