using System.Text.Json.Serialization;

namespace WebApi.Models.Models.User
{
    public class DeleteUserRequest
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
