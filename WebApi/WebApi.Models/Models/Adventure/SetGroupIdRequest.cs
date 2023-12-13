using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Adventure;

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
