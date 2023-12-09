using System.Text.Json.Serialization;
using WebApi.Main.Enums.Adventure;

namespace WebApi.Entities.AdventureEntities
{
    public class AttendeeInfo
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("status")]
        public AdventureAttendeeStatus Status { get; set; }
    }
}
