using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.Tag;
using WebApi.Main.Entities.Tag;

namespace WebApi.Main.Models.Sponsor;

public class AdvertisementTag
{
	[Key]
	public long TagId { get; set; }

	[Key]
	public long AdvertisementId { get; set; }

	[Key]
	public TagType TagType { get; set; }

	public virtual Tag Tag { get; set; }

	public AdvertisementTag()
	{ }

	public AdvertisementTag(long id, long advertisementId, TagType type)
	{
		TagId = id;
		AdvertisementId = advertisementId;
		TagType = type;
	}
}
