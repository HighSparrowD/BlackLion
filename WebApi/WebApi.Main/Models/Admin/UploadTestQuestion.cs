using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApi.Entities.TestEntities
{
    public class UploadTestQuestion
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("photo")]
        public string Photo { get; set; }
        [JsonPropertyName("scale")]
        public string Scale { get; set; }
        [JsonPropertyName("answers")]
        public List<UploadTestAnswer> Answers { get; set; }
    }
}
