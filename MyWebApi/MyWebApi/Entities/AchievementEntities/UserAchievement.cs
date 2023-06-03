using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Entities.LocalisationEntities;
using WebApi.Entities.UserInfoEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        public int AchievementClassLocalisationId{ get; set; }
        //[ForeignKey("AchievementId")]
        public virtual Achievement Achievement { get; set; }
        //[ForeignKey("UserBaseInfoId")]
        public virtual User User { get; set; }

        public UserAchievement()
        {
        }

        public UserAchievement(long achievementId, long userBaseInfoId, int achievementClassLocalisationId, string sysAchievementName, string sysAchievementDescription, int sysAchievementValue, int localisation)
        {
            AchievementId = achievementId;
            UserBaseInfoId = userBaseInfoId;
            AchievementClassLocalisationId = achievementClassLocalisationId;
            Progress = 0;
            IsAcquired = false;
            AcquireMessage = GenerateAcquireMessage(sysAchievementName, sysAchievementDescription, sysAchievementValue, localisation);
            ShortDescription = sysAchievementName; //GenerateShortDescription(sysAchievementDescription); //TODO: remember what purpose short description was entended to serve
        }
        public UserAchievement(long achievementId, long userBaseInfoId, int progress, string acquireMessage, string shortDescription, bool isAcquired, int achievementClassLocalisationId)
        {
            AchievementId = achievementId;
            UserBaseInfoId = userBaseInfoId;
            Progress = progress;
            AcquireMessage = acquireMessage;
            ShortDescription = shortDescription;
            IsAcquired = isAcquired;
            AchievementClassLocalisationId = achievementClassLocalisationId;
        }

        public void RetranslateAquireMessage(Achievement achievement, int localisation)
        {
            var locs = new Dictionary<int, string>();
            locs.Add(0, "✨✨Congrats! You have unlocked a new achievement!✨✨");
            locs.Add(1, "✨✨Поздравляем! Вы разблокировали новое достижение!✨✨");
            locs.Add(2, "✨✨Вітаємо! Ви розблокували нове досягнення!✨✨"); //TODO: relocate to localisation table

            AcquireMessage = $"{locs[localisation]}\n{achievement.Name}\n\n{achievement.Description}\n\n{achievement.Value}";
        }

        public string GenerateAcquireMessage(string name, string description, int value, int localisation)
        {
            var locs = new Dictionary<int, string>();
            locs.Add(0, "✨✨Congrats! You have unlocked a new achievement!✨✨");
            locs.Add(1, "✨✨Поздравляем! Вы разблокировали новое достижение!✨✨");
            locs.Add(2, "✨✨Вітаємо! Ви розблокували нове досягнення!✨✨"); //TODO: relocate to localisation table

            return $"{locs[localisation]}\n{name}\n\n{description}\n\n{value}";
        }

        private string GenerateShortDescription(string text, int length)
        {
            string description = "";

            foreach (char c in text)
            {
                if (description.Length + 1 <= length)
                {
                    description += c;
                }
            }

            return description;
        }
    }
}
