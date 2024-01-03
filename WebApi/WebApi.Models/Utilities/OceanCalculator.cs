using enums = WebApi.Enums.Enums.User;
using WebApi.Models.Models.User;

namespace WebApi.Models.Utilities;

public static class OceanCalculator
{
    public static async Task<CalculationResult> Calculate(OceanPoints userPoints, OceanStats userStats, OceanPoints user2Points, OceanStats user2Stats, 
        float valentineBonus, float deviation, float minDeviation, IEnumerable<enums.OceanStats> important)
    {
        var currentValueMax = 0f;
        var currentValueMin = 0f;
        var result = new CalculationResult();

        if (userPoints == null || userStats == null || user2Points == null || user2Stats == null)
            return result;

        //TODO: create its own deviation variable for Nature parameter depending on the number of natures
        //It is likely to be grater than the normal deviation
        foreach (var currentPoint in userPoints) 
        {
            var currentStat = userStats.Where(s => nameof(s) == nameof(currentPoint)).FirstOrDefault();
            var current2Point = user2Points.Where(s => nameof(s) == nameof(currentPoint)).FirstOrDefault();
            var current2Stat = user2Stats.Where(s => nameof(s) == nameof(currentPoint)).FirstOrDefault();

            var currentParamPercentage = userPoints.Where(s => nameof(s) == $"{nameof(currentPoint)}Percentage").FirstOrDefault();
            var currentParam2Percentage = user2Points.Where(s => nameof(s) == $"{nameof(currentPoint)}Percentage").FirstOrDefault();

            //Turns off the parameter if: 1. User has no relative tests passed; 2. No points where invested in it
            if (currentPoint > 0 && currentStat > 0 && current2Point > 0 && current2Stat > 0)
            {
                var paramSim = await CalculateSimilarityAsync(currentStat * valentineBonus, current2Stat);

                currentValueMax = ApplyMaxDeviation(currentParamPercentage, deviation);
                currentValueMin = ApplyMinDeviation(currentParamPercentage, minDeviation);

                //Negative conditions are applied, cuz this is an exclussive condition
                if (paramSim <= currentValueMax && paramSim >= currentValueMin)
                {
                    currentValueMax = ApplyMaxDeviation(currentParam2Percentage, deviation);
                    currentValueMin = ApplyMinDeviation(currentParam2Percentage, minDeviation);

                    if (paramSim <= currentValueMax && paramSim >= currentValueMin)
                    {
                        result.Bonus += $"{nameof(currentPoint)[0].ToString().ToUpper()} ";

                        if (important.Contains(Enum.Parse<enums.OceanStats>(nameof(currentPoint))))
                            result.ImportantMatches++;
                        else
                            result.SecondaryMatches++;
                    }
                }
            }
        }

        return result;
    }

    private static float ApplyMaxDeviation(float value, float deviation)
    {
        var currentValueMax = value + deviation;

        if (currentValueMax > 1)
            currentValueMax = 1;

        return currentValueMax;
    }

    private static float ApplyMinDeviation(float value, float deviation)
    {
        var currentValueMin = value - deviation;

        if (currentValueMin < 0)
            currentValueMin = 0;

        return currentValueMin;
    }

    private static async Task<double> CalculateSimilarityAsync(double param1, double param2)
    {
        double difference = 0;
        double x = 0;
        double c = 0;

        //Remove negative values. We are all possitive here ;)
        if (param1 < 0)
            param1 *= -1;

        if (param2 < 0)
            param2 *= -1;

        await Task.Run(() =>
        {
            if (param1 > param2)
            {
                difference = param1 - param2;
                x = (difference * 100) / param1;
                c = 0 + (x / 100);
            }
            else if (param2 > param1)
            {
                difference = param2 - param1;
                x = (difference * 100) / param2;
                c = 0 + (x / 100);
            }
            else if (param1 == param2)
                c = 0.00000001; //Similarity percentage will never be equal to 100% !
        });

        return c;
    }
}

public class CalculationResult()
{
    public string Bonus { get; set; } = string.Empty;
    public int ImportantMatches { get; set; } = 0;
    public int SecondaryMatches { get; set; } = 0;
}