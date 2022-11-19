using System.Collections.Generic;

namespace MyWebApi.Entities.UserActionEntities
{
    #nullable enable
    public class UpdateUserProfile
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? UserRealName { get; set; }
        public string? UserDescription { get; set; }
        public int UserAppLanguageId { get; set; }
        public string? UserPhoto { get; set; }
        public int? UserCountryCode { get; set; }
        public int? UserCityCode { get; set; }
        public List<int>? UserLanguages { get; set; }
        public short ReasonId { get; set; }
        public int UserAge { get; set; }
        public short UserGender { get; set; }
        public List<int>? UserLanguagePreferences { get; set; }
        public List<int>? UserLocationPreferences { get; set; }
        public List<int>? AgePrefs { get; set; }
        public int CommunicationPrefs { get; set; }
        public short UserGenderPrefs { get; set; }
        public bool ShouldUserPersonalityFunc { get; set; }
        public bool IsPhotoReal { get; set; }
        public bool WasChanged { get; set; }
    }
}
