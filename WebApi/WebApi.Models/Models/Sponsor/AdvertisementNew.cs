using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Sponsor;

#nullable enable
namespace WebApi.Models.Models.Sponsor;

public class AdvertisementNew
{
    [JsonPropertyName("sponsorId")]
    public long SponsorId { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("targetAudience")]
    public string? TargetAudience { get; set; }

    [JsonPropertyName("media")]
    public string? Media { get; set; }

    [JsonPropertyName("priority")]
    public AdvertisementPriority Priority { get; set; }

    [JsonPropertyName("mediaType")]
    public MediaType MediaType { get; set; }
}
