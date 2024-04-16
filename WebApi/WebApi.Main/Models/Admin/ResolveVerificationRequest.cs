using System.Text.Json.Serialization;
using WebApi.Enums.Enums.User;

namespace WebApi.Main.Entities.Admin;

#nullable enable
public class ResolveVerificationRequest
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("adminId")]
    public long AdminId { get; set; }

    [JsonPropertyName("status")]
    public VerificationRequestStatus Status { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
