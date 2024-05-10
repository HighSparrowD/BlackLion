#nullable enable
using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Advertisement;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Sponsor;

namespace WebApi.Models.Models.Sponsor;

public class Advertisement
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("sponsorId")]
    public long UserId { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("targetAudience")]
    public string? TargetAudience { get; set; }

    [JsonPropertyName("media")]
    public string? Media { get; set; }

    [JsonPropertyName("show")]
    public bool Show { get; set; }

    [JsonPropertyName("updated")]
    public bool Updated { get; set; }

    [JsonPropertyName("priority")]
    public AdvertisementPriority Priority { get; set; }

    [JsonPropertyName("mediaType")]
    public MediaType MediaType { get; set; }

    [JsonPropertyName("status")]
	public AdvertisementStatus Status { get; set; }

	[JsonPropertyName("adminId")]
    public long? AdminId { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
}
