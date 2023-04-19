using System.Text.Json.Serialization;

namespace MyWebApi.Entities.UserActionEntities
{
    public class SwitchBusyStatusResponse
    {
        [JsonPropertyName("status")]
        public SwitchBusyStatusResult Status { get; set; }
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
        [JsonPropertyName("hasVisited")]
        public bool? HasVisited { get; set; }
    }
}
