using System.Text.Json.Serialization;

namespace WebApi.Models.Models.User
{
    public class GetUserTags
    {
        [JsonPropertyName("fullTags")]
        public List<UserTags> FullTags { get; set; }
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }
}
