using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Admin
{
    public class RecentUpdates
    {
        [JsonPropertyName("recentFeedbackCount")]
        public int RecentFeedbackCount { get; set; }

        [JsonPropertyName("recentReportCount")]
        public int RecentReportCount { get; set; }

        [JsonPropertyName("verificationRequestCount")]
        public int VerificationRequestCount { get; set; }

        [JsonPropertyName("pendingAdvertisementCount")]
        public int PendingAdvertisementCount { get; set; }

        [JsonPropertyName("pendingAdventureCount")]
        public int PendingAdventureCount { get; set; }
    }
}
