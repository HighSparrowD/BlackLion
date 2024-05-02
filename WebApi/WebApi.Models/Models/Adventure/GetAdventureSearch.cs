using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Messaging;

namespace WebApi.Models.Models.Adventure;

public class GetAdventureSearch
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("media")]
    public string Media { get; set; }
    [JsonPropertyName("mediaType")]
    public MediaType MediaType { get; set; }
    [JsonPropertyName("autoReply")]
    public string AutoReply { get; set; }
    [JsonPropertyName("autoReplyType")]
    public ReplyType? AutoReplyType { get; set; }

    public static string GenerateDescription(Adventure adventure)
    {
        return $"{adventure.Name}\n{adventure.Country.CountryName} {adventure.City.CityName}\n\n{adventure.Description}\n".Replace("\n\n\n", "");
    }
}
