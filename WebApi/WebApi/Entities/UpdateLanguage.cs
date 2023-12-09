using System.Text.Json.Serialization;
using WebApi.Main.Enums.General;

namespace WebApi.Entities
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
