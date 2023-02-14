using static MyWebApi.Enums.SystemEnums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MyWebApi.Enums;

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
            Personality = 0;
            PersonalityPercentage = 1;
            EmotionalIntellect = 0;
            EmotionalIntellectPercentage = 1;
            Reliability = 0;
            ReliabilityPercentage = 1;
            Compassion = 0;
            CompassionPercentage = 1;
            OpenMindedness = 0;
            OpenMindednessPercentage = 1;
            Agreeableness = 0;
            AgreeablenessPercentage = 1;
            SelfAwareness = 0;
            SelfAwarenessPercentage = 1;
            LevelOfSense = 0;
            LevelOfSensePercentage = 1;
            Intellect = 0;
            IntellectPercentage = 1;
            Nature = 0;
            NaturePercentage = 1;
            Creativity = 0;
            CreativityPercentage = 1;
        }

        public async Task<List<PersonalityStats>> GetImportantParams()
        {
            var importantParams = new List<PersonalityStats>();
            var theBiggest = 0;

            await Task.Run(() =>
            {
                for (int i = 0; i < 3; i++)
                { 
                    if(Personality > theBiggest && !importantParams.Contains(PersonalityStats.PersonalityType))
                    {
                        importantParams.Add(PersonalityStats.PersonalityType);
                        theBiggest = Personality;
                    }
                    else if (EmotionalIntellect > theBiggest && !importantParams.Contains(PersonalityStats.EmotionalIntellect))
                    {
                        importantParams.Add(PersonalityStats.EmotionalIntellect);
                        theBiggest = EmotionalIntellect;
                    }
                    else if (Reliability > theBiggest && !importantParams.Contains(PersonalityStats.Reliability))
                    {
                        importantParams.Add(PersonalityStats.Reliability);
                        theBiggest = Reliability;
                    }
                    else if (Compassion > theBiggest && !importantParams.Contains(PersonalityStats.Compassion))
                    {
                        importantParams.Add(PersonalityStats.Compassion);
                        theBiggest = Compassion;
                    }
                    else if (OpenMindedness > theBiggest && !importantParams.Contains(PersonalityStats.OpenMindedness))
                    {
                        importantParams.Add(PersonalityStats.OpenMindedness);
                        theBiggest = OpenMindedness;
                    }
                    else if (Agreeableness > theBiggest && !importantParams.Contains(PersonalityStats.Agreeableness))
                    {
                        importantParams.Add(PersonalityStats.Agreeableness);
                        theBiggest = Agreeableness;
                    }
                    else if (SelfAwareness > theBiggest && !importantParams.Contains(PersonalityStats.SelfAwareness))
                    {
                        importantParams.Add(PersonalityStats.SelfAwareness);
                        theBiggest = SelfAwareness;
                    }
                    else if (LevelOfSense > theBiggest && !importantParams.Contains(PersonalityStats.LevelsOfSense))
                    {
                        importantParams.Add(PersonalityStats.LevelsOfSense);
                        theBiggest = LevelOfSense;
                    }
                    else if (Intellect > theBiggest && !importantParams.Contains(PersonalityStats.Intellect))
                    {
                        importantParams.Add(PersonalityStats.Intellect);
                        theBiggest = Intellect;
                    }
                    else if (Nature > theBiggest && !importantParams.Contains(PersonalityStats.Nature))
                    {
                        importantParams.Add(PersonalityStats.Nature);
                        theBiggest = Nature;
                    }
                    else if (Creativity > theBiggest && !importantParams.Contains(PersonalityStats.Creativity))
                    {
                        importantParams.Add(PersonalityStats.Creativity);
                        theBiggest = Creativity;
                    }

                    theBiggest = 0;
                }
            });

            return importantParams;
        }
    }
}
