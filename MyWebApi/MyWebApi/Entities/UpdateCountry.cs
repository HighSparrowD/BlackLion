using System.Text.Json.Serialization;
using WebApi.Enums;

namespace WebApi.Entities
{
    public class UpdateCountry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("lang")]
        public AppLanguage Lang { get; set; }
        [JsonPropertyName("countryName")]
        public string CountryName { get; set; }
        [JsonPropertyName("priority")]
        public short? Priority { get; set; }
    }
}
