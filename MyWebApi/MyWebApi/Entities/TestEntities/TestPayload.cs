﻿using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class TestPayload
    {
        [Key]
        public long UserId { get; set; }
        [Key]
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
    }
}