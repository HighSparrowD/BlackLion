using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.EffectEntities
{
    public class ActiveEffect
    {
        [Key]
        public long Id { get; set; }
        public Currency Effect { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public DateTime? ExpirationTime { get; set; }

        public ActiveEffect(long userId)
        {
            UserId = userId;
        }
    }
}
