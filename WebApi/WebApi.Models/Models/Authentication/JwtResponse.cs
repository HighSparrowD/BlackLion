using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Authentication;

namespace WebApi.Models.Models.Authentication;

public class JwtResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("roles")]
    public List<Role> Roles { get; set; }
}
