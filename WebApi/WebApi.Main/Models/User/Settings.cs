using System.ComponentModel.DataAnnotations;
using models = WebApi.Models.Models.User;

#nullable enable
namespace WebApi.Main.Models.User;

public class Settings
{
    [Key]
    public long Id { get; set; }
    public bool UsesOcean { get; set; }
    public bool ShouldFilterUsersWithoutRealPhoto { get; set; }
    public bool ShouldConsiderLanguages { get; set; }
    public bool ShouldComment { get; set; }
    public bool ShouldSendHints { get; set; }
    public bool IncreasedFamiliarity { get; set; }
    public bool? IsFree { get; set; }

    public Settings()
    { }

    public Settings(long id, bool shouldUserPersonality = false)
    {
        Id = id;
        UsesOcean = shouldUserPersonality;
        ShouldFilterUsersWithoutRealPhoto = false;
        ShouldConsiderLanguages = false;
        ShouldComment = false;
        ShouldSendHints = true;
        IncreasedFamiliarity = true;
        IsFree = false;
    }

    public static explicit operator Settings? (models.Settings? settings)
    {
        if (settings == null)
            return null;

        return new Settings
        {
            Id = settings.Id,
            ShouldComment = settings.ShouldComment,
            ShouldConsiderLanguages = settings.ShouldConsiderLanguages,
            ShouldFilterUsersWithoutRealPhoto = settings.ShouldFilterUsersWithoutRealPhoto,
            ShouldSendHints = settings.ShouldSendHints,
            IncreasedFamiliarity = settings.IncreasedFamiliarity,
            IsFree = settings.IsFree,
            UsesOcean = settings.UsesOcean
        };
    }
}
