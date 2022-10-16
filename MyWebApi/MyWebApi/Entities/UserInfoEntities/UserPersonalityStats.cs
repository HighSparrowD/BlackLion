using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserPersonalityStats
    {
        [Key]
        public long UserId { get; set; }
        public int Personality { get; set; }
        public int EmotionalIntellect { get; set; }
        public int Reliability { get; set; }
        public int Compassion { get; set; }
        public int OpenMindedness { get; set; }
        public int Agreeableness { get; set; }
        public int SelfAwareness { get; set; }
        public int LevelOfSense { get; set; }
        public int Intellect { get; set; }
        public int Nature { get; set; }
        public int Creativity { get; set; }

        public UserPersonalityStats()
        {}

        public UserPersonalityStats(long userId)
        {
            UserId = userId;
            Personality = 0;
            EmotionalIntellect = 0;
            Reliability = 0;
            Compassion = 0;
            OpenMindedness = 0;
            Agreeableness = 0;
            SelfAwareness = 0;
            LevelOfSense = 0;
            Intellect = 0;
            Nature = 0;
            Creativity = 0;
        }
    }
}
