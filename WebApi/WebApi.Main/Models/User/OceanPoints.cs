using System.ComponentModel.DataAnnotations;
using enums = WebApi.Enums.Enums.User;
using models = WebApi.Models.Models.User;

#nullable enable
namespace WebApi.Main.Entities.User;

public class OceanPoints
{
    [Key]
    public long UserId { get; set; }
    public float Openness { get; set; }
    public float OpennessPercentage { get; set; }
    public float Conscientiousness { get; set; }
    public float ConscientiousnessPercentage { get; set; }
    public float Extroversion { get; set; }
    public float ExtroversionPercentage { get; set; }
    public float Agreeableness { get; set; }
    public float AgreeablenessPercentage { get; set; }
    public float Neuroticism { get; set; }
    public float NeuroticismPercentage { get; set; }
    public float Nature { get; set; }
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

    public async Task<List<enums.OceanStats>> GetImportantParams()
    {
        var importantParams = new List<enums.OceanStats>();
        var theBiggest = 0f;

        await Task.Run(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                if (Openness > theBiggest && !importantParams.Contains(enums.OceanStats.Openness))
                {
                    importantParams.Add(enums.OceanStats.Openness);
                    theBiggest = Openness;
                }
                else if (Conscientiousness > theBiggest && !importantParams.Contains(enums.OceanStats.Conscientiousness))
                {
                    importantParams.Add(enums.OceanStats.Conscientiousness);
                    theBiggest = Conscientiousness;
                }
                else if (Extroversion > theBiggest && !importantParams.Contains(enums.OceanStats.Extroversion))
                {
                    importantParams.Add(enums.OceanStats.Extroversion);
                    theBiggest = Extroversion;
                }
                else if (Agreeableness > theBiggest && !importantParams.Contains(enums.OceanStats.Agreeableness))
                {
                    importantParams.Add(enums.OceanStats.Agreeableness);
                    theBiggest = Agreeableness;
                }
                else if (Neuroticism > theBiggest && !importantParams.Contains(enums.OceanStats.Neuroticism))
                {
                    importantParams.Add(enums.OceanStats.Neuroticism);
                    theBiggest = Neuroticism;
                }
                else if (Nature > theBiggest && !importantParams.Contains(enums.OceanStats.Nature))
                {
                    importantParams.Add(enums.OceanStats.Nature);
                    theBiggest = Nature;
                }

                theBiggest = 0;
            }
        });

        return importantParams;
    }

    public static explicit operator OceanPoints? (models.OceanPoints? op)
    {
        if (op == null)
            return null;

        return new OceanPoints
        {
            UserId = op.UserId,
            Agreeableness = op.Agreeableness,
            AgreeablenessPercentage = op.AgreeablenessPercentage,
            Conscientiousness = op.Conscientiousness,
            ConscientiousnessPercentage = op.ConscientiousnessPercentage,
            Extroversion = op.Extroversion,
            ExtroversionPercentage = op.ExtroversionPercentage,
            Nature = op.Nature,
            NaturePercentage = op.NaturePercentage,
            Neuroticism = op.Neuroticism,
            NeuroticismPercentage = op.NeuroticismPercentage,
            Openness = op.Openness,
            OpennessPercentage = op.OpennessPercentage
        };
    }

    public static implicit operator models.OceanPoints? (OceanPoints? op)
    {
        if (op == null)
            return null;

        return new models.OceanPoints
        {
            UserId = op.UserId,
            Agreeableness = op.Agreeableness,
            AgreeablenessPercentage = op.AgreeablenessPercentage,
            Conscientiousness = op.Conscientiousness,
            ConscientiousnessPercentage = op.ConscientiousnessPercentage,
            Extroversion = op.Extroversion,
            ExtroversionPercentage = op.ExtroversionPercentage,
            Nature = op.Nature,
            NaturePercentage = op.NaturePercentage,
            Neuroticism = op.Neuroticism,
            NeuroticismPercentage = op.NeuroticismPercentage,
            Openness = op.Openness,
            OpennessPercentage = op.OpennessPercentage
        };
    }
}
