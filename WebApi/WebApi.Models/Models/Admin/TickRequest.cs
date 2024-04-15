using System.Text.Json.Serialization;
using WebApi.Enums.Enums.User;

namespace WebApi.Models.Models.Admin;

#nullable enable
public class TickRequest
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("adminId")]
    public long? AdminId { get; set; }

    [JsonPropertyName("state")]
    public TickRequestStatus? State { get; set; }

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }

    [JsonPropertyName("video")]
    public string? Video { get; set; }

    [JsonPropertyName("circle")]
    public string? Circle { get; set; }

    [JsonPropertyName("gesture")]
    public string? Gesture { get; set; }

    [JsonPropertyName("type")]
    public IdentityConfirmationType Type { get; set; }
}
