using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;

namespace WebApi.Models.Models.User
{
    public class UserPartialData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("media")]
        public string Media { get; set; }
        [JsonPropertyName("mediaType")]
        public MediaType MediaType { get; set; }
        [JsonPropertyName("appLanguage")]
        public AppLanguage AppLanguage { get; set; }
    }
}
