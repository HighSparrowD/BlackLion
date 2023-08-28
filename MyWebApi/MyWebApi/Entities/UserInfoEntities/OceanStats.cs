using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserInfoEntities
{
    public class OceanStats
    {
        [Key]
        public long UserId { get; set; }
        public int Openness { get; set; }
        public int Conscientiousness { get; set; }
        public int Extroversion { get; set; }
        public int Agreeableness { get; set; }
        public int Neuroticism { get; set; }
        public int Nature { get; set; }

        public OceanStats()
        {}

        public OceanStats(long userId)
        {
            UserId = userId;
            Openness = 0;
            Conscientiousness = 0;
            Extroversion = 0;
            Agreeableness = 0;
            Neuroticism = 0;
            Nature = 0;
        }
    }
}
