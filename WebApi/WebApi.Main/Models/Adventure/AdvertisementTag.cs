using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.Tag;

namespace WebApi.Main.Entities.Adventure;

public class AdventureTag
{
	[Key]
	public long TagId { get; set; }

	[Key]
	public long AdventureId { get; set; }

	[Key]
	public TagType TagType { get; set; }

	public virtual Tag.Tag Tag { get; set; }

	public AdventureTag()
	{ }

	public AdventureTag(long id, long adventureId, TagType type)
	{
		TagId = id;
		AdventureId = adventureId;
		TagType = type;
	}
}
