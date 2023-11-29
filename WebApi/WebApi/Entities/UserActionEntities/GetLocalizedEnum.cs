using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    public class GetLocalizedEnum
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
