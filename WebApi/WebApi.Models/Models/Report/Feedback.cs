using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Report;

namespace WebApi.Models.Models.Report;

public class Feedback
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("adminId")]
    public long? AdminId { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("insertedUtc")]
    public string InsertedUtc { get; set; }

    [JsonPropertyName("reason")]
    public FeedbackReason Reason { get; set; }
}
