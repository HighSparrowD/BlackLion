using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;
using models = WebApi.Models.Models.User;
#nullable enable

namespace WebApi.Main.Models.User;

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

    public UserData()
    {}

    public static explicit operator UserData? (models.UserData? data)
    {
        if (data == null)
            return null;

        return new UserData
        {
            Id = data.Id,
            UserStory = data.UserStory,
            AgePrefs = data.AgePrefs,
            AutoReplyText = data.AutoReplyText,
            AutoReplyVoice = data.AutoReplyVoice,
            CommunicationPrefs = data.CommunicationPrefs,
            Language = data.Language,
            LanguagePreferences = data.LanguagePreferences,
            LocationPreferences = data.LocationPreferences,
            MediaType = data.MediaType,
            Reason = data.Reason,
            UserAge = data.UserAge,
            UserDescription = data.UserDescription,
            UserGender = data.UserGender,
            UserGenderPrefs = data.UserGenderPrefs,
            UserLanguages = data.UserLanguages,
            UserMedia = data.UserMedia,
            UserName = data.UserName,
            UserRawDescription = data.UserRawDescription,
            UserRealName = data.UserRealName
        };
    }

    public static implicit operator models.UserData?(UserData? data)
    {
        if (data == null)
            return null;

        return new models.UserData
        {
            Id = data.Id,
            UserStory = data.UserStory,
            AgePrefs = data.AgePrefs,
            AutoReplyText = data.AutoReplyText,
            AutoReplyVoice = data.AutoReplyVoice,
            CommunicationPrefs = data.CommunicationPrefs,
            Language = data.Language,
            LanguagePreferences = data.LanguagePreferences,
            LocationPreferences = data.LocationPreferences,
            MediaType = data.MediaType,
            Reason = data.Reason,
            UserAge = data.UserAge,
            UserDescription = data.UserDescription,
            UserGender = data.UserGender,
            UserGenderPrefs = data.UserGenderPrefs,
            UserLanguages = data.UserLanguages,
            UserMedia = data.UserMedia,
            UserName = data.UserName,
            UserRawDescription = data.UserRawDescription,
            UserRealName = data.UserRealName
        };
    }
}
