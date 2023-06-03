using WebApi.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    #nullable enable
    public class UpdateUserProfile
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
        public AppLanguage AppLanguageId { get; set; }
        [JsonPropertyName("userMedia")]
        public string? Media { get; set; }
        [JsonPropertyName("isMediaPhoto")]
        public bool IsMediaPhoto { get; set; }
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
        [JsonPropertyName("locationPreferences")]
        public List<int>? LocationPreferences { get; set; }
        [JsonPropertyName("agePrefs")]
        public List<int>? AgePrefs { get; set; }
        [JsonPropertyName("communicationPrefs")]
        public CommunicationPreference CommunicationPrefs { get; set; }
        [JsonPropertyName("userGenderPrefs")]
        public Gender GenderPrefs { get; set; }
        [JsonPropertyName("wasChanged")]
        public bool WasChanged { get; set; }
    }
}
