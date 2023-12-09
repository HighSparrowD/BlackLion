using System.Text.Json.Serialization;
using WebApi.Main.Enums.General;
using WebApi.Main.Enums.Notification;

#nullable enable
namespace WebApi.Entities.UserActionEntities
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
