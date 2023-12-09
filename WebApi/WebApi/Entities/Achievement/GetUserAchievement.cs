using System.Text.Json.Serialization;

namespace WebApi.Entities.AchievementEntities
{
    public class GetUserAchievement
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("progress")]
        public int Progress { get; set; }
        [JsonPropertyName("isAcquired")]
        public bool IsAcquired { get; set; }
        [JsonPropertyName("conditionValue")]
        public int ConditionValue { get; set; }
        [JsonPropertyName("reward")]
        public int Reward { get; set; }
    }
}
