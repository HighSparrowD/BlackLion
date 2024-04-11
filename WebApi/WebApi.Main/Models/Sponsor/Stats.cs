using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Entities.Sponsor;

public class Stats
{
    [Key]
    public long Id { get; set; }
    public long SponsorId { get; set; }
    public double? AverageRating { get; set; }
    public int ConductedEventsCount { get; set; }
    public int Level { get; set; }
    public double LevelProgress { get; set; }
    public double LevelGoal { get; set; }

    public static Stats CreateDefaultStats(long sponsorId)
    {
        return new Stats { Id = sponsorId, SponsorId = sponsorId, AverageRating = null, ConductedEventsCount = 0, Level = 1, LevelGoal = 800, LevelProgress = 0 };
    }
}
