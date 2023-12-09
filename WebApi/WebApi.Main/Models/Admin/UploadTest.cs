using WebApi.Entities.AdminEntities;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WebApi.Main.Enums.User;
using WebApi.Main.Enums.General;

#nullable enable
namespace WebApi.Entities.TestEntities
{
    public class UploadTest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("language")]
        public AppLanguage Language { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("testType")]
        public OceanStats? TestType { get; set; }
        [JsonPropertyName("canBePassedInDays")]
        public int CanBePassedInDays { get; set; }
        [JsonPropertyName("questions")]
        public List<UploadTestQuestion>? Questions { get; set; }
        [JsonPropertyName("results")]
        public List<UploadTestResult>? Results { get; set; }
        [JsonPropertyName("scales")]
        public List<UploadTestScale?>? Scales { get; set; }
    }
}
