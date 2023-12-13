using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Adventure;

namespace WebApi.Models.Models.Adventure;

public class GetAdventure
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("status")]
    public AdventureStatus Status { get; set; }
}
