using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Adventure;

namespace WebApi.Models.Models.Adventure;

public class AttendeeInfo
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("status")]
    public AdventureAttendeeStatus Status { get; set; }
}
