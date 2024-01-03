using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.Models.User;

public class OceanStats : IEnumerable<float>
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

    public IEnumerator<float> GetEnumerator()
    {
        yield return Openness;
        yield return Conscientiousness;
        yield return Extroversion;
        yield return Agreeableness;
        yield return Neuroticism;
        yield return Nature;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
