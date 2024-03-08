using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Sponsor;

public class AdvertisementStatsRequest
{
    [JsonPropertyName("from")]
    public DateOnly From { get; set; }

    [JsonPropertyName("to")]
    public DateOnly To { get; set; } 
}
