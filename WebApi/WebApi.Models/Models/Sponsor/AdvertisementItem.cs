#nullable enable
using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Sponsor
{
    public class AdvertisementItem
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
