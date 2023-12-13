using System.Text.Json.Serialization;

namespace WebApi.Models.Models.User
{
    public class GetLocalizedEnum
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
