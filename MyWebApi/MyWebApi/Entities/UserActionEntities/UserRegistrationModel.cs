using WebApi.Entities.SecondaryEntities;
using WebApi.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [JsonPropertyName("shouldUserPersonalityFunc")]
        public bool ShouldUserPersonalityFunc { get; set; }
        [JsonPropertyName("promo")]
        public string? Promo { get; set; }
    }
}
