using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebApi.Main.Models.User;

public class OceanPoints
{
    [Key]
    public long UserId { get; set; }
    public int Openness { get; set; }
    public float OpennessPercentage { get; set; }
    public int Conscientiousness { get; set; }
    public float ConscientiousnessPercentage { get; set; }
    public int Extroversion { get; set; }
    public float ExtroversionPercentage { get; set; }
    public int Agreeableness { get; set; }
    public float AgreeablenessPercentage { get; set; }
    public int Neuroticism { get; set; }
    public float NeuroticismPercentage { get; set; }
    public int Nature { get; set; }
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

    public async Task<List<Enums.User.OceanStats>> GetImportantParams()
    {
        var importantParams = new List<Enums.User.OceanStats>();
        var theBiggest = 0;

        await Task.Run(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                if (Openness > theBiggest && !importantParams.Contains(Enums.User.OceanStats.Openness))
                {
                    importantParams.Add(Enums.User.OceanStats.Openness);
                    theBiggest = Openness;
                }
                else if (Conscientiousness > theBiggest && !importantParams.Contains(Enums.User.OceanStats.Conscientiousness))
                {
                    importantParams.Add(Enums.User.OceanStats.Conscientiousness);
                    theBiggest = Conscientiousness;
                }
                else if (Extroversion > theBiggest && !importantParams.Contains(Enums.User.OceanStats.Extroversion))
                {
                    importantParams.Add(Enums.User.OceanStats.Extroversion);
                    theBiggest = Extroversion;
                }
                else if (Agreeableness > theBiggest && !importantParams.Contains(Enums.User.OceanStats.Agreeableness))
                {
                    importantParams.Add(Enums.User.OceanStats.Agreeableness);
                    theBiggest = Agreeableness;
                }
                else if (Neuroticism > theBiggest && !importantParams.Contains(Enums.User.OceanStats.Neuroticism))
                {
                    importantParams.Add(Enums.User.OceanStats.Neuroticism);
                    theBiggest = Neuroticism;
                }
                else if (Nature > theBiggest && !importantParams.Contains(Enums.User.OceanStats.Nature))
                {
                    importantParams.Add(Enums.User.OceanStats.Nature);
                    theBiggest = Nature;
                }

                theBiggest = 0;
            }
        });

        return importantParams;
    }
}
