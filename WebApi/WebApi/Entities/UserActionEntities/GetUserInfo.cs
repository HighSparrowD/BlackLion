using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebApi.Main.Enums.General;
using WebApi.Main.Enums.Media;
using WebApi.Main.Enums.User;
#nullable enable

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserInfo
    {
        // Data
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("userLanguages")]
        public List<int>? UserLanguages { get; set; }
        [JsonPropertyName("userAge")]
        public int UserAge { get; set; }
        [JsonPropertyName("userGender")]
        public Gender UserGender { get; set; }
        [JsonPropertyName("language")]
        public AppLanguage Language { get; set; } //AppLanguage
        [JsonPropertyName("autoReplyText")]
        public string? AutoReplyText { get; set; }
        [JsonPropertyName("autoReplyVoice")]
        public string? AutoReplyVoice { get; set; }
        [JsonPropertyName("languagePreferences")]
        public List<int>? LanguagePreferences { get; set; }
        [JsonPropertyName("locationPreferences")]
        public List<int>? LocationPreferences { get; set; }
        [JsonPropertyName("agePrefs")]
        public List<int>? AgePrefs { get; set; }
        [JsonPropertyName("communicationPrefs")]
        public CommunicationPreference CommunicationPrefs { get; set; }
        [JsonPropertyName("userGenderPrefs")]
        public Gender UserGenderPrefs { get; set; }
        [JsonPropertyName("reason")]
        public UsageReason Reason { get; set; }
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }
        [JsonPropertyName("userRealName")]
        public string? UserRealName { get; set; }
        [JsonPropertyName("userDescription")]
        public string? UserDescription { get; set; }
        [JsonPropertyName("userRawDescription")]
        public string? UserRawDescription { get; set; }
        [JsonPropertyName("userMedia")]
        public string? UserMedia { get; set; }
        [JsonPropertyName("mediaType")]
        public MediaType MediaType { get; set; }

        // Location
        [JsonPropertyName("cityId")]
        public int? CityId { get; set; }
        [JsonPropertyName("countryId")]
        public int? CountryId { get; set; }
        [JsonPropertyName("countryLang")]
        public AppLanguage? CountryLang { get; set; }
        [JsonPropertyName("cityCountryLang")]
        public AppLanguage? CityCountryLang { get; set; }

        // Other 
        [JsonPropertyName("identityType")]
        public IdentityConfirmationType IdentityType  { get; set; }
        [JsonPropertyName("hasPremium")]
        public bool HasPremium { get; set; }
    }
}
