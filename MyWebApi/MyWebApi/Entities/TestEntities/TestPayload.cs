using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.TestEntities
{
    public class TestPayload
    {
        public long UserId { get; set; }
        public long TestId { get; set; }
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
        public List<string> Tags{ get; set; }
    }
}
