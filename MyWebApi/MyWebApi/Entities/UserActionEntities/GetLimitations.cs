using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    public class GetLimitations
    {
        [JsonPropertyName("actualTagViews")]
        public int ActualTagViews { get; set; }
        [JsonPropertyName("actualRtViews")]
        public int ActualRtViews { get; set; }
        [JsonPropertyName("actualProfileViews")]
        public int ActualProfileViews { get; set; }
        [JsonPropertyName("maxRtViews")]
        public int MaxRtViews { get; set; }
        [JsonPropertyName("maxTagViews")]
        public int MaxTagViews { get; set; }
        [JsonPropertyName("maxProfileViews")]
        public int MaxProfileViews { get; set; }
        [JsonPropertyName("maxTagsPerSearch")]
        public int MaxTagsPerSearch { get; set; }
    }
}
