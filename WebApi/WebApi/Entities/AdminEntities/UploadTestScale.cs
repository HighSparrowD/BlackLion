using System.Text.Json.Serialization;

namespace WebApi.Entities.AdminEntities
{
    public class UploadTestScale
    {
        [JsonPropertyName("scale")]
        public string Scale { get; set; }
        [JsonPropertyName("minValue")]
        public int? MinValue { get; set; }
    }
}
