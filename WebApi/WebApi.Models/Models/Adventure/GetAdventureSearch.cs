using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Media;

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
    [JsonPropertyName("isAutoReplyText")]
    public bool? IsAutoReplyText { get; set; }

    public static string GenerateDescription(Adventure adventure)
    {
        return $"{adventure.Name}\n{adventure.Country.CountryName} {adventure.City.CityName}\n\n{adventure.Description}\n{adventure.Experience}\n{adventure.AttendeesDescription}\n{adventure.UnwantedAttendeesDescription}\n{adventure.Gratitude}\n{adventure.Date} {adventure.Time}\n{adventure.Duration}\n{adventure.Application}{adventure.Address}".Replace("\n\n\n", "");
    }
}
