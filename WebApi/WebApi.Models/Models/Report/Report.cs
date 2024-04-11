using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Report;

#nullable enable
namespace WebApi.Models.Models.Report;

public class Report
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("senderId")]
    public long SenderId { get; set; }

    [JsonPropertyName("userId")]
    public long? UserId { get; set; }

    [JsonPropertyName("adventureId")]
    public long? AdventureId { get; set; }

    [JsonPropertyName("adminId")]
    public long? AdminId { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("reason")]
    public ReportReason Reason { get; set; }

    [JsonPropertyName("insertedUtc")]
    public DateTime InsertedUtc { get; set; }
}
