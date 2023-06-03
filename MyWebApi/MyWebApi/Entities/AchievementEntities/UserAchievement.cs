using WebApi.Entities.UserInfoEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.AchievementEntities
{
    public class UserAchievement
    {
        [Key]
        public long AchievementId { get; set; }
        [Key]
        public long UserBaseInfoId { get; set; }
        public int Progress { get; set; }
        public string AcquireMessage { get; set; }
        public string ShortDescription { get; set; }
        public bool IsAcquired{ get; set; }
        public AppLanguage Language{ get; set; }
        //[ForeignKey("AchievementId")]
        public virtual Achievement Achievement { get; set; }
        //[ForeignKey("UserBaseInfoId")]
        public virtual User User { get; set; }

        public UserAchievement()
        {
        }

        public UserAchievement(long achievementId, long userBaseInfoId, AppLanguage language, string sysAchievementName, string sysAchievementDescription, int sysAchievementValue, AppLanguage localisation)
        {
            AchievementId = achievementId;
            UserBaseInfoId = userBaseInfoId;
            Language = language;
            Progress = 0;
            IsAcquired = false;
            AcquireMessage = GenerateAcquireMessage(sysAchievementName, sysAchievementDescription, sysAchievementValue, localisation);
            ShortDescription = sysAchievementName; //GenerateShortDescription(sysAchievementDescription); //TODO: remember what purpose short description was entended to serve
        }
        public UserAchievement(long achievementId, long userBaseInfoId, int progress, string acquireMessage, string shortDescription, bool isAcquired, AppLanguage language)
        {
            AchievementId = achievementId;
            UserBaseInfoId = userBaseInfoId;
            Progress = progress;
            AcquireMessage = acquireMessage;
            ShortDescription = shortDescription;
            IsAcquired = isAcquired;
            Language = language;
        }

        public void RetranslateAquireMessage(Achievement achievement, AppLanguage localisation)
        {
            var locs = new Dictionary<int, string>();
            locs.Add(0, "✨✨Congrats! You have unlocked a new achievement!✨✨");
            locs.Add(1, "✨✨Поздравляем! Вы разблокировали новое достижение!✨✨");
            locs.Add(2, "✨✨Вітаємо! Ви розблокували нове досягнення!✨✨"); //TODO: relocate to localisation table

            AcquireMessage = $"{locs[(byte)localisation]}\n{achievement.Name}\n\n{achievement.Description}\n\n{achievement.Value}";
        }

        public string GenerateAcquireMessage(string name, string description, int value, AppLanguage localisation)
        {
            var locs = new Dictionary<int, string>();
            locs.Add(0, "✨✨Congrats! You have unlocked a new achievement!✨✨");
            locs.Add(1, "✨✨Поздравляем! Вы разблокировали новое достижение!✨✨");
            locs.Add(2, "✨✨Вітаємо! Ви розблокували нове досягнення!✨✨"); //TODO: relocate to localisation table

            return $"{locs[(byte)localisation]}\n{name}\n\n{description}\n\n{value}";
        }
    }
}
