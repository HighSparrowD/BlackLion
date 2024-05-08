using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.Adventure;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Messaging;
using WebApi.Main.Entities.Location;
using models = WebApi.Models.Models.Adventure;

#nullable enable
namespace WebApi.Main.Entities.Adventure;

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
    public ReplyType? AutoReplyType { get; set; }
    public string? AutoReply { get; set; }
    public string? UniqueLink { get; set; }
    //Indicates, whether if adventure awaits for the group id
    public bool IsAwaiting { get; set; }
    public DateTime? DeleteDate { get; set; }
    public string? GroupLink { get; set; }
    public long? GroupId { get; set; }
    public AdventureStatus Status { get; set; }

    public long? AdminId { get; set; }

    public virtual User.User? Creator { get; set; }
    public virtual Country? Country { get; set; }
    public virtual City? City { get; set; }
    public virtual List<AdventureAttendee>? Attendees { get; set; }

    public static explicit operator Adventure?(models.Adventure? adventure)
    {
        if (adventure == null)
            return null;

        return new Adventure
        {
            Id = adventure.Id,
            Status = adventure.Status,
            AutoReply = adventure.AutoReply,
            CityId = adventure.CityId,
            CityCountryLang = adventure.CityCountryLang,
            CountryId = adventure.CountryId,
            CountryLang = adventure.CountryLang,
            DeleteDate = adventure.DeleteDate,
            Description = adventure.Description,
            GroupId = adventure.GroupId,
            GroupLink = adventure.GroupLink,
            AutoReplyType = adventure.AutoReplyType,
            IsAwaiting = adventure.IsAwaiting,
            IsOffline = adventure.IsOffline,
            Media = adventure.Media,
            MediaType = adventure.MediaType,
            Name = adventure.Name,
            UniqueLink = adventure.UniqueLink,
            UserId = adventure.UserId,
            AdminId = adventure.AdminId,
            City = (City)adventure.City!,
            Country = (Country)adventure.Country!,
            Creator = (User.User)adventure.Creator!
        };
    }

    public static implicit operator models.Adventure?(Adventure? adventure)
    {
        if (adventure == null)
            return null;

        return new models.Adventure
        {
            Id = adventure.Id,
            Status = adventure.Status,
            AutoReply = adventure.AutoReply,
            CityId = adventure.CityId,
            CityCountryLang = adventure.CityCountryLang,
            CountryId = adventure.CountryId,
            CountryLang = adventure.CountryLang,
            DeleteDate = adventure.DeleteDate,
            Description = adventure.Description,
            GroupId = adventure.GroupId,
            GroupLink = adventure.GroupLink,
            AutoReplyType = adventure.AutoReplyType,
            IsAwaiting = adventure.IsAwaiting,
            IsOffline = adventure.IsOffline,
            Media = adventure.Media,
            MediaType = adventure.MediaType,
            Name = adventure.Name,
            UniqueLink = adventure.UniqueLink,
            UserId = adventure.UserId,
            AdminId = adventure.AdminId,
            City = (City)adventure.City!,
            Country = (Country)adventure.Country!,
            Creator = (User.User)adventure.Creator!
        };
    }

	public static implicit operator models.ManageAdventure?(Adventure? adventure)
	{
		if (adventure == null)
			return null;

		return new models.ManageAdventure
		{
			Id = adventure.Id,
			AutoReply = adventure.AutoReply,
			CityId = adventure.CityId,
			CountryId = adventure.CountryId,
			Description = adventure.Description,
			GroupId = adventure.GroupId,
			AutoReplyType = adventure.AutoReplyType,
			IsAwaiting = adventure.IsAwaiting,
			IsOffline = adventure.IsOffline,
			Media = adventure.Media,
			MediaType = adventure.MediaType,
			Name = adventure.Name,
			UserId = adventure.UserId
		};
	}
}
