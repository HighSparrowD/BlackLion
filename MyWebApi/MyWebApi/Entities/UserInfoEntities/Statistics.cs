using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserInfoEntities
{
    public class Statistics
    {
        [Key]
        public long Id { get; set; }
        public int ProfileRegistrations { get; set; }
        public int TestsPassed { get; set; }
        public int DislikedProfiles { get; set; }
        public int DiscardedMatches { get; set; }
        public int LikesReceived { get; set; }
        public int Likes { get; set; }
        public int HighSimilarityMatches { get; set; }
        public int UseStreak { get; set; }
        public int IdeasGiven { get; set; }

        public Statistics()
        {}

        public Statistics(long userId)
        {
            Id = userId;
            ProfileRegistrations = 1;
        }
    }
}
