using System.Text.Json.Serialization;
using WebApi.Enums;

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserSettings
    {
        [JsonPropertyName("userOcean")]
        public bool UsesOcean { get; set; }
        [JsonPropertyName("shouldFilterUsersWithoutRealPhoto")]
        public bool ShouldFilterUsersWithoutRealPhoto { get; set; }
        [JsonPropertyName("shouldConsiderLanguages")]
        public bool ShouldConsiderLanguages { get; set; }
        [JsonPropertyName("shouldComment")]
        public bool ShouldComment { get; set; }
        [JsonPropertyName("shouldSendHints")]
        public bool ShouldSendHints { get; set; }
        [JsonPropertyName("increasedFamiliarity")]
        public bool IncreasedFamiliarity { get; set; }
        [JsonPropertyName("isFree")]
        public bool? IsFree { get; set; }
        [JsonPropertyName("hasPremium")]
        public bool HasPremium { get; set; }
        [JsonPropertyName("language")]
        public AppLanguage Language { get; set; }
    }
}
