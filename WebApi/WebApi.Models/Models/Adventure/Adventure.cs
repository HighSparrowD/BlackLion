using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Adventure;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;
using WebApi.Models.Models.Location;

#nullable enable
namespace WebApi.Models.Models.Adventure;

public class Adventure
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("userId")]
    public long UserId { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("isOffline")]
    public bool IsOffline { get; set; }

    [JsonPropertyName("countryId")]
    public int? CountryId { get; set; }

    [JsonPropertyName("cityId")]
    public int? CityId { get; set; }

    [JsonPropertyName("countryLang")]
    public AppLanguage? CountryLang { get; set; }

    [JsonPropertyName("cityCountryLang")]
    public AppLanguage? CityCountryLang { get; set; }

    [JsonPropertyName("media")]
    public string? Media { get; set; }

    [JsonPropertyName("mediaType")]
    public MediaType MediaType { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("experience")]
    public string? Experience { get; set; }

    [JsonPropertyName("attendeesDescription")]
    public string? AttendeesDescription { get; set; }

    [JsonPropertyName("unwantedAttendeesDescription")]
    public string? UnwantedAttendeesDescription { get; set; }

    [JsonPropertyName("gratitude")]
    public string? Gratitude { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }

    [JsonPropertyName("application")]
    public string? Application { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("isAutoReplyText")]
    public bool? IsAutoReplyText { get; set; }

    [JsonPropertyName("autoReply")]
    public string? AutoReply { get; set; }

    [JsonPropertyName("uniqueLink")]
    public string? UniqueLink { get; set; }

    [JsonPropertyName("isAwaiting")]
    public bool IsAwaiting { get; set; }

    [JsonPropertyName("deleteDate")]
    public DateTime? DeleteDate { get; set; }

    [JsonPropertyName("groupLink")]
    public string? GroupLink { get; set; }

    [JsonPropertyName("groupId")]
    public long? GroupId { get; set; }

    [JsonPropertyName("status")]
    public AdventureStatus Status { get; set; }

    [JsonPropertyName("adminId")]
    public long? AdminId { get; set; }

    public virtual User.User? Creator { get; set; }
    public virtual Country? Country { get; set; }
    public virtual City? City { get; set; }
}
