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
        public int LevelOfSense { get; set; }
        public double LevelOfSensePercentage { get; set; }
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
            Personality = 1;
            PersonalityPercentage = 1;
            EmotionalIntellect = 1;
            EmotionalIntellectPercentage = 1;
            Reliability = 1;
            ReliabilityPercentage = 1;
            Compassion = 1;
            CompassionPercentage = 1;
            OpenMindedness = 1;
            OpenMindednessPercentage = 1;
            Agreeableness = 1;
            AgreeablenessPercentage = 1;
            SelfAwareness = 1;
            SelfAwarenessPercentage = 1;
            LevelOfSense = 1;
            LevelOfSensePercentage = 1;
            Intellect = 1;
            IntellectPercentage = 1;
            Nature = 1;
            NaturePercentage = 1;
            Creativity = 1;
            CreativityPercentage = 1;
        }
    }
}
