using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;

#nullable enable
namespace WebApi.Models.Models.User
{
    public class UserRegistrationModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("username")]
        public string? UserName { get; set; }
        [JsonPropertyName("realName")]
        public string? RealName { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("appLanguage")]
        public AppLanguage AppLanguage { get; set; }
        [JsonPropertyName("media")]
        public string? Media { get; set; }
        [JsonPropertyName("mediaType")]
        public MediaType MediaType { get; set; }
        [JsonPropertyName("country")]
        public int? CountryCode { get; set; }
        [JsonPropertyName("city")]
        public int? CityCode { get; set; }
        [JsonPropertyName("languages")]
        public List<int>? Languages { get; set; }
        [JsonPropertyName("reason")]
        public UsageReason Reason { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }
        [JsonPropertyName("languagePreferences")]
        public List<int>? LanguagePreferences { get; set; }
        [JsonPropertyName("locationPreferences")]
        public List<int>? UserLocationPreferences { get; set; }
        [JsonPropertyName("agePrefs")]
        public List<int>? AgePrefs { get; set; }
        [JsonPropertyName("communicationPrefs")]
        public CommunicationPreference CommunicationPrefs { get; set; }
        [JsonPropertyName("genderPrefs")]
        public Gender GenderPrefs { get; set; }
        [JsonPropertyName("voice")]
        public string? Voice { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        [JsonPropertyName("tags")]
        public string? Tags { get; set; }
        [JsonPropertyName("usesOcean")]
        public bool UsesOcean { get; set; }
        [JsonPropertyName("promo")]
        public string? Promo { get; set; }
    }
}
