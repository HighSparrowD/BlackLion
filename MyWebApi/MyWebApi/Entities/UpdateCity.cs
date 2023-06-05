using System.Text.Json.Serialization;
using WebApi.Enums;

namespace WebApi.Entities
{
    public class UpdateCity
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("lang")]
        public AppLanguage Lang { get; set; }
        [JsonPropertyName("cityName")]
        public string CityName { get; set; }
        [JsonPropertyName("countryId")]
        public int CountryId { get; set; }
    }
}
