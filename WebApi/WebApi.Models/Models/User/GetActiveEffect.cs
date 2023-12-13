using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;
using WebApi.Models.Models.Effect;
using WebApi.Models.Utilities;

namespace WebApi.Models.Models.User
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

            Name = EnumLocalizer.GetLocalizedValue(effect.Effect);
            Effect = effect.Effect;
        }
    }
}
