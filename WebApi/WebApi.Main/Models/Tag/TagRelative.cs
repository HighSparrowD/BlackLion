using WebApi.Enums.Enums.Tag;

namespace WebApi.Main.Entities.Tag;

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
