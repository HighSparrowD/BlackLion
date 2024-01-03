using enums = WebApi.Enums.Enums.User;

namespace WebApi.Models.Models.User;

public class OceanPoints : OceanStats
{
    public float OpennessPercentage { get; set; }
    public float ConscientiousnessPercentage { get; set; }
    public float ExtroversionPercentage { get; set; }
    public float AgreeablenessPercentage { get; set; }
    public float NeuroticismPercentage { get; set; }
    public float NaturePercentage { get; set; }

    public OceanPoints()
    { }

    public OceanPoints(long userId)
    {
        UserId = userId;
        Openness = 0;
        OpennessPercentage = 1;
        Conscientiousness = 0;
        ConscientiousnessPercentage = 1;
        Extroversion = 0;
        ExtroversionPercentage = 1;
        Agreeableness = 0;
        AgreeablenessPercentage = 1;
        Neuroticism = 0;
        NeuroticismPercentage = 1;
        Nature = 0;
        NaturePercentage = 1;
    }
}
