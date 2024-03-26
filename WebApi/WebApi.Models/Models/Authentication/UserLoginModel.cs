using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Authentication;

public class UserLoginModel
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("appSecret")]
    public string AppSecret { get; set; }
}
