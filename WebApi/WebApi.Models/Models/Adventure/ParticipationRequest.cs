using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Adventure;

public class ParticipationRequest
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }
    [JsonPropertyName("invitationCode")]
    public string InvitationCode { get; set; }
}
