using WebApi.Main.Enums.Tag;

namespace WebApi.Main.Models.Tag;

public class TagRelative
{
    public long TagId { get; set; }
    public TagType TagType { get; set; }

    public TagRelative(long tagId, TagType type)
    {
        TagId = tagId;
        TagType = type;
    }
}
