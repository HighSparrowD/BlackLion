using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.User
{
#nullable enable
    public class AddRequest
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("senderId")]
        public long SenderId { get; set; }
        [JsonPropertyName("isMatch")]
        public bool IsMatch { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("messageType")]
        public MessageType MessageType { get; set; }
    }
}
