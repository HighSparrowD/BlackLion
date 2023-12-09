using System.Text.Json.Serialization;
using WebApi.Main.Enums.Adventure;

namespace WebApi.Entities.AdventureEntities
{
    public class GetAdventure
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        public AdventureStatus Status{ get; set; }
    }
}
