using System.Text.Json.Serialization;

namespace WebApi.Entities.AdventureEntities
{
    public class GetTemplateShort
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
