using System.Text.Json.Serialization;
using WebApi.Enums;

namespace WebApi.Entities.UserActionEntities
{
    public class RegisterEncounter
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("encounteredUserId")]
        public long EncounteredUserId { get; set; }
        [JsonPropertyName("section")]
        public Section Section { get; set; }
    }
}
