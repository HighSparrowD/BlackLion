using System.Text.Json.Serialization;
using WebApi.Enums;

#nullable enable
namespace WebApi.Entities.UserActionEntities
{
    public class GetRequests
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("senderId")]
        public long SenderId { get; set; }
        [JsonPropertyName("isMatch")]
        public bool IsMatch { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("systemMessage")]
        public string? SystemMessage { get; set; }
        [JsonPropertyName("type")]
        public MessageType Type { get; set; }
    }
}
