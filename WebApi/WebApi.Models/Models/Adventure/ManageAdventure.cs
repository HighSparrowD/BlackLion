using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Messaging;

namespace WebApi.Models.Models.Adventure;

public class ManageAdventure
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    [JsonPropertyName("userId")]
    public long UserId { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("isOffline")]
    public bool IsOffline { get; set; }
    [JsonPropertyName("countryId")]
    public int? CountryId { get; set; }
    [JsonPropertyName("cityId")]
    public int? CityId { get; set; }
    [JsonPropertyName("media")]
    public string Media { get; set; }
    [JsonPropertyName("mediaType")]
    public MediaType MediaType { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    //If is null -> auto reply is not present at all
    [JsonPropertyName("autoReplyType")]
    public ReplyType? AutoReplyType { get; set; }
    [JsonPropertyName("autoReply")]
    public string AutoReply { get; set; }
    [JsonPropertyName("isAwaiting")]
    public bool IsAwaiting { get; set; }
    [JsonPropertyName("groupId")]
    public long? GroupId { get; set; }

    public ManageAdventure()
    { }

    public ManageAdventure(Adventure adventure)
    {
        Id = adventure.Id;
        UserId = adventure.UserId;
        Name = adventure.Name;
        IsOffline = adventure.IsOffline;
        CountryId = adventure.CountryId;
        CityId = adventure.CityId;
        Media = adventure.Media;
        MediaType = adventure.MediaType;
        Description = adventure.Description;
        AutoReplyType = adventure.AutoReplyType;
        AutoReply = adventure.AutoReply;
        IsAwaiting = adventure.IsAwaiting;
        GroupId = adventure.GroupId;
    }
}
