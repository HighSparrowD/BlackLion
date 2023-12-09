using System.Text.Json.Serialization;
using WebApi.Main.Enums.General;
using WebApi.Main.Enums.Media;

namespace WebApi.Entities.UserInfoEntities
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
