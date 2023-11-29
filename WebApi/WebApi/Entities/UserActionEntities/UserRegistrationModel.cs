using WebApi.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable enable
namespace WebApi.Entities.UserActionEntities
{
    public class UserRegistrationModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("userName")]
        public string? UserName { get; set; }
        [JsonPropertyName("userRealName")]
        public string? RealName { get; set; }
        [JsonPropertyName("userDescription")]
        public string? Description { get; set; }
        [JsonPropertyName("userAppLanguageId")]
        public AppLanguage AppLanguage { get; set; }
        [JsonPropertyName("userMedia")]
        public string? Media { get; set; }
        [JsonPropertyName("mediaType")]
        public MediaType MediaType { get; set; }
        [JsonPropertyName("userCountryCode")]
        public int? CountryCode { get; set; }
        [JsonPropertyName("userCityCode")]
        public int? CityCode { get; set; }
        [JsonPropertyName("userLanguages")]
        public List<int>? Languages { get; set; }
        [JsonPropertyName("reasonId")]
        public UsageReason Reason { get; set; }
        [JsonPropertyName("userAge")]
        public int Age { get; set; }
        [JsonPropertyName("userGender")]
        public Gender Gender { get; set; }
        [JsonPropertyName("userLanguagePreferences")]
        public List<int>? LanguagePreferences { get; set; }
        [JsonPropertyName("userlocationPreferences")]
        public List<int>? UserLocationPreferences { get; set; }
        [JsonPropertyName("agePrefs")]
        public List<int>? AgePrefs { get; set; }
        [JsonPropertyName("communicationPrefs")]
        public CommunicationPreference CommunicationPrefs { get; set; }
        [JsonPropertyName("userGenderPrefs")]
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
