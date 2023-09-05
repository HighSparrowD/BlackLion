using System;
using System.Drawing;
using System.Text.Json.Serialization;
using WebApi.Entities.EffectEntities;
using WebApi.Enums;
using WebApi.Utilities;

namespace WebApi.Entities.UserActionEntities
{
    public class GetActiveEffect
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("effect")]
        public Currency Effect { get; set; }
        [JsonPropertyName("expirationTime")]
        public string ExpirationTime { get; set; }

        public GetActiveEffect(ActiveEffect effect)
        {
            if (effect.ExpirationTime != null)
                ExpirationTime = effect.ExpirationTime.Value.ToString("dd.MM.yyyy: H:m");

            Name = EnumLocalizer.GetLocalizedValue((Currency)effect.Effect);
            Effect = effect.Effect;
        }
    }
}
