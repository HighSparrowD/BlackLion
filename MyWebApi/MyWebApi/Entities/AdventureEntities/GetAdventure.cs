using WebApi.Enums;
using System;
using System.Text.Json.Serialization;

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
