using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Models.User;

public class OceanStats
{
    [Key]
    public long UserId { get; set; }
    public float Openness { get; set; }
    public float Conscientiousness { get; set; }
    public float Extroversion { get; set; }
    public float Agreeableness { get; set; }
    public float Neuroticism { get; set; }
    public float Nature { get; set; }

    public OceanStats()
    { }

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
