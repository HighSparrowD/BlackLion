using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.EffectEntities
{
    public class ActiveEffect
    {
        [Key]
        public long Id { get; set; }
        public Currency EffectId { get; set; }
        public long UserId{ get; set; }
        public string Name { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public short? ExpiresIn { get; set; }

        public ActiveEffect(long userId)
        {
            UserId = userId;
        }
    }
}
