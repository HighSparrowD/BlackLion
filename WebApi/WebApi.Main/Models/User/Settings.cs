using System.ComponentModel.DataAnnotations;

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
}
