using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.Admin;

public class UploadAchievement
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("language")]
    public AppLanguage Language { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("condition")]
    public int ConditionValue { get; set; }
    [JsonPropertyName("reward")]
    public int Reward { get; set; }
}
