using System.Text.Json.Serialization;


#nullable enable
namespace WebApi.Models.Models.User
{
    public class SwitchBusyStatusResponse
    {
        [JsonPropertyName("status")]
        public SwitchBusyStatusResult Status { get; set; }
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
        [JsonPropertyName("hasVisited")]
        public bool? HasVisited { get; set; }
        [JsonPropertyName("localization")]
        public Dictionary<string, string>? Localization { get; set; }
    }
}
