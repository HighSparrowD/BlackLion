using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.SystemEntitires
{
    public class Tag
    {
        [Key]
        public long Id { get; set; }
        [Key]
        public TagType Type { get; set; }
        public string Text { get; set; }

        public Tag()
        {}

        public Tag(string text, TagType type)
        {
            Text = text;
            Type = type;
        }
    }
}
