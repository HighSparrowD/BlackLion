using System.ComponentModel.DataAnnotations;
using models = WebApi.Models.Models.User;

#nullable enable
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

    public static explicit operator OceanStats? (models.OceanStats os)
    {
        if (os == null)
            return null;

        return new OceanStats
        {
            UserId = os.UserId,
            Agreeableness = os.Agreeableness,
            Conscientiousness = os.Conscientiousness,
            Extroversion = os.Extroversion,
            Nature = os.Nature,
            Neuroticism = os.Neuroticism,
            Openness = os.Openness
        };
    }

    public static implicit operator models.OceanStats?(OceanStats os)
    {
        if (os == null)
            return null;

        return new models.OceanStats
        {
            UserId = os.UserId,
            Agreeableness = os.Agreeableness,
            Conscientiousness = os.Conscientiousness,
            Extroversion = os.Extroversion,
            Nature = os.Nature,
            Neuroticism = os.Neuroticism,
            Openness = os.Openness
        };
    }
}
