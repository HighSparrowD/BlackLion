using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.Achievement;

public class UserAchievement
{
    [Key]
    public int AchievementId { get; set; }
    [Key]
    [ForeignKey("User")]
    public long UserId { get; set; }
    public int Progress { get; set; }
    public bool IsAcquired { get; set; }
    public AppLanguage AchievementLanguage { get; set; }

    [ForeignKey("AchievementId, AchievementLanguage")]
    public virtual Achievement Achievement { get; set; }
    public virtual User.User User { get; set; }

    public UserAchievement()
    {
    }

    public UserAchievement(int achievementId, long userId, AppLanguage language)
    {
        AchievementId = achievementId;
        UserId = userId;
        AchievementLanguage = language;
        Progress = 0;
        IsAcquired = false;
    }
}
