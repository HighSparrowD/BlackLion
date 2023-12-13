using System.Text.Json.Serialization;

namespace WebApi.Models.Models.User
{
    public class SetStory
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("story")]
        public string Story { get; set; }
    }
}
