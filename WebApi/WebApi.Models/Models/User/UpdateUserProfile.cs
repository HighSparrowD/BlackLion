using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;

namespace WebApi.Models.Models.User
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
        [JsonPropertyName("userAppLanguage")]
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
        [JsonPropertyName("locationPreferences")]
        public List<int>? LocationPreferences { get; set; }
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
        [JsonPropertyName("wasChanged")]
        public bool WasChanged { get; set; }
    }
}
