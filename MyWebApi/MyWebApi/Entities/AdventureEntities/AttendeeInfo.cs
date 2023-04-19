using MyWebApi.Enums;
using System.Text.Json.Serialization;

namespace MyWebApi.Entities.AdventureEntities
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
