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
        [JsonPropertyName("openness")]
        public float? Openness { get; set; }
        [JsonPropertyName("conscientiousness")]
        public float? Conscientiousness { get; set; }
        [JsonPropertyName("extroversion")]
        public float? Extroversion { get; set; }
        [JsonPropertyName("agreeableness")]
        public float? Agreeableness { get; set; }
        [JsonPropertyName("neuroticism")]
        public float? Neuroticism { get; set; }
        [JsonPropertyName("nature")]
        public float? Nature { get; set; }
        [JsonPropertyName("tags")]
        public List<long> Tags{ get; set; }
    }
}
