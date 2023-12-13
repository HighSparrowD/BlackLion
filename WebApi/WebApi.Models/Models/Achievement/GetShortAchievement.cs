using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Achievement;

public class GetShortAchievement
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("isAcquired")]
    public bool IsAcquired { get; set; }
}
