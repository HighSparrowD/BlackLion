using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;

#nullable enable
namespace WebApi.Models.Models.User
{
    public class SendVerificationRequest
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("media")]
        public string Media { get; set; } = default!;

        [JsonPropertyName("mediaType")]
        public MediaType MediaType { get; set; }

        [JsonPropertyName("gesture")]
        public string? Gesture { get; set; }

        [JsonPropertyName("type")]
        public IdentityConfirmationType ConfirmationType { get; set; }
    }
}
