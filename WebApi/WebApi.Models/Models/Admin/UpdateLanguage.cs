using System.Text.Json.Serialization;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Admin
{
    public class UpdateLanguage
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("languageName")]
        public string LanguageName { get; set; }
        [JsonPropertyName("languageNameNative")]
        public string LanguageNameNative { get; set; }
        [JsonPropertyName("priority")]
        public short? Priority { get; set; }
        [JsonPropertyName("lang")]
        public AppLanguage Lang { get; set; }
    }
}
