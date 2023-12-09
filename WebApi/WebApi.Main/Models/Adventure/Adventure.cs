using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Main.Enums.Adventure;
using WebApi.Main.Enums.Media;
using WebApi.Main.Enums.General;
using WebApi.Main.Models.Location;
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
using entities = WebApi.Main.Models.User;
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

#nullable enable
namespace WebApi.Main.Models.Adventure;

public class Adventure
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("Creator")]
    public long UserId { get; set; }
    public string? Name { get; set; }
    public bool IsOffline { get; set; }
    public int? CountryId { get; set; }
    public int? CityId { get; set; }
    public AppLanguage? CountryLang { get; set; }
    public AppLanguage? CityCountryLang { get; set; }
    public string? Media { get; set; }
    public MediaType MediaType { get; set; }
    public string? Description { get; set; }
    public string? Experience { get; set; }
    public string? AttendeesDescription { get; set; }
    public string? UnwantedAttendeesDescription { get; set; }
    public string? Gratitude { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public string? Duration { get; set; }
    public string? Application { get; set; }
    public string? Address { get; set; }
    public bool? IsAutoReplyText { get; set; }
    public string? AutoReply { get; set; }
    public string? UniqueLink { get; set; }
    //Indicates, whether if adventure awaits for the group id
    public bool IsAwaiting { get; set; }
    public DateTime? DeleteDate { get; set; }
    public string? GroupLink { get; set; }
    public long? GroupId { get; set; }
    public AdventureStatus Status { get; set; }

    public virtual entities.User? Creator { get; set; }
    public virtual Country? Country { get; set; }
    public virtual City? City { get; set; }
    public virtual List<AdventureAttendee>? Attendees { get; set; }
}
