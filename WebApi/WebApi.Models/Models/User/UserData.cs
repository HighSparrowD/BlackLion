using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;
#nullable enable

namespace WebApi.Models.Models.User;

public class UserData
{
    [Key]
    public long Id { get; set; }
    public List<int>? UserLanguages { get; set; }
    public int UserAge { get; set; }
    public Gender UserGender { get; set; }
    public AppLanguage Language { get; set; } //AppLanguage
    public string? AutoReplyText { get; set; }
    public string? AutoReplyVoice { get; set; }
    public List<int>? LanguagePreferences { get; set; }
    public List<int>? LocationPreferences { get; set; }
    public List<int>? AgePrefs { get; set; }
    public CommunicationPreference CommunicationPrefs { get; set; }
    public Gender UserGenderPrefs { get; set; }
    public UsageReason Reason { get; set; }
    public string? UserName { get; set; }
    public string? UserRealName { get; set; }
    public string? UserDescription { get; set; }
    public string? UserRawDescription { get; set; }
    public string? UserMedia { get; set; }
    public MediaType MediaType { get; set; }
    public string? UserStory { get; set; }
}
