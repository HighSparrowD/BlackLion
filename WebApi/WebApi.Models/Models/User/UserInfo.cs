using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;
#nullable enable

namespace WebApi.Models.Models.User
{
    public class UserInfo
    {
        // Data
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("languages")]
        public List<int>? Languages { get; set; }
        [JsonPropertyName("age")]
        public int Age { get; set; }
        [JsonPropertyName("gender")]
        public Gender Gender { get; set; }
        [JsonPropertyName("language")]
        public AppLanguage Language { get; set; } //AppLanguage
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        [JsonPropertyName("voice")]
        public string? Voice { get; set; }
        [JsonPropertyName("languagePreferences")]
        public List<int>? LanguagePreferences { get; set; }
        [JsonPropertyName("locationPreferences")]
        public List<int>? LocationPreferences { get; set; }
        [JsonPropertyName("agePrefs")]
        public List<int>? AgePrefs { get; set; }
        [JsonPropertyName("communicationPrefs")]
        public CommunicationPreference CommunicationPrefs { get; set; }
        [JsonPropertyName("genderPrefs")]
        public Gender GenderPrefs { get; set; }
        [JsonPropertyName("reason")]
        public UsageReason Reason { get; set; }
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        [JsonPropertyName("realName")]
        public string? RealName { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("rawDescription")]
        public string? RawDescription { get; set; }
        [JsonPropertyName("media")]
        public string? Media { get; set; }
        [JsonPropertyName("tags")]
        public string? Tags { get; set; }
        [JsonPropertyName("mediaType")]
        public MediaType MediaType { get; set; }

        // Location
        [JsonPropertyName("city")]
        public int? City { get; set; }
        [JsonPropertyName("country")]
        public int? Country { get; set; }
        [JsonPropertyName("countryLang")]
        public AppLanguage? CountryLang { get; set; }
        [JsonPropertyName("cityLang")]
        public AppLanguage? CityLang { get; set; }

        // Other 
        [JsonPropertyName("identityType")]
        public IdentityConfirmationType IdentityType { get; set; }
        [JsonPropertyName("hasPremium")]
        public bool HasPremium { get; set; }
    }
}
