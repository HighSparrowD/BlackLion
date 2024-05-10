using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Adventure;

namespace WebApi.Main.Entities.Admin;

#nullable enable
public class ResolveAdventure
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("adminId")]
    public long AdminId { get; set; }

    [JsonPropertyName("status")]
    public AdventureStatus Status { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    [JsonPropertyName("tags")]
    public string? Tags { get; set; }
}
