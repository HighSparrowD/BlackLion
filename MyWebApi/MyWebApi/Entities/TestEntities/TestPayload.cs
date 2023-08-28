using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApi.Entities.TestEntities
{
    public class TestPayload
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("testId")]
        public long TestId { get; set; }
        [JsonPropertyName("balance")]
        public int Balance { get; set; }
        [JsonPropertyName("openness")]
        public int? Openness { get; set; }
        [JsonPropertyName("conscientiousness")]
        public int? Conscientiousness { get; set; }
        [JsonPropertyName("extroversion")]
        public int? Extroversion { get; set; }
        [JsonPropertyName("agreeableness")]
        public int? Agreeableness { get; set; }
        [JsonPropertyName("neuroticism")]
        public int? Neuroticism { get; set; }
        [JsonPropertyName("nature")]
        public int? Nature { get; set; }
        [JsonPropertyName("tags")]
        public List<string> Tags{ get; set; }
    }
}
