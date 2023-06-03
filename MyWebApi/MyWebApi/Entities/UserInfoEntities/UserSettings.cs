using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserInfoEntities
{
    public class UserSettings
    {
        [Key]
        public long Id { get; set; }
        public bool ShouldUsePersonalityFunc { get; set; }
        public bool ShouldFilterUsersWithoutRealPhoto { get; set; }
        public bool ShouldConsiderLanguages { get; set; }
        public bool ShouldComment { get; set; }
        public bool ShouldSendHints { get; set; }
        public bool IncreasedFamiliarity { get; set; }
        public bool? IsFree { get; set; }

        public UserSettings()
        {}

        public UserSettings(long id, bool shouldUserPersonality = false)
        {
            Id = id;
            ShouldUsePersonalityFunc = shouldUserPersonality;
            ShouldFilterUsersWithoutRealPhoto = false;
            ShouldConsiderLanguages = false;
            ShouldComment = false;
            ShouldSendHints = true;
            IncreasedFamiliarity = true;
            IsFree = false;
        }
    }
}
