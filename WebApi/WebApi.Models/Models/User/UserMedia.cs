using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Media;

namespace WebApi.Models.Models.User;

public class UserMedia
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("media")]
    public string Media { get; set; }

    [JsonPropertyName("mediaType")]
    public MediaType MediaType { get; set; }
}
