using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Adventure;

public class GetTemplateShort
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
