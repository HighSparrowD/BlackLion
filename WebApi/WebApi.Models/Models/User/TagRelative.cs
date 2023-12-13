using WebApi.Enums.Enums.Tag;

namespace WebApi.Models.Models.User
{
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
}
