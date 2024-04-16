using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;

namespace WebApi.Models.Models.Admin;

#nullable enable
public class VerificationRequest
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("adminId")]
    public long? AdminId { get; set; }

    [JsonPropertyName("state")]
    public VerificationRequestStatus? State { get; set; }

    [JsonPropertyName("media")]
    public string Media { get; set; } = default!;

    [JsonPropertyName("mediaType")]
    public MediaType MediaType { get; set; }

    [JsonPropertyName("gesture")]
    public string? Gesture { get; set; }

    [JsonPropertyName("confirmationType")]
    public IdentityConfirmationType ConfirmationType { get; set; }
}
