using WebApi.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable enable

namespace WebApi.Entities.UserInfoEntities
{
    public class UserData
    {
        [Key]
        public long Id { get; set; }
        public List<int>? UserLanguages { get; set; } 
        public int UserAge { get; set; }
        public short UserGender { get; set; }
        public int LanguageId { get; set; } //AppLanguage
        public string? AutoReplyText { get; set; }
        public string? AutoReplyVoice { get; set; }
        public List<int>? LanguagePreferences { get; set; }
        public List<int>? LocationPreferences { get; set; }
        public List<int>? AgePrefs { get; set; }
        public int CommunicationPrefs { get; set; }
        public short UserGenderPrefs { get; set; }
        public virtual UsageReason Reason { get; set; }
        public string? UserName { get; set; }
        public string? UserRealName { get; set; }
        public string? UserDescription { get; set; }
        public string? UserRawDescription { get; set; }
        public string? UserMedia { get; set; }
        public bool IsMediaPhoto { get; set; }

        //public virtual Language? Language { get; set; }
    }
}
