using System.Text.Json.Serialization;

namespace WebApi.Entities.TestEntities
{
    public class UploadTestAnswer
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("value")]
        public double Value { get; set; }
        [JsonPropertyName("tags")]
        public string Tags { get; set; } = string.Empty;
    }
}
