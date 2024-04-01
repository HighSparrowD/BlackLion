using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Authentication;

public class MachineLoginModel
{
    [JsonPropertyName("appSecret")]
    public string AppSecret { get; set; }
}
