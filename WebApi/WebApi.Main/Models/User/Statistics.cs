using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Models.User;

public class Statistics
{
    [Key]
    public long UserId { get; set; }
    public int ProfileRegistrations { get; set; }
    public int TestsPassed { get; set; }
    public int DislikedProfiles { get; set; }
    public int DiscardedMatches { get; set; }
    public int LikesReceived { get; set; }
    public int Likes { get; set; }
    public int Matches { get; set; }
    public int HighSimilarityEncounters { get; set; }
    public int UseStreak { get; set; }
    public int IdeasGiven { get; set; }
    public int QuestionerPasses { get; set; }

    public Statistics()
    { }

    public Statistics(long userId)
    {
        UserId = userId;
        ProfileRegistrations = 1;
    }
}
