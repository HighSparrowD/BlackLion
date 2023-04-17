using System.Text.Json.Serialization;

namespace MyWebApi.Entities.AdventureEntities
{
    public class ParticipationRequest
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("invitationCode")]
        public string InvitationCode { get; set; }
    }
}
