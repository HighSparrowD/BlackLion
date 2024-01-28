#nullable enable
using System.Text.Json.Serialization;
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

    [JsonPropertyName("tagrgetAudience")]
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

    public static string TrancateDescription(string text, int leng)
    {
        string description = "";

        foreach (char c in text)
        {
            if (description.Length + 1 <= leng)
            {
                description += c;
            }
        }

        return description;
    }
}
