using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserPersonalityPoints
    {
        [Key]
        public long UserId { get; set; }
        public int Personality { get; set; }
        public double PersonalityPercentage { get; set; }
        public int EmotionalIntellect { get; set; }
        public double EmotionalIntellectPercentage { get; set; }
        public int Reliability { get; set; }
        public double ReliabilityPercentage { get; set; }
        public int Compassion { get; set; }
        public double CompassionPercentage { get; set; }
        public int OpenMindedness { get; set; }
        public double OpenMindednessPercentage { get; set; }
        public int Agreeableness { get; set; }
        public double AgreeablenessPercentage { get; set; }
        public int SelfAwareness { get; set; }
        public double SelfAwarenessPercentage { get; set; }
        public int LevelsOfSense { get; set; }
        public double LevelsOfSensePercentage { get; set; }
        public int Intellect { get; set; }
        public double IntellectPercentage { get; set; }
        public int Nature { get; set; }
        public double NaturePercentage { get; set; }
        public int Creativity { get; set; }
        public double CreativityPercentage { get; set; }

        public UserPersonalityPoints()
        {}

        public UserPersonalityPoints(long userId)
        {
            UserId = userId;
            Personality = 0;
            PersonalityPercentage = 0;
            EmotionalIntellect = 0;
            EmotionalIntellectPercentage = 0;
            Reliability = 0;
            ReliabilityPercentage = 0;
            Compassion = 0;
            CompassionPercentage = 0;
            OpenMindedness = 0;
            OpenMindednessPercentage = 0;
            Agreeableness = 0;
            AgreeablenessPercentage = 0;
            SelfAwareness = 0;
            SelfAwarenessPercentage = 0;
            LevelsOfSense = 0;
            LevelsOfSensePercentage = 0;
            Intellect = 0;
            IntellectPercentage = 0;
            Nature = 0;
            NaturePercentage = 0;
            Creativity = 0;
            CreativityPercentage = 0;
        }
    }
}
