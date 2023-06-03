using System.Text.Json.Serialization;

namespace WebApi.Entities.AdventureEntities
{
    public class SetGroupIdRequest
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("groupLink")]
        public string GroupLink { get; set; }
        [JsonPropertyName("groupId")]
        public long GroupId { get; set; }
        [JsonPropertyName("adventureName")]
        public string AdventureName { get; set; }
    }
}
