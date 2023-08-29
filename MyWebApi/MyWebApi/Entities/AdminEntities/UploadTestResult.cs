
using System.Text.Json.Serialization;

namespace WebApi.Entities.AdminEntities
{
    public class UploadTestResult
    {
        [JsonPropertyName("score")]
        public int Score { get; set; }
        [JsonPropertyName("result")]
        public string Result { get; set; }
        [JsonPropertyName("tags")]
        public string Tags { get; set; }
    }
}
