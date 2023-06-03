using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    public class GetReportReason
    {
        [JsonPropertyName("id")]
        public short Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
