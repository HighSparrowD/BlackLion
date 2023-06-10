using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace WebApi.Entities.UserInfoEntities
{
    public class Balance
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public int Points { get; set; }
        public int PersonalityPoints { get; set; }
        public int SecondChances { get; set; }
        public int Valentines { get; set; }
        public int Detectors { get; set; }
        public int Nullifiers { get; set; }
        public int CardDecksMini { get; set; }
        public int CardDecksPlatinum { get; set; }
        public int ThePersonalities { get; set; }
        public DateTime PointInTime { get; set; }

        public Balance()
        {}

        public Balance(long userId, int points, DateTime pointInTime)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Points = points;
            PersonalityPoints = 15;
            PointInTime = pointInTime;
            SecondChances = 0;
            Valentines = 0;
            Detectors = 0;
            Nullifiers = 0;
            CardDecksMini = 0;
            CardDecksPlatinum = 0;
            ThePersonalities = 0;
        }
    }
}
