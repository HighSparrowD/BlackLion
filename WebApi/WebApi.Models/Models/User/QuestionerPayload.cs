using System.Text.Json.Serialization;

namespace WebApi.Models.Models.User
{
    // TODO: Design
    public class QuestionerPayload
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; }
    }
}
