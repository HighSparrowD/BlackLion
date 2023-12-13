using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Notification;

#nullable enable
namespace WebApi.Models.Models.User
{
    public class AddNotification
    {
        [JsonPropertyName("senderId")]
        public long SenderId { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("isLikedBack")]
        public bool IsLikedBack { get; set; }
        [JsonPropertyName("type")]
        public NotificationType Type { get; set; }
        [JsonPropertyName("section")]
        public Section Section { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
