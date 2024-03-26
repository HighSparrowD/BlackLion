using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Authentication;

public class JwtResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}
